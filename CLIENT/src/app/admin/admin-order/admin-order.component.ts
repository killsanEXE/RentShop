import { Component, Input, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { User } from 'src/app/models/user';
import { UserService } from 'src/app/services/user.service';

@Component({
  selector: 'app-admin-order',
  templateUrl: './admin-order.component.html',
  styleUrls: ['./admin-order.component.css']
})
export class AdminOrderComponent implements OnInit {

  @Input() toastr: ToastrService;
  deliverymans: User[] = [];

  constructor(private userService: UserService) { }

  ngOnInit(): void {
    this.loadDeliverymans()
  }

  loadDeliverymans(){
    this.userService.getDeliverymans().subscribe(deliverymans => this.deliverymans = deliverymans);
  }

}
