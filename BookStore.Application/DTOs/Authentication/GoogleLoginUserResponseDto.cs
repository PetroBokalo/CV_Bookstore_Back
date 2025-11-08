namespace BookStore.Application.DTOs.Authentication
{
    public record GoogleLoginUserResponseDto(string accessToken, bool IsPhoneNumberProvided, string Link);

}
