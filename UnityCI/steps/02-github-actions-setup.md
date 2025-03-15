# Step 2: GitHub Actions Environment Setup

This step covers setting up the GitHub Actions environment and Unity license activation.

## 2.1 Unity License Activation

For Unity license activation, follow the official GameCI documentation:
[Unity License Activation Guide](https://game.ci/docs/github/activation)

The guide covers different activation methods based on your license type (Personal, Professional, or License Server).

**Important**: After completing the activation process, you should have a `.ulf` (Unity License File) ready to use in the next step. This file contains your Unity license information and is required for the GitHub Actions workflow.

## 2.2 Configure GitHub Secrets

GitHub offers two ways to store configuration information: **Secrets** (for sensitive data) and **Variables** (for non-sensitive data). For our Unity CI/CD pipeline, we'll primarily use Secrets to store sensitive information safely.

### Types of GitHub Secrets

1. **Repository Secrets**: Available to all workflows in a repository
2. **Environment Secrets**: Limited to specific deployment environments
3. **Organization Secrets**: Shared across multiple repositories in an organization

For this guide, we'll use **Repository Secrets** which are the most straightforward to set up.

### How to Create Repository Secrets

1. On GitHub, navigate to your repository's main page
2. Click on **Settings** tab
3. In the left sidebar, under "Security", click on **Secrets and variables** > **Actions**
4. Click on the **New repository secret** button
5. Enter the secret name (use the names listed below)
6. Paste the secret value
7. Click **Add secret**

### Required Secrets

Add the following secrets to your repository:

#### Unity Secrets

- `UNITY_LICENSE` - The entire content of the .ulf file you received
- `UNITY_EMAIL` - Your Unity account email
- `UNITY_PASSWORD` - Your Unity account password

#### Telegram Notification Secrets

- `TELEGRAM_TOKEN` - Your Telegram bot token
- `TELEGRAM_TO` - Your Telegram chat ID

#### iOS/TestFlight Secrets

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

#### Android Signing Secrets

- `ANDROID_KEYSTORE_BASE64` - Your Android keystore file encoded in base64
- `ANDROID_KEYSTORE_PASS` - Your Android keystore password
- `ANDROID_KEY_ALIAS` - Your Android key alias
- `ANDROID_KEY_PASS` - Your Android key password

### Preparing the Android Keystore File for GitHub Actions

To use your Android keystore file with GitHub Actions, you need to encode it as a base64 string:

1. **Locate your keystore file** (typically with .jks or .keystore extension)
2. **Encode the file to base64**:

   **On macOS/Linux**:
   ```bash
   openssl base64 < your_keystore.jks | tr -d '\n' > keystore_base64.txt
   ```

   **On Windows (using Git Bash or PowerShell)**:
   ```bash
   # Git Bash
   openssl base64 < your_keystore.jks | tr -d '\n' > keystore_base64.txt
   
   # PowerShell
   [Convert]::ToBase64String([IO.File]::ReadAllBytes("your_keystore.jks")) | Out-File -NoNewline keystore_base64.txt
   ```

3. **Open the generated `keystore_base64.txt` file** and copy its entire content
4. **Create the `ANDROID_KEYSTORE_BASE64` secret** in your repository with this content

In your GitHub Actions workflow, you'll decode this base64 string back to a keystore file before building your Android app.

### When to Use Environment Secrets Instead

If you have different configurations for development, staging, and production environments, consider using **Environment Secrets**:

1. In your repository, go to **Settings** > **Environments**
2. Create environments like "Development", "Staging", "Production"
3. For each environment, add the environment-specific secrets
4. In your workflow file, specify the environment using the `environment` key

```yaml
jobs:
  deploy-to-staging:
    runs-on: ubuntu-latest
    environment: Staging
    steps:
      # Your workflow steps here
```

### Using Secrets in Workflows

Reference secrets in your workflow files using the `secrets` context:

```yaml
steps:
  - name: Deploy to App Store
    env:
      APPLE_API_KEY: ${{ secrets.APPSTORE_P8 }}
      APPLE_ISSUER_ID: ${{ secrets.APPSTORE_ISSUER_ID }}
    run: fastlane ios release
```

## Next Steps

After configuring the GitHub Actions environment, proceed to [Unity Build Configuration](03-unity-build-configuration.md). 