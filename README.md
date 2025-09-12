# Tattoo Vision App

A Unity-based augmented reality application that leverages computer vision and AR technology to enhance tattoo visualization and design experiences.

## 🌟 Features

### Core Functionality
- **Skin Scanner**: Advanced tattoo detection and analysis using OpenCV
- **AR Visualization**: Real-time augmented reality tattoo preview on skin
- **Interactive Dashboard**: User-friendly interface for managing tattoo designs
- **Content Management**: Organize and manage tattoo designs and collections
- **Firebase Integration**: Cloud-based data storage and synchronization
- **Cross-Platform Support**: Android and iOS deployment capabilities

### Technical Features
- **Computer Vision**: OpenCV integration for image processing
- **XR/AR Foundation**: Unity's AR framework for immersive experiences
- **Real-time Camera**: Live camera feed with overlay capabilities
- **Input System**: Advanced input handling for various devices
- **UI/UX**: Modern interface with TextMesh Pro integration

## 🛠️ Technology Stack

- **Engine**: Unity 3D
- **Computer Vision**: OpenCV for Unity
- **Backend**: Firebase (Authentication, Database, Storage)
- **AR/VR**: Unity XR Foundation, ARCore, ARKit
- **UI Framework**: Unity UI with TextMesh Pro
- **Input System**: Unity Input System
- **Platform**: Android (primary), iOS (secondary)

## 📋 Prerequisites

- **Unity Version**: 2021.3 or later
- **Android SDK**: API Level 24+ (for Android builds)
- **iOS SDK**: 12.0+ (for iOS builds)
- **OpenCV for Unity**: Included in project
- **Firebase Account**: Required for backend services

## 🚀 Installation & Setup

### 1. Clone the Repository
```bash
git clone https://github.com/your-username/TattooVisionApp2.git
cd TattooVisionApp2-main
```

### 2. Open in Unity
1. Launch Unity Hub
2. Add project from disk
3. Select the cloned directory
4. Open with Unity 2021.3+

### 3. Firebase Configuration
1. Create a Firebase project at https://console.firebase.google.com/
2. Enable Authentication, Firestore Database, and Storage
3. Download `google-services.json` and place it in `Assets/`
4. Configure Firebase settings in Unity

### 4. OpenCV Setup
- OpenCV for Unity is already included in the project
- No additional setup required for basic functionality
- Advanced features may require additional OpenCV modules

### 5. Build Settings
1. **Android**:
   - Go to File > Build Settings
   - Select Android platform
   - Set minimum API level to 24
   - Enable ARCore support in Player Settings > XR Plug-in Management

2. **iOS**:
   - Select iOS platform in Build Settings
   - Configure bundle identifier
   - Enable ARKit in Player Settings

## 📱 Usage

### Basic Operation
1. **Launch App**: Start the application on your device
2. **Grant Permissions**: Allow camera and storage access
3. **Skin Scanning**: Use camera to scan skin areas
4. **AR Preview**: Visualize tattoos in augmented reality
5. **Design Management**: Browse and manage tattoo collections

### Development Workflow
1. **Scene Navigation**: Use `Assets/Scenes/` for different app sections
2. **Script Editing**: Modify behavior in `Assets/Script/`
3. **UI Customization**: Adjust interfaces in `Assets/UX_UI/`
4. **Asset Management**: Organize resources in respective folders

## 🏗️ Project Structure

```
Assets/
├── Scenes/           # Unity scenes
├── Script/           # C# scripts
│   ├── Dashboard/    # Dashboard functionality
│   └── Firebase/     # Firebase integration
├── XR/              # XR/AR configurations
├── UX_UI/           # User interface assets
├── OpenCV+Unity/    # Computer vision library
├── Firebase/        # Firebase SDK
├── Prefab/          # Reusable game objects
├── Images/          # Image assets
└── Plugins/         # Native plugins
```

## 🔧 Key Components

### Core Scripts
- **SkinScanner.cs**: Handles tattoo detection and analysis
- **ARCameraUIManager.cs**: Manages AR camera and overlays
- **UIManager.cs**: Controls user interface flow
- **ContentManager.cs**: Manages tattoo design collections
- **NavigationManager.cs**: Handles scene transitions

### Firebase Integration
- User authentication
- Data synchronization
- Cloud storage for designs
- Real-time database updates

## 🐛 Troubleshooting

### Common Issues
1. **Camera Permission Denied**
   - Ensure camera permissions are granted in device settings
   - Check Unity Player Settings for camera usage description

2. **AR Not Working**
   - Verify ARCore/ARKit is installed on device
   - Check XR Plug-in Management settings

3. **Firebase Connection Failed**
   - Verify `google-services.json` is properly configured
   - Check internet connection
   - Validate Firebase project settings

### Debug Mode
- Enable development build for detailed logging
- Check Unity Console for error messages
- Use Android Logcat for device-specific issues

## 📊 Performance Optimization

- **Target Frame Rate**: 30-60 FPS
- **Texture Compression**: Use ASTC for Android, PVRTC for iOS
- **Script Optimization**: Use object pooling for frequently spawned objects
- **AR Performance**: Limit simultaneous tracked objects

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

### Development Guidelines
- Follow Unity coding standards
- Use meaningful commit messages
- Test on multiple devices before submitting
- Document new features in README

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 👥 Support

For support and questions:
- Create an issue in this repository
- Check the Unity documentation
- Review Firebase documentation
- Consult OpenCV for Unity guides

## 🚀 Future Enhancements

- [ ] Multi-user collaboration
- [ ] Advanced AI tattoo suggestions
- [ ] 3D tattoo modeling
- [ ] Social sharing features
- [ ] Offline mode support
- [ ] Custom design tools

---

**Note**: This application is for entertainment and design visualization purposes. Always consult with licensed tattoo artists for actual tattoo work.
