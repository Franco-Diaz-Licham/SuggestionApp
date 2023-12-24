namespace SuggestionAppUI.Pages;

public partial class Details
{
    // parameters
    [Parameter] public string? id { get; set; }

    // services
    [Inject] private ISuggestionData _suggestionData {  get; set; }
    [Inject] private NavigationManager _navigate {  get; set; }

    // data variables
    private SuggestionModel _suggestion { get; set; }

    protected override async Task OnInitializedAsync()
    {
        _suggestion = await _suggestionData.GetSuggestion(id);
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
            return "Click To";
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
}