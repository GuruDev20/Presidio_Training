/** @type {import('tailwindcss').Config} */
module.exports = {
    darkMode: 'class',
    content: ["./src/**/*.{html,ts}"],
    theme: {
        extend: {
            colors:{
                'dark-bg': '#242F40',
                'dark-bg-grad-from': '#242F40',
                'dark-bg-grad-to': '#3D2645',
            }
        },
    },
    plugins: [],
}