namespace BookStoreAPI.DTOs.Authentication
{
    public record AuthUserResponseDto(int Id, string Email, string PhoneNumber, string FirstName, string LastName, string AccessToken);
}
