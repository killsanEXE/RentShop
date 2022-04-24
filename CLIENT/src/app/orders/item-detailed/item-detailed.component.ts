import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Item, Unit } from 'src/app/models/item';
import { AbstractControl, FormBuilder, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import { AccountService } from 'src/app/services/account.service';
import { User, UserLocation } from 'src/app/models/user';
import { delay, take } from 'rxjs/operators';
import { OrderService } from 'src/app/services/order.service';
import { Order } from 'src/app/models/order';

@Component({
  selector: 'app-item-detailed',
  templateUrl: './item-detailed.component.html',
  styleUrls: ['./item-detailed.component.css']
})
export class ItemDetailedComponent implements OnInit {

  item: Item;
  orderForm: FormGroup;

  selectedDeliveryWay: string;
  selectedUnit: Unit;

  ordering: boolean = false;
  selectingUnit: boolean = false;
  delivery: boolean = false;
  timespanSelect: boolean = false;
  deliveryDateSelect: Date;
  returnDateSelect: Date;

  unitsToSelect: Unit[] = [];
  allUnits: Unit[] = [];

  todayDate = new Date();
  minDate: Date = this.todayDate;
  user: User;

  createdOrder: Order;

  constructor(private route: ActivatedRoute, private fb: FormBuilder, private accountServcie: AccountService,
    private orderServcie: OrderService, public router: Router) {
    this.accountServcie.currentUser$.pipe(take(1)).subscribe(user => this.user = user);
  }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.item = data.item;
      this.allUnits = this.item.units.filter(f => f.isAvailable && !f.disabled);
    });
    
    this.initializeForm();
  }

  initializeForm(){
    this.orderForm = this.fb.group({
      unitId: [null, [Validators.required]],
      deliveryLocation: [null, [this.requireDeliveryLocation()]],
      deliveryDate: [this.deliveryDateSelect, [Validators.required]],
      returnDate: [this.returnDateSelect, [Validators.required]],
    });
  }

  requireDeliveryLocation(): ValidatorFn{
    return (control: AbstractControl) => {
      if(this.delivery){
        return control?.value !== null ? null : { isRequired: true }
      }else{
        return null;
      }
    }
  }

  getImages = () => {
    const imageUrls = []
    for(const photo of this.item.photos){
      imageUrls.push({
        small: photo?.url,
        medium: photo?.url,
        big: photo?.url,
      })
    }
    return imageUrls;
  }

  selectSelfick(){
    this.minDate = this.todayDate;
    this.delivery = false;
    this.selectingUnit = true;
    this.orderForm.reset();
    this.timespanSelect = false;
    this.selectedUnit = null;
    this.unitsToSelect = this.allUnits;
  }

  selectDelivery(){
    this.minDate = new Date(this.todayDate.getFullYear(), this.todayDate.getMonth(), this.todayDate.getDate() + 3);
    this.delivery = true;
    this.selectingUnit = false;
    this.orderForm.reset();
    this.timespanSelect = false;
    this.selectedUnit = null;
    this.unitsToSelect = [];
  }

  selectedLocation(){
    this.selectedUnit = null;
    this.orderForm.controls["unitId"].setValue(null);
    let location: UserLocation = this.user.locations.find(f => f.id === parseInt(this.orderForm.controls["deliveryLocation"].value));
    this.unitsToSelect = this.allUnits.filter(f => f.point.country === location.country);
    this.selectingUnit = true;
  }

  selectUnit(unit: Unit){
    this.selectedUnit = unit;
    this.orderForm.controls["unitId"].setValue(unit.id);
    this.timespanSelect = true;
    this.selectingUnit = false;
  }

  selectAnotherUnit(){
    this.selectedUnit = null;
    this.orderForm.controls["unitId"].setValue(null);
    this.selectingUnit = true;
  }

  selectDate(type: string, event: MatDatepickerInputEvent<Date>){
    if(type === "delivery"){
      this.deliveryDateSelect = event.value;
    }else if(type === "return"){
      this.returnDateSelect = event.value;
    }
  }

  confirmOrder(){
    this.orderServcie.createOrder(this.orderForm.value).subscribe(order => {
      this.delivery = false;
      this.selectingUnit = false;
      this.orderForm.reset();
      this.timespanSelect = false;
      this.selectedUnit = null;
      this.unitsToSelect = [];
      this.createdOrder = order;
    })
  }

}
