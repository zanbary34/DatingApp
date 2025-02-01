import { Photo } from "./photo"

export interface Member {
    id: number
    userName: string
    age: number
    photoURL: string
    knownAs: string
    created: Date
    lastActive: Date
    gender: string
    interests: any
    lookingFor: string
    city: any
    country: string
    photos: Photo[]
  }

 