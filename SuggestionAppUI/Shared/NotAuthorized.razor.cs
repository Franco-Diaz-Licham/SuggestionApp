namespace SuggestionAppUI.Shared
{
    public partial class NotAuthorized
    {
        [Inject] private NavigationManager _navigate {  get; set; }
        
        private void ClosePage()
        {
            _navigate.NavigateTo("/");
        }

    }
}