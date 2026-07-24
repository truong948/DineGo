import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    port: 3000,
    open: true,
    proxy: {
      '/api': {
        target: 'https://localhost:7123',
        changeOrigin: true,
        secure: false, // Disables SSL cert verification for local dev backend
      },
    },
  },
});
