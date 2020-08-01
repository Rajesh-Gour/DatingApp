import { User } from 'src/app/_models/User';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AdminService {

  baseUrl = environment.apiUrl;
  
  constructor(private http: HttpClient) { }

  getUserWithRoles(){
    return this.http.get(this.baseUrl + 'admin/usersWithRoles');
  }

  updateUserRoles(user: User, roleNames: {}) {
    console.log(roleNames);
    return this.http.post(this.baseUrl + 'admin/editRoles/' + user.userName, roleNames);
  }

  getPhotosForApproval() {
    return this.http.get(this.baseUrl + 'admin/photosForModeration');
  }

  approvePhoto(photoId) {
    return this.http.post(this.baseUrl + 'admin/approvePhoto/' + photoId , {});
  }

  rejectPhoto(photoId) {
    return this.http.post(this.baseUrl + 'admin/rejectPhoto' + photoId , {});
  }
}
