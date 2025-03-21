# Step 3: Unity Build Configuration

This step covers setting up the main GitHub Actions workflow file for building your Unity project.

## 3.1 Create Main Workflow File

Create a file named `.github/workflows/main.yml` with the following content:

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

## 3.2 Add Test Job

Add the test job to your main.yml file:

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

## 3.3 Add Build Jobs for Android and iOS

Add the Android build job to your main.yml file:

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

Add the iOS build job to your main.yml file:

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

## Next Steps

After configuring the Unity build process, proceed to [TestFlight Integration](04-testflight-integration.md). 