import { Component, Input, OnInit, EventEmitter, Output } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmService } from 'src/app/services/confirm.service';

@Component({
  selector: 'app-create-item',
  templateUrl: './create-item.component.html',
  styleUrls: ['./create-item.component.css']
})
export class CreateItemComponent implements OnInit {

  // @Input() item = new EventEmitter();
  @Output() passEntry: EventEmitter<any> = new EventEmitter();
  confirmServcie: ConfirmService;
  itemForm: FormGroup;
  validationErrors: string[] = [];

  constructor(private fb: FormBuilder, public modal: NgbActiveModal) {}

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm(){
    this.itemForm = this.fb.group({
      name: ["", Validators.required],
      description: ["", Validators.required],
      pricePerDay: ["", [Validators.required, Validators.pattern("^[0-9]*$"), this.checkNumbers()]],
      ageRestriction: ["", [Validators.required, Validators.pattern("^[0-9]*$"), this.checkNumbers()]]
    });
  }

  checkNumbers(): ValidatorFn{
    return (control: AbstractControl) => {
      return parseInt(control?.value) >= 1 ? null : { isBelowZero: true }
    }
  }

  exit(){
    if(this.itemForm.dirty){
      this.confirmServcie.confirm().subscribe(f => {
        if(f) this.modal.close()
      })
    }else{
      this.modal.close();
    }
  }

  createItem(){
    this.passEntry.emit(this.itemForm.value);
  }

}
