import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  base: '/makura-ramen-house/', // Replace 'makura-ramen-house' with your GitHub repository name
})
