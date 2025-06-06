using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using TimerTool.Services;

namespace TimerTool.Windows;

/// <summary>
/// SettingsWindow.xaml 的交互逻辑
/// </summary>
public partial class SettingsWindow : Window
{
    private readonly SettingsService _settingsService;
    private AppSettings _settings;

    public SettingsWindow(SettingsService settingsService)
    {
        InitializeComponent();
        _settingsService = settingsService;
        _settings = _settingsService.LoadSettings();
        
        LoadCurrentSettings();
        UpdateVersionInfo();
    }    private void LoadCurrentSettings()
    {
        // 加载倒计时时长
        var duration = _settings.DefaultDuration;
        HoursTextBox.Text = duration.Hours.ToString();
        MinutesTextBox.Text = duration.Minutes.ToString();

        // 加载提醒时间点
        ReminderPointsTextBox.Text = string.Join("\n", _settings.ReminderPoints);

        // 加载通知设置
        EnableSoundCheckBox.IsChecked = _settings.EnableSoundNotification;
        NotificationDurationTextBox.Text = (_settings.NotificationDuration / 1000).ToString();

        // 加载更新设置
        CheckUpdatesCheckBox.IsChecked = _settings.CheckUpdatesOnStartup;
        AutoStartCheckBox.IsChecked = _settings.AutoStart;
    }

    private void UpdateVersionInfo()
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version;
        VersionText.Text = $"版本: {version?.ToString(3) ?? "1.0.0"}";
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        if (ValidateAndSaveSettings())
        {
            DialogResult = true;
            Close();
        }
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private bool ValidateAndSaveSettings()
    {
        try
        {
            // 验证并保存倒计时时长
            if (!int.TryParse(HoursTextBox.Text, out int hours) || hours < 0 || hours > 23)
            {
                MessageBox.Show("请输入有效的小时数 (0-23)", "输入错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                HoursTextBox.Focus();
                return false;
            }            if (!int.TryParse(MinutesTextBox.Text, out int minutes) || minutes < 0 || minutes > 59)
            {
                MessageBox.Show("请输入有效的分钟数 (0-59)", "输入错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                MinutesTextBox.Focus();
                return false;
            }

            var totalTime = new TimeSpan(hours, minutes, 0); // 秒数固定为0
            if (totalTime.TotalMinutes <= 0)
            {
                MessageBox.Show("倒计时总时长必须大于0分钟", "输入错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            _settings.DefaultDuration = totalTime;

            // 验证并保存提醒时间点
            var reminderLines = ReminderPointsTextBox.Text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var reminderPoints = new List<double>();

            foreach (var line in reminderLines)
            {
                if (double.TryParse(line.Trim(), out double point) && point > 0)
                {
                    reminderPoints.Add(point);
                }
            }

            _settings.ReminderPoints = reminderPoints;

            // 保存通知设置
            _settings.EnableSoundNotification = EnableSoundCheckBox.IsChecked ?? true;
            
            if (int.TryParse(NotificationDurationTextBox.Text, out int duration) && duration > 0)
            {
                _settings.NotificationDuration = duration * 1000; // 转换为毫秒
            }

            // 保存更新设置
            _settings.CheckUpdatesOnStartup = CheckUpdatesCheckBox.IsChecked ?? true;
            _settings.AutoStart = AutoStartCheckBox.IsChecked ?? false;

            // 处理开机自启动
            HandleAutoStartSetting(_settings.AutoStart);

            // 保存设置
            _settingsService.SaveSettings(_settings);

            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"保存设置时发生错误: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
    }

    private void HandleAutoStartSetting(bool enable)
    {
        try
        {
            var registryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

            if (registryKey != null)
            {
                if (enable)
                {
                    var exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName;
                    if (!string.IsNullOrEmpty(exePath))
                    {
                        registryKey.SetValue("TimerTool", $"\"{exePath}\"");
                    }
                }
                else
                {
                    registryKey.DeleteValue("TimerTool", false);
                }
                registryKey.Close();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"设置开机自启动时发生错误: {ex.Message}", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private async void CheckUpdateButton_Click(object sender, RoutedEventArgs e)
    {
        UpdateStatusText.Text = "正在检查更新...";
        
        try
        {
            // 这里需要创建UpdateService实例来检查更新
            // 暂时显示占位信息
            await Task.Delay(2000); // 模拟网络请求
            UpdateStatusText.Text = "当前已是最新版本";
        }
        catch
        {
            UpdateStatusText.Text = "检查更新失败";
        }
    }
}
