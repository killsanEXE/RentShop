import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
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
  registrationComplete = false;

  constructor(private fb: FormBuilder, private accountService: AccountService, private toastr: ToastrService) { }

  ngOnInit(): void {
    this.initializeForm();
    this.maxDate = new Date();
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 16);
  }

  initializeForm(){
    this.registerForm = this.fb.group({
      username: ["", Validators.required],
      name: ["", Validators.required],
      dateOfBirth: ["", [Validators.required]],
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

  register(){
    this.accountService.register(this.registerForm.value).subscribe(() => {
      this.registrationComplete = true;
      this.toastr.info("Please confirm your email");
    }, error => console.log(error));
  }
}
