import { Photo } from "./item"

export interface DatasetItems{
    items: DatasetItem[];
}

export interface DatasetItem {
    name: string
    description: string
    pricePerDay: number
    ageRestriction: number
    previewPhoto?: Photo
    photos: Photo[]
}