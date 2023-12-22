namespace SuggestionAppLibrary.Models;

public class BasicSuggestionMOdel
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Title { get; set; }
}
