import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Item } from 'src/app/models/item';
import { NgxGalleryOptions, NgxGalleryImage, NgxGalleryAnimation } from '@kolkov/ngx-gallery';

@Component({
  selector: 'app-item-detailed',
  templateUrl: './item-detailed.component.html',
  styleUrls: ['./item-detailed.component.css']
})
export class ItemDetailedComponent implements OnInit {

  item: Item;
  // gallaryOptions: NgxGalleryOptions[];
  // gallaryImages: NgxGalleryImage[];

  constructor(private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.item = data.item;
    });

    // this.gallaryOptions = [
    //   {
    //     imagePercent: 100,
    //     thumbnailsColumns: 4,
    //     imageAnimation: NgxGalleryAnimation.Slide,
    //     preview: false
    //   }
    // ]

    
    // this.gallaryImages = this.getImages();
  }

  getImages = () => {
    const imageUrls = []
    for(const photo of this.item.photos){
      imageUrls.push({
        small: photo?.url,
        medium: photo?.url,
        big: photo?.url,
      })
    }
    return imageUrls;
  }

}
