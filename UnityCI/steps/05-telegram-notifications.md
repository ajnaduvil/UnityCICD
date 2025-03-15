# Step 5: Telegram Notifications

This step covers setting up Telegram notifications for build status updates.

## 5.1 Create a Telegram Bot

1. Open Telegram and search for @BotFather
2. Send `/newbot` to create a new bot
3. Follow the instructions to name your bot
4. Save the bot token provided (will look like `123456789:ABCdefGhIJKlmNoPQRsTUVwxyZ`)
5. Start a chat with your bot and send it a message

## 5.2 Get Chat ID

1. Send a message to your bot 
2. Access `https://api.telegram.org/bot<YOUR_BOT_TOKEN>/getUpdates`
3. Find the `chat.id` value in the response (will be a number like `65382999`)
4. Save this value as your `TELEGRAM_TO` secret in GitHub repository settings

## 5.3 Add Notification Jobs

Add a notification for build start to your main.yml file:

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

Add a notification for Android build completion to your main.yml file:

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

## 5.4 Add Notification with Image (Optional)

If you want to include an image in your notification (e.g., a screenshot or app icon), you can do it as follows:

```yaml
  notifyWithImage:
    name: Notify with Image
    runs-on: ubuntu-latest
    needs: [setup]
    steps:
      - name: Checkout Repository
        uses: actions/checkout@v4
        
      - name: Send Telegram Notification with Image
        uses: appleboy/telegram-action@master
        with:
          to: ${{ secrets.TELEGRAM_TO }}
          token: ${{ secrets.TELEGRAM_TOKEN }}
          message: |
            📱 New build initiated!
            
            Project: ${{ github.repository }}
            Version: ${{ needs.setup.outputs.buildVersion }}
          photo: ./path/to/your/icon.png
```

## Next Steps

After setting up Telegram notifications, proceed to [Performance Optimization](06-performance-optimization.md). 