import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { FileUploader } from 'ng2-file-upload';
import { Item, Photo } from 'src/app/models/item';
import { User } from 'src/app/models/user';
import { BusyService } from 'src/app/services/busy.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-upload-item-photo',
  templateUrl: './upload-item-photo.component.html',
  styleUrls: ['./upload-item-photo.component.css']
})
export class UploadItemPhotoComponent implements OnInit {
  
  uploader: FileUploader;
  user: User;
  item: Item;
  uploaded = false;
  baseUrl = environment.apiUrl;
  @Output() handler: EventEmitter<Photo> = new EventEmitter();

  constructor(public modal: NgbActiveModal, private busyService: BusyService) { }

  ngOnInit(): void {
    this.initializeUploader();
  }

  initializeUploader(){
    this.uploader = new FileUploader({
      url: this.baseUrl + `item/add-photo/${this.item.id}`,
      authToken: "Bearer " + this.user.token,
      isHTML5: true,
      allowedFileType: ["image"],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10*1024*1024
    })

    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false;
    }

    this.uploader.onBeforeUploadItem = () => {
      this.busyService.busy();
    }

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      this.busyService.idle();
      if(response){
        this.handler.emit(JSON.parse(response))
      }
    }
  }
}
