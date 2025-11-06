import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { NavigationExtras, Router } from '@angular/router';
import { catchError } from 'rxjs/operators';
import { ToastService } from '../services/toast-service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const toast = inject(ToastService);

  return next(req).pipe(
    catchError((error) => {
      if (error) {
        debugger;
        switch (error.status) {
          case 400:
            if (error?.error?.errors) {
              const modelStateErrors: string[] = [];
              for (const key in error?.error?.errors) {
                if (error?.error?.errors[key]) {
                  error?.error?.errors[key].forEach((error: string) => {
                    toast.error(error);
                    modelStateErrors.push(error);
                  });
                }
              }

              throw modelStateErrors.flat();
            } else {
              toast.error(error.error);
            }
            break;
          case 401:
            toast.error(error.error.message);
            break;
          case 404:
            toast.error('Not Found');
            router.navigate(['/not-found']);
            break;
          case 500:
            toast.error('Internal Server Error');
            const navigationExtras: NavigationExtras = {
              state: { error: error.error }
            }
            router.navigateByUrl('/server-error', navigationExtras);
            break;
          default:
            toast.error('Something unexpected went wrong');
            break;
        }
      }
      throw error;
    })
  );
};
