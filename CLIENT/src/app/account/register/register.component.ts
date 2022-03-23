import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AccountService } from 'src/app/services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  registerForm: FormGroup;
  validationErrors: string[] = [];
  maxDate: Date;

  constructor(private fb: FormBuilder, private accountService: AccountService, private router: Router) { }

  ngOnInit(): void {
    this.initializeForm();
    this.maxDate = new Date();
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 16);
  }

  initializeForm(){
    this.registerForm = this.fb.group({
      username: ["", Validators.required],
      name: ["", Validators.required],
      dateOfBirth: ["", [Validators.required, this.checkAge()]],
      email: ["", Validators.required],
      password: ["", [Validators.required, Validators.minLength(6), Validators.maxLength(16)]],
      confirmPassword: ["", [Validators.required, this.matchValues()]],
    });
    this.registerForm.controls.password.valueChanges.subscribe(() => {
      this.registerForm.controls?.confirmPassword?.updateValueAndValidity();
    });
  }  

  matchValues(): ValidatorFn{
    return (control: AbstractControl) => {
      return control?.value === this.registerForm?.controls?.password?.value ? null : { isMatching: true }
    }
  }

  checkAge(): ValidatorFn{
    return (control: AbstractControl) => {
      if(control.dirty) return new Date().getFullYear() - control?.value.getFullYear() >= 130 ? {veryOld: true} : null;  
      return null;
    }
  }

  register(){
    this.accountService.register(this.registerForm.value).subscribe(() => {
      this.router.navigateByUrl("/");
    }, error => console.log(error));
  }
}
