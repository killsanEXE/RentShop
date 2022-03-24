import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanDeactivate, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { CreateItemComponent } from '../admin/items/create-item/create-item.component';
import { ConfirmService } from '../services/confirm.service';

@Injectable({
  providedIn: 'root'
})
export class PreventUnsavedChangesGuard implements CanDeactivate<unknown> {

  constructor(private confirmService: ConfirmService) {}

  canDeactivate(
    component: CreateItemComponent): Observable<boolean> | boolean{
      if(component.itemForm.dirty){
        return this.confirmService.confirm()
      }
      return true;
    }
  
}
