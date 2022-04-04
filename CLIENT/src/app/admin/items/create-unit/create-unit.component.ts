import { Component, EventEmitter, HostListener, OnInit, Output } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Item, Point } from 'src/app/models/item';
import { ConfirmService } from 'src/app/services/confirm.service';
import { PointService } from 'src/app/services/point.service';

@Component({
  selector: 'app-create-unit',
  templateUrl: './create-unit.component.html',
  styleUrls: ['./create-unit.component.css']
})
export class CreateUnitComponent implements OnInit {

  @Output() handler: EventEmitter<any> = new EventEmitter();
  confirmServcie: ConfirmService;
  points: Point[];
  unitForm: FormGroup;
  validationErrors: string[] = [];
  selectedPoint: string;
  
  @HostListener("window:beforeunload", ["$event"]) unloadNotification($event: any){
    if(this.unitForm.dirty){
      $event.returnValue = true;
    }
  }

  constructor(private fb: FormBuilder, public modal: NgbActiveModal,
    private router: Router) {}

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm(){
    this.unitForm = this.fb.group({
      description: ["", Validators.required],
      pointId: [this.selectedPoint, Validators.required],
    });
  }

  exit(){
    if(this.unitForm.dirty){
      this.confirmServcie.confirm().subscribe(f => {
        if(f) this.modal.close()
      })
    }else{
      this.modal.close();
    }
  }

  setPoint(){
    if(this.unitForm.controls["pointId"].value === "MANAGE_POINTS"){
      this.modal.close();
      this.router.navigateByUrl("/admin/points");
    }
  }

  createUnit(){
    this.handler.emit(this.unitForm.value);
  }

}
