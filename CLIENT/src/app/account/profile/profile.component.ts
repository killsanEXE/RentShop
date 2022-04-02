import { Component, OnInit } from '@angular/core';
import { FileItem, FileUploader } from 'ng2-file-upload';
import { take } from 'rxjs/operators';
import { User } from 'src/app/models/user';
import { AccountService } from 'src/app/services/account.service';
import { BusyService } from 'src/app/services/busy.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {

  uploader: FileUploader;
  baseUrl = environment.apiUrl;
  user: User;

  constructor(private busyService: BusyService, private accountService: AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user);
  }

  ngOnInit(): void {
    this.initializeUploader();
  }

  initializeUploader(){
    this.uploader = new FileUploader({
      url: this.baseUrl + "users/add-photo",
      authToken: "Bearer " + this.user.token,
      isHTML5: true,
      allowedFileType: ["image"],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10*1024*1024
    })

    this.uploader.onAfterAddingFile = (file: FileItem) => {
      if (this.uploader.queue.length > 1) {
        this.uploader.removeFromQueue(this.uploader.queue[0]);
      }
      file.withCredentials = false;
    }

    this.uploader.onBeforeUploadItem = () => {
      this.busyService.busy();
    }

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      this.busyService.idle();
      if(response){
        this.user.photoUrl = response;
        this.accountService.setCurrentUser(this.user);
      }
    }
  }

}
