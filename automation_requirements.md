# Unity Build Automation Requirements

## Overview
This document outlines the requirements for automating Unity builds using GameCI on GitHub for Android and iOS platforms.

## Target Platforms
- Android
- iOS

## Unity Version Configuration
- Ability to specify Unity version for builds
- Version selection through:
  - Manual input parameter
  - Configuration file (.unity-version)
  - Environment variable
- Support for multiple Unity versions in the same repository
- Path configuration for different Unity installations

## Build Environments

### Android Build Environment (Ubuntu)
- Unity Editor Installation Requirements
  - Latest LTS version
  - Android Build Support Module
- Android SDK/NDK Setup
  - Latest stable SDK version
  - Required NDK version
  - Platform tools and build tools
- JDK Requirements
  - OpenJDK 11 or later
- Gradle Configuration
  - Latest stable version
  - Custom gradle templates if needed

### iOS Build Environment (macOS)
- macOS Runner Requirements
  - GitHub Actions macOS runner
  - Minimum macOS version requirements
- Xcode Setup
  - Latest stable Xcode version
  - Command line tools
- Unity Editor Requirements
  - macOS version of Unity Editor
  - iOS Build Support Module
- iOS Build Tools
  - Certificates setup
  - Provisioning profiles
  - CocoaPods if required

## Workflow Configuration

### Manual Trigger System
- Implementation using GitHub workflow_dispatch
- Input Parameters:
  - Build version
  - Target platform selection
  - Build configuration (Debug/Release)
  - Environment selection
  - Custom build options

### Version Management
- Automatic version calculation:
  - Based on Git tags
  - Based on semantic versioning
  - Incremental build numbers
- Version extraction from:
  - ProjectSettings.bundleVersion
  - Custom version file
  - Git commit count
- Version output to:
  - Build artifacts
  - Build logs
  - Output variables

### Security Requirements
- Secure Storage:
  - Android keystore files
  - iOS certificates
  - Provisioning profiles
  - Unity license activation
- GitHub Secrets Management:
  - API keys
  - Signing credentials
  - Store credentials
  - Unity serial number

## Build Process

### Pre-build Phase
- Project Validation:
  - Unity project structure check
  - Dependencies verification
  - Asset validation
- Environment Setup:
  - Cache configuration
  - Workspace preparation
  - Tool version verification

### Build Phase
- Unity Build Process:
  - Editor activation
  - Platform-specific build commands
  - Build parameters configuration
- Error Handling:
  - Comprehensive error logging
  - Build failure notifications
  - Retry mechanisms for transient failures

### Post-build Phase
- Artifact Management:
  - Organized output structure
  - Version naming convention
  - Build metadata
- Testing:
  - Basic smoke tests
  - Installation package validation
  - Build verification tests

### TestFlight Distribution (iOS)
- App Store Connect Integration:
  - API key configuration
  - App Store Connect authentication
  - Application management
- Upload Process:
  - Automatic IPA upload to TestFlight
  - Upload verification and status monitoring
  - Upload retry mechanism
- Build Processing:
  - Build processing status tracking
  - Compliance verification monitoring
  - Processing failure handling
- TestFlight Group Management:
  - Internal tester group configuration
  - External tester group selection
  - Build availability settings
- Release Notes:
  - Automatic release notes generation
  - Change log extraction from commits
  - Custom release notes support
- Beta Review Process:
  - Export compliance information
  - Test information provision
  - Review status monitoring

## Artifact Management

### Output Organization
- Structured artifact storage
- Clear versioning system
- Build metadata inclusion
- Automated cleanup of old artifacts

### Testing Integration
- Automated testing suite
- Test reports generation
- Coverage reports
- Performance metrics

## Notification System
- Build Status Alerts:
  - Start notification
  - Completion notification
  - Failure alerts
- Report Generation:
  - Build summary
  - Test results
  - Error logs
  - Performance metrics

### Telegram Integration
- Telegram Bot API integration
- Alert types:
  - Build started notification
  - Build completed notification with download links
  - Build failure alerts with error details
  - Test results summary
- Customization options:
  - Message templates
  - Alert severity levels
  - User/group targeting
  - Rich message formatting
- Attachments:
  - Build logs
  - Screenshots
  - QR codes for download links
  - Version information

## Performance Optimization

### Caching Strategy
- Unity Library caching
- Gradle dependencies
- Pod dependencies
- Build artifacts

### Resource Management
- Runner resource allocation
- Parallel job execution
- Timeout configurations
- Cleanup procedures

## Documentation Requirements
- Setup Instructions:
  - Environment setup guide
  - Configuration guide
  - Security setup
- Workflow Documentation:
  - Process flow documentation
  - Configuration options
  - Best practices
- Troubleshooting Guide:
  - Common issues
  - Solutions
  - Support contacts

## Maintenance
- Regular updates for:
  - Unity version
  - Build tools
  - Dependencies
  - Security patches
- Monitoring:
  - Build performance
  - Resource usage
  - Error patterns
  - Success rates
