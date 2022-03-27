import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Point } from '../models/item';

@Injectable({
  providedIn: 'root'
})
export class PointService {

  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  loadPoints(){
    return this.http.get<Point[]>(this.baseUrl + "point").pipe(map((points: Point[]) => {
      return points;
    }));
  }

  addPoint(point: any){
    return this.http.post<Point>(this.baseUrl + "point", point).pipe(map((point: Point) => {
      return point;
    }));
  }

  getPoint(id: number){
    return this.http.get<Point>(this.baseUrl + `point/${id}`).pipe(map((point: Point) => {
      return point;
    }));
  }

  editPoint(id: number, point: any){
    return this.http.put<Point>(this.baseUrl + `point/${id}`, point).pipe(map((point: Point) => {
      return point;
    }));
  }

  // deletePoint(point1: number, point2: number){
  //   return this.http.delete(this.baseUrl + `point/${point1}-${point2}`);
  // }

  disablePoint(id: number){
    return this.http.put(this.baseUrl + `point/disable/${id}`, {});
  }

  enablePoint(id: number){
    return this.http.put(this.baseUrl + `point/enable/${id}`, {});
  }
}
