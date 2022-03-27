import { Component, EventEmitter, HostListener, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Point, Unit } from 'src/app/models/item';
import { ConfirmService } from 'src/app/services/confirm.service';
import { PointService } from 'src/app/services/point.service';

@Component({
  selector: 'app-edit-unit',
  templateUrl: './edit-unit.component.html',
  styleUrls: ['./edit-unit.component.css']
})
export class EditUnitComponent implements OnInit {

  @Output() handler: EventEmitter<any> = new EventEmitter();
  confirmServcie: ConfirmService;
  points: Point[] = [];
  unit: Unit;
  pointId: number;
  unitForm: FormGroup;
  validationErrors: string[] = [];
  selectedPoint: string;
  
  @HostListener("window:beforeunload", ["$event"]) unloadNotification($event: any){
    if(this.unitForm.dirty){
      $event.returnValue = true;
    }
  }

  constructor(private fb: FormBuilder, public modal: NgbActiveModal, private pointServcie: PointService,
    private router: Router) {}

  ngOnInit(): void {
    this.initializeForm();
    this.pointServcie.loadPoints().subscribe(points => this.points = points); 
  }

  initializeForm(){
    this.unitForm = this.fb.group({
      description: [this.unit.description, Validators.required],
      pointId: [this.pointId, Validators.required],
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

  editUnit(){
    if(this.unitForm.dirty){
      this.handler.emit(this.unitForm.value);
    }
    this.modal.close();
  }

}
