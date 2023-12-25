namespace SuggestionAppUI.Helpers;

public static class AuthenticationStateProviderHelpers
{
    public static async Task<UserModel> GetUserFromAuth(
            this AuthenticationStateProvider provider,
            IUserData userData)
    {
        var authState = await provider.GetAuthenticationStateAsync();

        string objectId = authState.User.Claims.FirstOrDefault(x => x.Type.Contains("objectidentifier"))?.Value ?? "";

        var loggedUser = await userData.GetUserFromAuthenticationAsync(objectId);

        return loggedUser;
    }
}
