using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Eklee.Azure.Functions.Http.Tests.Core
{
	public class LocalSettings
	{
		public ApplicationItem Application { get; set; }
		public List<UserItem> Users { get; set; }
		public string[] Issuers { get; set; }
		public string Audience { get; set; }

		public UserItem User1 => Users[0];
		public UserItem User2 => Users[1];

		public async Task<JwtToken> GetUser1Token()
		{
			return await GetToken(User1);
		}

		public async Task<JwtToken> GetUser2Token()
		{
			return await GetToken(User2);
		}

		private async Task<JwtToken> GetToken(UserItem userItem)
		{
			return await GetToken(Application, userItem);
		}

		private async Task<JwtToken> GetToken(ApplicationItem apiApplication, UserItem userItem)
		{
			using (var client = new HttpClient())
			{
				var tokenEndpoint = $"https://login.microsoftonline.com/{userItem.TenantId}/oauth2/token";

				var dictionary = new Dictionary<string, string>
				{
					{ "client_secret", apiApplication.Secret },
					{ "client_id", apiApplication.Id },
					{ "grant_type", "password" },
					{ "username", userItem.Username },
					{ "password", userItem.Password },
					{ "resource", apiApplication.Id }
				};

				var content = new FormUrlEncodedContent(dictionary);

				using (var response = await client.PostAsync(tokenEndpoint, content))
				{
					var responseBody = await response.Content.ReadAsStringAsync();

					try
					{
						response.EnsureSuccessStatusCode();
					}
					catch (HttpRequestException)
					{
						throw new System.Exception(responseBody);
					}

					return JsonConvert.DeserializeObject<JwtToken>(responseBody);
				}
			}
		}
	}

	public static class MyExtensions
	{
		public static LocalSettings Load(this ResourceOwnerTokenProvider resourceOwnerTokenProvider, string fileName)
		{
			var text = File.ReadAllText(fileName);
			return JsonConvert.DeserializeObject<LocalSettings>(text);
		}

		public static TokenItems LoadTokens(this ResourceOwnerTokenProvider resourceOwnerTokenProvider, string fileName)
		{
			var text = File.ReadAllText(fileName);
			return JsonConvert.DeserializeObject<TokenItems>(text);
		}
	}

	public class ApplicationItem
	{
		public string Id { get; set; }
		public string Secret { get; set; }
	}

	public class UserItem
	{
		public string TenantId { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public string Role { get; set; }
	}

	public class TokenItems
	{
		public List<TokenItem> Tokens { get; set; }
	}

	public class TokenItem
	{
		public string Type { get; set; }
		public string Value { get; set; }
	}
}
