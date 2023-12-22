namespace SuggestionAppLibrary.DataAccess.Interfaces;

public interface IDbConnection
{
    IMongoCollection<CategoryModel> CategoryCollection { get; }
    string CategoryCollectionName { get; }
    MongoClient Client { get; }
    string DbName { get; }
    IMongoCollection<StatusModel> StatusCollection { get; }
    string StatusConnectionName { get; }
    IMongoCollection<SuggestionModel> SuggestionCollection { get; set; }
    string SuggestionCollectionName { get; }
    IMongoCollection<UserModel> UserCollection { get; set; }
    string UserCollectionName { get; }
}