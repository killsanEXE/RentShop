import { HttpClient } from '@angular/common/http';
import { Component, HostListener, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs/operators';
import { Item, Photo, Unit } from 'src/app/models/item';
import { User } from 'src/app/models/user';
import { AccountService } from 'src/app/services/account.service';
import { ConfirmService } from 'src/app/services/confirm.service';
import { ItemService } from 'src/app/services/item.service';
import { environment } from 'src/environments/environment';
import { CreateUnitComponent } from '../create-unit/create-unit.component';
import { EditUnitComponent } from '../edit-unit/edit-unit.component';
import { UploadItemPhotoComponent } from '../upload-item-photo/upload-item-photo.component';
import { UploadMainPhotoComponent } from '../upload-main-photo/upload-main-photo.component';

@Component({
  selector: 'app-edit-item',
  templateUrl: './edit-item.component.html',
  styleUrls: ['./edit-item.component.css']
})
export class EditItemComponent implements OnInit {

  confirmServcie: ConfirmService;
  itemForm: FormGroup;
  item: Item;
  admin: User; 
  baseUrl = environment.apiUrl;
  
  @HostListener("window:beforeunload", ["$event"]) unloadNotification($event: any){
    if(this.itemForm.dirty){
      $event.returnValue = true;
    }
  }

  constructor(private route: ActivatedRoute, private fb: FormBuilder, private itemServcie: ItemService,
    private toastr: ToastrService, private modalService: NgbModal, private accountService: AccountService,
    private http: HttpClient) {
      this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.admin = user);
    }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.item = data.item;
    });
    this.initializeForm();
  }

  initializeForm(){
    this.itemForm = this.fb.group({
      name: [this.item.name, Validators.required],
      description: [this.item.description, Validators.required],
      pricePerDay: [this.item.pricePerDay, [Validators.required, Validators.pattern("^[0-9]*$"), this.checkNumbers()]],
      ageRestriction: [this.item.ageRestriction, [Validators.required, Validators.pattern("^[0-9]*$"), this.checkNumbers()]]
    });
  }

  checkNumbers(): ValidatorFn{
    return (control: AbstractControl) => {
      return parseInt(control?.value) >= 1 ? null : { isBelowZero: true }
    }
  }

  editItem(){
    if(this.itemForm.dirty){
      this.itemServcie.editItem(this.item.id, this.itemForm.value).subscribe(item => {
        this.item = item;
        this.itemForm.reset();
        this.initializeForm();
        this.toastr.success("Updated item");
      })
    }
  }

  uploadPhotos(item: Item){
    const modalRef = this.modalService.open(UploadItemPhotoComponent, {
      size: "lg",
      backdrop: "static",
      keyboard: false,
    })
    
    modalRef.componentInstance.user = this.admin;
    modalRef.componentInstance.item = item;
    modalRef.componentInstance.handler.subscribe((photo: Photo) => {
      this.item.photos.push(photo);
      modalRef.componentInstance.uploaded = true;
    });
  }

  uploadMainPhoto(item: Item){
    const modalRef = this.modalService.open(UploadMainPhotoComponent, {
      size: "lg",
      backdrop: "static",
      keyboard: false,
    })
    
    modalRef.componentInstance.user = this.admin;
    modalRef.componentInstance.item = item;
    modalRef.componentInstance.handler.subscribe((photo: Photo) => {
      this.item.previewPhoto = photo;
      modalRef.close();
    });
  }

  deletePhoto(photo: Photo){
    this.http.delete(this.baseUrl + `item/delete-photo/${this.item.id}-${photo.id}`).subscribe(() => {
      this.item.photos.splice(this.item.photos.indexOf(photo, 0), 1);
      this.toastr.success("Photo was deleted");
    })
  }

  createUnit(){
    const modalRef = this.modalService.open(CreateUnitComponent, {
      size: "lg",
      backdrop: "static",
      keyboard: false,
    })
    
    modalRef.componentInstance.handler.subscribe((unit) => {
      this.itemServcie.createUnit(this.item.id, unit).subscribe((createdUnit: Unit) => {
        this.item.units.push(createdUnit);
        this.toastr.success("Created unit");
      })
      modalRef.close();
    });
  }

  editUnit(unit: Unit){
    const modalRef = this.modalService.open(EditUnitComponent, {
      size: "lg",
      backdrop: "static",
      keyboard: false,
    })

    modalRef.componentInstance.unit = unit;
    modalRef.componentInstance.pointId = unit.point.id;
    modalRef.componentInstance.handler.subscribe((editedUnit) => {
      this.itemServcie.editUnit(this.item.id, editedUnit).subscribe((responseUnit: Unit) => {
        this.item.units.splice(this.item.units.indexOf(unit, 0), 1);
        this.item.units.push(responseUnit);
        this.toastr.success("Saved unit");
      })
      modalRef.close();
    });
  }

  enableUnit(unit: Unit){
    this.itemServcie.enableUnit(unit.id).subscribe(() => {
      this.item.units.find(f => f.id === unit.id).disabled = false;
      this.toastr.success("Activated unit");
    })
  }

  disableUnit(unit: Unit){
    this.itemServcie.disableUnit(unit.id).subscribe(() => {
      this.item.units.find(f => f.id === unit.id).disabled = true;
      this.toastr.success("Disabled unit");
    })
  }

}
