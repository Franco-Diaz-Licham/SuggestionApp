namespace SuggestionAppLibrary.DataAccess.Interfaces;

public interface ICategoryData
{
    Task CreateCategory(CategoryModel category);
    Task<List<CategoryModel>> GetAllCategoriesAsync();
}