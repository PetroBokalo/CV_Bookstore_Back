namespace BookStoreAPI.DTOs
{
    public record GoogleLoginUserResponseDto(string accessToken, bool IsPhoneNumberProvided = false);

}
