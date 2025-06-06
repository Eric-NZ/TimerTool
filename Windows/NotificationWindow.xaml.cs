using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace TimerTool.Windows;

/// <summary>
/// NotificationWindow.xaml 的交互逻辑
/// </summary>
public partial class NotificationWindow : Window
{
    private readonly DispatcherTimer _autoCloseTimer;

    public NotificationWindow(string message, int autoCloseAfterSeconds = 5)
    {
        InitializeComponent();
        
        MessageText.Text = message;
        
        // 设置自动关闭计时器
        _autoCloseTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(autoCloseAfterSeconds)
        };
        _autoCloseTimer.Tick += (s, e) =>
        {
            _autoCloseTimer.Stop();
            Close();
        };
        _autoCloseTimer.Start();
        
        // 设置位置在屏幕右下角
        Left = SystemParameters.PrimaryScreenWidth - Width - 20;
        Top = SystemParameters.PrimaryScreenHeight - Height - 100;
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        _autoCloseTimer?.Stop();
        Close();
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        _autoCloseTimer?.Stop();
        Close();
    }

    protected override void OnClosed(EventArgs e)
    {
        _autoCloseTimer?.Stop();
        base.OnClosed(e);
    }

    /// <summary>
    /// 显示通知窗口
    /// </summary>
    /// <param name="message">提醒消息</param>
    /// <param name="autoCloseAfterSeconds">自动关闭时间（秒）</param>
    public static void ShowNotification(string message, int autoCloseAfterSeconds = 5)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            var notification = new NotificationWindow(message, autoCloseAfterSeconds);
            notification.Show();
        });
    }
}
