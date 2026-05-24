# Makura Ramen House - React Application

A modern, responsive React application for Makura Ramen House, converted from HTML/CSS to React with Tailwind CSS.

## 🚀 Features

- **Fully Responsive Design** - Works seamlessly on desktop, tablet, and mobile devices
- **React Router** - Smooth navigation between pages
- **Tailwind CSS** - Modern utility-first CSS framework
- **Component-Based Architecture** - Reusable and maintainable components
- **Interactive UI** - Hover effects, transitions, and animations

## 📁 Project Structure

```
src/
├── components/
│   ├── Header.jsx       # Navigation header with burger menu
│   └── Footer.jsx       # Footer with business info
├── pages/
│   ├── Home.jsx         # Landing page
│   ├── About.jsx        # About page
│   ├── Menu.jsx         # Menu page
│   ├── Promo.jsx        # Promotions page
│   ├── Contact.jsx      # Contact page with form
│   ├── Login.jsx        # Login page
│   └── Signup.jsx       # Signup page
├── App.jsx              # Main app with routing
├── main.jsx             # Entry point
└── index.css            # Global styles with Tailwind
```

## 🛠️ Technologies Used

- **React 19.2.5** - JavaScript library for building user interfaces
- **React Router DOM** - Declarative routing for React
- **Tailwind CSS 3.4.19** - Utility-first CSS framework
- **Vite 8.0.10** - Next generation frontend tooling
- **Google Fonts** - Poppins and Montserrat fonts

## 📦 Installation

1. **Clone the repository** (if applicable)
   ```bash
   git clone <repository-url>
   cd makura-ramen-final
   ```

2. **Install dependencies**
   ```bash
   npm install
   ```

3. **Start the development server**
   ```bash
   npm run dev
   ```

4. **Open your browser**
   - Navigate to `http://localhost:5173` (or the port shown in terminal)

## 🎨 Pages

### 1. Home (`/`)
- Hero section with featured ramen bowl
- Best Selling Bowls showcase
- Promotional offers
- Barkada Feast section

### 2. About (`/about`)
- Company story
- Vision & Mission
- What makes the ramen unique

### 3. Menu (`/menu`)
- Menu categories
- Budget Meals
- Platters
- Snacks
- Bento Meals
- Ramen selection

### 4. Promo (`/promo`)
- Unlimited Promos
- Barkada Feast bundles
- Special offers with pricing and deadlines

### 5. Contact (`/contact`)
- Contact information
- Contact form
- Google Maps integration
- Business hours

### 6. Login (`/login`)
- Email and password login
- Password visibility toggle
- Remember me option
- Link to signup

### 7. Signup (`/signup`)
- Full name, email, password fields
- Password confirmation
- Terms & conditions checkbox
- Link to login

## 🎯 Key Features

### Responsive Navigation
- Desktop: Full navigation menu
- Tablet/Mobile: Burger menu with slide-out panel
- Smooth transitions and animations

### Interactive Elements
- Hover effects on cards and images
- Form validation
- Password visibility toggles
- Smooth page transitions

### Design System
- **Colors**: Golden gradient theme (#f2e074, #635c31)
- **Fonts**: Poppins (headings), Montserrat (body)
- **Spacing**: Consistent padding and margins
- **Shadows**: Subtle shadows for depth

## 🔧 Build Commands

```bash
# Development
npm run dev

# Build for production
npm run build

# Preview production build
npm run preview

# Lint code
npm run lint
```

## 📱 Responsive Breakpoints

- **Desktop**: > 1024px
- **Tablet**: 768px - 1024px
- **Mobile**: < 768px

## 🖼️ Image Assets

All images are located in the `/public` folder:
- Logo: `Group-39@2x.png`
- Ramen bowls: Various product images
- Promotional images: `unli1@2x.png`, `unli2@2x.png`, etc.
- Barkada feast images: `image-1@2x.png`, `image-2@2x.png`, etc.

## 🎨 Customization

### Colors
Edit `tailwind.config.js` to customize the color palette:
```javascript
colors: {
  'golden': '#f2e074',
  'golden-dark': '#635c31',
  'firebrick': '#a52a2a',
  'sienna': '#a0522d',
}
```

### Fonts
Fonts are imported in `src/index.css`:
```css
@import url('https://fonts.googleapis.com/css2?family=Poppins:wght@400;500;600;700;800&display=swap');
@import url('https://fonts.googleapis.com/css2?family=Montserrat:wght@500;600;700;800&display=swap');
```

## 🐛 Troubleshooting

### Port Already in Use
If port 5173 is in use, Vite will automatically try another port (e.g., 5174).

### Images Not Loading
Ensure all images are in the `/public` folder and paths start with `/public/`.

### Tailwind Styles Not Working
1. Make sure `tailwind.config.js` is properly configured
2. Check that `@tailwind` directives are in `src/index.css`
3. Restart the dev server

## 📄 License

This project is for Makura Ramen House.

## 👥 Contact

For questions or support, contact Makura Ramen House:
- **Phone**: 09534879391
- **Location**: Gingoog City, Misamis Oriental
- **Hours**: Mon-Fri 11am-10pm, Sat-Sun 11am-12am

---

Built with ❤️ using React and Tailwind CSS
