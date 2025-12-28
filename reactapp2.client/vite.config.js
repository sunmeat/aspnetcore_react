import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig({
    plugins: [react()],
    server: {
        port: 5173,
        proxy: {
            '/api/realweather': { // співпадає з маршрутом контролера
                target: 'https://localhost:7000',
                changeOrigin: true,
                secure: false,  // ігноруємо самопідписаний сертифікат
                rewrite: (path) => path.replace(/^\/api\/realweather/, '/api/realweather')
            }
        }
    }
});