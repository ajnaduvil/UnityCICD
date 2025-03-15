# Step 7: Maintenance and Cleanup

This step covers maintaining your build pipeline and implementing cleanup procedures to keep your GitHub Actions workflow efficient and manageable.

## 7.1 Add Cleanup Job

Add a cleanup job to your main.yml file to manage storage and clean up artifacts:

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

This job:
- Runs after all other jobs (successful or not) due to `if: always()`
- Deletes artifacts to save storage space
- Ignores failures with `failOnError: false`

## 7.2 Add Workflow Summary Job

Add a job to summarize the workflow results and send a notification:

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

## 7.3 Set Up Regular Maintenance

Add a scheduled workflow for regular maintenance tasks:

Create a file named `.github/workflows/maintenance.yml`:

```yaml
name: Maintenance

on:
  schedule:
    - cron: '0 0 * * 0'  # Run at midnight every Sunday

jobs:
  cleanup-old-runs:
    name: Clean up old workflow runs
    runs-on: ubuntu-latest
    steps:
      - name: Delete old workflow runs
        uses: Mattraks/delete-workflow-runs@v2
        with:
          token: ${{ github.token }}
          repository: ${{ github.repository }}
          retain_days: 30
          keep_minimum_runs: 10
```

## 7.4 License Return (Optional)

For professional Unity licenses, you might want to return the license after your build to free up the license seat:

```yaml
  return-license:
    name: Return Unity License
    needs: [buildAndroid, buildiOS]
    if: always()
    runs-on: ubuntu-latest
    steps:
      - name: Return license
        uses: game-ci/unity-return-license@v2
        if: always()
```

## Next Steps

After setting up maintenance and cleanup procedures, proceed to [Usage Guide](08-usage-guide.md). 