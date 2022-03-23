import { Component, OnInit } from '@angular/core';
import { User } from './models/user';
import { AccountService } from './services/account.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit{
  title = 'Rent shop';

  constructor(private accountServcie: AccountService) {}

  ngOnInit(){
    this.setCurrentUser();
  }

  setCurrentUser(){
    const user: User = JSON.parse(localStorage.getItem("user") || "{}");
    if(user.username){
      this.accountServcie.setCurrentUser(user);
    }
  }
}
