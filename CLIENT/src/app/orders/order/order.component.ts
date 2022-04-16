import { Component, Input, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Client } from 'src/app/models/client';
import { Point } from 'src/app/models/item';
import { Order } from 'src/app/models/order';
import { User, UserLocation } from 'src/app/models/user';
import { MessageService } from 'src/app/services/message.service';
import { OrderService } from 'src/app/services/order.service';
import { PointService } from 'src/app/services/point.service';

@Component({
  selector: 'app-order',
  templateUrl: './order.component.html',
  styleUrls: ['./order.component.css']
})
export class OrderComponent implements OnInit {

  @Input() order: Order;
  @Input() user: User;

  @Input() orderFromPage: boolean = false;

  selectedReturnWay: string;
  showReturnOptions = true;
  showPointsToReturn = false;
  showLocationsReturn = false;
  selectedReturnPoint: Point;
  points: Point[] = [];

  returnLocation: UserLocation;  
  returnLocationId: number;  
  returnForm: FormGroup;

  constructor(private orderService: OrderService, private messageService: MessageService, 
    private pointService: PointService, private fb: FormBuilder) {}

  ngOnInit(): void {
    this.initializeForm();

    if(this.user.roles.includes('Deliveryman')){
      if(this.points.length <= 0){ this.pointService.loadPoints().subscribe(points => {
        this.points = points.filter(f => f.country === this.order.unit.point.country);
      })}
      this.showReturnOptions = false;
      this.selectSelfReturn();
    }

    console.log(this.order);
  }

  initializeForm(){
    this.returnForm = this.fb.group({
      id: [this.order.id, []],
      returnFromLocation: [null, [this.requireFromLocation()]],
      returnPoint: [null, [this.requirePoint()]],
    });
  }

  requireFromLocation(): ValidatorFn{
    return (control: AbstractControl) => {
      // return (control?.value !== null && this.showLocationsReturn) ? null : { isRequired: true }
      if(this.showLocationsReturn){
        if(control?.value !== null){
          return null;
        }else{
          return { isRequired: true };
        }
      }else{
        return null;
      }
    }
  }

  requirePoint(): ValidatorFn{
    return (control: AbstractControl) => {
      // return (control?.value !== null && this.showPointsToReturn) ? null : { isRequired: true }
      if(this.showPointsToReturn){
        if(control?.value !== null){
          return null;
        }else{
          return { isRequired: true };
        }
      }else{
        return null;
      }
    }
  }

  acceptOrder() { this.orderService.acceptOrder(this.order.id).subscribe(order => { this.order = order; }); }
  startDelivery(){ this.orderService.startDelivery(this.order.id).subscribe(order => { this.order = order; }); }
  finishDelivery(){ this.orderService.completeDelivery(this.order.id).subscribe(order => { this.order = order; }); }
  message(otherUser: Client){ this.messageService.openDialog(this.user, otherUser.username); }



  cancelOrder(){ this.orderService.cancelOrder(this.order.id).subscribe((order) => { this.order = order; }) }
  selfpickOrder(){ this.orderService.selfpickOrder(this.order.id).subscribe((order) => this.order = order); }
  receivedOrder(){ this.orderService.receivedOrder(this.order.id).subscribe(order => this.order = order); }


  selectSelfReturn(){
    this.showPointsToReturn = true;
    this.showLocationsReturn = false;
    this.returnForm.reset();
    this.initializeForm();
    if(this.points.length <= 0){ this.pointService.loadPoints().subscribe(points => {
      this.points = points.filter(f => f.country === this.order.unit.point.country);
    })}
  }

  selectDeliveryReturn(){ 
    this.showPointsToReturn = false; 
    this.showLocationsReturn = true;
    this.returnForm.reset();
    this.initializeForm();
  }

  save(){
    this.returnForm.controls["id"].setValue(this.order.id);
    this.orderService.setReturnForm(this.returnForm.value).subscribe(order => this.order = order);
  }
}
