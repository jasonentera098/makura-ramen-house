# Makura Ramen House - GitHub Deployment Guide

## Prerequisites
- GitHub account
- Git installed on your computer
- Node.js and npm installed

## Step-by-Step Deployment Instructions

### 1. Create a New GitHub Repository
1. Go to [GitHub](https://github.com) and log in
2. Click the **"+"** icon in the top right corner
3. Select **"New repository"**
4. Repository settings:
   - **Repository name**: `makura-ramen-house` (or any name you prefer)
   - **Description**: "Makura Ramen House - Japanese Restaurant Website"
   - **Visibility**: Choose Public or Private
   - **DO NOT** initialize with README, .gitignore, or license (we already have these)
5. Click **"Create repository"**

### 2. Initialize Git and Push to GitHub

Open your terminal/command prompt in the project folder and run these commands:

```bash
# Initialize git repository
git init

# Add all files to staging
git add .

# Create first commit
git commit -m "Initial commit: Makura Ramen House website"

# Add your GitHub repository as remote (replace YOUR_USERNAME and YOUR_REPO_NAME)
git remote add origin https://github.com/YOUR_USERNAME/YOUR_REPO_NAME.git

# Push to GitHub
git branch -M main
git push -u origin main
```

**Example:**
If your GitHub username is `jasonentera` and repository name is `makura-ramen-house`:
```bash
git remote add origin https://github.com/jasonentera/makura-ramen-house.git
```

### 3. Update vite.config.js

**IMPORTANT:** Update the `base` path in `vite.config.js` to match your repository name:

```javascript
export default defineConfig({
  plugins: [react()],
  base: '/YOUR_REPO_NAME/', // Replace with your actual repository name
})
```

**Example:**
If your repository is named `makura-ramen-house`:
```javascript
base: '/makura-ramen-house/',
```

### 4. Deploy to GitHub Pages

Run this command to build and deploy:

```bash
npm run deploy
```

This will:
- Build your project
- Create a `gh-pages` branch
- Deploy the built files to GitHub Pages

### 5. Enable GitHub Pages

1. Go to your GitHub repository
2. Click **"Settings"** tab
3. Scroll down to **"Pages"** in the left sidebar
4. Under **"Source"**, select:
   - Branch: `gh-pages`
   - Folder: `/ (root)`
5. Click **"Save"**
6. Wait 1-2 minutes for deployment

### 6. Access Your Website

Your website will be available at:
```
https://YOUR_USERNAME.github.io/YOUR_REPO_NAME/
```

**Example:**
```
https://jasonentera.github.io/makura-ramen-house/
```

## Updating Your Website

Whenever you make changes to your website:

```bash
# Stage all changes
git add .

# Commit changes
git commit -m "Description of your changes"

# Push to GitHub
git push origin main

# Deploy updated version to GitHub Pages
npm run deploy
```

## Important Notes

### EmailJS Configuration
Before deploying, make sure you've configured EmailJS in `src/pages/Order.jsx`:
- Replace `YOUR_SERVICE_ID` with your EmailJS Service ID
- Replace `YOUR_TEMPLATE_ID` with your EmailJS Template ID
- Replace `YOUR_PUBLIC_KEY` with your EmailJS Public Key

See `EMAILJS_SETUP.md` for detailed instructions.

### Image Paths
All images are in the `/public` folder and should work correctly with the deployment.

### Custom Domain (Optional)
If you want to use a custom domain:
1. Create a file named `CNAME` in the `public` folder
2. Add your domain name (e.g., `makuraramen.com`)
3. Configure your domain's DNS settings to point to GitHub Pages
4. Redeploy: `npm run deploy`

## Troubleshooting

### Issue: Blank page after deployment
**Solution:** Make sure the `base` path in `vite.config.js` matches your repository name exactly.

### Issue: Images not loading
**Solution:** Check that all image paths start with `/public/` and images exist in the public folder.

### Issue: 404 errors on page refresh
**Solution:** This is normal for GitHub Pages with React Router. Users should navigate using the website's links.

### Issue: Changes not showing
**Solution:** 
1. Clear your browser cache (Ctrl+Shift+R or Cmd+Shift+R)
2. Wait a few minutes for GitHub Pages to update
3. Check if deployment was successful: `git log --oneline gh-pages`

## Repository Structure

```
makura-ramen-house/
├── public/              # Static assets (images, icons)
├── src/
│   ├── Components/      # Reusable components (Header, Footer)
│   ├── pages/          # Page components (Home, Menu, Order, etc.)
│   ├── assets/         # Additional assets
│   ├── App.jsx         # Main app component
│   └── main.jsx        # Entry point
├── .gitignore          # Git ignore rules
├── package.json        # Dependencies and scripts
├── vite.config.js      # Vite configuration
└── README.md           # Project documentation

```

## Support

If you encounter any issues:
1. Check the GitHub Actions tab in your repository for deployment logs
2. Review the browser console for errors (F12)
3. Verify all configuration files are correct

## Security Notes

- Never commit sensitive information (API keys, passwords)
- EmailJS keys are safe to include as they're meant for client-side use
- Keep your GitHub repository private if it contains sensitive business data

---

**Congratulations!** Your Makura Ramen House website is now live on GitHub Pages! 🍜✨
