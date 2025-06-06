@echo off
echo ======================================
echo      PPT倒计时工具 - 构建脚本
echo ======================================
echo.

echo 正在清理旧的构建文件...
dotnet clean

echo 正在恢复NuGet包...
dotnet restore

echo 正在构建Debug版本...
dotnet build -c Debug

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ======================================
    echo        构建成功完成！
    echo ======================================
    echo 可执行文件位置: bin\Debug\net8.0-windows\win-x64\TimerTool.exe
    echo.
    echo 是否要启动应用程序？ (Y/N)
    set /p choice=
    if /i "%choice%"=="Y" (
        start "" "bin\Debug\net8.0-windows\win-x64\TimerTool.exe"
    )
) else (
    echo.
    echo ======================================
    echo        构建失败！
    echo ======================================
)

pause
