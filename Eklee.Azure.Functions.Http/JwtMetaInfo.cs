using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Eklee.Azure.Functions.Http
{
	public class JwtMetaInfo
	{
		[JsonProperty("jwks_uri")]
		public string DiscoveryKeysUri { get; set; }
	}

	public class JwtKeys
	{
		public List<JwtKey> Keys { get; set; }
	}

	public class JwtKey
	{
		public string Kty { get; set; }
		public  string Use { get; set; }
		public string Kid { get; set; }
		public string X5t { get; set; }
		public string N { get; set; }
		public string E { get; set; }
		public string[] X5c { get; set; }
	}
}
