# Unity Build Automation - Implementation Guide

This guide provides step-by-step instructions for implementing Unity build automation using GitHub Actions and GameCI. Each section is in a separate file for easier navigation and preparation.

## Implementation Steps

1. [Repository Setup](steps/01-repository-setup.md)
   - Git initialization
   - Git LFS configuration
   - Unity-specific .gitignore
   - GitHub Actions workflow directory

2. [GitHub Actions Environment Setup](steps/02-github-actions-setup.md)
   - Unity license activation
   - GitHub Secrets configuration

3. [Unity Build Configuration](steps/03-unity-build-configuration.md)
   - Main workflow file
   - Test job setup
   - Build jobs for Android and iOS

4. [TestFlight Integration](steps/04-testflight-integration.md)
   - Fastlane configuration
   - TestFlight deployment job

5. [Telegram Notifications](steps/05-telegram-notifications.md)
   - Telegram bot creation
   - Chat ID configuration
   - Notification jobs

6. [Performance Optimization](steps/06-performance-optimization.md)
   - Unity cache optimization
   - Parallel builds setup

7. [Maintenance and Cleanup](steps/07-maintenance-cleanup.md)
   - Artifact cleanup job
   - Pipeline monitoring

8. [Usage Guide](steps/08-usage-guide.md)
   - Running the workflow
   - Monitoring and troubleshooting

## Prerequisites

- Unity project
- GitHub repository
- Unity account
- Telegram account (for notifications)
- Apple Developer account (for iOS/TestFlight)
- Google Play Developer account (for Android) 