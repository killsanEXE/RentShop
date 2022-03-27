import { Injectable } from '@angular/core';
import {
  Router, Resolve,
  RouterStateSnapshot,
  ActivatedRouteSnapshot
} from '@angular/router';
import { Observable, of } from 'rxjs';
import { Item } from '../models/item';
import { ItemService } from '../services/item.service';

@Injectable({
  providedIn: 'root'
})
export class ItemDetailedResolver implements Resolve<Item> {

  constructor(private itemServcie: ItemService) {}

  resolve(route: ActivatedRouteSnapshot): Observable<Item> {

    return this.itemServcie.getItem(parseInt(route.paramMap.get("id")));
    
  }
}
