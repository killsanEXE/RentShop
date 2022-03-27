import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs/operators';
import { Point } from 'src/app/models/item';
import { User } from 'src/app/models/user';
import { AccountService } from 'src/app/services/account.service';
import { PointService } from 'src/app/services/point.service';
import { AddPointComponent } from '../add-point/add-point.component';
import { EditPointComponent } from '../edit-point/edit-point.component';

@Component({
  selector: 'app-point',
  templateUrl: './point.component.html',
  styleUrls: ['./point.component.css']
})
export class PointComponent implements OnInit {

  points: Point[] = [];
  admin: User;

  constructor(private accountService: AccountService, private modalService: NgbModal, private toastr: ToastrService,
    private pointService: PointService) {
      this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.admin = user);
    }

  ngOnInit(): void {
    this.loadPoints();
  }

  loadPoints(){
    this.pointService.loadPoints().subscribe(points => this.points = points);
  }

  addPoint(){
    const modalRef = this.modalService.open(AddPointComponent, {
      size: "lg",
      backdrop: "static",
      keyboard: false,
    })

    modalRef.componentInstance.points = this.points;

    modalRef.componentInstance.passEntry.subscribe((point) => {
      this.pointService.addPoint(point).subscribe(point => {
        this.points.push(point);
        this.toastr.success("Created point");
        modalRef.close();
      })
    }) 
  }

  editPoint(point: Point){
    const modalRef = this.modalService.open(EditPointComponent, {
      size: "lg",
      backdrop: "static",
      keyboard: false,
    })

    modalRef.componentInstance.editPoint = point;
    modalRef.componentInstance.country = point.country;
    modalRef.componentInstance.points = this.points;

    modalRef.componentInstance.passEntry.subscribe((editedPoint) => {
      this.pointService.editPoint(point.id, editedPoint).subscribe(newPoint => {
        this.points.splice(this.points.indexOf(point, 0), 1);
        this.points.push(newPoint);
        this.toastr.success("Edited point");
        modalRef.close();
      })
    }) 
  }

  enablePoint(point: Point){
    this.pointService.enablePoint(point.id).subscribe(() => {
      this.points.find(f => f.id === point.id).disabled = false;
      this.toastr.success("Activated point");
    })
  }

  disablePoint(point: Point){
    this.pointService.enablePoint(point.id).subscribe(() => {
      this.points.find(f => f.id === point.id).disabled = true;
      this.toastr.success("Disabled point");
    })
  }

}
