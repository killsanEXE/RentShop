import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { take } from 'rxjs/operators';
import { User } from '../models/user';
import { AccountService } from '../services/account.service';
import { BusyService } from '../services/busy.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  user: User | null;

  constructor(public accountService: AccountService, private router: Router, 
    public busyService: BusyService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
      this.user = user;
    })
  }

  ngOnInit(): void {
    
  }

  logout(){
    this.router.navigateByUrl("/");
    this.accountService.logout();
  }

  showNavMenu(){
    const menuBtn = document.querySelector(".menu-icon span");
    const cancelBtn = document.querySelector(".cancel-icon");
    const items = document.querySelector(".nav-items");
    items.classList.add("active");
    menuBtn.classList.add("hide");
    cancelBtn.classList.add("show");
  }
  
  hideNavMenu(){
    const menuBtn = document.querySelector(".menu-icon span");
    const cancelBtn = document.querySelector(".cancel-icon");
    const items = document.querySelector(".nav-items");
    items.classList.remove("active");
    menuBtn.classList.remove("hide");
    cancelBtn.classList.remove("show");
  }

}
