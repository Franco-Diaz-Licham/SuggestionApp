namespace SuggestionAppUI.Pages;

public partial class Details
{
    // parameters
    [Parameter] public string? id { get; set; }

    // services
    [Inject] private ISuggestionData _suggestionData {  get; set; }
    [Inject] private IUserData _userData { get; set; }
    [Inject] private NavigationManager _navigate {  get; set; }
    [Inject] private AuthenticationStateProvider _auth { get; set; }

    // data variables
    private SuggestionModel _suggestion { get; set; }
    private UserModel _loggedInUser { get; set; }

    protected override async Task OnInitializedAsync()
    {
        _suggestion = await _suggestionData.GetSuggestion(id);
        _loggedInUser = await _auth.GetUserFromAuth(_userData);
    }

    private void ClosePage()
    {
        _navigate.NavigateTo("/");
    }

    private string GetUpvoteTopText()
    {
        if (_suggestion.UserVotes?.Count > 0)
        {
            return _suggestion.UserVotes.Count.ToString("00");
        }
        else
        {
            if (_suggestion.Author.Id == _loggedInUser?.Id)
            {
                return "Awaiting";
            }
            else
            {
                return "Click To";
            }
        }
    }

    private string GetUpBoteBottomText()
    {
        if (_suggestion.UserVotes?.Count > 1)
        {
            return "Upvotes";
        }
        else
        {
            return "Upvote";
        }
    }

    private async Task VoteUp()
    {
        if (_loggedInUser is not null)
        {
            if (_suggestion.Author.Id == _loggedInUser.Id)
            {
                // cannot vote on your own suggestion
                return;
            }

            // allow user to vote and remove vote
            if (_suggestion.UserVotes.Add(_loggedInUser.Id) == false)
            {
                _suggestion.UserVotes.Remove(_loggedInUser.Id);
            }

            await _suggestionData.UpvoteSuggestion(_suggestion.Id, _loggedInUser.Id);
        }
        else
        {
            _navigate.NavigateTo("/MicrosoftIdentity/Account/SignIn", true);
        }
    }
}