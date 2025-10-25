namespace BookStoreAPI.DTOs.Authentication
{
    public record GoogleLoginUserResponseDto(string accessToken, bool IsPhoneNumberProvided = false);

}
