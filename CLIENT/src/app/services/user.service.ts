import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Client } from '../models/client';
import { getPaginatedResult, getPaginationHeaders } from '../models/paginationHelper';
import { UserParams } from '../models/userParams';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  
  baseUrl = environment.apiUrl;
  userParams: UserParams;
  userCache = new Map();

  constructor(private http: HttpClient) {
    this.userParams = new UserParams();
    this.userParams.PageSize = 10;
  }

  getUserParams(){
    return this.userParams;
  }

  getUsers(userParams: UserParams){
    var response = this.userCache.get(Object.values(userParams).join("-"));
    if(response) return of(response);

    let params = getPaginationHeaders(userParams.pageNumber, userParams.PageSize);
    return getPaginatedResult<Client[]>(this.baseUrl + "users", params, this.http).pipe(map(response => {
      this.userCache.set(Object.values(userParams).join("-"), response);
      return response.result;
    }));
  }

}
