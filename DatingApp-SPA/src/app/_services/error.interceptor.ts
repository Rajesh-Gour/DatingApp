import { Injectable } from '@angular/core';
import { HttpInterceptor, HTTP_INTERCEPTORS } from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';


@Injectable({
    providedIn: 'root'
  })
export class ErrorInterceptor implements HttpInterceptor {
    intercept(
        req: import('@angular/common/http').HttpRequest < any > ,
        next: import('@angular/common/http').HttpHandler
        ): import('rxjs').Observable<import('@angular/common/http').HttpEvent<any>> {
        return next.handle(req).pipe(
            catchError(error => {
                if (error.status === 401) {
                    return throwError(error.statusText);
                }
               /* if (error.status === 400) {
                    return throwError(error.error);
                }*/
                const applicationError = error.headers.get('Application-Error');
                if (applicationError) {
                    return throwError(applicationError);
                }

                const serverError = error.error;
                let modelStateErrors = '';

                if ((error.status === 400) || (serverError.errors && typeof serverError.errors === 'object')) {
                    for (const key in serverError.errors) {
                        if (serverError.errors[key]) {
                           modelStateErrors += serverError.errors[key] + '\n' ;
                        }
                    }

                    return throwError(modelStateErrors || serverError || 'Server Error');
                }

            })
        );
    }
}

export const ErrorInterceptorProvider = [{
    provider: HTTP_INTERCEPTORS,
    useClass: ErrorInterceptor,
    multi: true
}];

