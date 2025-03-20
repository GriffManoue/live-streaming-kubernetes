import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { providePrimeNG } from 'primeng/config';
import { routes } from './app.routes';
import ColorPreset from '../ColorPreset';
import { provideHttpClient } from '@angular/common/http';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }), 
    provideRouter(routes),
    provideAnimationsAsync(),
    provideHttpClient(),
    providePrimeNG({
        theme: {
            preset: ColorPreset,
            options: {
              cssLayer: {
                  name: 'primeng',
                  order: 'theme, base, primeng'
              },
              darkModeSelector: '.my-app-dark'
          }

        },
        ripple: true,
        inputVariant: 'filled',
       
    })
  ]
};
