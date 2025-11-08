using System.Text.Json.Serialization;

namespace BookStore.Application.DTOs.Authentication
{
    public record RegisterResponseDto(string userEmail, string AccessToken)
    {
        [JsonPropertyName("_links")]
        public RegisterLinksDto? Links {  get; init; }
    }

}

