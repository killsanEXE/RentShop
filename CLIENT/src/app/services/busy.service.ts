import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class BusyService {
  
  public isLoading: BehaviorSubject<boolean>= new BehaviorSubject<boolean>(false);

  constructor() { }

  // busy(){
  //   this.busyRequestCount++;
  //   this.spinnerService.show(undefined, {
  //     bdColor: "rgba(255,255,255,0)",
  //     color: "#333333",
  //   });
  // }

  // idle(){
  //   this.busyRequestCount--;
  //   if(this.busyRequestCount <= 0){
  //     this.busyRequestCount = 0;
  //     this.spinnerService.hide();
  //   }
  // }

  busy(){
    this.isLoading.next(true);
  }

  idle(){
    this.isLoading.next(false);
  }

}
