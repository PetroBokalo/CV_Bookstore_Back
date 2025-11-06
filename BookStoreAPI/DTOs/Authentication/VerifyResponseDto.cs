using System.Text.Json.Serialization;

namespace BookStoreAPI.DTOs.Authentication
{
    public record VerifyResponseDto()
    {
        [JsonPropertyName("_links")]
        public VerifyLinksDto? Links { get; init; }
    }
}
