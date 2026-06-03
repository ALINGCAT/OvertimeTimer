using Prism.Commands;

namespace OvertimeTimer.App.ViewModels;

public sealed class AppearanceColorItemViewModel : ViewModelBase
{
    private string _colorText;
    private readonly Action<AppearanceColorItemViewModel> _requestColorSelection;
    private readonly Action<AppearanceColorItemViewModel, string> _colorTextChanged;

    public AppearanceColorItemViewModel(
        string label,
        string colorText,
        Action<AppearanceColorItemViewModel> requestColorSelection,
        Action<AppearanceColorItemViewModel, string> colorTextChanged)
    {
        Label = label;
        _colorText = colorText;
        _requestColorSelection = requestColorSelection;
        _colorTextChanged = colorTextChanged;
        SelectColorCommand = new DelegateCommand(SelectColor);
    }

    public string Label { get; }

    public string ColorText
    {
        get => _colorText;
        set
        {
            if (SetProperty(ref _colorText, value))
            {
                _colorTextChanged(this, value);
            }
        }
    }

    public DelegateCommand SelectColorCommand { get; }

    private void SelectColor()
    {
        _requestColorSelection(this);
    }
}
