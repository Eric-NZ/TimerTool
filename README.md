# TimerTool - PPT Countdown Timer

A simple Windows desktop floating countdown timer tool designed specifically for PPT presentations.

## ✨ Features

- ⏰ **Floating Display**: Always stays on top without interfering with other applications
- 🎯 **Smart Interaction**: Expands when mouse hovers, collapses to small button when mouse leaves
- 📌 **Pin Mode**: Pin the interface to keep it always expanded
- 🔔 **Smart Reminders**: Configurable reminder time points with intelligent interval management
- 🎨 **Visual Feedback**: Interface highlights during reminders with color changes
- 🔄 **Remote Updates**: Automatic update checking and installation support
- ⚙️ **Customizable Settings**: Configure countdown duration, reminder points, sound notifications, etc.

## 🛠️ Technical Features

- Built with **WPF + .NET 8**
- Self-contained deployment - no additional dependencies required
- Modern UI design with intuitive user experience
- Comprehensive error handling and logging
- Dependency injection architecture
- Modular service design

## 💻 System Requirements

- Windows 10 or higher
- No .NET Runtime installation required (self-contained deployment)

## 🚀 Quick Start

1. Download and run `TimerTool.exe`
2. The tool appears in the top-right corner as a green clock icon
3. Hover your mouse over the icon to expand the detailed countdown panel
4. Click the "Settings" (⚙) button to configure countdown duration and reminder points
5. Click the "Start" button to begin countdown
6. Use the "Pin" (📌) button to keep the interface always expanded

## 📖 User Guide

### Basic Operations
- **Start/Pause**: Click the start button to begin or pause the countdown
- **Reset**: Reset the countdown to the configured duration
- **Settings**: Configure all timer parameters
- **Pin/Unpin**: Toggle between auto-hide and always-visible modes

### Smart Reminders
- Set multiple reminder time points (in minutes)
- Reminders trigger only once per time point to avoid interruption
- 1-minute interval between reminders to prevent frequent notifications
- Visual feedback: interface background changes to orange during reminders
- Optional sound notifications

### Hover Behavior
- **Compact Mode**: Shows as a small green clock icon
- **Expanded Mode**: Shows full timer interface with controls
- **Auto-hide**: Automatically collapses when mouse leaves (unless pinned)
- **Pin Mode**: Keeps interface expanded regardless of mouse position

## 🏗️ Development

### Project Structure
```
TimerTool/
├── Services/                   # Service layer
│   ├── UpdateService.cs       # Remote update functionality
│   ├── SettingsService.cs     # Configuration management
│   └── SoundService.cs        # Audio notifications
├── ViewModels/                 # MVVM view models
│   └── MainWindowViewModel.cs
├── Windows/                    # Window components
│   ├── SettingsWindow.xaml    # Settings dialog
│   └── NotificationWindow.xaml # Notification popups (deprecated)
├── Styles/                     # UI styles and themes
│   ├── ButtonStyles.xaml
│   └── Colors.xaml
├── Assets/                     # Application resources
│   └── timer.ico
└── App.xaml                   # Application entry point
```

### Build Instructions

#### Development Build
```powershell
dotnet build
```

#### Release Build
```powershell
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

#### Using Build Scripts
```powershell
# Development build
.\build.bat

# Production release
.\publish.bat
```

### Configuration

The application uses JSON configuration stored in the user's application data folder:
- **Settings Location**: `%APPDATA%\TimerTool\settings.json`
- **Default Duration**: 20 minutes
- **Default Reminders**: 10, 5, 2, 1 minutes
- **Sound Notifications**: Enabled by default

## 🔄 Remote Update System

The application includes a complete remote update framework:

### Client-side Features
- Automatic update checking (hourly)
- Secure download and installation
- File integrity verification
- Graceful update process with user confirmation

### Server Requirements (To be implemented)
```
Update Server Endpoints:
- GET /api/version - Version information
- GET /api/download/{version} - Download specific version
- File hosting for update packages
```

## 🎯 Usage Scenarios

- **PPT Presentations**: Keep track of presentation time limits
- **Meeting Management**: Monitor speaking time during meetings
- **Study Sessions**: Pomodoro technique implementation
- **Cooking Timer**: Kitchen countdown timer
- **General Purpose**: Any time-sensitive activity

## 🔧 Advanced Features

### Settings Configuration
- **Countdown Duration**: Hours and minutes precision
- **Reminder Points**: Multiple reminder time points (minutes)
- **Sound Notifications**: Enable/disable audio alerts
- **Auto-start**: Launch with Windows (future feature)
- **Update Settings**: Automatic update preferences

### Keyboard Shortcuts
- **Right-click**: Context menu with settings and exit options
- **Mouse drag**: Move the timer window when expanded

## 🐛 Troubleshooting

### Common Issues
1. **Timer not visible**: Check if it's positioned off-screen, restart the application
2. **Settings not saved**: Ensure the application has write permissions to %APPDATA%
3. **Update failures**: Check internet connection and firewall settings

### Debug Information
- Application logs are stored in the application directory
- Use the built-in error reporting for technical issues

## 🎨 UI/UX Improvements (v1.1.0 Changes)

### Enhanced Reminder System
- **No More Popup Windows**: Reminders now highlight the main interface instead of showing separate popup windows
- **Smart Reminder Intervals**: 1-minute cooldown between reminders to prevent interruption
- **Visual Feedback**: Orange background highlight during reminder periods
- **One-time Reminders**: Each reminder point triggers only once per countdown session

### Pin Functionality
- **Pin Button**: 📌 icon to keep the interface always expanded
- **Visual Indicator**: Pin button shows current pin state
- **Smart Auto-hide**: Respects pin state for mouse leave behavior

### Improved User Experience
- **Reduced Interruption**: No more frequent popup notifications
- **Better Visual Cues**: Clear indication of timer state and reminders
- **Flexible Display**: Choose between auto-hide and always-visible modes

## 📋 Roadmap

### Version 1.1.0 (Current)
- [x] Enhanced reminder system without popup windows
- [x] Pin functionality for persistent display
- [x] Smart reminder intervals
- [x] Visual feedback improvements

### Version 1.2.0 (Planned)
- [ ] Backend update server implementation
- [ ] Custom sound file support
- [ ] UI animations and transitions
- [ ] Multi-language support (Chinese, English)
- [ ] Themes and customization options

### Version 1.3.0 (Future)
- [ ] Plugin system for extensions
- [ ] Multiple timer support
- [ ] Statistics and usage tracking
- [ ] Cloud synchronization

## 📄 License

This project is proprietary software. All rights reserved.

## 🤝 Contributing

This is a private project. For bug reports or feature requests, please contact the development team.

## 📞 Support

For technical support or questions:
- Create an issue in the project repository
- Contact the development team directly

## 📝 Version History

### v1.1.0 (2025-01-06)
- Enhanced reminder system without popup interruptions
- Added pin functionality for persistent display
- Improved visual feedback and user experience
- Smart reminder interval management
- Better mouse interaction behavior

### v1.0.0 (2025-01-06)
- Initial release
- Basic countdown timer functionality
- Floating window with smart interaction
- Settings interface
- Remote update framework (client-side)
- Sound notification support

---

**Built with ❤️ for better presentation management**
