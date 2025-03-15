# Step 4: TestFlight Integration

This step covers setting up TestFlight integration for distributing iOS builds.

## 4.1 Create Fastlane Configuration

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

## 4.2 Add TestFlight Deployment Job

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

## Next Steps

After setting up TestFlight integration, proceed to [Telegram Notifications](05-telegram-notifications.md). 