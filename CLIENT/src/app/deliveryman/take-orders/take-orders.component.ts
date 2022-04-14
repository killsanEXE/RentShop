import { Component, Input, OnInit } from '@angular/core';
import { Order } from 'src/app/models/order';
import { User } from 'src/app/models/user';

@Component({
  selector: 'app-take-orders',
  templateUrl: './take-orders.component.html',
  styleUrls: ['./take-orders.component.css']
})
export class TakeOrdersComponent implements OnInit {

  @Input() orders: Order[] = []
  @Input() user: User;

  constructor() { }

  ngOnInit(): void {
  }

}
