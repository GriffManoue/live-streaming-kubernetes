import { definePreset } from '@primeng/themes';
import Aura from '@primeng/themes/aura';

const ColorPreset = definePreset(Aura, {
    semantic: {
        primary: {
            50: '{sky.50}',
            100: '{sky.100}',
            200: '{sky.200}',
            300: '{sky.300}',
            400: '{sky.400}',
            500: '{sky.500}',
            600: '{sky.600}',
            700: '{sky.700}',
            800: '{sky.800}',
            900: '{sky.900}',
            950: '{sky.950}'
        },
       colorScheme: {
            light: {
                surface: {
                    0: '#ffffff',
                    50: '{teal.50}',
                    100: '{teal.100}',
                    200: '{teal.200}',
                    300: '{teal.300}',
                    400: '{teal.400}',
                    500: '{teal.500}',
                    600: '{teal.600}',
                    700: '{teal.700}',
                    800: '{teal.800}',
                    900: '{teal.900}',
                    950: '{teal.950}'
                }
            },
            dark: {
                surface: {
                    0: '#ffffff',
                    50: '{cyan.50}',
                    100: '{cyan.100}',
                    200: '{cyan.200}',
                    300: '{cyan.300}',
                    400: '{cyan.400}',
                    500: '{cyan.500}',
                    600: '{cyan.600}',
                    700: '{cyan.700}',
                    800: '{cyan.800}',
                    900: '{cyan.900}',
                    950: '{cyan.950}'
                }
            }
        }
    },
    }
);

export default ColorPreset;
