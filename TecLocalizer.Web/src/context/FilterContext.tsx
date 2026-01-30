import type { ReactNode } from "react";
import { createContext, useContext, useState } from "react";
import { Province } from "../types/api";

interface FilterContextType {
  selectedProvinces: Set<Province>;
  setSelectedProvinces: (provinces: Set<Province>) => void;
  showStops: boolean;
  setShowStops: (show: boolean) => void;
  showVehicles: boolean;
  setShowVehicles: (show: boolean) => void;
}

const FilterContext = createContext<FilterContextType | undefined>(undefined);

export function FilterProvider({ children }: { children: ReactNode }) {
  const [selectedProvinces, setSelectedProvinces] = useState<Set<Province>>(
    new Set([Province.Liege, Province.Namur, Province.Hainaut, Province.BrabantWallon, Province.Luxembourg])
  );
  const [showStops, setShowStops] = useState(false);
  const [showVehicles, setShowVehicles] = useState(true);

  return (
    <FilterContext.Provider
      value={{
        selectedProvinces,
        setSelectedProvinces,
        showStops,
        setShowStops,
        showVehicles,
        setShowVehicles,
      }}
    >
      {children}
    </FilterContext.Provider>
  );
}

export function useFilters(): FilterContextType {
  const context = useContext(FilterContext);
  if (!context) {
    throw new Error("useFilters must be used within FilterProvider");
  }
  return context;
}
