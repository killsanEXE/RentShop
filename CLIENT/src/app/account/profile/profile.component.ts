import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { FileItem, FileUploader } from 'ng2-file-upload';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs/operators';
import { Point } from 'src/app/models/item';
import { UserLocation, User } from 'src/app/models/user';
import { AccountService } from 'src/app/services/account.service';
import { BusyService } from 'src/app/services/busy.service';
import { PointService } from 'src/app/services/point.service';
import { environment } from 'src/environments/environment';
import { AddLocationComponent } from '../add-location/add-location.component';
import { BecomeDeliverymanComponent } from '../become-deliveryman/become-deliveryman.component';
import { EditLocationComponent } from '../edit-location/edit-location.component';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {

  uploader: FileUploader;
  baseUrl = environment.apiUrl;
  user: User;
  countries: string[] = [];

  constructor(private busyService: BusyService, private accountService: AccountService, 
    private modalService: NgbModal, private toastr: ToastrService, private pointServcie: PointService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user);
  }

  ngOnInit(): void {
    this.initializeUploader();
    this.pointServcie.loadPoints().subscribe(points => {
      this.countries = [...new Set(points.map(point => point.country))];
    })
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
      this.busyService.busy();
      this.uploader.uploadAll();
    }

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      this.busyService.idle();
      if(response){
        this.user.photoUrl = response;
        this.accountService.setCurrentUser(this.user);
      }
    }
  }

  becomeDeliveryman(){
    const modalRef = this.modalService.open(BecomeDeliverymanComponent, {
      size: "lg",
      backdrop: "static",
      keyboard: false,
    })

    modalRef.componentInstance.countries = this.countries;

    modalRef.componentInstance.handler.subscribe((joinRequest) => {
      this.accountService.becomeDeliveryman(joinRequest).subscribe((deliveryman: User) => {
        if(deliveryman.deliverymanRequest){
          this.user.deliverymanRequest = true;
          this.accountService.setCurrentUser(this.user);
          this.toastr.success("Sended request");  
          modalRef.close();
        }else this.toastr.error("Failed to send request");
      });
    }) 
  }

  addLocation(){
    const modalRef = this.modalService.open(AddLocationComponent, {
      size: "lg",
      backdrop: "static",
      keyboard: false,
    })

    modalRef.componentInstance.countries = this.countries;

    modalRef.componentInstance.handler.subscribe((location) => {
      this.accountService.addLocation(location).subscribe((addedLocation: UserLocation) => {
        this.user.locations.push(addedLocation);
        this.accountService.setCurrentUser(this.user);
        this.toastr.success("Added new location");  
        modalRef.close();
      });
    }) 
  }

  editLocation(location: UserLocation){
    const modalRef = this.modalService.open(EditLocationComponent, {
      size: "lg",
      backdrop: "static",
      keyboard: false,
    })

    modalRef.componentInstance.countries = this.countries;
    modalRef.componentInstance.location = location;

    modalRef.componentInstance.handler.subscribe((editedLocation) => {
      this.accountService.editLocation(location.id, editedLocation).subscribe((responseLocation: UserLocation) => {
      this.user.locations.splice(this.user.locations.indexOf(location, 0), 1);
        this.user.locations.push(responseLocation);
        this.accountService.setCurrentUser(this.user);
        this.toastr.success("Updated location");  
        modalRef.close();
      });
    }) 
  }

  deleteLocation(location: UserLocation){
    this.accountService.deleteLocation(location.id).subscribe(() => {
      this.user.locations.splice(this.user.locations.indexOf(location, 0), 1);
      this.accountService.setCurrentUser(this.user);
      this.toastr.success("Deleted location");
    })
  }

}
