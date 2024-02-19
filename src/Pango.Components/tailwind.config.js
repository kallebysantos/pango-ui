/** @type {import('tailwindcss').Config} */
module.exports = {
    darkMode: ["class"],
    content: ["./**/*.{cshtml,razor,cs,html}"],
    theme: {
        container: {
            center: true,
            padding: "2rem",
            screens: {
                "2xl": "1400px",
            },
        },
        extend: {
            colors: {
                border: "hsl(var(--border))",
                input: "hsl(var(--input))",
                ring: "hsl(var(--ring))",
                background: "hsl(var(--background))",
                foreground: "hsl(var(--foreground))",
                primary: {
                    DEFAULT: "hsl(var(--primary))",
                    foreground: "hsl(var(--primary-foreground))",
                },
                secondary: {
                    DEFAULT: "hsl(var(--secondary))",
                    foreground: "hsl(var(--secondary-foreground))",
                },
                destructive: {
                    DEFAULT: "hsl(var(--destructive))",
                    foreground: "hsl(var(--destructive-foreground))",
                },
                muted: {
                    DEFAULT: "hsl(var(--muted))",
                    foreground: "hsl(var(--muted-foreground))",
                },
                accent: {
                    DEFAULT: "hsl(var(--accent))",
                    foreground: "hsl(var(--accent-foreground))",
                },
                popover: {
                    DEFAULT: "hsl(var(--popover))",
                    foreground: "hsl(var(--popover-foreground))",
                },
                card: {
                    DEFAULT: "hsl(var(--card))",
                    foreground: "hsl(var(--card-foreground))",
                },
            },
            borderRadius: {
                lg: `var(--radius)`,
                md: `calc(var(--radius) - 2px)`,
                sm: "calc(var(--radius) - 4px)",
            },
            keyframes: {
                "accordion-down": {
                    from: { height: "0" },
                    to: { height: "var(--radix-accordion-content-height)" },
                },
                "accordion-up": {
                    from: { height: "var(--radix-accordion-content-height)" },
                    to: { height: "0" },
                },
                "dialog-show": {
                    from: {
                        transform: 'scale(.95)',
                        opacity: '0',
                        display: 'none'
                    },
                    to: {
                        transform: 'scale(1)',
                        opacity: '1',
                        display: 'block'
                    },
                },
                "dialog-close": {
                    from: {
                        transform: 'scale(1)',
                        opacity: '1',
                        display: 'block'
                    },
                    to: {
                        transform: 'scale(.95)',
                        opacity: '0',
                        display: 'none'
                    },
                },
                "slide-in-from-bottom": {
                    from: {
                        transform: 'translateY(2px)',
                    },
                    to: {
                        transform: 'translateY(0)',
                    },
                },
                "slide-in-from-top": {
                    from: {
                        transform: 'translateY(-2px)',
                    },
                    to: {
                        transform: 'translateY(0)',
                    },
                }
            },
            animation: {
                "accordion-down": "accordion-down 0.2s ease-out",
                "accordion-up": "accordion-up 0.2s ease-out",
                "dialog-show": "dialog-show ease-out 100ms",
                "dialog-close": "dialog-close ease-in 75ms",
                "slide-in-from-bottom": "slide-in-from-bottom ease-in 50ms",
                "slide-in-from-top": "slide-in-from-top ease-in 50ms"
            },
        },
    },
    plugins: [
    ],
}
