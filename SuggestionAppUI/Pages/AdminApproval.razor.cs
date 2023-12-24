namespace SuggestionAppUI.Pages;

public partial class AdminApproval
{
    // services
    [Inject] private NavigationManager _navigate { get; set; }
    [Inject] private ISuggestionData _suggestionData { get; set; }
    [Inject] private IUserData _userData { get; set; }

    // data
    private string _editedTitle { get; set; } = string.Empty;
    private string _editedDescription { get; set; } = string.Empty;
    private string _currentEditingTitle { get; set; } = string.Empty;
    private string _currentEditingDescription {  get; set; } = string.Empty;
    private SuggestionModel _editingModel { get; set; }
    private List<SuggestionModel> _submissions { get; set; }

    protected override async Task OnInitializedAsync()
    {
        _submissions = await _suggestionData.GetAllSuggestionsWaitingForApproval();
    }

    private async Task ApprovedSubmission(SuggestionModel submission)
    {
        submission.ApprovedForRelease = true;
        _submissions.Remove(submission);
        await _suggestionData.UpdateSuggestion(submission);
    }

    private async Task RejectSubmission(SuggestionModel submission)
    {
        submission.Rejected = true;
        _submissions.Remove(submission);
        await _suggestionData.UpdateSuggestion(submission);
    }

    private void EditTitle(SuggestionModel model)
    {
        _editingModel = model;
        _editedTitle = model.Suggestion;
        _currentEditingTitle = model.Id;
        _currentEditingDescription = "";
    }

    private async Task SaveTitle(SuggestionModel model)
    {
        _currentEditingTitle = string.Empty;
        model.Suggestion = _editedTitle;

        await _suggestionData.UpdateSuggestion(model);
    }

    private void ClosePage()
    {
        _navigate.NavigateTo("/");
    }

    private void EditDescription(SuggestionModel model)
    {
        _editingModel = model;
        _editedDescription = model.Description;
        _currentEditingTitle = string.Empty;
        _currentEditingDescription = model.Id;
    }

    private async Task SaveDescription(SuggestionModel model)
    {
        _currentEditingDescription = string.Empty;
        model.Description = _editedDescription;
        await _suggestionData.UpdateSuggestion(model);
    }
}