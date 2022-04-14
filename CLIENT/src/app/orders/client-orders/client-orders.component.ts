import { Component, OnInit } from '@angular/core';
import { take } from 'rxjs/operators';
import { Order } from 'src/app/models/order';
import { User } from 'src/app/models/user';
import { AccountService } from 'src/app/services/account.service';
import { OrderService } from 'src/app/services/order.service';

@Component({
  selector: 'app-client-orders',
  templateUrl: './client-orders.component.html',
  styleUrls: ['./client-orders.component.css']
})
export class ClientOrdersComponent implements OnInit {

  user: User;
  orders: Order[] = [];
  filteredOrders: Order[] = [];

  constructor(private accountService: AccountService, private orderService: OrderService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user);
  }

  ngOnInit(): void {
    this.orderService.loadOrders().subscribe(orders => {
      this.orders = orders;
      this.filteredOrders = orders.filter(f => !f.cancelled);
    });
  }

}
