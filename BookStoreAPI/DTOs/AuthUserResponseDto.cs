namespace BookStoreAPI.DTOs
{
    public record AuthUserResponseDto(int Id, string Email, string PhoneNumber, string FirstName, string LastName, string AccessToken);
}
