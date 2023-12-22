namespace SuggestionAppLibrary.DataAccess.Interfaces;

public interface IStatusData
{
    Task CreateStatusAsync(StatusModel status);
    Task<List<StatusModel>> GetAllStatusesAsync();
}