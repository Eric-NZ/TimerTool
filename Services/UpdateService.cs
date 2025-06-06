using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace TimerTool.Services;

public class UpdateService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UpdateService> _logger;
    private const string UPDATE_CHECK_URL = "https://your-server.com/api/version"; // TODO: 替换为实际的API地址

    public UpdateService(HttpClient httpClient, ILogger<UpdateService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<UpdateInfo?> CheckForUpdatesAsync()
    {
        try
        {
            _logger.LogInformation("检查更新...");
            
            var response = await _httpClient.GetStringAsync(UPDATE_CHECK_URL);
            var updateInfo = JsonSerializer.Deserialize<UpdateInfo>(response, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (updateInfo != null)
            {
                var currentVersion = GetCurrentVersion();
                var newVersion = Version.Parse(updateInfo.Version);
                
                if (newVersion > currentVersion)
                {
                    _logger.LogInformation($"发现新版本: {updateInfo.Version}");
                    return updateInfo;
                }
                else
                {
                    _logger.LogInformation("当前已是最新版本");
                }
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "检查更新时网络请求失败");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "检查更新时发生错误");
        }

        return null;
    }

    public async Task<bool> DownloadAndInstallUpdateAsync(UpdateInfo updateInfo)
    {
        try
        {
            _logger.LogInformation($"开始下载更新: {updateInfo.Version}");

            // 下载新版本
            using var response = await _httpClient.GetAsync(updateInfo.DownloadUrl);
            response.EnsureSuccessStatusCode();
            
            var tempPath = Path.Combine(Path.GetTempPath(), $"TimerTool_Update_{Guid.NewGuid()}.exe");
            
            await using var fileStream = File.Create(tempPath);
            await response.Content.CopyToAsync(fileStream);
            
            _logger.LogInformation($"下载完成: {tempPath}");

            // 验证文件完整性（如果提供了校验值）
            if (!string.IsNullOrEmpty(updateInfo.Sha256))
            {
                if (!VerifyFileHash(tempPath, updateInfo.Sha256))
                {
                    _logger.LogError("文件校验失败");
                    File.Delete(tempPath);
                    return false;
                }
            }

            // 创建更新脚本
            var scriptPath = CreateUpdateScript(tempPath);
            
            _logger.LogInformation("启动更新脚本并退出应用程序");

            // 启动更新脚本并退出当前程序
            StartUpdateProcess(scriptPath);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "下载和安装更新时发生错误");
            return false;
        }
    }

    private Version GetCurrentVersion()
    {
        return Assembly.GetExecutingAssembly().GetName().Version ?? new Version("1.0.0");
    }

    private bool VerifyFileHash(string filePath, string expectedHash)
    {
        try
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            using var stream = File.OpenRead(filePath);
            var hashBytes = sha256.ComputeHash(stream);
            var actualHash = Convert.ToHexString(hashBytes);
            
            return string.Equals(actualHash, expectedHash, StringComparison.OrdinalIgnoreCase);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "验证文件哈希时发生错误");
            return false;
        }
    }    private string CreateUpdateScript(string newExePath)
    {        var currentExePath = System.AppContext.BaseDirectory + "TimerTool.exe";
        var mainModule = System.Diagnostics.Process.GetCurrentProcess().MainModule;
        if (mainModule?.FileName != null)
        {
            currentExePath = mainModule.FileName;
        }
        
        var scriptPath = Path.Combine(Path.GetTempPath(), $"update_{Guid.NewGuid()}.bat");

        var script = $@"@echo off
echo 正在更新 TimerTool...
timeout /t 3 /nobreak > nul

:RETRY
tasklist /FI ""IMAGENAME eq {Path.GetFileName(currentExePath)}"" 2>NUL | find /I /N ""{Path.GetFileName(currentExePath)}"">NUL
if ""%ERRORLEVEL%""==""0"" (
    echo 等待应用程序关闭...
    timeout /t 2 /nobreak > nul
    goto RETRY
)

echo 复制新版本文件...
copy ""{newExePath}"" ""{currentExePath}"" /y
if errorlevel 1 (
    echo 更新失败！
    pause
    goto END
)

echo 启动新版本...
start """" ""{currentExePath}""

echo 清理临时文件...
del ""{newExePath}"" 2>nul
del ""{scriptPath}"" 2>nul

:END
";

        File.WriteAllText(scriptPath, script, System.Text.Encoding.Default);
        _logger.LogInformation($"创建更新脚本: {scriptPath}");
        
        return scriptPath;
    }

    private void StartUpdateProcess(string scriptPath)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = $"/c \"{scriptPath}\"",
            WindowStyle = ProcessWindowStyle.Hidden,
            UseShellExecute = true
        };

        Process.Start(startInfo);
        
        // 给脚本一些时间启动，然后退出当前进程
        Task.Delay(1000).ContinueWith(_ => 
        {
            Environment.Exit(0);
        });
    }
}

public class UpdateInfo
{
    public string Version { get; set; } = string.Empty;
    public string DownloadUrl { get; set; } = string.Empty;
    public string ReleaseNotes { get; set; } = string.Empty;
    public bool ForceUpdate { get; set; }
    public DateTime ReleaseDate { get; set; }
    public long FileSize { get; set; }
    public string Sha256 { get; set; } = string.Empty;
}
