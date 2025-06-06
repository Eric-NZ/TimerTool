@echo off
echo ========================================
echo       TimerTool 最终发布脚本
echo ========================================
echo.

echo 1. 清理旧文件...
dotnet clean --configuration Release
if %errorlevel% neq 0 goto :error

echo.
echo 2. 构建Release版本...
dotnet build --configuration Release
if %errorlevel% neq 0 goto :error

echo.
echo 3. 发布自包含版本...
dotnet publish --configuration Release --runtime win-x64 --self-contained true --output publish --property:PublishSingleFile=true --property:PublishTrimmed=false
if %errorlevel% neq 0 goto :error

echo.
echo 4. 重命名发布文件...
cd publish
if exist TimerTool-v1.0.0.exe del TimerTool-v1.0.0.exe
ren TimerTool.exe TimerTool-v1.0.0.exe
cd ..

echo.
echo 5. 创建发布包...
if exist TimerTool-Release.zip del TimerTool-Release.zip
powershell -Command "Compress-Archive -Path 'publish\*' -DestinationPath 'TimerTool-Release.zip' -Force"

echo.
echo ========================================
echo           发布完成！
echo ========================================
echo.
echo 发布文件位置:
echo - 可执行文件: publish\TimerTool-v1.0.0.exe
echo - 发布包: TimerTool-Release.zip
echo.
echo 文件大小:
dir publish\TimerTool-v1.0.0.exe
echo.
echo 可以直接运行 TimerTool-v1.0.0.exe 或解压 TimerTool-Release.zip
echo.
pause
goto :end

:error
echo.
echo ========================================
echo           发布失败！
echo ========================================
echo 错误代码: %errorlevel%
pause
exit /b %errorlevel%

:end
