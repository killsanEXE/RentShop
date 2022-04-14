import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Point } from 'src/app/models/item';
import { PointService } from 'src/app/services/point.service';

@Component({
  selector: 'app-add-location',
  templateUrl: './add-location.component.html',
  styleUrls: ['./add-location.component.css']
})
export class AddLocationComponent implements OnInit {

  @Output() handler: EventEmitter<any> = new EventEmitter();
  locationForm: FormGroup;
  country: string;
  countries: string[] = [];

  constructor(private fb: FormBuilder, public modal: NgbActiveModal) {}

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm(){
    this.locationForm = this.fb.group({
      country: [this.country, [Validators.required]],
      city: ["", [Validators.required]],
      address: ["", [Validators.required]],
      floor: [null],
      apartment: [null],
    });
  }

  exit(){
    this.modal.close();
  }

  addLocation(){
    let floor = this.locationForm.controls["floor"];
    if(floor.value === null || floor.value === undefined || floor.value === "") floor.setValue(0);
    this.handler.emit(this.locationForm.value);
  }

}
