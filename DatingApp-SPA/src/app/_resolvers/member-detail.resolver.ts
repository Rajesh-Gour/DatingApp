import { catchError } from 'rxjs/operators';
import { Observable, of } from 'rxjs';
import { AlertifyService } from './../_services/alertify.service';
import { UserService } from './../_services/user.service';
import { User } from './../_models/User';
import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRoute, ActivatedRouteSnapshot } from '@angular/router';


@Injectable({
    providedIn: 'root'
})
export class MemberDetailResolver implements Resolve<User> {

    constructor(
                private userService: UserService,
                private router: Router,
                private alertify: AlertifyService) {}

        resolve(route: ActivatedRouteSnapshot): Observable<User> {
            return this.userService.getUser(route.params['id']).pipe(
                catchError(error => {
                    this.alertify.error('problem retriving data');
                    this.router.navigate(['/members']);
                    return of(null);
                })
            );
        }
    }
