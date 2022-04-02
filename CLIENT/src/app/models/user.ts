export interface User {
    username: string
    token: string
    roles: string[]
    photoUrl: string
    age: number
    location: Location
    locations: Location[];
}

export interface Location {
    id: number
    country: string
    city: string
    address: string
    floor: number
    apartment: any
}