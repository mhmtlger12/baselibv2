import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'
import path from 'node:path'

// Standalone React application configuration. The previous Figma Make plugins
// depended on files that are intentionally not part of this repository.
export default defineConfig({
  plugins: [react(), tailwindcss()],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src'),
    },
  },
  server: {
    host: '0.0.0.0',
    port: 8443,
    strictPort: true,
  },
  preview: {
    host: '0.0.0.0',
    port: 8443,
  },
})
