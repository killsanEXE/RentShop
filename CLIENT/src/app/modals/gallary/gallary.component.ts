import { Component, Input, OnInit } from '@angular/core';
import { Photo } from 'src/app/models/item';

@Component({
  selector: 'app-gallary',
  templateUrl: './gallary.component.html',
  styleUrls: ['./gallary.component.css']
})
export class GallaryComponent implements OnInit {

  @Input() photos: Photo[];
  currentPhoto: Photo;
  currentPhotoIndex: number;

  constructor() { }

  ngOnInit(): void {
    if(this.photos.length > 0){
      this.currentPhoto = this.photos[0];
      this.currentPhotoIndex = 0;
    }
  }

  changeForward(){
    if(this.currentPhotoIndex < this.photos.length-1){
      this.currentPhoto = this.photos[this.currentPhotoIndex+1];
      this.currentPhotoIndex++;
    }
  }

  changeBackward(){
    if(this.currentPhotoIndex > 0){
      this.currentPhoto = this.photos[this.currentPhotoIndex-1];
      this.currentPhotoIndex--;
    }
  }

}
