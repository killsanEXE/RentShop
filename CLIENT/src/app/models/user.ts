export interface User {
    username: string
    token: string
    roles: string[]
    photoUrl: string
    age: number
    deliverymanRequest: boolean
    location: UserLocation
    locations: UserLocation[];
}

export interface UserLocation {
    id: number
    country: string
    city: string
    address: string
    floor: number
    apartment: any
}