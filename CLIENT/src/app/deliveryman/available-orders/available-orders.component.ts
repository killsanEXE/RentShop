import { Component, Input, OnInit } from '@angular/core';
import { Order } from 'src/app/models/order';
import { User } from 'src/app/models/user';

@Component({
  selector: 'app-available-orders',
  templateUrl: './available-orders.component.html',
  styleUrls: ['./available-orders.component.css']
})
export class AvailableOrdersComponent implements OnInit {

  @Input() orders: Order[] = [];
  @Input() user: User;

  constructor() { }

  ngOnInit(): void {
  }

}
