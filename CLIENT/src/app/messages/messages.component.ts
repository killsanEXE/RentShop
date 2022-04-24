import { Component, OnInit } from '@angular/core';
import { take } from 'rxjs/operators';
import { User } from '../models/user';
import { AccountService } from '../services/account.service';
import { MessageService } from '../services/message.service';
import { PresenceService } from '../services/presence.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {

  user: User;

  constructor(public presenceService: PresenceService, private accountService: AccountService,
    private messageService: MessageService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user);
  }

  ngOnInit(): void {
  }

  message(username: string){
    this.messageService.openDialog(this.user, username);
  }

}
