using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Eklee.Azure.Functions.Http
{
	public class JwtTokenValidator : IJwtTokenValidator
	{
		private readonly IHttpRequestContext _httpRequestContext;
		private readonly ILogger _logger;
		private readonly TokenValidationParameters _tokenValidationParameters;

		protected virtual string GetBaseUrl(string issuer)
		{
			return issuer;
		}

		protected const string OPEN_ID = ".well-known/openid-configuration";

		protected virtual string GetOpenIdConfiguration()
		{
			return OPEN_ID;
		}

		public JwtTokenValidator(
			ICacheManager cacheManager,
			IHttpRequestContext httpRequestContext,
			IJwtTokenValidatorParameters tokenValidatorParameters,
			ILogger logger)
		{
			_httpRequestContext = httpRequestContext;
			_logger = logger;
			_tokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidIssuers = tokenValidatorParameters.Issuers.ToList(),
				ValidAudience = tokenValidatorParameters.Audience,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				RequireExpirationTime = true,
				IssuerSigningKeyResolver = (token, securityToken, kid, validationParameters) =>
				{
					if (!string.IsNullOrEmpty(securityToken.Issuer))
					{
						var found = validationParameters.ValidIssuers.SingleOrDefault(x => x == securityToken.Issuer);
						if (found != null)
						{
							var jwtKeys = cacheManager.TryGetOrSetIfNotExistAsync(() =>
							{
								var httpClient = new HttpClient { BaseAddress = new Uri(GetBaseUrl(found)) };
								var result = httpClient.GetStringAsync(GetOpenIdConfiguration())
									.ConfigureAwait(false).GetAwaiter().GetResult();
								var jwtMetaInfo = JsonConvert.DeserializeObject<JwtMetaInfo>(result);

								httpClient = new HttpClient();
								result = httpClient.GetStringAsync(jwtMetaInfo.DiscoveryKeysUri)
									.ConfigureAwait(false).GetAwaiter().GetResult();

								logger.LogInformation($"Found keys information for issuer: {found}. {result}");

								return JsonConvert.DeserializeObject<JwtKeys>(result);
							}, securityToken.Issuer, new DistributedCacheEntryOptions
							{
								AbsoluteExpiration = DateTimeOffset.UtcNow.AddDays(1)
							}).ConfigureAwait(false).GetAwaiter().GetResult().Result;

							return jwtKeys.Keys.Select(x =>
							{
								if (x.X5c != null && x.X5c.Length > 0)
								{
									var cert = new X509Certificate2(Encoding.UTF8.GetBytes(x.X5c.First()));
									return (SecurityKey)new X509SecurityKey(cert) { KeyId = x.Kid };
								}

								// Ref: https://blog.simontimms.com/2019/02/13/2019-02-12-Getting-b2c-claims-in-an-azure-function/
								byte[] exponent = Base64UrlDecode(x.E);
								byte[] modulus = Base64UrlDecode(x.N);

								return new RsaSecurityKey(new RSAParameters
								{
									Exponent = exponent,
									Modulus = modulus
								})
								{
									KeyId = x.Kid
								};
							});
						}

						logger.LogInformation($"Issuer: {securityToken.Issuer} does not exist.");
					}
					else
					{
						logger.LogInformation("Security token does not contain issuer.");
					}

					return null;
				}
			};
		}

		private static byte[] Base64UrlDecode(string arg)
		{
			string s = arg;
			s = s.Replace('-', '+'); // 62nd char of encoding
			s = s.Replace('_', '/'); // 63rd char of encoding
			switch (s.Length % 4) // Pad with trailing '='s
			{
				case 0: break; // No pad chars in this case
				case 2: s += "=="; break; // Two pad chars
				case 3: s += "="; break; // One pad char
				default:
					throw new Exception("Illegal base64url string!");
			}
			return Convert.FromBase64String(s); // Standard base64 decoder
		}

		public JwtTokenValidator(IHttpRequestContext httpRequestContext,
			TokenValidationParameters tokenValidationParameters)
		{
			_httpRequestContext = httpRequestContext;
			_tokenValidationParameters = tokenValidationParameters;
		}

		public bool Validate()
		{
			var authorizationHeaders = _httpRequestContext.Request.Headers["Authorization"];

			if (!string.IsNullOrEmpty(authorizationHeaders))
			{
				return ValidateBearerToken(authorizationHeaders.FirstOrDefault());
			}

			_logger.LogInformation("Authorization header is missing!");
			return false;
		}

		private bool ValidateBearerToken(string bearerToken)
		{
			if (bearerToken != null && bearerToken.ToLower().StartsWith("bearer "))
			{
				var token = bearerToken.Substring(7);
				if (!string.IsNullOrEmpty(token))
				{
					// ReSharper disable once NotAccessedVariable
					JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
					var claimsPrincipal = handler.ValidateToken(token, _tokenValidationParameters, out _);

					_httpRequestContext.Security.ClaimsPrincipal = claimsPrincipal;
					return true;
				}
			}

			_logger.LogInformation("Bearer token is missing from Authorization Header!");
			return false;
		}
	}
}