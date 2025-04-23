import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { providePrimeNG } from 'primeng/config';
import { routes } from './app.routes';
import ColorPreset from '../ColorPreset';
import { provideHttpClient, HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthInterceptor } from './services/auth-interceptor.service';
import { MessageService } from 'primeng/api';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideAnimationsAsync(),
    provideHttpClient(),
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    },
    MessageService, 
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
