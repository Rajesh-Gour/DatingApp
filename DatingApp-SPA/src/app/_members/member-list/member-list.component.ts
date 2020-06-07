import { Pagination, PaginatedResult } from './../../_models/pagination';
import { ActivatedRoute } from '@angular/router';
import { AlertifyService } from '../../_services/alertify.service';
import { AuthService } from '../../_services/auth.service';
import { UserService } from '../../_services/user.service';
import { Component, OnInit } from '@angular/core';
import { User } from '../../_models/User';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {

  users: User[];
  user: User = JSON.parse(localStorage.getItem('user'));
  genderList: [{value: 'male', display: 'Males'}, {value: 'female', display: 'Female'}];
  userParams: any = {};
  pagination: Pagination;
  constructor(private userService: UserService,
              private alertfyService: AlertifyService,
              private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.data.subscribe((data) => {
      this.users = data['users'].result;
      this.pagination = data['users'].pagination;
    });

    this.userParams.gender = this.userParams.gender === 'female' ? 'male' : 'female';
    this.userParams.minAge = 18;
    this.userParams.maxAge = 99;
    this.userParams.orderBy = 'lastActive';
    }

    resetFilters() {

    this.userParams.gender = this.userParams.gender === 'female' ? 'male' : 'female';
    this.userParams.minAge = 18;
    this.userParams.maxAge = 99;
    this.userParams.orderBy = 'lastActive';
    this.loadUsers();
    }

    filterChanged(value) {
      console.log(value);
      this.userParams.orderBy = value;
      this.loadUsers();
      // example usage, other things will happen here...
  }

    pageChanged(event: any): void {
      this.pagination.currentPage = event.page;
      this.loadUsers();
    }

    loadUsers() {
      console.log(this.userParams);
      this.userService.getUsers(this.pagination.currentPage, this.pagination.itemsPerPage, this.userParams)
      .subscribe((res: PaginatedResult<User[]>) => {
        this.users = res.result;
        this.pagination = res.pagination;
      }, error => {
        this.alertfyService.error(error);
      });
    }
  
}
