using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TimerTool.Services;
using TimerTool.ViewModels;
using TimerTool.Windows;

namespace TimerTool;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly MainWindowViewModel _viewModel;
    private readonly UpdateService _updateService;
    private readonly SettingsService _settingsService;
    private readonly SoundService _soundService;
    private readonly ILogger<MainWindow> _logger;
    private readonly DispatcherTimer _countdownTimer;
    private readonly DispatcherTimer _updateCheckTimer;
    private TimeSpan _remainingTime;
    private TimeSpan _totalTime;
    private bool _isRunning;
    private bool _isExpanded;

    public MainWindow(MainWindowViewModel viewModel, UpdateService updateService, 
        SettingsService settingsService, SoundService soundService, ILogger<MainWindow> logger)    {
        InitializeComponent();
        
        _viewModel = viewModel;
        _updateService = updateService;
        _settingsService = settingsService;
        _soundService = soundService;
        _logger = logger;
        
        DataContext = _viewModel;
        
        // 初始化计时器
        _countdownTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _countdownTimer.Tick += CountdownTimer_Tick;
        
        // 初始化更新检查计时器（每小时检查一次）
        _updateCheckTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromHours(1)
        };
        _updateCheckTimer.Tick += UpdateCheckTimer_Tick;
        _updateCheckTimer.Start();
        
        // 加载设置
        LoadSettings();
        
        // 启动时检查更新
        _ = CheckForUpdatesAsync();
        
        _logger.LogInformation("应用程序启动完成");
    }

    private void LoadSettings()
    {
        var settings = _settingsService.LoadSettings();
        
        // 设置窗口位置
        if (settings.WindowPosition.HasValue)
        {
            Left = settings.WindowPosition.Value.X;
            Top = settings.WindowPosition.Value.Y;
        }
        else
        {
            // 默认位置在屏幕右上角
            Left = SystemParameters.PrimaryScreenWidth - Width - 50;
            Top = 50;
        }
        
        // 设置默认倒计时时间
        _totalTime = settings.DefaultDuration;
        _remainingTime = _totalTime;
        UpdateCountdownDisplay();
    }

    private void SaveSettings()
    {
        var settings = _settingsService.LoadSettings();
        settings.WindowPosition = new System.Drawing.Point((int)Left, (int)Top);
        _settingsService.SaveSettings(settings);
    }

    private void Window_MouseEnter(object sender, MouseEventArgs e)
    {
        if (!_isExpanded)
        {
            ShowExpandedPanel();
        }
    }    private void Window_MouseLeave(object sender, MouseEventArgs e)
    {
        // 使用延迟检查，确保鼠标真的离开了窗口区域
        if (_isExpanded)
        {
            Task.Delay(200).ContinueWith(_ =>
            {
                Dispatcher.Invoke(() =>
                {
                    if (_isExpanded && !IsMouseOverWindow())
                    {
                        ShowCompactButton();
                    }
                });
            });
        }
    }

    private bool IsMouseOverWindow()
    {
        try
        {
            // 获取鼠标相对于窗口的位置
            var mousePosition = Mouse.GetPosition(this);
            
            // 检查鼠标是否在窗口边界内（加一点容差）
            var tolerance = 5; // 5像素的容差
            return mousePosition.X >= -tolerance && 
                   mousePosition.X <= ActualWidth + tolerance &&
                   mousePosition.Y >= -tolerance && 
                   mousePosition.Y <= ActualHeight + tolerance;
        }
        catch
        {
            return false;
        }
    }

    private void ShowExpandedPanel()
    {
        _isExpanded = true;
        CompactButton.Visibility = Visibility.Collapsed;
        ExpandedPanel.Visibility = Visibility.Visible;
        Width = 300;
        Height = 150;
    }

    private void ShowCompactButton()
    {
        _isExpanded = false;
        ExpandedPanel.Visibility = Visibility.Collapsed;
        CompactButton.Visibility = Visibility.Visible;
        Width = 120;
        Height = 120;
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (!_isExpanded)
        {
            ShowExpandedPanel();
        }
        else
        {
            DragMove();
        }
    }

    private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        var contextMenu = new System.Windows.Controls.ContextMenu();
        
        var settingsMenuItem = new System.Windows.Controls.MenuItem { Header = "设置" };
        settingsMenuItem.Click += SettingsButton_Click;
        contextMenu.Items.Add(settingsMenuItem);
          var exitMenuItem = new System.Windows.Controls.MenuItem { Header = "退出程序" };
        exitMenuItem.Click += (s, args) => {
            var result = MessageBox.Show(
                "确定要退出PPT倒计时工具吗？",
                "确认退出",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        };
        contextMenu.Items.Add(exitMenuItem);
        
        contextMenu.IsOpen = true;
    }

    private void StartPauseButton_Click(object sender, RoutedEventArgs e)
    {
        if (_isRunning)
        {
            PauseCountdown();
        }
        else
        {
            StartCountdown();
        }
    }

    private void ResetButton_Click(object sender, RoutedEventArgs e)
    {
        ResetCountdown();
    }

    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        var settingsWindow = new SettingsWindow(_settingsService);
        if (settingsWindow.ShowDialog() == true)
        {
            LoadSettings();
        }
    }    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        // 确认是否要退出程序
        var result = MessageBox.Show(
            "确定要退出PPT倒计时工具吗？",
            "确认退出",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            Application.Current.Shutdown();
        }
    }

    private void StartCountdown()
    {
        if (_remainingTime.TotalSeconds <= 0)
        {
            _remainingTime = _totalTime;
        }
        
        _isRunning = true;
        _countdownTimer.Start();
        StartPauseButton.Content = "暂停";
        StatusText.Text = "倒计时进行中...";
        
        _logger.LogInformation($"开始倒计时: {_remainingTime}");
    }

    private void PauseCountdown()
    {
        _isRunning = false;
        _countdownTimer.Stop();
        StartPauseButton.Content = "继续";
        StatusText.Text = "倒计时已暂停";
        
        _logger.LogInformation("倒计时已暂停");
    }

    private void ResetCountdown()
    {
        _isRunning = false;
        _countdownTimer.Stop();
        _remainingTime = _totalTime;
        StartPauseButton.Content = "开始";
        StatusText.Text = "点击开始倒计时";
        UpdateCountdownDisplay();
        
        _logger.LogInformation("倒计时已重置");
    }

    private void CountdownTimer_Tick(object? sender, EventArgs e)
    {
        _remainingTime = _remainingTime.Subtract(TimeSpan.FromSeconds(1));
        UpdateCountdownDisplay();
        
        // 检查提醒时间点
        CheckReminders();
        
        if (_remainingTime.TotalSeconds <= 0)
        {
            // 倒计时结束
            CountdownFinished();
        }
    }

    private void UpdateCountdownDisplay()
    {
        var timeString = _remainingTime.ToString(@"hh\:mm\:ss");
        CountdownText.Text = timeString;
        
        // 根据剩余时间改变颜色
        if (_remainingTime.TotalMinutes <= 1)
        {
            CountdownText.Foreground = System.Windows.Media.Brushes.Red;
        }
        else if (_remainingTime.TotalMinutes <= 5)
        {
            CountdownText.Foreground = System.Windows.Media.Brushes.Orange;
        }
        else
        {
            CountdownText.Foreground = System.Windows.Media.Brushes.White;
        }
    }    private void CheckReminders()
    {
        var settings = _settingsService.LoadSettings();
        var totalMinutes = _totalTime.TotalMinutes;
        var remainingMinutes = _remainingTime.TotalMinutes;
        
        foreach (var reminder in settings.ReminderPoints)
        {
            // 只有当提醒时间点小于总时长时才检查
            if (reminder < totalMinutes && Math.Abs(remainingMinutes - reminder) < 0.5) // 30秒误差范围
            {
                ShowReminder($"提醒：还剩 {reminder} 分钟");
                break;
            }
        }
    }private void ShowReminder(string message)
    {
        if (_isExpanded)
        {
            StatusText.Text = message;
        }
        
        // 播放声音通知
        var settings = _settingsService.LoadSettings();
        if (settings.EnableSoundNotification)
        {
            _soundService.PlayNotificationSound();
        }
        
        // 显示自定义通知窗口
        var displayDuration = settings.NotificationDuration / 1000;
        NotificationWindow.ShowNotification(message, displayDuration);
        
        _logger.LogInformation($"显示提醒: {message}");
    }    private void CountdownFinished()
    {
        _isRunning = false;
        _countdownTimer.Stop();
        StartPauseButton.Content = "开始";
        StatusText.Text = "倒计时结束！";
        CountdownText.Text = "00:00:00";
        CountdownText.Foreground = System.Windows.Media.Brushes.Red;
        
        // 播放结束声音
        var settings = _settingsService.LoadSettings();
        if (settings.EnableSoundNotification)
        {
            _soundService.PlayFinishSound();
        }
        
        ShowReminder("时间到！PPT汇报时间结束");
        
        _logger.LogInformation("倒计时结束");
    }

    private async void UpdateCheckTimer_Tick(object? sender, EventArgs e)
    {
        await CheckForUpdatesAsync();
    }

    private async Task CheckForUpdatesAsync()
    {
        try
        {
            var updateInfo = await _updateService.CheckForUpdatesAsync();
            if (updateInfo != null)
            {
                ShowUpdateNotification(updateInfo);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "检查更新失败");
        }
    }

    private void ShowUpdateNotification(UpdateInfo updateInfo)
    {
        var result = MessageBox.Show(
            $"发现新版本 {updateInfo.Version}\n\n{updateInfo.ReleaseNotes}\n\n是否立即更新？",
            "软件更新",
            MessageBoxButton.YesNo,
            MessageBoxImage.Information);

        if (result == MessageBoxResult.Yes)
        {
            _ = Task.Run(async () =>
            {
                var success = await _updateService.DownloadAndInstallUpdateAsync(updateInfo);
                if (!success)
                {
                    Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show("更新失败，请稍后重试", "错误", 
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    });
                }
            });
        }
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        SaveSettings();
        _countdownTimer?.Stop();
        _updateCheckTimer?.Stop();
        base.OnClosing(e);
    }
}
