namespace SuggestionAppLibrary.DataAccess.Classes;

public class MongoUserData : IUserData
{
    private readonly IMongoCollection<UserModel> _users;

    public MongoUserData(IDbConnection db)
    {
        _users = db.UserCollection;
    }

    /// <summary>
    /// Method that gets all users from db.
    /// </summary>
    /// <returns></returns>
    public async Task<List<UserModel>> GetAllUsersAsync()
    {
        var result = await _users.FindAsync(_ => true);

        return result.ToList();
    }

    /// <summary>
    /// Method which find user based on an id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<UserModel> GetUserAsync(string id)
    {
        var result = await _users.FindAsync(x => x.Id == id);

        return result.FirstOrDefault();
    }


    /// <summary>
    /// Method which gets a user based on the object id.
    /// </summary>
    /// <param name="objectId">Azure B2D object identifier</param>
    /// <returns>UserModel</returns>
    public async Task<UserModel> GetUserFromAuthenticationAsync(string objectId)
    {
        var result = await _users.FindAsync(x => x.ObjectIdentifier == objectId);

        return result.FirstOrDefault();
    }

    /// <summary>
    /// Method which creates a user
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public Task CreateUserAsync(UserModel user)
    {
        return _users.InsertOneAsync(user);
    }

    /// <summary>
    /// Method which updates a user if found, otherwise it will create a user.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public Task UpdateUserAsync(UserModel user)
    {
        var filter = Builders<UserModel>.Filter.Eq("Id", user.Id);

        // update if found, otherwise create entry
        return _users.ReplaceOneAsync(filter, user, options: new ReplaceOptions { IsUpsert = true });
    }
}
