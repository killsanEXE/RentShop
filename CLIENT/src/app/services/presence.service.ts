import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Group } from '../models/group';
import { User } from '../models/user';
import { AccountService } from './account.service';
import { MessageService } from './message.service';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {

  hubUrl = environment.hubUrl;
  private hubConnection: HubConnection;
  private onlineUserSource = new BehaviorSubject<string[]>([]);
  onlineUsers$ = this.onlineUserSource.asObservable();

  private groupsSource = new BehaviorSubject<Group[]>([]);
  groups$ = this.groupsSource.asObservable();

  constructor(private toastr: ToastrService, private messageService: MessageService) {
  }

  createHubConnection(user: User){
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + "presence", {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch(error => console.log(error));
    this.hubConnection.on("UserIsOnline", username => {
      this.onlineUsers$.pipe(take(1)).subscribe(usernames => {
        this.onlineUserSource.next([...usernames, username])
      });
    })

    this.hubConnection.on("UserIsOffline", username => {
      this.onlineUsers$.pipe(take(1)).subscribe(usernames => {
        this.onlineUserSource.next([...usernames.filter(x => x !== username)]);
      });
    })

    this.hubConnection.on("GetOnlineUsers", (usernames: string[]) => {
      this.onlineUserSource.next(usernames);
    });

    this.hubConnection.on("NewMessageReceived", ({username, knownAs}) => {
      this.toastr.info(knownAs + " sent u a new message")
        .onTap.pipe(take(1)).subscribe(() => {
          this.messageService.openDialog(user, username)
        });
    })

    this.hubConnection.on("ReceiveGroupsMessages", (respGroups: Group[]) => {
      this.updateGroups(respGroups);
    })
  }

  stopHubConnection(){
    this.hubConnection.stop().catch(error => console.log(error));
  }

  updateGroups(groups: Group[]){
    this.groupsSource.next(groups);
  }
}
