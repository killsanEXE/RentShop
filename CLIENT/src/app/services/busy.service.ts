import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class BusyService {
  
  public isLoading: BehaviorSubject<boolean>= new BehaviorSubject<boolean>(false);

  constructor() { }

  busy(){
    this.isLoading.next(true);
  }

  idle(){
    this.isLoading.next(false);
  }

}
