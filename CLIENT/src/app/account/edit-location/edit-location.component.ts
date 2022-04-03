import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { UserLocation } from 'src/app/models/user';

@Component({
  selector: 'app-edit-location',
  templateUrl: './edit-location.component.html',
  styleUrls: ['./edit-location.component.css']
})
export class EditLocationComponent implements OnInit {

  @Output() handler: EventEmitter<any> = new EventEmitter();
  locationForm: FormGroup;
  country: string;
  countries: string[] = [];
  location: UserLocation;

  constructor(private fb: FormBuilder, public modal: NgbActiveModal) {}

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm(){
    this.locationForm = this.fb.group({
      country: [this.location.country, [Validators.required]],
      city: [this.location.city, [Validators.required]],
      address: [this.location.address, [Validators.required]],
      floor: [this.location.floor],
      apartment: [this.location.apartment],
    });
  }

  exit(){
    this.modal.close();
  }

  editLocation(){
    if(this.locationForm.dirty) this.handler.emit(this.locationForm.value);
    else this.modal.close()
  }
}
