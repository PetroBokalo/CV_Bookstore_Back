using BookStoreAPI.Common;
using BookStoreAPI.Services.Interfaces;
using Google.Apis.Auth;
using System.Text.Json.Serialization;

namespace BookStoreAPI.Services
{
    public static class GoogleOAuthService
    {

        public static async Task<ServiceResult<GoogleJsonWebSignature.Payload?>> GetUserInfoAsync(string code, string clientId, string clientSecret,
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

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                var errorMessage = $"Google API error ({(int)response.StatusCode} {response.StatusCode}): {errorContent}";

                return ServiceResult<GoogleJsonWebSignature.Payload?>.Fail(message: errorMessage, statuscode: (int)response.StatusCode);
            }

            var json = await response.Content.ReadAsStringAsync();

            var tokenData = System.Text.Json.JsonSerializer.Deserialize<GoogleTokenResponse>(json);

            if (string.IsNullOrEmpty(tokenData?.IdToken))
                return ServiceResult<GoogleJsonWebSignature.Payload?>.Fail("Missing idToken in response", StatusCodes.Status400BadRequest);

            var payload = await GoogleJsonWebSignature.ValidateAsync(tokenData.IdToken);

            return ServiceResult<GoogleJsonWebSignature.Payload?>.Ok(data: payload);

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
