/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{html,ts}",
    "./node_modules/@angular-material-components/**/*.js"
  ],
  theme: {
    extend: {
      colors: {
        primary: '#1976d2',
        secondary: '#6c757d',
        accent: '#ff4081',
        warn: '#f44336'
      }
    },
  },
  plugins: [
    require('@tailwindcss/typography'),
    require('@tailwindcss/forms')
  ],
}
