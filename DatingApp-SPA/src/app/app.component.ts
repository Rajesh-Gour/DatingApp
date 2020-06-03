import { User } from 'src/app/_models/User';
import { AuthService } from './_services/auth.service';
import { Component, OnInit } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  decodedToken: any;
  jwtHelper = new JwtHelperService();
  constructor(private autheService: AuthService) {
  }

  ngOnInit(): void {
    const token = localStorage.getItem('token');
    const user: User = JSON.parse(localStorage.getItem('user'));
    if (token) {
      this.autheService.decodedToken = this.jwtHelper.decodeToken(token);
      console.log(this.decodedToken);
    }

    if (user) {
      this.autheService.currentUser = user;
      this.autheService.changePhotoUrl(user.photoUrl);
    }
  }
}
