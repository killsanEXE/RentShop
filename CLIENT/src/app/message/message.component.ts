import { AfterViewChecked, AfterViewInit, ChangeDetectionStrategy, Component, ElementRef, OnInit, QueryList, ViewChild } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { take } from 'rxjs/operators';
import { Client } from '../models/client';
import { Message } from '../models/message';
import { User } from '../models/user';
import { MessageService } from '../services/message.service';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush, 
  //DO YOU SEEE THIS 
  //THIS IS THE WAY TO GET RID OF THAT STUPID ERROR WHEN NEW MESSAGE WILL BE RECEIVED
  selector: 'app-message',
  templateUrl: './message.component.html',
  styleUrls: ['./message.component.css']
})
export class MessageComponent{

  user: User;
  otehrUserusername: string;
  message: string = "";
  @ViewChild("scrollMe") scrollMe: ElementRef;

  constructor(public modal: NgbActiveModal, public messageService: MessageService) {}

  scrollToBottom(){
    this.scrollMe.nativeElement.scrollTop = this.scrollMe.nativeElement.scrollHeight;
  }


  sendMessage(){
    this.messageService.sendMessage(this.otehrUserusername, this.message);
    this.message = "";
  }

  exit(){
    this.messageService.stopHubConnection();
    this.modal.close();
  }

}
