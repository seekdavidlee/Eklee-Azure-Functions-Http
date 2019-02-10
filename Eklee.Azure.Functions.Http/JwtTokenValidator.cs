using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
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
							var certs = cacheManager.TryGetOrSetIfNotExistAsync(() =>
							{
								var httpClient = new HttpClient { BaseAddress = new Uri(found) };
								var result = httpClient.GetStringAsync(".well-known/openid-configuration")
									.ConfigureAwait(false).GetAwaiter().GetResult();
								var jwtMetaInfo = JsonConvert.DeserializeObject<JwtMetaInfo>(result);

								httpClient = new HttpClient();
								result = httpClient.GetStringAsync(jwtMetaInfo.DiscoveryKeysUri)
									.ConfigureAwait(false).GetAwaiter().GetResult();

								logger.LogInformation($"Found keys information for issuer: {found}. {result}");

								var jwtKeys = JsonConvert.DeserializeObject<JwtKeys>(result);

								return new JwtMetaInfoKeyCerts
								{
									Values = jwtKeys.Keys.Select(x => x.X5c.First()).ToArray()
								};
							}, securityToken.Issuer, new DistributedCacheEntryOptions
							{
								AbsoluteExpiration = DateTimeOffset.UtcNow.AddDays(1)
							}).ConfigureAwait(false).GetAwaiter().GetResult().Result;


							return certs.Values.Select(value =>
							{
								var cert = new X509Certificate2(Encoding.UTF8.GetBytes(value));
								return (SecurityKey)new X509SecurityKey(cert);
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
					SecurityToken validatedToken;
					JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
					var claimsPrincipal = handler.ValidateToken(token, _tokenValidationParameters, out validatedToken);

					_httpRequestContext.Security.ClaimsPrincipal = claimsPrincipal;
					return true;
				}
			}

			_logger.LogInformation("Bearer token is missing from Authorization Header!");
			return false;
		}
	}
}