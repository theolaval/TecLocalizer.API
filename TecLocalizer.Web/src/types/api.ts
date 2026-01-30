/**
 * Types partagés avec l'API backend
 */

export interface StopDto {
  stopId: string;
  name: string;
  latitude: number;
  longitude: number;
  province: string;
  code: string;
}

export interface VehicleDto {
  vehicleId: string;
  routeShortName: string;
  latitude: number;
  longitude: number;
  speed: number;
  delayMinutes: number;
  province: string;
  updatedAt: string;
}

export interface VehicleStats {
  totalVehicles: number;
  lastUpdate: string;
  byProvince: Array<{
    province: string;
    count: number;
  }>;
}

export enum Province {
  All = "All",
  Liege = "Liege",
  Namur = "Namur",
  Hainaut = "Hainaut",
  BrabantWallon = "BrabantWallon",
  Luxembourg = "Luxembourg",
}

export type ProvinceType = keyof typeof Province;
