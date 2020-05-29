import { Observable } from 'rxjs';
import { AuthService } from './../../_services/auth.service';
import { UserService } from './../../_services/user.service';
import { AlertifyService } from './../../_services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { Component, OnInit, ViewChild, HostListener } from '@angular/core';
import { User } from 'src/app/_models/User';
import { NgForm } from '@angular/forms';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
user: User;
@HostListener('window:beforeunload',['$event'])
unloadNotification($event: any) {
  if (this.editForm.dirty) {

    $event.returnValue = true;
  }
}
@ViewChild('editForm', {static: true}) editForm: NgForm;

  constructor(
    private route:ActivatedRoute,
    private alertify: AlertifyService,
    private userService: UserService,
    private authService: AuthService) { }

  ngOnInit() {

    this.route.data.subscribe(data => {
      this.user = data['user'];
    });
  }

  updateUser()
  {
    this.userService.updateUser(this.authService.decodedToken.nameid, this.user).subscribe(next  => {
      this.alertify.success('profile updated');
      this.editForm.reset(this.user);
    }, error => {
      this.alertify.error(error);
    });
  }

}
