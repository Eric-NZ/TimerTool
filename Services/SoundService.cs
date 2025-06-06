using System;
using System.Media;
using Microsoft.Extensions.Logging;

namespace TimerTool.Services;

/// <summary>
/// 声音通知服务
/// </summary>
public class SoundService
{
    private readonly ILogger<SoundService> _logger;

    public SoundService(ILogger<SoundService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 播放系统提示音
    /// </summary>
    public void PlayNotificationSound()
    {
        try
        {
            // 播放系统提示音
            SystemSounds.Asterisk.Play();
            _logger.LogInformation("播放通知声音");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "播放通知声音时发生错误");
        }
    }

    /// <summary>
    /// 播放警告音
    /// </summary>
    public void PlayWarningSound()
    {
        try
        {
            // 播放系统警告音
            SystemSounds.Exclamation.Play();
            _logger.LogInformation("播放警告声音");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "播放警告声音时发生错误");
        }
    }

    /// <summary>
    /// 播放结束音
    /// </summary>
    public void PlayFinishSound()
    {
        try
        {
            // 播放系统手势音
            SystemSounds.Hand.Play();
            _logger.LogInformation("播放结束声音");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "播放结束声音时发生错误");
        }
    }
}
