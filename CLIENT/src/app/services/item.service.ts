import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { of } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Item } from '../models/item';
import { getPaginatedResult, getPaginationHeaders } from '../models/paginationHelper';
import { UserParams } from '../models/userParams';

@Injectable({
  providedIn: 'root'
})
export class ItemService {

  baseUrl = environment.apiUrl;
  items: Item[] = [];
  itemCache = new Map();
  userParams: UserParams;

  constructor(private http: HttpClient) {
    this.userParams = new UserParams();
  }

  getUserParams(){
    return this.userParams;
  }

  setUserParams(params: UserParams){
    this.userParams = params;
  }

  loadItems(userParams: UserParams, old = false){
    var response = this.itemCache.get(Object.values(userParams).join("-"));
    if(response && !old) return of(response);

    let params = getPaginationHeaders(userParams.pageNumber, userParams.PageSize);
    return getPaginatedResult<Item[]>(this.baseUrl + "item", params, this.http).pipe(map(response => {
      this.itemCache.set(Object.values(userParams).join("-"), response);
      return response;  
    }));
  }

  getItem(id: number){
    const item = [...this.itemCache.values()]
      .reduce((arr, elem) => arr.concat(elem.result), [])
      .find((item: Item) => item.id === id);
    if(item) return of(item);
    return this.http.get<Item>(this.baseUrl + `item/${id}`); 
  }

  addItem(item: any){
    return this.http.post<Item>(this.baseUrl + "item/create", item).pipe(map((item: Item) => {
      return item
    }));
  }

  deleteItem(item: Item){
    return this.http.delete(this.baseUrl + `item/${item.id}`);
  }
}
