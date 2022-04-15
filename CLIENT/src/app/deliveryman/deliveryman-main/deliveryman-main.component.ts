import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs/operators';
import { Order } from 'src/app/models/order';
import { User } from 'src/app/models/user';
import { ReceiveOrderConfirmComponent } from 'src/app/orders/receive-order-confirm/receive-order-confirm.component';
import { ReturnOrderConfirmComponent } from 'src/app/orders/return-order-confirm/return-order-confirm.component';
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

  constructor(private orderService: OrderService, private accountServcie: AccountService, 
    private toastr: ToastrService, private modalService: NgbModal) {
    this.accountServcie.currentUser$.pipe(take(1)).subscribe(user => this.user = user);
  }
  ngOnInit(): void {
    this.orderService.getAvailableOrders().subscribe(orders => this.availableOrders = orders);
    this.orderService.getTakenOrders().subscribe(orders => this.takenOrders = orders);
  }

  confirmReturn(){
    const modalRef = this.modalService.open(ReturnOrderConfirmComponent, {
      size: "lg",
    })

    modalRef.componentInstance.toastr = this.toastr;
    modalRef.componentInstance.orderService = this.orderService;
  }

  confirmReceive(){
    const modalRef = this.modalService.open(ReceiveOrderConfirmComponent, {
      size: "lg",
    })

    modalRef.componentInstance.orderService = this.orderService;
    modalRef.componentInstance.toastr = this.toastr;
  }

}
