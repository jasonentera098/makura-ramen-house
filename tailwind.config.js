/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      screens: {
        'xs': '450px',
        // sm: 640px (default)
        // md: 768px (default)
        // lg: 1024px (default)
        // xl: 1280px (default)
      },
      colors: {
        'golden': '#f2e074',
        'golden-dark': '#635c31',
        'firebrick': '#a52a2a',
        'sienna': '#a0522d',
      },
      fontFamily: {
        'poppins': ['Poppins', 'sans-serif'],
        'montserrat': ['Montserrat', 'sans-serif'],
      },
      backgroundImage: {
        'gradient-golden': 'linear-gradient(180deg, #f2e074, #635c31)',
        'gradient-text': 'linear-gradient(90deg, #a52a2a, #000000ff)',
      },
    },
  },
  plugins: [],
}

