import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { User } from '../models/user';
import { environment } from 'src/environments/environment';
import { Observable, ReplaySubject } from 'rxjs';
import { map } from "rxjs/operators";

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  baseUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User | null>(1);
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient) { }

  register(model: any){
    return this.http.post<User>(this.baseUrl + "account/register", model).pipe(map((user: User) => {
      if(user){
        this.setCurrentUser(user);
      }
    }));
  }
  
  login(model: any){
    return this.http.post<User>(this.baseUrl + "account/login", model).pipe(map(response => {
      const user = response;
      if(user){
        this.setCurrentUser(user);
      }
    }));
  }

  logout(){
    localStorage.removeItem("user");
    this.currentUserSource.next(null);
  }

  setCurrentUser(user: User){
    user.roles = [];
    const roles = this.getDecodedToken(user.token).role;
    Array.isArray(roles) ? user.roles = roles : user.roles.push(roles);
    localStorage.setItem("user", JSON.stringify(user));
    this.currentUserSource.next(user);
  }

  getDecodedToken(token: string){
    return JSON.parse(atob(token.split(".")[1]));
  }

}
