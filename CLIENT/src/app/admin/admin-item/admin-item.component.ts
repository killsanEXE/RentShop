import { Component, Input, OnInit } from '@angular/core';
import { PageEvent } from '@angular/material/paginator';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs/operators';
import { Item, Photo } from 'src/app/models/item';
import { Pagination } from 'src/app/models/pagination';
import { User } from 'src/app/models/user';
import { UserParams } from 'src/app/models/userParams';
import { AccountService } from 'src/app/services/account.service';
import { ConfirmService } from 'src/app/services/confirm.service';
import { ItemService } from 'src/app/services/item.service';
import { CreateItemComponent } from '../create-item/create-item.component';
import { UploadItemPhotoComponent } from '../upload-item-photo/upload-item-photo.component';

@Component({
  selector: 'app-admin-item',
  templateUrl: './admin-item.component.html',
  styleUrls: ['./admin-item.component.css']
})
export class AdminItemComponent implements OnInit {

  @Input() toastr: ToastrService;
  items: Item[] = [];
  pagination: Pagination = {
    currentPage: -1,
    totalItems: 1,
    totalPages: 1,
    itemsPerPage: 1
  };
  pageSizeOptions: number[] = [5, 10, 25];
  userParams: UserParams;
  pageEvent: PageEvent;
  admin: User;

  check: boolean = false;
  selectedItems: Item[] = [];

  constructor(private itemService: ItemService, private modalService: NgbModal,
    private confirmServcie: ConfirmService, private accountService: AccountService) {
    this.userParams = this.itemService.getUserParams();
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.admin = user);
  }

  ngOnInit(): void {
    this.loadItems();
  }

  popUp(){
    const modalRef = this.modalService.open(CreateItemComponent, {
      size: "lg",
      backdrop: "static",
      keyboard: false,
    })

    modalRef.componentInstance.confirmServcie = this.confirmServcie;
    modalRef.componentInstance.passEntry.subscribe((item) => {
      this.itemService.addItem(item).subscribe(item => {
        this.items.push(item);
        this.toastr.success("Item was added");  
        modalRef.close();
        this.uploadPhotos(item);
      });
    })    
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
      this.items.find(f => f.id === item.id).photos.push(photo);
      modalRef.componentInstance.uploaded = true;
    });
  }

  editItem(item: Item){
    console.log(item);
  }

  selectItem(item: Item, checked: boolean, mainCheckbox: any){
    if(!checked){
      this.selectedItems.splice(this.selectedItems.indexOf(item, 0), 1);
      mainCheckbox.checked = false;
    }
    else this.selectedItems.push(item)
  }

  selectAll(checked: boolean){
    if(!checked) this.selectedItems = [];
    else{
      this.items.forEach(f => this.selectedItems.push(f));
    }
  }

  deleteItems(){
    this.confirmServcie.confirm("Are you sure", `You are about to delete ${this.selectedItems.length} item(s)`)
      .subscribe(f => {
        if(f){
          this.selectedItems.forEach(f => {
            this.itemService.deleteItem(f).subscribe(() => {
              this.selectedItems.splice(this.selectedItems.indexOf(f, 0), 1);
              this.items.splice(this.items.indexOf(f, 0), 1);
              this.loadItems(true);
              this.toastr.success("Deleted item");
            })
          })
        }
      }
    )
  }

  loadItems(refresh = false){
    this.itemService.loadItems(this.itemService.getUserParams(), refresh).subscribe(response => {
      this.items = response.result;
      this.pagination = response.pagination;
    })
  }

  pageChanged(event: PageEvent){
    this.userParams.PageSize = event.pageSize;
    this.userParams.pageNumber = event.pageIndex+1;
    this.itemService.setUserParams(this.userParams);
    this.loadItems();
    return event;
  }

}
