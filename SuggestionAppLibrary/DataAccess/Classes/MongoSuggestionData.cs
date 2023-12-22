namespace SuggestionAppLibrary.DataAccess.Classes;

public class MongoSuggestionData : ISuggestionData
{
    private readonly IDbConnection _db;
    private readonly IUserData _userData;
    private readonly IMemoryCache _cache;
    private readonly IMongoCollection<SuggestionModel> _suggestions;
    private const string CacheName = "SuggestionData";

    public MongoSuggestionData(
        IDbConnection db,
        IUserData userData,
        IMemoryCache cache)
    {
        _cache = cache;
        _db = db;
        _userData = userData;
        _suggestions = db.SuggestionCollection;
    }

    /// <summary>
    /// Method which gets all suggestions that are not archived from the database.
    /// </summary>
    /// <returns></returns>
    public async Task<List<SuggestionModel>> GetAllSuggestionsAsync()
    {
        var output = _cache.Get<List<SuggestionModel>>(CacheName);

        if (output == null)
        {
            var result = await _suggestions.FindAsync(x => x.Archived == false);

            output = result.ToList();

            _cache.Set(CacheName, output, TimeSpan.FromMinutes(1));
        }

        return output;
    }

    /// <summary>
    /// Method which gets all approved suggestions and filters through all that have been
    /// approved for release.
    /// </summary>
    /// <returns></returns>
    public async Task<List<SuggestionModel>> GetAllApprovedSuggestionsAsync()
    {
        var output = await GetAllSuggestionsAsync();

        return output.Where(x => x.ApprovedForRelease).ToList();
    }

    /// <summary>
    /// Method which gets a single suggestion.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<SuggestionModel> GetSuggestion(string id)
    {
        var result = await _suggestions.FindAsync(x => x.Id == id);

        return result.FirstOrDefault();
    }


    /// <summary>
    /// Method which gets all suggestions that are waiting to be approved.
    /// </summary>
    /// <returns></returns>
    public async Task<List<SuggestionModel>> GetAllSuggestionsWaitingForApproval()
    {
        var output = await GetAllSuggestionsAsync();
        var result = output.Where(x => x.ApprovedForRelease == false && x.Rejected == false).ToList();

        return result;
    }

    /// <summary>
    /// Method which updates a suggestion.
    /// </summary>
    /// <param name="suggestion"></param>
    /// <returns></returns>
    public async Task UpdateSuggestion(SuggestionModel suggestion)
    {
        await _suggestions.ReplaceOneAsync(x => x.Id == suggestion.Id, suggestion);
        _cache.Remove(CacheName);
    }

    /// <summary>
    /// Method which updates the suggestion upvote information and related user information.
    /// Handles multiple steps at a time before committing transactions.
    /// </summary>
    /// <param name="suggestionId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task UpvoteSuggestion(string suggestionId, string userId)
    {
        // create session so multiple steps can be completed
        var client = _db.Client;
        using var session = await client.StartSessionAsync();
        session.StartTransaction();

        try
        {
            // find suggestion of interest
            var db = client.GetDatabase(_db.DbName);
            var suggestionsInTraction = db.GetCollection<SuggestionModel>(_db.SuggestionCollectionName);
            var suggestion = (await suggestionsInTraction.FindAsync(x => x.Id == suggestionId)).First();

            // attempt to add user who voted to the suggestion list of users
            bool isUpvote = suggestion.UserVotes.Add(userId);

            if (isUpvote == false)
            {
                suggestion.UserVotes.Remove(userId);
            }

            // complete update suggestion with the new one
            await suggestionsInTraction.ReplaceOneAsync(x => x.Id == suggestionId, suggestion);

            // find user details who upvoted
            var usersInTransaction = db.GetCollection<UserModel>(_db.UserCollectionName);
            var user = await _userData.GetUserAsync(suggestion.Author.Id);

            // add upvoted suggestion to list of voted suggestion of user.
            if (isUpvote == true)
            {
                user.VotedOnSuggestions.Add(new BasicSuggestionModel(suggestion));
            }
            else
            {
                var suggestionToRemove = user.VotedOnSuggestions.Where(x => x.Id == suggestionId).First();
                user.VotedOnSuggestions.Remove(suggestionToRemove);
            }

            await usersInTransaction.ReplaceOneAsync(x => x.Id == userId, user);

            await session.CommitTransactionAsync();

            _cache.Remove(CacheName);
        }
        catch (Exception ex)
        {
            await session.AbortTransactionAsync();
            throw;
        }
    }

    /// <summary>
    /// Method which creates a suggestion and updates related information for user.
    /// Handles multiple steps at a time before committing transactions.
    /// </summary>
    /// <param name="suggestion"></param>
    /// <returns></returns>
    public async Task CreateSuggestionAsync(SuggestionModel suggestion)
    {
        // create session
        var client = _db.Client;
        using var session = await client.StartSessionAsync();
        session.StartTransaction();

        try
        {
            // insert new suggestion into collection
            var db = client.GetDatabase(_db.DbName);
            var suggestionsInTransaction = db.GetCollection<SuggestionModel>(_db.SuggestionCollectionName);
            await suggestionsInTransaction.InsertOneAsync(suggestion);

            // find user who authored the suggestion
            var usersInTransaction = db.GetCollection<UserModel>(_db.UserCollectionName);
            var user = await _userData.GetUserAsync(suggestion.Author.Id);

            // update authored list of suggestions for user.
            user.AuthoredSuggestions.Add(new BasicSuggestionModel(suggestion));
            await usersInTransaction.ReplaceOneAsync(x => x.Id == user.Id, user);

            await session.CommitTransactionAsync();
        }
        catch (Exception ex)
        {
            await session.AbortTransactionAsync();
            throw;
        }
    }
}
