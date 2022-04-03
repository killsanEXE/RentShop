import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-become-deliveryman',
  templateUrl: './become-deliveryman.component.html',
  styleUrls: ['./become-deliveryman.component.css']
})
export class BecomeDeliverymanComponent implements OnInit {

  @Output() handler: EventEmitter<any> = new EventEmitter();
  requestForm: FormGroup;
  country: string;
  countries: string[] = [];

  constructor(private fb: FormBuilder, public modal: NgbActiveModal) {}

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm(){
    this.requestForm = this.fb.group({
      country: [this.country, [Validators.required]],
    });
  }

  exit(){
    this.modal.close();
  }

  send(){
    this.handler.emit(this.requestForm.value);
  }

}
