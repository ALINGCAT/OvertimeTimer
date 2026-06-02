using System.ComponentModel;

namespace OvertimeTimer.App.Services;

public interface IStatusMessageService : INotifyPropertyChanged
{
    string Message { get; }

    void Show(string message);
}
