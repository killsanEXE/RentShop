import { Component, OnInit } from '@angular/core';
import { Item } from '../models/item';
import { Pagination } from '../models/pagination';
import { ItemService } from '../services/item.service';
import {PageEvent} from '@angular/material/paginator';
import { UserParams } from '../models/userParams';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  items: Item[] = [];
  pagination: Pagination = {
    currentPage: -1,
    totalItems: 1,
    totalPages: 1,
    itemsPerPage: 1
  };
  pageSizeOptions: number[] = [5, 9];
  userParams: UserParams;
  pageEvent: PageEvent;

  constructor(private itemService: ItemService) {
    this.userParams = this.itemService.getUserParams();
  }

  ngOnInit(): void {
    this.loadItems();
  }

  loadItems(){
    this.itemService.loadItems(this.itemService.getUserParams()).subscribe(response => {
      this.items = response.result;
      this.pagination = response.pagination;
    })
  }

  pageChanged(event: PageEvent){
    this.userParams.PageSize = event.pageSize
    this.userParams.pageNumber = event.pageIndex+1;
    this.itemService.setUserParams(this.userParams);
    this.loadItems();
    return event;
  }
}
