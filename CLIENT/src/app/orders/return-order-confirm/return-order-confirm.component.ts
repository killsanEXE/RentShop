import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { OrderService } from 'src/app/services/order.service';

@Component({
  selector: 'app-return-order-confirm',
  templateUrl: './return-order-confirm.component.html',
  styleUrls: ['./return-order-confirm.component.css']
})
export class ReturnOrderConfirmComponent implements OnInit {

  orderService: OrderService;
  orderId: string = "";
  toastr: ToastrService;

  constructor(public modal: NgbActiveModal) { }

  ngOnInit(): void {
  }

  confirm(){
    this.orderService.confirmReturn(this.orderId.trim()).subscribe(order => {
      if(order.unitReturned && order.unit.isAvailable){
        this.toastr.success("Order was confirmed");
        this.modal.close();
      }else{
        this.toastr.error("Some error i guess idk?");
      }
    })
  }

}
