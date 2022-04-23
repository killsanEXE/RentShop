import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { take } from 'rxjs/operators';
import { User } from 'src/app/models/user';
import { AccountService } from 'src/app/services/account.service';

@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.css']
})
export class ForgotPasswordComponent implements OnInit {

  email: string = "";
  sended = false;

  constructor(private accountService: AccountService, private router: Router) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
      if(user !== null){
        this.router.navigateByUrl("/");
      }
    });
  }

  ngOnInit(): void {
    
  }

  send(){
    if(this.email.trim() !== "")
      this.accountService.forgotPassword(this.email.trim()).subscribe(() => this.sended = true);
  }
}
