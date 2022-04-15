import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { OrderService } from 'src/app/services/order.service';

@Component({
  selector: 'app-receive-order-confirm',
  templateUrl: './receive-order-confirm.component.html',
  styleUrls: ['./receive-order-confirm.component.css']
})
export class ReceiveOrderConfirmComponent implements OnInit {

  orderService: OrderService;
  orderId: string = "";
  toastr: ToastrService;

  constructor(public modal: NgbActiveModal) { }

  ngOnInit(): void {
  }

  confirm(){
    this.orderService.confirmReceive(this.orderId.trim()).subscribe(order => {
      if(order.deliveryCompleted){
        this.toastr.success("Confirmed");
        this.modal.close();
      }else{
        this.toastr.error("Some error i guess idk?");
      }
    })
  }

}
