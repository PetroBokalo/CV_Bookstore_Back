using Google.Apis.Auth;
using System.Text.Json.Serialization;

namespace BookStoreAPI.Common
{
    public static class GoogleOAuthHelper
    {

        public static async Task<GoogleJsonWebSignature.Payload?> GetUserInfoAsync(string code, string clientId, string clientSecret,
            string redirectUri)
        {
            using var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://oauth2.googleapis.com/token");
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["code"] = code,
                ["client_id"] = clientId,
                ["client_secret"] = clientSecret,
                ["redirect_uri"] = redirectUri,
                ["grant_type"] = "authorization_code"

            });

            var response = await client.SendAsync(request);

            if(!response.IsSuccessStatusCode) return null; // bad practice 

            var json = await response.Content.ReadAsStringAsync();

            var tokenData = System.Text.Json.JsonSerializer.Deserialize<GoogleTokenResponse>(json);

            if (tokenData == null)  return null; // bad practice

            var payload = await GoogleJsonWebSignature.ValidateAsync(tokenData.IdToken);

            return payload;

        }

    }

    public class GoogleTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;
        [JsonPropertyName("id_token")]
        public string IdToken { get; set; } = string.Empty;
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; } = string.Empty;

    }
}
