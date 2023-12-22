namespace SuggestionAppLibrary.DataAccess.Classes;

public class DbConnection : IDbConnection
{
    private IConfiguration _config;
    private IMongoDatabase _db;
    private string _connectionId = "MongoDB";
    public string DbName { get; private set; }

    // set collection names to be able to request data
    public string CategoryCollectionName { get; private set; } = "categories";
    public string StatusConnectionName { get; private set; } = "statuses";
    public string UserCollectionName { get; private set; } = "users";
    public string SuggestionCollectionName { get; private set; } = "suggestions";

    // variables to load data from mongo
    public MongoClient Client { get; private set; }
    public IMongoCollection<CategoryModel> CategoryCollection { get; private set; }
    public IMongoCollection<StatusModel> StatusCollection { get; private set; }
    public IMongoCollection<UserModel> UserCollection { get; set; }
    public IMongoCollection<SuggestionModel> SuggestionCollection { get; set; }

    public DbConnection(IConfiguration config)
    {
        _config = config;

        // configure client
        Client = new MongoClient(_config.GetConnectionString(_connectionId));
        DbName = _config["DatabaseName"];
        _db = Client.GetDatabase(DbName);

        // get collections
        CategoryCollection = _db.GetCollection<CategoryModel>(CategoryCollectionName);
        StatusCollection = _db.GetCollection<StatusModel>(StatusConnectionName);
        UserCollection = _db.GetCollection<UserModel>(UserCollectionName);
        SuggestionCollection = _db.GetCollection<SuggestionModel>(SuggestionCollectionName);
    }
}
