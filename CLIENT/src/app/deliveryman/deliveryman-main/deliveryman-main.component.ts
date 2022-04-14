import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs/operators';
import { Order } from 'src/app/models/order';
import { User } from 'src/app/models/user';
import { AccountService } from 'src/app/services/account.service';
import { OrderService } from 'src/app/services/order.service';

@Component({
  selector: 'app-deliveryman-main',
  templateUrl: './deliveryman-main.component.html',
  styleUrls: ['./deliveryman-main.component.css']
})
export class DeliverymanMainComponent implements OnInit {

  availableOrders: Order[] = [];
  takenOrders: Order[] = [];
  user: User;
  orderId: string = "";

  constructor(private orderService: OrderService, private accountServcie: AccountService, private toastr: ToastrService) {
    this.accountServcie.currentUser$.pipe(take(1)).subscribe(user => this.user = user);
  }
  ngOnInit(): void {
    this.orderService.getAvailableOrders().subscribe(orders => this.availableOrders = orders);
    this.orderService.getTakenOrders().subscribe(orders => this.takenOrders = orders);
  }

  confirmReturn(){
    this.orderService.confirmReturn(this.orderId.trim()).subscribe(order => {
      if(order.unitReturned && order.unit.isAvailable){
        this.toastr.success("Order was confirmed");
      }else{
        this.toastr.error("Some error i guess idk?");
      }
    })
  }

}
