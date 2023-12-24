namespace SuggestionAppLibrary.DataAccess.Interfaces;

public interface ISuggestionData
{
    Task CreateSuggestionAsync(SuggestionModel suggestion);
    Task<List<SuggestionModel>> GetAllApprovedSuggestionsAsync();
    Task<List<SuggestionModel>> GetAllSuggestionsAsync();
    Task<List<SuggestionModel>> GetAllSuggestionsWaitingForApproval();
    Task<SuggestionModel> GetSuggestion(string id);
    Task<List<SuggestionModel>> GetUsersSuggestionsAsync(string userId);
    Task UpdateSuggestion(SuggestionModel suggestion);
    Task UpvoteSuggestion(string suggestionId, string userId);
}