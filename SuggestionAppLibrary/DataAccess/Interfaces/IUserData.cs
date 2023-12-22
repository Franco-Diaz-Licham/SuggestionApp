namespace SuggestionAppLibrary.DataAccess.Interfaces;

public interface IUserData
{
    Task CreateUserAsync(UserModel user);
    Task<List<UserModel>> GetAllUsersAsync();
    Task<UserModel> GetUserAsync(string id);
    Task<UserModel> GetUserFromAuthenticationAsync(string objectId);
    Task UpdateUserAsync(UserModel user);
}