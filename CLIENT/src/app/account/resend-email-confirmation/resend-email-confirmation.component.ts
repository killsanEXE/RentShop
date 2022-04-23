import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { take } from 'rxjs/operators';
import { AccountService } from 'src/app/services/account.service';

@Component({
  selector: 'app-resend-email-confirmation',
  templateUrl: './resend-email-confirmation.component.html',
  styleUrls: ['./resend-email-confirmation.component.css']
})
export class ResendEmailConfirmationComponent implements OnInit {

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
      this.accountService.resendEmailConfirmation(this.email.trim()).subscribe(() => this.sended = true);
  }

}
