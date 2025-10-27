using System.Text.Json.Serialization;

namespace BookStoreAPI.DTOs.Authentication
{
    public record VerifyResponseDto(string AccessToken)
    {
        [JsonPropertyName("_links")]
        public VerifyLinksDto? Links { get; init; }
    }
}
