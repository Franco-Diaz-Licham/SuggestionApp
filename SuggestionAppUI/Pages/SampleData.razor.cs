namespace SuggestionAppUI.Pages;

public partial class SampleData
{
    // services
    [Inject] private ICategoryData _categoryData { get; set; }
    [Inject] private IStatusData _statusData { get; set; }
    [Inject] private IUserData _userData { get; set; }
    [Inject] private ISuggestionData _suggestionData { get; set; }

    // ui variables
    private bool categoriesCreated = false;
    private bool statusesCreated = false;

    private async Task CreateCategories()
    {
        var categories = await _categoryData.GetAllCategoriesAsync();

        if(categories?.Count > 0)
        {
            return;
        }

        CategoryModel category = new()
        {
            CategoryName = "Courses",
            CategoryDescription = "Full paid courses."
        };

        await _categoryData.CreateCategoryAsync(category);

        category = new()
        {
            CategoryName = "Dev Questions",
            CategoryDescription = "Advice on being a developer."
        };

        await _categoryData.CreateCategoryAsync(category);

        category = new()
        {
            CategoryName = "In-Depth Tutorial",
            CategoryDescription = "A deep-dive video on how to user a topic."
        };

        await _categoryData.CreateCategoryAsync(category);

        category = new()
        {
            CategoryName = "10-Minute Training",
            CategoryDescription = "A quick \"How do I use this?\" video."
        };

        await _categoryData.CreateCategoryAsync(category);

        category = new()
        {
            CategoryName = "Other",
            CategoryDescription = "Not sure which category this fits in."
        };

        await _categoryData.CreateCategoryAsync(category);

        categoriesCreated = true;
    }

    private async Task CreateStatuses()
    {
        var status = await _statusData.GetAllStatusesAsync();

        if(status?.Count > 0)
        {
            return;
        }

        StatusModel stat = new()
        {
            StatusName = "Completed",
            StatusDescription = "The suggestion was accepted and the corresponding item was created."
        };

        await _statusData.CreateStatusAsync(stat);

        stat = new()
        {
            StatusName = "Watching",
            StatusDescription = "The suggestion is interesting. We are watching to see how much interest there is in it."
        };

        await _statusData.CreateStatusAsync(stat);

        stat = new()
        {
            StatusName = "Upcoming",
            StatusDescription = "The suggestion was accepted and it will be released soon."
        };

        await _statusData.CreateStatusAsync(stat);

        stat = new()
        {
            StatusName = "Dismissed",
            StatusDescription = "The suggestion was not something that wer are going to undertake."
        };

        await _statusData.CreateStatusAsync(stat);

        statusesCreated = true;
    }

    private async Task GenerateSampleData()
    {
        UserModel user = new()
        {
            FirstName = "Tim",
            LastName = "Corey,",
            EmailAddress = "tim@test.com",
            DisplayName = "Name",
            ObjectIdentifier = "1233"
        };

        await _userData.CreateUserAsync(user);

        // get data
        var foundUser = await _userData.GetUserFromAuthenticationAsync("1233");
        var categories = await _categoryData.GetAllCategoriesAsync();
        var statuses = await _statusData.GetAllStatusesAsync();

        SuggestionModel suggestion = new()
        {
            Author = new BasicUserModel(foundUser),
            Category = categories[1],
            Suggestion = "Our First Suggestion",
            Description = "This is a test suggestion"
        };

        await _suggestionData.CreateSuggestionAsync(suggestion);

        suggestion = new()
        {
            Author = new BasicUserModel(foundUser),
            Category = categories[0],
            Suggestion = "Our Second Suggestion",
            Description = "This is a test suggestion",
            SuggestionStatus = statuses[1],
            OwnerNotes = "This is the notes for the status"
        };

        await _suggestionData.CreateSuggestionAsync(suggestion);

        HashSet<string> votes = new() { "1", "2", "3"};

        suggestion = new()
        {
            Author = new BasicUserModel(foundUser),
            Category = categories[2],
            Suggestion = "Our fourth Suggestion",
            Description = "This is a test suggestion",
            SuggestionStatus = statuses[2],
            UserVotes = votes,
            OwnerNotes = "This is the notes for the status"
        };

        await _suggestionData.CreateSuggestionAsync(suggestion);

        votes.Add("4");

        suggestion = new()
        {
            Author = new BasicUserModel(foundUser),
            Category = categories[0],
            Suggestion = "Our fifth Suggestion",
            Description = "This is a test suggestion",
            SuggestionStatus = statuses[3],
            UserVotes= votes,
            OwnerNotes = "This is the notes for the status"
        };

        await _suggestionData.CreateSuggestionAsync(suggestion);
    }
}