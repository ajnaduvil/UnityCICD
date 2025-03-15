# Step 6: Performance Optimization

This step covers optimizing the build pipeline for better performance and efficiency.

## 6.1 Optimize Unity Cache

Optimize the cache configuration in your workflow for better performance:

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

This configuration:
- Caches both the Library folder and the Unity lockfile
- Uses a hash of your project files as part of the cache key
- Provides fallback restore keys for partial matches

## 6.2 Parallel Builds

Update your main.yml file to include a matrix for parallel builds:

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

This configuration:
- Uses a matrix strategy to build for multiple platforms in parallel
- Sets platform-specific parameters for each build
- Uploads build artifacts with platform-specific names

## 6.3 Timeout Configuration

Add timeout limits to your jobs to prevent builds from running indefinitely:

```yaml
  buildAndroid:
    name: Build Android
    needs: [setup, test]
    if: contains(fromJson(needs.setup.outputs.platforms), 'Android')
    runs-on: ubuntu-latest
    timeout-minutes: 60  # Set a 1-hour timeout
    steps:
      # ... existing steps ...
```

## 6.4 Fetch Optimization

Optimize the repository checkout to improve performance:

```yaml
- name: Checkout repository
  uses: actions/checkout@v4
  with:
    lfs: true
    fetch-depth: 1  # Shallow clone - only get the latest commit
```

## Next Steps

After optimizing your build pipeline, proceed to [Maintenance and Cleanup](07-maintenance-cleanup.md). 