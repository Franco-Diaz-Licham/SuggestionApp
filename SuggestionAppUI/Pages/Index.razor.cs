namespace SuggestionAppUI.Pages;

public partial class Index
{
    // services
    [Inject] private ICategoryData _categoryData {  get; set; }
    [Inject] private IStatusData _statusData { get; set; }
    [Inject] private ISuggestionData _suggestionData { get; set; }
    [Inject] private IUserData _userData { get; set; }
    [Inject] private NavigationManager _navigate {  get; set; }
    [Inject] private ProtectedSessionStorage _sessionStorage { get; set; }
    [Inject] private AuthenticationStateProvider _auth { get; set; }

    // data variables
    private List<SuggestionModel> _suggestions { get; set; } = new();
    private List<CategoryModel> _categories { get; set; } = new();
    private List<StatusModel> _statuses { get; set; } = new();

    // filter variables
    private string _selectedCategory { get; set; } = "All";
    private string _selectedStatus { get; set; } = "All";
    private string _searchText { get; set; } = "";
    private bool _isSortedByNew { get; set; } = true;
    private UserModel _loggedInUser { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await GetDataAsync();
    }

    private async Task GetDataAsync()
    {
        _categories = await _categoryData.GetAllCategoriesAsync();
        _statuses = await _statusData.GetAllStatusesAsync();
        _categories = await _categoryData.GetAllCategoriesAsync();
        await LoadAndVerifyUser();
    }

