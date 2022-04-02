import { Component, Input, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { User } from 'src/app/models/user';
import { UserService } from 'src/app/services/user.service';

@Component({
  selector: 'app-admin-user',
  templateUrl: './admin-user.component.html',
  styleUrls: ['./admin-user.component.css']
})
export class AdminUserComponent implements OnInit {

  @Input() toastr: ToastrService;
  deliverymans: User[] = [];
  joinRequests: User[] = [];

  constructor(private userService: UserService) { }

  ngOnInit(): void {
    this.loadDeliverymans()
  }

  loadDeliverymans(){
    this.userService.getDeliverymans().subscribe(deliverymans => this.deliverymans = deliverymans);
    this.userService.getJoinRequests().subscribe(response => this.joinRequests = response);
  }

  addDeliveryman(user: User){
    this.userService.addDeliveryman(user.username).subscribe(response => {
      this.deliverymans.push(response);
      this.joinRequests.splice(this.joinRequests.indexOf(user, 0), 1);
      this.toastr.success(`${user.username} is deliveryman now`);
    })
  }

  denyDeliveryman(user: User){
    this.userService.denyDeliveryman(user.username).subscribe(() => {
      this.joinRequests.splice(this.joinRequests.indexOf(user, 0), 1);
      this.toastr.success("Deliveryman request was denied");
    })
  }

}
