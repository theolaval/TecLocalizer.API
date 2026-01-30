import axios, { type AxiosInstance } from "axios";
import type { StopDto, VehicleDto, VehicleStats } from "../types/api";
import { Province } from "../types/api";

const API_BASE_URL = import.meta.env.VITE_API_URL || "http://localhost:5000/api";

class ApiClient {
  private client: AxiosInstance;

  constructor() {
    this.client = axios.create({
      baseURL: API_BASE_URL,
      timeout: 10000,
    });
  }

  /**
   * Récupère tous les arrêts, optionnellement filtrés par province
   */
  async getStops(province?: Province): Promise<StopDto[]> {
    const params = province && province !== Province.All ? { province } : {};
    const { data } = await this.client.get<StopDto[]>("/stops", { params });
    return data;
  }

  /**
   * Récupère les positions actuelles des véhicules
   */
  async getVehicles(province?: Province): Promise<VehicleDto[]> {
    const params = province && province !== Province.All ? { province } : {};
    const { data } = await this.client.get<VehicleDto[]>("/vehicles", { params });
    return data;
  }

  /**
   * Récupère les statistiques des véhicules
   */
  async getVehicleStats(): Promise<VehicleStats> {
    const { data } = await this.client.get<VehicleStats>("/vehicles/stats/summary");
    return data;
  }

  /**
   * Obtient l'instance du client pour un accès direct si nécessaire
   */
  getClient(): AxiosInstance {
    return this.client;
  }
}

export const apiClient = new ApiClient();
