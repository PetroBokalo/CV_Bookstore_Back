namespace BookStoreAPI.DTOs
{
    public record RegisterUserResponceDto(int Id, string Email, string PhoneNumber, string FirstName, string LastName, string AccessToken);
}
