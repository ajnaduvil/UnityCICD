# Step 1: Repository Setup

This step covers the initial Git repository setup for your Unity project.

## 1.1 Initialize Git Repository

If starting from scratch with an existing Unity project:

```bash
# Initialize git in your Unity project folder
git init

# Add GitHub remote repository
git remote add origin https://github.com/yourusername/your-repo-name.git
```

## 1.2 Configure Git LFS for Large Unity Files

Unity projects contain large binary files that should be tracked using Git LFS (Large File Storage).

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

## 1.3 Add Unity-specific .gitignore

Create a `.gitignore` file in your repository root with Unity-specific ignore patterns:

```bash
# Download Unity-specific .gitignore
curl -o .gitignore https://raw.githubusercontent.com/github/gitignore/main/Unity.gitignore

# Add GameCI-specific ignores
echo "# Ignore temporaries from GameCI" >> .gitignore
echo "/[Aa]rtifacts/" >> .gitignore
echo "/[Cc]odeCoverage/" >> .gitignore
```

## 1.4 Create GitHub Actions Workflow Directory

Set up the directory structure for GitHub Actions workflows:

```bash
# Create the workflows directory
mkdir -p .github/workflows
```

## Next Steps

After completing the repository setup, proceed to [GitHub Actions Environment Setup](02-github-actions-setup.md). 