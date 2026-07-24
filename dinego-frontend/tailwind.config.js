/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        brand: {
          50: '#fff7ed',
          100: '#ffedd5',
          500: '#f97316',
          600: '#ea580c',
          700: '#c2410c',
        },
        table: {
          free: '#10b981',      // 0: Trống (Green)
          reserved: '#f59e0b',  // 1: Đã đặt (Yellow/Amber)
          occupied: '#ef4444',  // 2: Đang có khách (Red)
          cleaning: '#64748b',  // 3: Đang dọn (Gray)
        }
      },
      fontFamily: {
        sans: ['Inter', 'system-ui', 'sans-serif'],
      }
    },
  },
  plugins: [],
}
