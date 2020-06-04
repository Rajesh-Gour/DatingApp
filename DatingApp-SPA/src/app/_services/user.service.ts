import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from './../../environments/environment';
import { Injectable } from '@angular/core';
import { User } from '../_models/User';
import { Observable } from 'rxjs';

// const httpOptions = {

//   headers: new HttpHeaders({
//     Authorization: 'Bearer ' + localStorage.getItem('token')
//   })
// };

// const httpOptions1 = new Headers({
//   'Content-Type': 'application/json',
//   'Authorization': 'Bearer ' + localStorage.getItem('token')
// });

@Injectable({
  providedIn: 'root'
})
export class UserService {

baseUrl = environment.apiUrl;
constructor(private http: HttpClient) { }

getUsers(): Observable<User[]> {

  return this.http.get<User[]>(this.baseUrl + 'users/GetUsers');

}

getUser(id): Observable<User> {

  //console.log(this.baseUrl + 'Users/GetUser/' + id);
  return this.http.get<User>('http://localhost:5000/api/Users/' + id);

}

updateUser(id: number , user: User) {

  return this.http.put(this.baseUrl + 'users/' + id, user );

}

setMainPhoto(userId: number, id: number) {

  return this.http.post(this.baseUrl + 'users/' + userId + '/photos/' + id + '/setMain', {});
}

deletePhoto(userId: number, id: number) {

  return this.http.delete(this.baseUrl + 'users/' + userId + '/photos/' + id);
}

}
