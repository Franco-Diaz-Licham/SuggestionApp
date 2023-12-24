namespace SuggestionAppUI.Pages;

public partial class Profile
{
    // services
    [Inject] private ISuggestionData _suggestionData { get; set; }
    [Inject] private IUserData _userData { get; set; }
    [Inject] private NavigationManager _navigate { get; set; }

    //
    private UserModel _loggedInUser { get; set; }
    private List<SuggestionModel> _submissions { get; set; }
    private List<SuggestionModel> _approved { get; set; }
    private List<SuggestionModel> _archived { get; set; }
    private List<SuggestionModel> _pending { get; set; }
    private List<SuggestionModel> _rejected { get; set; }

    protected override async Task OnInitializedAsync()
    {
        _loggedInUser = await _userData.GetUserFromAuthenticationAsync("1233");

        var result = await _suggestionData.GetUsersSuggestionsAsync(_loggedInUser.Id);

        if(_loggedInUser is not null && result is not null)
        {
            _submissions = result.OrderByDescending(x => x.DateCreated).ToList();

            _approved = _submissions.Where(x => x.ApprovedForRelease && 
                                                x.Archived == false & 
                                                x.Rejected == false
                                                ).ToList();

            _archived = _submissions.Where(x => x.Archived && 
                                                x.Rejected == false
                                                ).ToList();

            _pending = _submissions.Where(x => x.ApprovedForRelease == false && 
                                                x.Rejected == false
                                                ).ToList();

            _rejected = _submissions.Where(x => x.Rejected).ToList();
        }
    }

    private void ClosePage()
    {
        _navigate.NavigateTo("/");
    }
}