using SuggestionAppUI.Helpers;

namespace SuggestionAppUI.Pages;

public partial class Create
{
    // services
    [Inject] private ICategoryData _categoryData { get; set; }
    [Inject] private ISuggestionData _suggestionData { get; set; }
    [Inject] private IUserData _userData { get; set; }
    [Inject] private NavigationManager _navigate { get; set; }
    [Inject] private AuthenticationStateProvider _auth {  get; set; }

    private CreateSuggestionModel _suggestion { get; set; } = new();
    private List<CategoryModel> _categories {  get; set; } = new();
    private UserModel _loggedInUser { get; set; }

    protected override async Task OnInitializedAsync()
    {
        _loggedInUser = await _auth.GetUserFromAuth(_userData);
        _categories = await _categoryData.GetAllCategoriesAsync();
    }

    private void ClosePage()
    {
        _navigate.NavigateTo("/");
    }

    private async Task CreateSuggestion()
    {
        SuggestionModel s = new()
        {
            Suggestion = _suggestion.Suggestion,
            Description = _suggestion.Description,
            Author = new BasicUserModel(_loggedInUser),
            Category = _categories.Where(x => x.Id == _suggestion.CategoryId).FirstOrDefault(),
        };

        if(s.Category == null)
        {
            _suggestion.CategoryId = "";
        }

        await _suggestionData.CreateSuggestionAsync(s);
        _suggestion = new();
    }
}