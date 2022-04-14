import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Order } from '../models/order';

@Injectable({
  providedIn: 'root'
})
export class OrderService {

  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

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

}
