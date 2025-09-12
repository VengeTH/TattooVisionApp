# Contributing to Tattoo Vision App

Thank you for your interest in contributing to the Tattoo Vision App! We welcome contributions from developers of all skill levels.

## 🚀 Getting Started

### Development Environment Setup

1. **Prerequisites**:

   - Unity 2021.3 or later
   - Git
   - Android SDK (for Android development)
   - Xcode (for iOS development, macOS only)

2. **Clone and Setup**:

   ```bash
   git clone https://github.com/your-username/TattooVisionApp2.git
   cd TattooVisionApp2-main
   git submodule update --init --recursive
   ```

3. **Unity Setup**:
   - Open the project in Unity
   - Install required packages via Package Manager
   - Configure Firebase (see README.md)
   - Set up build targets

## 📝 Contribution Guidelines

### Code Style

- Follow Unity's C# coding conventions
- Use meaningful variable and method names
- Add comments for complex logic
- Keep methods focused and single-purpose

### Commit Messages

Use clear, descriptive commit messages:

```
feat: add AR tattoo preview functionality
fix: resolve camera permission issue on Android
docs: update installation instructions
refactor: optimize skin scanning algorithm
```

### Pull Request Process

1. Create a feature branch from `main`
2. Make your changes
3. Test thoroughly on target platforms
4. Update documentation if needed
5. Submit a pull request with detailed description
6. Address review feedback

## 🧪 Testing

### Testing Requirements

- Test on Android devices (primary target)
- Test on iOS devices (if possible)
- Verify AR functionality works correctly
- Check UI responsiveness on different screen sizes

### Test Scenarios

- [ ] Camera permissions granted/denied
- [ ] AR tracking stability
- [ ] UI navigation flow
- [ ] Firebase connectivity
- [ ] Offline functionality

## 🐛 Bug Reports

When reporting bugs, please include:

- Unity version
- Device model and OS version
- Steps to reproduce
- Expected vs actual behavior
- Screenshots/logs if applicable

## 💡 Feature Requests

Feature requests should include:

- Clear description of the proposed feature
- Use case and benefits
- Technical feasibility considerations
- Mockups or examples if applicable

## 📚 Documentation

### Code Documentation

- Add XML documentation comments to public methods
- Update README.md for new features
- Document any new dependencies or setup requirements

### User Documentation

- Update user-facing documentation
- Provide clear setup instructions
- Include troubleshooting guides

## 🔧 Development Workflow

### Branching Strategy

```
main        - Production-ready code
develop     - Integration branch
feature/*   - New features
bugfix/*    - Bug fixes
hotfix/*    - Critical fixes
```

### Code Review Process

- All changes require review
- At least one maintainer approval required
- Address all review comments
- Maintainers may request changes before merge

## 🎯 Areas for Contribution

### High Priority

- [ ] Performance optimization
- [ ] Bug fixes
- [ ] UI/UX improvements
- [ ] Documentation updates

### Medium Priority

- [ ] New AR features
- [ ] Enhanced computer vision
- [ ] Cross-platform improvements

### Low Priority

- [ ] Advanced design tools
- [ ] Social features
- [ ] Analytics integration

## 📞 Communication

- **Issues**: GitHub Issues for bugs and features
- **Discussions**: GitHub Discussions for general questions
- **Discord**: [Join our community] (if applicable)

## 🙏 Code of Conduct

- Be respectful and inclusive
- Focus on constructive feedback
- Help newcomers learn and contribute
- Maintain professional communication

## 📋 Checklist for Contributors

- [ ] Code follows project style guidelines
- [ ] Tests pass on target platforms
- [ ] Documentation updated
- [ ] Commit messages are clear
- [ ] Changes don't break existing functionality
- [ ] Pull request description is detailed

Thank you for contributing to Tattoo Vision App! 🎨✨
