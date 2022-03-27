import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Point } from 'src/app/models/item';

@Component({
  selector: 'app-add-point',
  templateUrl: './add-point.component.html',
  styleUrls: ['./add-point.component.css']
})
export class AddPointComponent implements OnInit {

  @Output() passEntry: EventEmitter<any> = new EventEmitter();
  pointForm: FormGroup;
  validationErrors: string[] = [];
  country: string;
  points: Point[];
  addNewCountry = false;
  countries: string[] = [];

  constructor(private fb: FormBuilder, public modal: NgbActiveModal) {}

  ngOnInit(): void {
    this.initializeForm();

    this.countries = [...new Set(this.points.map(point => point.country))]
  }

  initializeForm(){
    this.pointForm = this.fb.group({
      country: [this.country, [Validators.required]],
      city: ["", [Validators.required]],
      address: ["", [Validators.required]],
    });
  }

  exit(){
    this.modal.close();
  }

  createPoint(){
    if(this.pointForm.controls["country"].value !== "ADD_NEW_COUNTRY"){
      this.passEntry.emit(this.pointForm.value);
    }else{
      this.pointForm.reset();
    }
  }

  selectCountry(){
    if(this.pointForm.controls["country"].value === "ADD_NEW_COUNTRY"){
      this.pointForm.controls["country"].reset();
      this.addNewCountry = true;
    }
  }

}
