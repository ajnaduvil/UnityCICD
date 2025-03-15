# Step 2: GitHub Actions Environment Setup

This step covers setting up the GitHub Actions environment and Unity license activation.

## 2.1 Unity License Activation

For Unity license activation, follow the official GameCI documentation:
[Unity License Activation Guide](https://game.ci/docs/github/activation)

The guide covers different activation methods based on your license type (Personal, Professional, or License Server).

**Important**: After completing the activation process, you should have a `.ulf` (Unity License File) ready to use in the next step. This file contains your Unity license information and is required for the GitHub Actions workflow.

## 2.2 Configure GitHub Secrets

In your GitHub repository, navigate to Settings > Secrets and variables > Actions and add the following secrets:

### Unity Secrets

- `UNITY_LICENSE` - The entire content of the .ulf file you received
- `UNITY_EMAIL` - Your Unity account email
- `UNITY_PASSWORD` - Your Unity account password

### Telegram Notification Secrets

- `TELEGRAM_TOKEN` - Your Telegram bot token
- `TELEGRAM_TO` - Your Telegram chat ID

### iOS/TestFlight Secrets

- `APPLE_CONNECT_EMAIL` - Your App Store Connect email
- `APPLE_DEVELOPER_EMAIL` - Your Apple Developer email
- `APPLE_TEAM_ID` - Your Apple Team ID
- `MATCH_REPOSITORY` - SSH URL of your private match repository
- `MATCH_DEPLOY_KEY` - SSH private key with access to the match repository
- `MATCH_PASSWORD` - Password for the match repository
- `APPSTORE_ISSUER_ID` - App Store Connect API Issuer ID
- `APPSTORE_KEY_ID` - App Store Connect API Key ID
- `APPSTORE_P8` - App Store Connect API Private Key (p8 file content)
- `IOS_BUNDLE_ID` - Your iOS application bundle ID

### Android Signing Secrets

- `ANDROID_KEYSTORE_BASE64` - Your Android keystore file encoded in base64
- `ANDROID_KEYSTORE_PASS` - Your Android keystore password
- `ANDROID_KEY_ALIAS` - Your Android key alias
- `ANDROID_KEY_PASS` - Your Android key password

## Next Steps

After configuring the GitHub Actions environment, proceed to [Unity Build Configuration](03-unity-build-configuration.md). 