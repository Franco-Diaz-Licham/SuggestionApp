namespace SuggestionAppLibrary.Models;

public class BasicSuggestionModel
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Title { get; set; }

    public BasicSuggestionModel() { }

    public BasicSuggestionModel(BasicSuggestionModel suggestion)
    {
        Id = suggestion.Id;
        Title = suggestion.Title;
    }
}
