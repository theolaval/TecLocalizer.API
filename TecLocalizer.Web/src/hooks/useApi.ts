import { useQuery, useQueryClient } from "@tanstack/react-query";
import { apiClient } from "../services/api";
import type { VehicleDto } from "../types/api";
import { Province } from "../types/api";

/**
 * Hook pour récupérer les positions actuelles des véhicules
 */
export function useVehicles(province?: Province) {
  return useQuery({
    queryKey: ["vehicles", province || "all"],
    queryFn: () => apiClient.getVehicles(province),
    staleTime: 10000, // 10 secondes
    refetchInterval: 30000, // Rafraîchir toutes les 30 secondes
    refetchOnWindowFocus: true,
  });
}

/**
 * Hook pour récupérer tous les arrêts
 */
export function useStops(province?: Province) {
  return useQuery({
    queryKey: ["stops", province || "all"],
    queryFn: () => apiClient.getStops(province),
    staleTime: 5 * 60 * 1000, // 5 minutes (données statiques)
    refetchOnWindowFocus: false,
  });
}

/**
 * Hook pour forcer un rafraîchissement manuel
 */
export function useRefresh() {
  const queryClient = useQueryClient();

  return async () => {
    await queryClient.refetchQueries({
      queryKey: ["vehicles"],
    });
    await queryClient.refetchQueries({
      queryKey: ["stops"],
    });
  };
}

/**
 * Hook pour obtenir les véhicules filtrés avec statut de délai
 */
export function useVehiclesWithStatus(province?: Province) {
  const { data: vehicles = [], ...rest } = useVehicles(province);

  const vehiclesWithStatus = vehicles.map((vehicle: VehicleDto) => ({
    ...vehicle,
    status: vehicle.delayMinutes > 5 ? "late" : vehicle.delayMinutes < -2 ? "early" : "ontime",
  }));

  return {
    data: vehiclesWithStatus,
    ...rest,
  };
}

/**
 * Hook pour grouper les véhicules par ligne
 */
export function useVehiclesByLine(province?: Province) {
  const { data: vehicles = [], ...rest } = useVehicles(province);

  const grouped = vehicles.reduce(
    (acc: Record<string, VehicleDto[]>, vehicle: VehicleDto) => {
      if (!acc[vehicle.routeShortName]) {
        acc[vehicle.routeShortName] = [];
      }
      acc[vehicle.routeShortName].push(vehicle);
      return acc;
    },
    {} as Record<string, VehicleDto[]>
  );

  return {
    data: grouped,
    ...rest,
  };
}
