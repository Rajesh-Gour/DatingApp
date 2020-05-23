import { AuthService } from './../_services/auth.service';
import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
model: any = {};
@Input() valuesFromHome: any;
@Output() cancelRegister = new EventEmitter();
  constructor(private authService: AuthService) { }

  ngOnInit() {

    /*console.log('values from parent', this.valuesFromHome);*/
  }

register() {
  
  this.authService.register(this.model).subscribe(() => {
    console.log(this.model);
    console.log('registeration successful');
  }, error => {
    console.log(error);
  });
}
cancel() {
  this.cancelRegister.emit(false);
  console.log('cancel event call from child');
}
}
