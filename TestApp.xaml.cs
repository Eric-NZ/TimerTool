using System.Windows;

namespace TimerTool;

public partial class TestApp : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        var window = new Window
        {
            Title = "测试窗口",
            Width = 300,
            Height = 200,
            Content = new System.Windows.Controls.TextBlock 
            { 
                Text = "程序正常启动！", 
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        };
        
        window.Show();
    }
}
