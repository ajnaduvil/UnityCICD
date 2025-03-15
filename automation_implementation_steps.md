# Unity Build Automation Implementation Steps

This document provides step-by-step instructions for implementing the Unity build automation requirements using GitHub Actions and GameCI.

## 1. Repository Setup

### 1.1 Initialize Git Repository

If starting from scratch with an existing Unity project:

```bash
# Initialize git in your Unity project folder
git init

# Add GitHub remote repository
git remote add origin https://github.com/yourusername/your-repo-name.git
```

### 1.2 Configure Git LFS for Large Unity Files

```bash
# Install Git LFS if you haven't already
# Then initialize it in your repository
git lfs install

# Track common Unity large file types
git lfs track "*.psd"
git lfs track "*.tga"
git lfs track "*.tif"
git lfs track "*.png"
git lfs track "*.jpg"
git lfs track "*.fbx"
git lfs track "*.wav"
git lfs track "*.mp3"
git lfs track "*.mp4"
git lfs track "*.mov"
git lfs track "*.unitypackage"
git lfs track "*.asset"

# Make sure .gitattributes is tracked
git add .gitattributes
```

### 1.3 Add Unity-specific .gitignore

Create a `.gitignore` file in your repository root with Unity-specific ignore patterns:

```bash
# Download Unity-specific .gitignore
curl -o .gitignore https://raw.githubusercontent.com/github/gitignore/main/Unity.gitignore

# Add GameCI-specific ignores
echo "# Ignore temporaries from GameCI" >> .gitignore
echo "/[Aa]rtifacts/" >> .gitignore
echo "/[Cc]odeCoverage/" >> .gitignore
```

### 1.4 Create GitHub Actions Workflow Directory

```bash
# Create the workflows directory
mkdir -p .github/workflows
```

## 2. GitHub Actions Environment Setup

### 2.1 Create Unity Activation Workflow

Create a file named `.github/workflows/activation.yml`:

```yaml
name: Acquire activation file
on:
  workflow_dispatch: {}
jobs:
  activation:
    name: Request manual activation file 🔑
    runs-on: ubuntu-latest
    steps:
      # Request manual activation file
      - name: Request manual activation file
        id: getManualLicenseFile
        uses: game-ci/unity-request-activation-file@v2
        with:
          unityVersion: 2022.3.15f1 # Replace with your Unity version

      # Upload artifact (Unity_v20XX.X.XXXX.alf)
      - name: Upload activation file
        uses: actions/upload-artifact@v3
        with:
          name: ${{ steps.getManualLicenseFile.outputs.activationFile }}
          path: ${{ steps.getManualLicenseFile.outputs.activationFile }}
```

### 2.2 Run Activation Workflow and Get License