    private async Task LoadAndVerifyUser()
    {
        var authState = await _auth.GetAuthenticationStateAsync();
        string objectId = authState.User.Claims.FirstOrDefault(x => x.Type.Contains("objectidentifier"))?.Value ?? "";

        if(string.IsNullOrEmpty(objectId) == false)
        {
            _loggedInUser = await _userData.GetUserFromAuthenticationAsync(objectId) ?? new();

            string firstName = authState.User.Claims.FirstOrDefault(x => x.Type.Contains("givenname"))?.Value;
            string lastName = authState.User.Claims.FirstOrDefault(x => x.Type.Contains("surname"))?.Value;
            string displayName = authState.User.Claims.FirstOrDefault(x => x.Type.Equals("name"))?.Value;
            string email = authState.User.Claims.FirstOrDefault(x => x.Type.Contains("email"))?.Value;

            bool isDirty = false;

            if(objectId.Equals(_loggedInUser.ObjectIdentifier) == false)
            {
                isDirty = true;

                _loggedInUser.ObjectIdentifier = objectId;
            }
            if (firstName.Equals(_loggedInUser.FirstName) == false)
            {
                isDirty = true;

                _loggedInUser.FirstName = firstName;
            }
            if (lastName.Equals(_loggedInUser.LastName) == false)
            {
                isDirty = true;

                _loggedInUser.LastName = lastName;
            }
            if (displayName.Equals(_loggedInUser.DisplayName) == false)
            {
                isDirty = true;

                _loggedInUser.DisplayName = displayName;
            }
            if (email.Equals(_loggedInUser.EmailAddress) == false)
            {
                isDirty = true;

                _loggedInUser.EmailAddress = email;
            }

            // update
            if (isDirty)
            {
                if(string.IsNullOrEmpty(_loggedInUser.Id))
                {
                    await _userData.CreateUserAsync(_loggedInUser);
                }
                else
                {
                    await _userData.UpdateUserAsync(_loggedInUser);
                }
            }
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if(firstRender == true)
        {
            await LoadFilterState();
            await FilterSuggestions();

            StateHasChanged();
        }
    }

    /// <summary>
    /// Method which filters the suggestions based on the set filters.
    /// </summary>
    /// <returns></returns>
    private async Task FilterSuggestions()
    {
        var output = await _suggestionData.GetAllApprovedSuggestionsAsync();

        if(_selectedCategory != "All")
        {
            output = output.Where(x => x.Category?.CategoryName == _selectedCategory).ToList();
        }

        if(_selectedStatus != "All")
        {
            output = output.Where(x => x.SuggestionStatus?.StatusName == _selectedStatus).ToList();
        }

        if(string.IsNullOrEmpty(_searchText) == false)
        {
            output = output.Where(x => 
                        x.Suggestion.Contains(_searchText, StringComparison.InvariantCultureIgnoreCase) ||
                        x.Description.Contains(_searchText, StringComparison.InvariantCultureIgnoreCase)
                        ).ToList();
        }

        if(_isSortedByNew == true)
        {
            output = output.OrderByDescending(x => x.DateCreated).ToList();
        }
        else
        {
            output = output.OrderByDescending(x => x.UserVotes.Count)
                            .ThenByDescending(x => x.DateCreated)
                            .ToList();
        }

        _suggestions = output;

        await SaveFilterState();
    }

    /// <summary>
    /// Method which saves the filter state of the user.
    /// </summary>
    /// <returns></returns>
    private async Task SaveFilterState()
    {
        await _sessionStorage.SetAsync(nameof(_selectedCategory), _selectedCategory);
        await _sessionStorage.SetAsync(nameof(_selectedStatus), _selectedStatus);
        await _sessionStorage.SetAsync(nameof(_searchText), _searchText);
        await _sessionStorage.SetAsync(nameof(_isSortedByNew), _isSortedByNew);
    }

    /// <summary>
    /// Method which gets the filter state of the user from session storage.
    /// </summary>
    /// <returns></returns>
    private async Task LoadFilterState()
    {
        var stringResults = await _sessionStorage.GetAsync<string>(nameof(_selectedCategory));
        _selectedCategory = stringResults.Success == true ? stringResults.Value! : "All";

        stringResults = await _sessionStorage.GetAsync<string>(nameof(_selectedStatus));
        _selectedStatus = stringResults.Success == true ? stringResults.Value! : "All";

        stringResults = await _sessionStorage.GetAsync<string>(nameof(_searchText));
        _searchText = stringResults.Success == true ? stringResults.Value! : "";

        var sortedResults = await _sessionStorage.GetAsync<bool>(nameof(_isSortedByNew));
        _isSortedByNew = stringResults.Success == true ? sortedResults.Value! : true;
    }

    /// <summary>
    /// Method which will handle the component event.
    /// </summary>
    /// <param name="isNew">bool</param>
    /// <returns>awaitable task</returns>
    private async Task OrderByNew(bool isNew)
    {
        _isSortedByNew = isNew;
        await FilterSuggestions();
    }

    private async Task OnSearchChanged(string text)
    {
        _searchText = text;
        await FilterSuggestions();
    }

    private async Task OnCategoryChanged(string category = "All")
    {
        _selectedCategory = category;
        await FilterSuggestions();
    }

    private async Task OnStatusChanged(string status = "All")
    {
        _selectedStatus = status;
        await FilterSuggestions();
    }

    private async Task VoteUp(SuggestionModel suggestion)
    {
        if(_loggedInUser is not null)
        {
            if(suggestion.Author.Id == _loggedInUser.Id)
            {
                // cannot vote on your own suggestion
                return;
            }
            
            // allow user to vote and remove vote
            if(suggestion.UserVotes.Add(_loggedInUser.Id) == false)
            {
                suggestion.UserVotes.Remove(_loggedInUser.Id);
            }

            await _suggestionData.UpvoteSuggestion(suggestion.Id, _loggedInUser.Id);

            if(_isSortedByNew == false)
            {
                _suggestions = _suggestions.OrderByDescending(x => x.UserVotes.Count())
                                           .ThenByDescending(x => x.DateCreated)
                                           .ToList();
            }
        }
        else
        {
            _navigate.NavigateTo("/MicrosoftIdentity/Account/SignIn", true);
        }
    }

    private string GetUpvoteTopText(SuggestionModel suggestion)
    {
        if(suggestion.UserVotes?.Count > 0)
        {
            return suggestion.UserVotes.Count.ToString("00");
        }
        else
        {
            if(suggestion.Author.Id == _loggedInUser?.Id)
            {
                return "Awaiting";
            }
            else
            {
                return "Click To";
            }
        }
    }

    private string GetUpBoteBottomText(SuggestionModel suggestion)
    {
        if(suggestion.UserVotes?.Count > 1)
        {
            return "Upvotes";
        }
        else
        {
            return "Upvote";
        }
    }

    private void OpenDetails(SuggestionModel suggestion)
    {
        _navigate.NavigateTo($"/details/{suggestion.Id}");
    }
}