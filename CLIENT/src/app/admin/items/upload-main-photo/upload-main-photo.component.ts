import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { FileItem, FileUploader } from 'ng2-file-upload';
import { Item, Photo } from 'src/app/models/item';
import { User } from 'src/app/models/user';
import { BusyService } from 'src/app/services/busy.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-upload-main-photo',
  templateUrl: './upload-main-photo.component.html',
  styleUrls: ['./upload-main-photo.component.css']
})
export class UploadMainPhotoComponent implements OnInit {

  uploader: FileUploader;
  item: Item;
  baseUrl = environment.apiUrl;
  user: User;
  @Output() handler: EventEmitter<Photo> = new EventEmitter();

  constructor(public modal: NgbActiveModal, private busyService: BusyService) { }

  ngOnInit(): void {
    this.initializeUploader();
  }

  initializeUploader(){
    this.uploader = new FileUploader({
      url: this.baseUrl + `item/add-main-photo/${this.item.id}`,
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
      this.busyService.busy();
      this.uploader.uploadAll();
    }

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      this.busyService.idle();
      if(response){
        this.handler.emit(JSON.parse(response))
      }
    }
  }

}
