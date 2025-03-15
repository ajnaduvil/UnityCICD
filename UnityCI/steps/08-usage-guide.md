# Step 8: Usage Guide

This step provides instructions on how to use, monitor, and troubleshoot your Unity build automation pipeline.

## 8.1 Running the Workflow

To trigger a build using your workflow:

1. Go to your GitHub repository
2. Navigate to the "Actions" tab
3. Select "Unity Build Pipeline" from the list of workflows
4. Click "Run workflow" button on the right side
5. Fill in the parameters:
   - Unity Version: The Unity version to use (e.g., 2022.3.15f1)
   - Build Version: The version number for this build (e.g., 1.0.0)
   - Target Platforms: Comma-separated list of platforms to build for (e.g., Android,iOS)
   - Build Configuration: Choose between Debug or Release
6. Click "Run workflow" to start the build process

![Workflow Run Interface](../images/workflow-run.png)

## 8.2 Monitoring Builds

### GitHub Actions Tab

- The Actions tab shows all workflow runs, their status, and duration
- Click on a workflow run to see detailed logs
- Each job in the workflow can be expanded to see step-by-step logs

### Telegram Notifications

If you've set up Telegram notifications as described in Step 5:

- You'll receive a notification when a build starts
- You'll receive notifications when builds complete
- You'll receive a summary of all jobs at the end of the workflow

## 8.3 Accessing Build Artifacts

To download the build artifacts:

1. Go to the completed workflow run
2. Scroll down to the "Artifacts" section
3. Click on the artifact you want to download (e.g., "Android Build" or "iOS Build")
4. The artifact will be downloaded as a zip file

For iOS builds deployed to TestFlight:

1. Log in to App Store Connect
2. Go to "TestFlight" section
3. Find your app and select the build

## 8.4 Troubleshooting

### Common Issues and Solutions

| Issue | Possible Solution |
|-------|-------------------|
| Unity license activation fails | Check that your Unity credentials and license file are correctly set in GitHub Secrets |
| Android build fails | Verify that your Android keystore information is correct in GitHub Secrets |
| iOS TestFlight upload fails | Check App Store Connect API key and certificate configuration in fastlane |
| Cache not working | Make sure the cache key is correctly set and check if GitHub Actions has cache size limits |
| Build taking too long | Optimize your Unity project, use cache effectively, and consider incremental builds |

### Log Reading Tips

- GitHub Actions logs are searchable - use the search box to find specific errors
- Expand the job that failed to see detailed logs
- Unity build logs are usually verbose - look for lines with "Error:" or "Exception:"
- For fastlane errors, check the fastlane logs in the "Deploy to TestFlight" job

### Getting Help

If you encounter issues:

- Check the [GameCI documentation](https://game.ci/docs)
- Visit the [GameCI Discord](https://discord.gg/WyPN5r9) for community help
- For GitHub Actions issues, refer to [GitHub Actions documentation](https://docs.github.com/en/actions)
- For Unity-specific build issues, check the [Unity forums](https://forum.unity.com/)

## 8.5 Maintenance

Regular maintenance tasks:

- Update the Unity version in your workflow as new versions are released
- Update GitHub Action versions periodically
- Clean up old artifacts and workflow runs
- Review and optimize your build process
- Test your pipeline with new Unity project updates

## Conclusion

You now have a complete CI/CD pipeline for your Unity project using GitHub Actions and GameCI. This setup allows you to:

- Build your Unity project for multiple platforms
- Run tests automatically
- Deploy to TestFlight for iOS
- Receive notifications via Telegram
- Optimize and maintain your build process

For additional platforms or customization, refer to the documentation of the specific tools used in this setup. 