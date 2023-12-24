namespace SuggestionAppLibrary.DataAccess.Interfaces;

public interface ICategoryData
{
    Task CreateCategoryAsync(CategoryModel category);
    Task<List<CategoryModel>> GetAllCategoriesAsync();
}