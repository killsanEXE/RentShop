export interface Item {
    id: number
    name: string
    description: string
    previewPhotoUrl: string
    pricePerDay: number
    ageRestriction: number
    units: Unit[]
    photos: Photo[]
  }
  
  export interface Unit {
    id: number
    pointId: number
    description: string
    whenWillBeAvaliable: Date
    isAvaliable: boolean
    point: Point
  }
  
  export interface Point {
    photoUrl: any
    id: number
    country: string
    city: string
    address: string
    floor: number
    apartment: any
  }
  
  export interface Photo {
    id: number
    url: string
    isPreview: boolean
  }