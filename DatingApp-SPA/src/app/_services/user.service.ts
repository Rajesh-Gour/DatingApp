import { map } from 'rxjs/operators';
import { PaginatedResult } from './../_models/pagination';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { environment } from './../../environments/environment';
import { Injectable } from '@angular/core';
import { User } from '../_models/User';
import { Observable } from 'rxjs';
import { analyzeAndValidateNgModules } from '@angular/compiler';
import { Message } from '../_models/message';
// import { Message } from '@angular/compiler/src/i18n/i18n_ast';

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
urlm: string;
constructor(private http: HttpClient) { }

getUsers(page?, itemsPerPage?, userParams?, likesParams?): Observable<PaginatedResult<User[]>> {

  const paginatedResult: PaginatedResult<User[]> = new PaginatedResult<User[]>();
  
  let params = new HttpParams();

  if (page != null && itemsPerPage != null) {
    params = params.append('pageNumber', page);
    params = params.append('pageSize', itemsPerPage);
  }
 
  if (userParams != null) {
    params = params.append('minAge', userParams.minAge);
    params = params.append('maxAge', userParams.maxAge);
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);
  }
 
  if (likesParams === 'likers') {
    console.log('service var'+likesParams);
    params = params.append('likers', 'true');
  }

  if (likesParams === 'likees') {
    console.log('service var'+likesParams);
    params = params.append('likees', 'true');
  }
  
  return this.http.get<User[]>(this.baseUrl + 'users/GetUsers', { observe: 'response', params})
  .pipe(
    map(response => {
      paginatedResult.result = response.body;
      if (response.headers.get('Pagination') != null) {
        paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'))
      }

      return paginatedResult;

    })
  )

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

sendLike(id: number, recipientId: number){
  return this.http.post(this.baseUrl + 'users/' + id + '/like/' + recipientId, {});
}

getMessages(Id: number, page?, itemsPerPage?, messageContainer?){

  const paginatedResult: PaginatedResult<Message[]> = new PaginatedResult<Message[]>();
  
  let params = new HttpParams();

  params = params.append('MessageContainer', messageContainer);

  if (page != null && itemsPerPage != null) {
    params = params.append('pageNumber', page);
    params = params.append('pageSize', itemsPerPage);
  }

  return this.http.get<Message[]>(this.baseUrl + 'users/' + Id + '/messages' , {observe : 'response', params})
  .pipe(
    map(response => {
      paginatedResult.result = response.body;
      if (response.headers.get('Pagination') !== null) {
        paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
      }

      return paginatedResult;
    })
  );

}

getMessageThread(id: number, recipientId: number) {

  return this.http.get<Message[]>(this.baseUrl + 'users/' + id + '/messages/thread/' + recipientId);
}

sendMessage(id: number, message: Message) {

  return this.http.post(this.baseUrl + 'users/' + id + '/messages', message);
}

deleteMessage(id: number, userId: number) {

  return this.http.post(this.baseUrl + 'users/' + userId + '/messages/' + id , {});
}

markAsRead(userId: number, messageId: number) {
  this.http.post(this.baseUrl + 'users/' + userId + '/messages/' + messageId + '/read', {} )
  .subscribe();
}
}
