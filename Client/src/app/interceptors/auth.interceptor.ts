import { HttpInterceptorFn } from '@angular/common/http';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  console.log('HTTP Request:', req.method, req.url, req.headers);
    const token = localStorage.getItem('auth_token') || sessionStorage.getItem('auth_token');
    if (token) {
      const cloned = req.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      });
      // Debug: Log outgoing request with Authorization
      console.log('With Authorization:', cloned.headers.get('Authorization'));
      return next(cloned);
    }
    
  return next(req);
};
