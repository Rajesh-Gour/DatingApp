import { PaginatedResult } from './../_models/pagination';
import { AuthService } from './../_services/auth.service';
import { Component, OnInit } from '@angular/core';
import { User } from '../_models/User';
import { Pagination } from '../_models/pagination';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {
users: User[];
pagination: Pagination;
likesParam: string;

  constructor(
    private userService: UserService,
    private alertfyService: AlertifyService,
    private route: ActivatedRoute,
    private authService: AuthService) { }

  ngOnInit() {

    this.route.data.subscribe(data => {
      this.users = data['users'].results;
      this.pagination = data['users'].pagination;
    });

    console.log(this.users);
    this.likesParam = 'likers';
  }

  loadUsers() {
    console.log(this.likesParam);
    this.userService.getUsers(this.pagination.currentPage, 
      this.pagination.itemsPerPage, null, this.likesParam)
    .subscribe((res: PaginatedResult<User[]>) => {
      this.users = res.result;
      this.pagination = res.pagination;
    }, error => {
      this.alertfyService.error(error);
    });
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadUsers();
  }

}
