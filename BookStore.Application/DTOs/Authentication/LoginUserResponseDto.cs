using System.Text.Json.Serialization;

namespace BookStore.Application.DTOs.Authentication
{
    public record LoginUserResponseDto (string Email = "", string AccessToken = "")
    {
        [JsonPropertyName ("_links")]
        public LoginLinksDto? Links { get; init; }
    }
}
