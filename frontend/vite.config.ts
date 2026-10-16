import { fileURLToPath, URL } from 'node:url'
import tailwindcss from '@tailwindcss/vite'
import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'

// Tailwind v4 entra como plugin. O tema mora em src/index.css, sem tailwind.config.js.
// O alias @ aponta para src/ e precisa do mesmo caminho no tsconfig.app.json.
export default defineConfig({
  plugins: [react(), tailwindcss()],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url)),
    },
  },
})
