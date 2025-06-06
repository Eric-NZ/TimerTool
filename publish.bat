@echo off
echo ======================================
echo      PPT倒计时工具 - 发布脚本
echo ======================================
echo.

set /p VERSION="请输入版本号 (例如: 1.0.0): "
if "%VERSION%"=="" set VERSION=1.0.0

echo 正在清理旧的发布文件...
if exist "publish" rmdir /s /q "publish"

echo 正在发布Release版本...
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o "publish"

if %ERRORLEVEL% EQU 0 (
    echo.
    echo 正在重命名发布文件...
    if exist "publish\TimerTool.exe" (
        copy "publish\TimerTool.exe" "publish\TimerTool-v%VERSION%.exe"
        echo.
        echo ======================================
        echo        发布成功完成！
        echo ======================================
        echo 发布文件位置: publish\TimerTool-v%VERSION%.exe
        echo 文件大小: 
        dir "publish\TimerTool-v%VERSION%.exe" | findstr /C:"TimerTool-v"
        echo.
        echo 发布的文件可以独立运行，无需安装任何依赖！
    ) else (
        echo 错误: 未找到发布的可执行文件
    )
) else (
    echo.
    echo ======================================
    echo        发布失败！
    echo ======================================
)

pause
