using System.Text.Json.Serialization;

namespace BookStoreAPI.DTOs.Authentication
{
    public record LoginUserResponseDto (string Email = "", string AccessToken = "")
    {
        [JsonPropertyName ("_links")]
        public LoginLinksDto? Links { get; init; }
    }
}
