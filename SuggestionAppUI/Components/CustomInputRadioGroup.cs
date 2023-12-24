namespace SuggestionAppUI.Components;

public class CustomInputRadioGroup<T> : InputRadioGroup<T>
{
    public string _name { get; set; }
    public string _fieldClass { get; set; }

    protected override void OnParametersSet()
    {
        var fieldClass  = EditContext.FieldCssClass(FieldIdentifier) ?? string.Empty;

        if(fieldClass != _fieldClass || Name != _name)
        {
            _fieldClass = fieldClass;
            _name = Name;

            base.OnParametersSet();
        }
    }
}
