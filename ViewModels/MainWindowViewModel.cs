using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TimerTool.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private string _countdownText = "00:00:00";
    private string _statusText = "点击设置开始倒计时";
    private bool _isRunning = false;

    public string CountdownText
    {
        get => _countdownText;
        set
        {
            _countdownText = value;
            OnPropertyChanged();
        }
    }

    public string StatusText
    {
        get => _statusText;
        set
        {
            _statusText = value;
            OnPropertyChanged();
        }
    }

    public bool IsRunning
    {
        get => _isRunning;
        set
        {
            _isRunning = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
