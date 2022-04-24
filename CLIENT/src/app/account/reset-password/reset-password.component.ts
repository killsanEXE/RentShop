import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { ActivatedRoute, Route, Router } from '@angular/router';
import { take } from 'rxjs/operators';
import { AccountService } from 'src/app/services/account.service';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.css']
})
export class ResetPasswordComponent implements OnInit {

  form: FormGroup;
  token: string;

  constructor(private fb: FormBuilder, private activatedRoute: ActivatedRoute, 
    private accountService: AccountService, private router: Router) {
      this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
        if(user !== null){
          this.router.navigateByUrl("/");
        }
      });
    }

  ngOnInit(): void {
    this.token = this.activatedRoute.snapshot.queryParams["token"];
    this.initializeForm();
  }

  initializeForm(){
    this.form = this.fb.group({
      email: ["", Validators.required],
      token: [this.token],
      password: ["", [Validators.required, Validators.minLength(6), Validators.maxLength(16)]],
      confirmPassword: ["", [Validators.required, this.matchValues()]],
    });
    this.form.controls.password.valueChanges.subscribe(() => {
      this.form.controls?.confirmPassword?.updateValueAndValidity();
    });
  }  

  matchValues(): ValidatorFn{
    return (control: AbstractControl) => {
      return control?.value === this.form?.controls?.password?.value ? null : { isMatching: true }
    }
  }

  reset(){
    this.accountService.resetPassword(this.form.value).subscribe(() => this.router.navigateByUrl("/regandlog"))
  }

}
