import { useState } from "react";
import { QueryClientProvider, QueryClient } from "@tanstack/react-query";
import { FilterProvider } from "./context/FilterContext";
import { BusMap } from "./components/BusMap";
import { FilterMenu } from "./components/FilterMenu";
import "./App.css";

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      retry: 1,
      refetchOnWindowFocus: false,
    },
  },
});

export function App() {
  const [filterOpen, setFilterOpen] = useState(false);

  return (
    <QueryClientProvider client={queryClient}>
      <FilterProvider>
        {/* Unified Layout - Full screen map with floating button */}
        <div className="h-screen bg-dark-950">
          {/* Map - Full screen */}
          <div className="h-full relative">
            <BusMap />

            {/* Floating Settings Button */}
            <button
              onClick={() => setFilterOpen(true)}
              className="absolute bottom-8 right-6 z-[1000] glass glass-hover rounded-2xl p-4 shadow-2xl transition-all duration-300 hover:scale-105 active:scale-95"
              style={{ 
                pointerEvents: 'auto',
                background: 'linear-gradient(135deg, rgba(96, 165, 250, 0.2) 0%, rgba(167, 139, 250, 0.2) 100%)',
              }}
              title="Paramètres"
            >
              <svg className="w-7 h-7 text-accent-blue" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 6V4m0 2a2 2 0 100 4m0-4a2 2 0 110 4m-6 8a2 2 0 100-4m0 4a2 2 0 110-4m0 4v2m0-6V4m6 6v10m6-2a2 2 0 100-4m0 4a2 2 0 110-4m0 4v2m0-6V4" />
              </svg>
            </button>
          </div>

          {/* Unified Filter Menu (Mobile & Desktop) */}
          <FilterMenu isOpen={filterOpen} onClose={() => setFilterOpen(false)} />
        </div>
      </FilterProvider>
    </QueryClientProvider>
  );
}

export default App;