using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace TimerTool.Services;

public class SettingsService
{
    private readonly ILogger<SettingsService> _logger;
    private readonly string _settingsPath;

    public SettingsService(ILogger<SettingsService> logger)
    {
        _logger = logger;
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var appFolder = Path.Combine(appDataPath, "TimerTool");
        Directory.CreateDirectory(appFolder);
        _settingsPath = Path.Combine(appFolder, "settings.json");
    }

    public AppSettings LoadSettings()
    {
        try
        {
            if (File.Exists(_settingsPath))
            {
                var json = File.ReadAllText(_settingsPath);
                var settings = JsonSerializer.Deserialize<AppSettings>(json);
                if (settings != null)
                {
                    _logger.LogInformation("设置加载成功");
                    return settings;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "加载设置时发生错误");
        }

        _logger.LogInformation("使用默认设置");
        return new AppSettings();
    }

    public void SaveSettings(AppSettings settings)
    {
        try
        {
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(_settingsPath, json);
            _logger.LogInformation("设置保存成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "保存设置时发生错误");
        }
    }
}

public class AppSettings
{
    public TimeSpan DefaultDuration { get; set; } = TimeSpan.FromMinutes(20); // 默认20分钟
    public List<double> ReminderPoints { get; set; } = new() { 10, 5, 2, 1 }; // 提醒时间点（分钟）
    public System.Drawing.Point? WindowPosition { get; set; }
    public bool AutoStart { get; set; } = false;
    public bool CheckUpdatesOnStartup { get; set; } = true;
    public string UpdateServerUrl { get; set; } = "https://your-server.com/api/version";
    public bool EnableSoundNotification { get; set; } = true;
    public int NotificationDuration { get; set; } = 3000; // 通知显示时长（毫秒）
}
