import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Order } from '../models/order';
import { getPaginatedResult, getPaginationHeaders } from '../models/paginationHelper';
import { UserParams } from '../models/userParams';

@Injectable({
  providedIn: 'root'
})
export class OrderService {

  baseUrl = environment.apiUrl;

  userParams: UserParams;
  orderCache = new Map();
  allOrders: Order[] = [];

  constructor(private http: HttpClient) {
    this.userParams = new UserParams();
  }

  loadOrders(){
    return this.http.get<Order[]>(this.baseUrl + "order/my-orders").pipe(map((orders: Order[]) => {
      return orders;
    }))
  }

  createOrder(order: any){
    return this.http.post<Order>(this.baseUrl + "order", order).pipe(map((order: Order) => {
      return order;
    }));
  }

  cancelOrder(id: number){
    return this.http.put<Order>(this.baseUrl + `order/cancel/${id}`, {}).pipe(map((order: Order) => {
      return order;
    }));
  }

  selfpickOrder(id: number){
    return this.http.put<Order>(this.baseUrl + `order/selfpick/${id}`, {}).pipe(map((order: Order) => {
      console.log(order);
      return order;
    }));
  }

  receivedOrder(id: number){
    return this.http.put<Order>(this.baseUrl + `order/received/${id}`, {}).pipe(map((order: Order) => {
      return order;
    }));
  }

  setReturnForm(form: any){
    return this.http.put<Order>(this.baseUrl + "order/return", form).pipe(map((order: Order) => {
      return order;
    }));
  }



  getAvailableOrders(){
    return this.http.get<Order[]>(this.baseUrl + "order/available").pipe(map((orders: Order[]) => {
      return orders;
    }))
  }

  getTakenOrders(){
    return this.http.get<Order[]>(this.baseUrl + "order/taken").pipe(map((orders: Order[]) => {
      return orders;
    }))
  }

  acceptOrder(id: number){
    return this.http.post<Order>(this.baseUrl + `order/${id}`, {}).pipe(map((order: Order) => {
      return order;
    }));
  }

  startDelivery(id: number){
    return this.http.put<Order>(this.baseUrl + `order/start-delivery/${id}`, {}).pipe(map((order: Order) => {
      return order;
    }));
  }

  completeDelivery(id: number){
    return this.http.put<Order>(this.baseUrl + `order/delivered/${id}`, {}).pipe(map((order: Order) => {
      return order;
    }));
  }

  

  confirmReturn(id: string){
    return this.http.put<Order>(this.baseUrl + `order/confirm-return/${id}`, {}).pipe(map((order: Order) => {
      return order;
    }));
  }

  confirmReceive(id: string){
    return this.http.put<Order>(this.baseUrl + `order/confirm-receive/${id}`, {}).pipe(map((order: Order) => { return order; }));
  }

  getUserParams(){ return this.userParams; }
  setUserParams(params: UserParams){ this.userParams = params; }

  loadAllOrders(userParams: UserParams, old: boolean = false){
    var response = this.orderCache.get(Object.values(userParams).join("-"));
    if(response && !old) return of(response);

    let params = getPaginationHeaders(userParams.pageNumber, userParams.PageSize);

    params = params.append("showAll", userParams.showAll);

    return getPaginatedResult<Order[]>(this.baseUrl + "order", params, this.http).pipe(map(response => {
      this.orderCache.set(Object.values(userParams).join("-"), response);
      return response;  
    }));
    // return this.http.get<Order[]>(this.baseUrl + "order").pipe(map((orders: Order[]) => {
    //   return orders;
    // }));
  }

}
