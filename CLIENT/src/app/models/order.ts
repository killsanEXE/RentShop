import { Client } from "./client"
import { Point, Unit } from "./item"
import { UserLocation } from "./user"

export interface Order{
    id: number
    unit: Unit
    client: Client
    deliveryMan: Client
    deliveryLocation: UserLocation
    deliveryDate: string
    returnDate: string
    deliveryInProcess: boolean
    cancelled: boolean
    deliveryCompleted: boolean
    clientGotDelivery: boolean
    inUsage: boolean
    returnDeliveryman: Client
    unitReturned: boolean
    returnFromLocation: UserLocation
    returnPoint: Point
}