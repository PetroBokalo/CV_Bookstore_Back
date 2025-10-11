namespace BookStoreAPI.DTOs
{
    public record AuthUserResponceDto(int Id, string Email, string PhoneNumber, string FirstName, string LastName, string AccessToken);
}
