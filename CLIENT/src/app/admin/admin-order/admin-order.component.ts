import { Component, Input, OnInit } from '@angular/core';
import { PageEvent } from '@angular/material/paginator';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs/operators';
import { Client } from 'src/app/models/client';
import { Order } from 'src/app/models/order';
import { Pagination } from 'src/app/models/pagination';
import { User } from 'src/app/models/user';
import { UserParams } from 'src/app/models/userParams';
import { AccountService } from 'src/app/services/account.service';
import { MessageService } from 'src/app/services/message.service';
import { OrderService } from 'src/app/services/order.service';

@Component({
  selector: 'app-admin-order',
  templateUrl: './admin-order.component.html',
  styleUrls: ['./admin-order.component.css']
})
export class AdminOrderComponent implements OnInit {

  @Input() toastr: ToastrService;
  orders: Order[] = [];
  pagination: Pagination = {
    currentPage: -1,
    totalItems: 1,
    totalPages: 1,
    itemsPerPage: 1
  };
  pageSizeOptions: number[] = [5, 20, 50];
  userParams: UserParams;
  pageEvent: PageEvent;
  admin: User;

  showCancelled = false;

  constructor(private orderService: OrderService, private messageService: MessageService, 
    private accountService: AccountService) {
      this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.admin = user);
    }

  ngOnInit(): void {
    this.userParams = this.orderService.getUserParams();
    this.loadOrders()
  }

  loadOrders(){
    this.orderService.loadAllOrders(this.orderService.getUserParams()).subscribe(response => {
      this.orders = response.result;
      // this.orders.map(f => console.log(f));
      this.pagination = response.pagination;
    })
  }

  message(otherUser: Client){ this.messageService.openDialog(this.admin, otherUser.username); }

  pageChanged(event: PageEvent){
    this.userParams.PageSize = event.pageSize;
    this.userParams.pageNumber = event.pageIndex+1;
    this.orderService.setUserParams(this.userParams);
    this.loadOrders();
    return event;
  }

  showAll(){
    this.userParams.showAll ? this.userParams.showAll = false : this.userParams.showAll = true;
    this.orderService.setUserParams(this.userParams);
    this.loadOrders();
  }

}
