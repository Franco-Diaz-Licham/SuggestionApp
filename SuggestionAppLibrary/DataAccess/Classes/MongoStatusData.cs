namespace SuggestionAppLibrary.DataAccess.Classes;

public class MongoStatusData : IStatusData
{
    private readonly IMongoCollection<StatusModel> _statuses;
    private readonly IMemoryCache _cache;
    private readonly string CacheName = "StatusData";

    public MongoStatusData(
        IDbConnection db,
        IMemoryCache cache)
    {
        _cache = cache;
        _statuses = db.StatusCollection;
    }

    /// <summary>
    /// Method which gets all statuses from database and caches the response.
    /// </summary>
    /// <returns></returns>
    public async Task<List<StatusModel>> GetAllStatusesAsync()
    {
        var output = _cache.Get<List<StatusModel>>(CacheName);

        if (output == null)
        {
            var result = await _statuses.FindAsync(_ => true);
            output = result.ToList();

            _cache.Set(CacheName, output, TimeSpan.FromDays(1));
        }

        return output;
    }

    /// <summary>
    /// Method which insert a status into the collection.
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    public Task CreateStatusAsync(StatusModel status)
    {
        return _statuses.InsertOneAsync(status);
    }
}
