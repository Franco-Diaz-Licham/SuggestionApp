namespace SuggestionAppLibrary.DataAccess.Classes;

public class MongoCategoryData : ICategoryData
{
    private IMemoryCache _cache;
    private IMongoCollection<CategoryModel> _categories;
    private const string CacheName = "CategoryData";

    public MongoCategoryData(
        IDbConnection db,
        IMemoryCache cache)
    {
        _cache = cache;
        _categories = db.CategoryCollection;
    }

    /// <summary>
    /// Method which gets all categories and sets the cache for categories.
    /// </summary>
    /// <returns></returns>
    public async Task<List<CategoryModel>> GetAllCategoriesAsync()
    {
        var output = _cache.Get<List<CategoryModel>>(CacheName);

        if (output == null)
        {
            var results = await _categories.FindAsync(_ => true);
            output = results.ToList();

            _cache.Set(CacheName, output, TimeSpan.FromDays(1));
        }

        return output;
    }

    /// <summary>
    /// Method which inserts a category into the database.
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    public Task CreateCategoryAsync(CategoryModel category)
    {
        return _categories.InsertOneAsync(category);
    }
}
