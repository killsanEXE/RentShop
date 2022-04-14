import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { MessageComponent } from '../message/message.component';
import { Client } from '../models/client';
import { Group } from '../models/group';
import { Message } from '../models/message';
import { User } from '../models/user';
import { BusyService } from './busy.service';

@Injectable({
  providedIn: 'root'
})
export class MessageService {

  baseUrl = environment.apiUrl;
  hubUrl = environment.hubUrl;
  private hubConnection: HubConnection;
  private messageThreadSource = new BehaviorSubject<Message[]>([]);
  messageThread$ = this.messageThreadSource.asObservable();

  constructor(private modalService: NgbModal, private busyService: BusyService) { }

  openDialog(user: User, otehrUserusername: string){

    this.createHubConnection(user, otehrUserusername);

    const modalRef = this.modalService.open(MessageComponent, {
      size: "lg",
      backdrop: "static",
      keyboard: false,
    })


    modalRef.componentInstance.otehrUserusername = otehrUserusername;
    modalRef.componentInstance.user = user;
    

    modalRef.result.then(() => {
      this.stopHubConnection();
    });
  }

  createHubConnection(user: User, otherUsername: string){
    this.busyService.busy();
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + "message?user=" + otherUsername, {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start()
      .catch(error => console.log(error))
      .finally(() => this.busyService.idle());

    this.hubConnection.on("ReceiveMessageThread", messages => {
      this.messageThreadSource.next(messages);
    })

    this.hubConnection.on("NewMessage", message => {
      this.messageThread$.pipe(take(1)).subscribe(messages => {
        this.messageThreadSource.next([...messages, message]);
      });
    });

    this.hubConnection.on("UpdatedGroup", (group: Group) => { 
      if(group.connections.some(f => f.username === otherUsername)){
        this.messageThread$.pipe(take(1)).subscribe(messages => {
          messages.forEach(message => {
            if(!message.dateRead){
              message.dateRead = new Date(Date.now());
            }
          });
          this.messageThreadSource.next([...messages]);
        })
      }
    });
  }

  stopHubConnection(){
    if(this.hubConnection){
      this.messageThreadSource.next([]);
      this.hubConnection.stop();
    }
  }

  async sendMessage(username: string, content: string){
    return this.hubConnection.invoke("SendMessage", {recipientUsername: username, content})
      .catch(error => console.log(error));
  }

}