1. Push the activation workflow to GitHub
2. Go to your GitHub repository's Actions tab
3. Run the "Acquire activation file" workflow
4. Download the Unity_vXXXX.X.XXf.alf file
5. Visit [Unity License Activation](https://license.unity3d.com/manual) and upload the .alf file
6. Sign in with your Unity account
7. Download the Unity license file (.ulf)

### 2.3 Configure GitHub Secrets

In your GitHub repository, navigate to Settings > Secrets and variables > Actions and add the following secrets:

- `UNITY_LICENSE` - The entire content of the .ulf file you received
- `UNITY_EMAIL` - Your Unity account email
- `UNITY_PASSWORD` - Your Unity account password
- `TELEGRAM_TOKEN` - Your Telegram bot token
- `TELEGRAM_TO` - Your Telegram chat ID

For iOS/TestFlight integration:
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

For Android signing:
- `ANDROID_KEYSTORE_BASE64` - Your Android keystore file encoded in base64
- `ANDROID_KEYSTORE_PASS` - Your Android keystore password
- `ANDROID_KEY_ALIAS` - Your Android key alias
- `ANDROID_KEY_PASS` - Your Android key password

## 3. Unity Build Configuration

### 3.1 Create Main Workflow File

Create a file named `.github/workflows/main.yml`:

```yaml
name: Unity Build Pipeline

on:
  workflow_dispatch:
    inputs:
      unityVersion:
        description: 'Unity Version to use'
        required: true
        default: '2022.3.15f1'
        type: string
      buildVersion:
        description: 'Build version'
        required: true
        default: '1.0.0'
        type: string
      targetPlatforms:
        description: 'Target platforms to build (comma-separated)'
        required: true
        default: 'Android,iOS'
        type: string
      buildConfiguration:
        description: 'Build configuration'
        required: true
        default: 'Release'
        type: choice
        options:
          - Debug
          - Release

jobs:
  setup:
    name: Setup Build Parameters
    runs-on: ubuntu-latest
    outputs:
      buildVersion: ${{ steps.set-version.outputs.buildVersion }}
      androidVersionCode: ${{ steps.set-version.outputs.androidVersionCode }}
      platforms: ${{ steps.set-platforms.outputs.platforms }}
      unityVersion: ${{ steps.set-unity-version.outputs.unityVersion }}
      
    steps:
      - name: Checkout repository
        uses: actions/checkout@v4
        with:
          lfs: true
          
      - name: Set version info
        id: set-version
        run: |
          echo "buildVersion=${{ github.event.inputs.buildVersion }}" >> $GITHUB_OUTPUT
          # Calculate Android version code from semantic version
          VERSION=${{ github.event.inputs.buildVersion }}
          # Convert version like 1.2.3 to integer 1002003
          MAJOR=$(echo $VERSION | cut -d. -f1)
          MINOR=$(echo $VERSION | cut -d. -f2)
          PATCH=$(echo $VERSION | cut -d. -f3)
          VERSION_CODE=$(($MAJOR * 1000000 + $MINOR * 1000 + $PATCH))
          echo "androidVersionCode=$VERSION_CODE" >> $GITHUB_OUTPUT
          
      - name: Set platforms
        id: set-platforms
        run: |
          # Convert comma-separated list to JSON array
          PLATFORMS="${{ github.event.inputs.targetPlatforms }}"
          PLATFORMS_JSON="$(echo $PLATFORMS | sed 's/,/","/g')"
          PLATFORMS_JSON="[\"$PLATFORMS_JSON\"]"
          echo "platforms=$PLATFORMS_JSON" >> $GITHUB_OUTPUT
          
      - name: Set Unity version
        id: set-unity-version
        run: |
          echo "unityVersion=${{ github.event.inputs.unityVersion }}" >> $GITHUB_OUTPUT
```

### 3.2 Add Test Job

Extend the main.yml with the test job:

```yaml
  test:
    name: Test Unity Project
    needs: setup
    runs-on: ubuntu-latest
    steps:
      - name: Checkout repository
        uses: actions/checkout@v4
        with:
          lfs: true
          
      - name: Cache Unity Library
        uses: actions/cache@v3
        with:
          path: Library
          key: Library-Test-${{ hashFiles('Assets/**', 'Packages/**', 'ProjectSettings/**') }}
          restore-keys: |
            Library-Test-
            Library-
            
      - name: Run Unity Tests
        uses: game-ci/unity-test-runner@v4
        env:
          UNITY_LICENSE: ${{ secrets.UNITY_LICENSE }}
          UNITY_EMAIL: ${{ secrets.UNITY_EMAIL }}
          UNITY_PASSWORD: ${{ secrets.UNITY_PASSWORD }}
        with:
          unityVersion: ${{ needs.setup.outputs.unityVersion }}
          githubToken: ${{ secrets.GITHUB_TOKEN }}
          
      - name: Upload Test Results
        uses: actions/upload-artifact@v3
        if: always()
        with:
          name: Test Results
          path: artifacts
```

### 3.3 Add Build Jobs for Android and iOS

Add the Android build job to main.yml:

```yaml
  buildAndroid:
    name: Build Android
    needs: [setup, test]
    if: contains(fromJson(needs.setup.outputs.platforms), 'Android')
    runs-on: ubuntu-latest
    steps:
      - name: Checkout repository
        uses: actions/checkout@v4
        with:
          lfs: true
          
      - name: Cache Unity Library
        uses: actions/cache@v3
        with:
          path: Library
          key: Library-Android-${{ hashFiles('Assets/**', 'Packages/**', 'ProjectSettings/**') }}
          restore-keys: |
            Library-Android-
            Library-
            
      - name: Set up build version
        run: |
          # Update PlayerSettings with version
          echo "Setting build version to ${{ needs.setup.outputs.buildVersion }}"
          sed -i "s/bundleVersion:.*/bundleVersion: ${{ needs.setup.outputs.buildVersion }}/g" ProjectSettings/ProjectSettings.asset
          sed -i "s/AndroidBundleVersionCode:.*/AndroidBundleVersionCode: ${{ needs.setup.outputs.androidVersionCode }}/g" ProjectSettings/ProjectSettings.asset
          
      - name: Build Android
        uses: game-ci/unity-builder@v4
        env:
          UNITY_LICENSE: ${{ secrets.UNITY_LICENSE }}
          UNITY_EMAIL: ${{ secrets.UNITY_EMAIL }}
          UNITY_PASSWORD: ${{ secrets.UNITY_PASSWORD }}
        with:
          unityVersion: ${{ needs.setup.outputs.unityVersion }}
          targetPlatform: Android
          androidAppBundle: true
          androidKeystoreName: user.keystore
          androidKeystoreBase64: ${{ secrets.ANDROID_KEYSTORE_BASE64 }}
          androidKeystorePass: ${{ secrets.ANDROID_KEYSTORE_PASS }}
          androidKeyaliasName: ${{ secrets.ANDROID_KEY_ALIAS }}
          androidKeyaliasPass: ${{ secrets.ANDROID_KEY_PASS }}
          customParameters: -buildConfiguration ${{ github.event.inputs.buildConfiguration }}
          
      - name: Upload Android Build
        uses: actions/upload-artifact@v3
        with:
          name: Android Build
          path: build/Android
```

Add the iOS build job to main.yml:

```yaml
  buildiOS:
    name: Build iOS
    needs: [setup, test]
    if: contains(fromJson(needs.setup.outputs.platforms), 'iOS')
    runs-on: ubuntu-latest
    steps:
      - name: Checkout repository
        uses: actions/checkout@v4
        with:
          lfs: true
          
      - name: Cache Unity Library
        uses: actions/cache@v3
        with:
          path: Library
          key: Library-iOS-${{ hashFiles('Assets/**', 'Packages/**', 'ProjectSettings/**') }}
          restore-keys: |
            Library-iOS-
            Library-
            
      - name: Set up build version
        run: |
          # Update PlayerSettings with version
          echo "Setting build version to ${{ needs.setup.outputs.buildVersion }}"
          sed -i "s/bundleVersion:.*/bundleVersion: ${{ needs.setup.outputs.buildVersion }}/g" ProjectSettings/ProjectSettings.asset
          
      - name: Build iOS
        uses: game-ci/unity-builder@v4
        env:
          UNITY_LICENSE: ${{ secrets.UNITY_LICENSE }}
          UNITY_EMAIL: ${{ secrets.UNITY_EMAIL }}
          UNITY_PASSWORD: ${{ secrets.UNITY_PASSWORD }}
        with:
          unityVersion: ${{ needs.setup.outputs.unityVersion }}
          targetPlatform: iOS
          customParameters: -buildConfiguration ${{ github.event.inputs.buildConfiguration }}
          
      - name: Upload iOS Build
        uses: actions/upload-artifact@v3
        with:
          name: iOS Build
          path: build/iOS
```

## 4. TestFlight Integration

### 4.1 Create Fastlane Configuration

Create a `fastlane` directory in your repository:

```bash
mkdir -p fastlane
```

Create a `fastlane/Fastfile` with the following content:

```ruby
default_platform(:ios)

platform :ios do
  desc "Build the iOS app"
  lane :build do
    xcodebuild(
      project: "#{ENV['IOS_BUILD_PATH']}/Unity-iPhone.xcodeproj",
      scheme: "Unity-iPhone",
      configuration: "Release",
      clean: true,
      build: true,
      output_directory: "#{ENV['IOS_BUILD_PATH']}/build",
      sdk: "iphoneos"
    )
  end

  desc "Push a new beta build to TestFlight"
  lane :beta do
    api_key = app_store_connect_api_key(
      key_id: ENV["APPSTORE_KEY_ID"],
      issuer_id: ENV["APPSTORE_ISSUER_ID"],
      key_content: ENV["APPSTORE_P8"],
    )

    match(
      git_url: ENV["MATCH_REPOSITORY"],
      type: "appstore",
      app_identifier: ENV["IOS_BUNDLE_ID"],
      readonly: true,
      api_key: api_key
    )

    update_code_signing_settings(
      use_automatic_signing: false,
      path: "#{ENV['IOS_BUILD_PATH']}/Unity-iPhone.xcodeproj",
      code_sign_identity: "iPhone Distribution",
      team_id: ENV["APPLE_TEAM_ID"],
      bundle_identifier: ENV["IOS_BUNDLE_ID"],
      profile_name: "match AppStore #{ENV['IOS_BUNDLE_ID']}"
    )

    build_app(
      project: "#{ENV['IOS_BUILD_PATH']}/Unity-iPhone.xcodeproj",
      scheme: "Unity-iPhone",
      output_directory: "#{ENV['IOS_BUILD_PATH']}/build",
      output_name: "#{ENV['PROJECT_NAME']}.ipa",
      export_method: "app-store",
      export_options: {
        provisioningProfiles: { 
          ENV["IOS_BUNDLE_ID"] => "match AppStore #{ENV['IOS_BUNDLE_ID']}"
        }
      }
    )

    upload_to_testflight(
      api_key: api_key,
      skip_waiting_for_build_processing: true,
      apple_id: ENV["APPLE_CONNECT_EMAIL"],
      changelog: "Build #{ENV['buildVersion']} uploaded via CI"
    )
  end
end
```

Create a `fastlane/Appfile` with the following content:

```ruby
app_identifier(ENV["IOS_BUNDLE_ID"])
apple_id(ENV["APPLE_CONNECT_EMAIL"])
team_id(ENV["APPLE_TEAM_ID"])
```

### 4.2 Add TestFlight Deployment Job

Add this job to your main.yml file:

```yaml
  deployToTestFlight:
    name: Deploy to TestFlight
    runs-on: macos-latest
    needs: [setup, buildiOS]
    if: contains(fromJson(needs.setup.outputs.platforms), 'iOS')
    steps:
      - name: Checkout Repository
        uses: actions/checkout@v4

      - name: Install Ruby and Fastlane
        uses: ruby/setup-ruby@v1
        with:
          ruby-version: '3.0'
          bundler-cache: true

      - name: Install Fastlane Plugins
        run: |
          gem install fastlane
          fastlane add_plugin match

      - name: Download iOS Build
        uses: actions/download-artifact@v3
        with:
          name: iOS Build
          path: build/iOS

      - name: Fix File Permissions
        run: |
          find build/iOS -type f -name "**.sh" -exec chmod +x {} \;
          find build/iOS -type f -iname "usymtool" -exec chmod +x {} \;

      - name: Deploy to TestFlight
        env:
          APPLE_CONNECT_EMAIL: ${{ secrets.APPLE_CONNECT_EMAIL }}
          APPLE_DEVELOPER_EMAIL: ${{ secrets.APPLE_DEVELOPER_EMAIL }}
          APPLE_TEAM_ID: ${{ secrets.APPLE_TEAM_ID }}
          MATCH_REPOSITORY: ${{ secrets.MATCH_REPOSITORY }}
          MATCH_DEPLOY_KEY: ${{ secrets.MATCH_DEPLOY_KEY }}
          MATCH_PASSWORD: ${{ secrets.MATCH_PASSWORD }}
          APPSTORE_ISSUER_ID: ${{ secrets.APPSTORE_ISSUER_ID }}
          APPSTORE_KEY_ID: ${{ secrets.APPSTORE_KEY_ID }}
          APPSTORE_P8: ${{ secrets.APPSTORE_P8 }}
          IOS_BUILD_PATH: ${{ github.workspace }}/build/iOS
          IOS_BUNDLE_ID: ${{ secrets.IOS_BUNDLE_ID }}
          PROJECT_NAME: YourProjectName
          buildVersion: ${{ needs.setup.outputs.buildVersion }}
        run: |
          eval "$(ssh-agent -s)"
          ssh-add - <<< "${MATCH_DEPLOY_KEY}"
          cd fastlane
          fastlane ios beta

      - name: Send Telegram Notification
        uses: appleboy/telegram-action@master
        with:
          to: ${{ secrets.TELEGRAM_TO }}
          token: ${{ secrets.TELEGRAM_TOKEN }}
          message: |
            🎉 iOS build v${{ needs.setup.outputs.buildVersion }} successfully uploaded to TestFlight!
            
            Project: ${{ github.repository }}
            Branch: ${{ github.ref }}
            
            Check your TestFlight dashboard for availability.
```

## 5. Telegram Notifications

### 5.1 Create a Telegram Bot

1. Open Telegram and search for @BotFather
2. Send `/newbot` to create a new bot
3. Follow the instructions to name your bot
4. Save the bot token provided
5. Start a chat with your bot and send it a message

### 5.2 Get Chat ID

1. Send a message to your bot 
2. Access `https://api.telegram.org/bot<YOUR_BOT_TOKEN>/getUpdates`
3. Find the `chat.id` value in the response
4. Save this value as your `TELEGRAM_TO` secret

### 5.3 Add Notification Jobs

Add notifications for build start and completion to main.yml:

```yaml
  notifyBuildStart:
    name: Notify Build Start
    runs-on: ubuntu-latest
    needs: setup
    steps:
      - name: Send Telegram Notification
        uses: appleboy/telegram-action@master
        with:
          to: ${{ secrets.TELEGRAM_TO }}
          token: ${{ secrets.TELEGRAM_TOKEN }}
          message: |
            🚀 Build process started!
            
            Project: ${{ github.repository }}
            Version: ${{ needs.setup.outputs.buildVersion }}
            Platforms: ${{ github.event.inputs.targetPlatforms }}
            Configuration: ${{ github.event.inputs.buildConfiguration }}
            
            This build was triggered by ${{ github.actor }}.
```

Add a notification for Android build completion:

```yaml
  notifyAndroidBuildComplete:
    name: Notify Android Build Complete
    runs-on: ubuntu-latest
    needs: [setup, buildAndroid]
    if: contains(fromJson(needs.setup.outputs.platforms), 'Android')
    steps:
      - name: Send Telegram Notification
        uses: appleboy/telegram-action@master
        with:
          to: ${{ secrets.TELEGRAM_TO }}
          token: ${{ secrets.TELEGRAM_TOKEN }}
          message: |
            ✅ Android build v${{ needs.setup.outputs.buildVersion }} completed successfully!
            
            Project: ${{ github.repository }}
            Branch: ${{ github.ref }}
            
            The build artifact is available in GitHub Actions.
```

## 6. Performance Optimization

### 6.1 Optimize Unity Cache

Modify the cache configuration in your workflow for better performance:

```yaml
- name: Cache Unity Library
  uses: actions/cache@v3
  with:
    path: |
      Library
      Temp/UnityLockfile
    key: Library-${{ matrix.targetPlatform }}-${{ hashFiles('Assets/**', 'Packages/**', 'ProjectSettings/**') }}
    restore-keys: |
      Library-${{ matrix.targetPlatform }}-
      Library-
```

### 6.2 Parallel Builds

Update the main.yml to include a matrix for parallel builds:

```yaml
  build:
    name: Build for ${{ matrix.targetPlatform }}
    needs: [setup, test]
    runs-on: ubuntu-latest
    strategy:
      fail-fast: false
      matrix:
        targetPlatform:
          - Android
          - iOS
        include:
          - targetPlatform: Android
            buildPath: build/Android
          - targetPlatform: iOS
            buildPath: build/iOS
    if: contains(fromJson(needs.setup.outputs.platforms), matrix.targetPlatform)
    steps:
      - name: Checkout repository
        uses: actions/checkout@v4
        with:
          lfs: true
          
      - name: Cache Unity Library
        uses: actions/cache@v3
        with:
          path: Library
          key: Library-${{ matrix.targetPlatform }}-${{ hashFiles('Assets/**', 'Packages/**', 'ProjectSettings/**') }}
          restore-keys: |
            Library-${{ matrix.targetPlatform }}-
            Library-
            
      - name: Set up build version
        run: |
          # Update PlayerSettings with version
          echo "Setting build version to ${{ needs.setup.outputs.buildVersion }}"
          sed -i "s/bundleVersion:.*/bundleVersion: ${{ needs.setup.outputs.buildVersion }}/g" ProjectSettings/ProjectSettings.asset
          if [ "${{ matrix.targetPlatform }}" == "Android" ]; then
            sed -i "s/AndroidBundleVersionCode:.*/AndroidBundleVersionCode: ${{ needs.setup.outputs.androidVersionCode }}/g" ProjectSettings/ProjectSettings.asset
          fi
          
      - name: Build
        uses: game-ci/unity-builder@v4
        env:
          UNITY_LICENSE: ${{ secrets.UNITY_LICENSE }}
          UNITY_EMAIL: ${{ secrets.UNITY_EMAIL }}
          UNITY_PASSWORD: ${{ secrets.UNITY_PASSWORD }}
        with:
          unityVersion: ${{ needs.setup.outputs.unityVersion }}
          targetPlatform: ${{ matrix.targetPlatform }}
          androidAppBundle: ${{ matrix.targetPlatform == 'Android' }}
          androidKeystoreName: ${{ matrix.targetPlatform == 'Android' && 'user.keystore' || '' }}
          androidKeystoreBase64: ${{ matrix.targetPlatform == 'Android' && secrets.ANDROID_KEYSTORE_BASE64 || '' }}
          androidKeystorePass: ${{ matrix.targetPlatform == 'Android' && secrets.ANDROID_KEYSTORE_PASS || '' }}
          androidKeyaliasName: ${{ matrix.targetPlatform == 'Android' && secrets.ANDROID_KEY_ALIAS || '' }}
          androidKeyaliasPass: ${{ matrix.targetPlatform == 'Android' && secrets.ANDROID_KEY_PASS || '' }}
          customParameters: -buildConfiguration ${{ github.event.inputs.buildConfiguration }}
          
      - name: Upload Build
        uses: actions/upload-artifact@v3
        with:
          name: ${{ matrix.targetPlatform }} Build
          path: ${{ matrix.buildPath }}
```

## 7. Maintenance and Cleanup

### 7.1 Add Cleanup Job

Add a cleanup job to manage storage:

```yaml
  cleanup:
    name: Cleanup
    runs-on: ubuntu-latest
    needs: [deployToTestFlight, notifyAndroidBuildComplete, notifyBuildStart]
    if: always()
    steps:
      - name: Delete Old Artifacts
        uses: geekyeggo/delete-artifact@v2
        with:
          name: |
            Test Results
            Android Build
            iOS Build
          failOnError: false
```

### 7.2 Update Pipeline Monitoring

Add a job status summary:

```yaml
  workflow-summary:
    name: Workflow Summary
    runs-on: ubuntu-latest
    needs: [setup, test, buildAndroid, buildiOS, deployToTestFlight]
    if: always()
    steps:
      - name: Status Check
        id: status
        run: |
          echo "test_status=${{ needs.test.result }}" >> $GITHUB_OUTPUT
          echo "android_status=${{ needs.buildAndroid.result }}" >> $GITHUB_OUTPUT
          echo "ios_status=${{ needs.buildiOS.result }}" >> $GITHUB_OUTPUT
          echo "testflight_status=${{ needs.deployToTestFlight.result }}" >> $GITHUB_OUTPUT
          
      - name: Send Summary Notification
        uses: appleboy/telegram-action@master
        with:
          to: ${{ secrets.TELEGRAM_TO }}
          token: ${{ secrets.TELEGRAM_TOKEN }}
          message: |
            📊 Build Pipeline Summary
            
            Project: ${{ github.repository }}
            Version: ${{ needs.setup.outputs.buildVersion }}
            
            Tests: ${{ steps.status.outputs.test_status || 'Not Run' }}
            Android Build: ${{ steps.status.outputs.android_status || 'Not Run' }}
            iOS Build: ${{ steps.status.outputs.ios_status || 'Not Run' }}
            TestFlight Deployment: ${{ steps.status.outputs.testflight_status || 'Not Run' }}
            
            Workflow: ${{ github.server_url }}/${{ github.repository }}/actions/runs/${{ github.run_id }}
```

## 8. Usage Guide

### 8.1 Running the Workflow

1. Go to your GitHub repository
2. Navigate to the "Actions" tab
3. Select "Unity Build Pipeline" from the list of workflows
4. Click "Run workflow"
5. Fill in the parameters:
   - Unity Version: The Unity version to use (e.g., 2022.3.15f1)
   - Build Version: The version number for this build (e.g., 1.0.0)
   - Target Platforms: Select which platforms to build for (Android, iOS)
   - Build Configuration: Choose between Debug or Release
6. Click "Run workflow" to start the build process

### 8.2 Monitoring and Troubleshooting

1. Build progress can be monitored in the GitHub Actions tab
2. Build notifications will be sent to the configured Telegram chat
3. For iOS builds, check TestFlight for the uploaded build
4. For troubleshooting, examine the logs in the GitHub Actions run
