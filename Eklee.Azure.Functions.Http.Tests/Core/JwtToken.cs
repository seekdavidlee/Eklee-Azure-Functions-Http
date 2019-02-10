using Newtonsoft.Json;

namespace Eklee.Azure.Functions.Http.Tests.Core
{
	public class JwtToken
	{
		[JsonProperty("token_type")]
		public string TokenType { get; set; }

		[JsonProperty("access_token")]
		public string AccessToken { get; set; }
	}
}
