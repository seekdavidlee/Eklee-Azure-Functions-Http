using System.Linq;
using System.Threading.Tasks;
using Eklee.Azure.Functions.Http.Models;
using Eklee.Azure.Functions.Http.Tests.Core;
using Eklee.Azure.Functions.Http.Tests.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eklee.Azure.Functions.Http.Tests
{
	public class JwtTokenValidatorTests
	{
		private IJwtTokenValidator _jwtTokenValidator;
		private readonly IHttpRequestContext _httpRequestContext;
		private readonly ResourceOwnerTokenProvider _resourceOwnerTokenProvider = new ResourceOwnerTokenProvider();
		private readonly Security _security = new Security();
		private readonly ICacheManager _cacheManager;

		public JwtTokenValidatorTests()
		{
			_httpRequestContext = Substitute.For<IHttpRequestContext>();
			_httpRequestContext.Security.Returns(x => _security);

			var distributedCache = Substitute.For<IDistributedCache>();
			distributedCache.GetAsync(Arg.Any<string>()).Returns(Task.FromResult((byte[])null));

			_cacheManager = new CacheManager(distributedCache);
		}

		private void UseLocalSettings1()
		{
			var mock = Substitute.For<IJwtTokenValidatorParameters>();
			mock.Audience.Returns(_resourceOwnerTokenProvider.LocalSettings1.Audience);
			mock.Issuers.Returns(_resourceOwnerTokenProvider.LocalSettings1.Issuers);
			_jwtTokenValidator = new JwtTokenValidator(_cacheManager, _httpRequestContext, mock, Substitute.For<ILogger>());
		}

		private void UseLocalSettings2()
		{
			var mock = Substitute.For<IJwtTokenValidatorParameters>();
			mock.Audience.Returns(_resourceOwnerTokenProvider.LocalSettings2.Audience);
			mock.Issuers.Returns(_resourceOwnerTokenProvider.LocalSettings2.Issuers);
			_jwtTokenValidator = new JwtTokenValidator(_cacheManager, _httpRequestContext, mock, Substitute.For<ILogger>());
		}

		private void UseLocalSettings3()
		{
			var mock = Substitute.For<IJwtTokenValidatorParameters>();
			mock.Audience.Returns(_resourceOwnerTokenProvider.LocalSettings3.Audience);
			mock.Issuers.Returns(_resourceOwnerTokenProvider.LocalSettings3.Issuers);
			_jwtTokenValidator = new JwtTokenValidator(_cacheManager, _httpRequestContext, mock, Substitute.For<ILogger>());
		}

		[Fact]
		public async Task ValidateTokenIsValidForUser1WithRole()
		{
			UseLocalSettings1();

			var token = await _resourceOwnerTokenProvider.LocalSettings1.GetUser1Token();

			var mock = new MockHttpRequest();
			mock.Headers.Add("Authorization", $"Bearer {token.AccessToken}");

			_httpRequestContext.Request = mock;

			_jwtTokenValidator.Validate().ShouldBe(true);
			_security.ClaimsPrincipal.ShouldNotBeNull();
			var username = _security.ClaimsPrincipal.Claims.Single(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn");
			// ReSharper disable once PossibleNullReferenceException
			username.Value.ShouldBe(_resourceOwnerTokenProvider.LocalSettings1.User1.Username);

			var role = _security.ClaimsPrincipal.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
			role.Value.ShouldBe(_resourceOwnerTokenProvider.LocalSettings1.User1.Role);
		}

		[Fact]
		public async Task ValidateTokenIsValidForUser2WithoutRole()
		{
			UseLocalSettings1();

			var token = await _resourceOwnerTokenProvider.LocalSettings1.GetUser2Token();

			var mock = new MockHttpRequest();
			mock.Headers.Add("Authorization", $"Bearer {token.AccessToken}");

			_httpRequestContext.Request = mock;

			_jwtTokenValidator.Validate().ShouldBe(true);
			_security.ClaimsPrincipal.ShouldNotBeNull();
			var username = _security.ClaimsPrincipal.Claims.Single(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn");
			// ReSharper disable once PossibleNullReferenceException
			username.Value.ShouldBe(_resourceOwnerTokenProvider.LocalSettings1.User2.Username);

			var role = _security.ClaimsPrincipal.Claims.SingleOrDefault(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
			role.ShouldBeNull();
		}

		[Fact]
		public void ValidateExpiredTokenIsInvalid()
		{
			UseLocalSettings1();

			var tokenItems = _resourceOwnerTokenProvider.LoadTokens("tokens.json");

			var token = new JwtToken { AccessToken = tokenItems.Tokens.Single(x => x.Type == "expired").Value };

			var mock = new MockHttpRequest();
			mock.Headers.Add("Authorization", $"Bearer {token.AccessToken}");

			_httpRequestContext.Request = mock;

			Should.Throw<SecurityTokenExpiredException>(() => _jwtTokenValidator.Validate());
		}

		[Fact]
		public async Task ValidateTokenIsInValidForIssuerNotInList()
		{
			UseLocalSettings2();

			var token = await _resourceOwnerTokenProvider.LocalSettings2.GetUser2Token();

			var mock = new MockHttpRequest();
			mock.Headers.Add("Authorization", $"Bearer {token.AccessToken}");

			_httpRequestContext.Request = mock;

			Should.Throw<SecurityTokenSignatureKeyNotFoundException>(() => _jwtTokenValidator.Validate());
		}

		[Fact]
		public async Task ValidateTokenIsInValidForAudience()
		{
			UseLocalSettings3();

			var token = await _resourceOwnerTokenProvider.LocalSettings3.GetUser1Token();

			var mock = new MockHttpRequest();
			mock.Headers.Add("Authorization", $"Bearer {token.AccessToken}");

			_httpRequestContext.Request = mock;

			Should.Throw<SecurityTokenInvalidAudienceException>(() => _jwtTokenValidator.Validate());
		}
	}
}
