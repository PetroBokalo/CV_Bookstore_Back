using System.Text.Json.Serialization;

namespace BookStore.Application.DTOs.Authentication
{
    public record VerifyResponseDto()
    {
        [JsonPropertyName("_links")]
        public VerifyLinksDto? Links { get; init; }
    }
}
