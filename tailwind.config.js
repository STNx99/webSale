/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./Views/**/*.{cshtml,js}",
    "./Areas/**/*.{cshtml,js}"
  ],
  theme: {
    extend: {
      colors: {
        primary: {
          DEFAULT: '#3f51b5',
          dark: '#303f9f',
          light: '#7986cb'
        },
        secondary: {
          DEFAULT: '#ff4081',
          dark: '#c60055',
          light: '#ff79b0'
        }
      },
      fontFamily: {
        sans: ['Poppins', 'sans-serif']
      }
    },
  },
  plugins: [
    require('@tailwindcss/forms')
  ],
}
