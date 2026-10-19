import { fileURLToPath, URL } from 'node:url'
import { defineConfig } from 'vitest/config'

// Arquivo separado do vite.config.ts para o teste não carregar o plugin do Tailwind.
export default defineConfig({
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url)),
    },
  },
  test: {
    globals: true,
    environment: 'jsdom',
    setupFiles: ['./src/test/setup.ts'],
    include: ['src/**/*.test.{ts,tsx}'],
    css: false,
    restoreMocks: true,
  },
})
