import { useState } from "react";
import { useFilters } from "../context/FilterContext";
import { useVehicles } from "../hooks/useApi";
import { Province } from "../types/api";

interface FilterMenuProps {
  isOpen: boolean;
  onClose: () => void;
}

export function FilterMenu({ isOpen, onClose }: FilterMenuProps) {
  const { selectedProvinces, setSelectedProvinces, showStops, setShowStops, showVehicles, setShowVehicles } = useFilters();
  const { data: allVehicles = [], isLoading } = useVehicles(Province.All);
  const [lastUpdate] = useState(new Date());

  // Filter vehicles based on selected provinces
  const vehicles = allVehicles.filter((vehicle) => 
    selectedProvinces.has(vehicle.province as Province)
  );

  const provinces = [
    { value: Province.Liege, label: "Liège" },
    { value: Province.Namur, label: "Namur" },
    { value: Province.Hainaut, label: "Hainaut" },
    { value: Province.BrabantWallon, label: "Brabant Wallon" },
    { value: Province.Luxembourg, label: "Luxembourg" },
  ];

  const handleProvinceToggle = (province: Province) => {
    const newSelected = new Set(selectedProvinces);
    
    if (newSelected.has(province)) {
      newSelected.delete(province);
    } else {
      newSelected.add(province);
    }
    
    setSelectedProvinces(newSelected);
  };

  if (!isOpen) return null;

  return (
    <>
      {/* Backdrop */}
      <div
        className="fixed inset-0 bg-black/70 backdrop-blur-sm z-[999] animate-fadeIn"
        onClick={onClose}
      />
      
      {/* Slide-in Panel from Right - Responsive width */}
      <div className="fixed right-0 top-0 bottom-0 w-full lg:w-[420px] bg-dark-900 z-[1001] shadow-2xl animate-slide-right flex flex-col">
        {/* Header with Title and Close Button */}
        <div 
          className="shrink-0 p-5 border-b border-white/10"
          style={{
            background: 'linear-gradient(135deg, rgba(96, 165, 250, 0.15) 0%, rgba(167, 139, 250, 0.15) 100%)',
          }}
        >
          <h2 className="text-3xl font-bold text-center bg-gradient-to-r from-accent-blue to-accent-purple bg-clip-text text-transparent mb-4">
            Paramètres
          </h2>
          <button
            onClick={onClose}
            className="w-full glass glass-hover rounded-lg py-2.5 px-4 flex items-center justify-center gap-2 transition-all hover:scale-[1.02] active:scale-95"
          >
            <svg className="w-4 h-4 text-slate-300" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
            </svg>
            <span className="text-sm font-semibold text-slate-300">Fermer</span>
          </button>
        </div>

        {/* Content - Scrollable */}
        <div className="flex-1 overflow-y-auto p-4 space-y-4">
          {/* Province Selection - Checkboxes */}
          <div className="space-y-2 animate-slide-up">
            <label className="block text-xs font-bold text-slate-400 uppercase tracking-widest">
              Provinces
            </label>
            <div className="grid grid-cols-1 lg:grid-cols-2 gap-2">
              {provinces.map((province) => (
                <label 
                  key={province.value}
                  className="flex items-center gap-2 cursor-pointer px-3 py-2 glass glass-hover rounded-lg transition-all"
                >
                  <input
                    type="checkbox"
                    checked={selectedProvinces.has(province.value)}
                    onChange={() => handleProvinceToggle(province.value)}
                    className="w-3.5 h-3.5 text-accent-blue bg-dark-700 border-slate-600 rounded focus:ring-2 focus:ring-accent-blue/50 cursor-pointer"
                  />
                  <span className="text-sm text-slate-200 font-medium">
                    {province.label}
                  </span>
                </label>
              ))}
            </div>
          </div>

          {/* Display Options - Checkboxes */}
          <div className="glass rounded-lg p-4 space-y-2 border border-white/10 animate-slide-up" style={{ animationDelay: '0.1s' }}>
            <p className="text-xs font-bold text-slate-400 uppercase tracking-widest mb-2">Affichage</p>
            <label className="flex items-center gap-2.5 cursor-pointer glass-hover px-3 py-2 rounded-lg transition-all group">
              <input
                type="checkbox"
                checked={showStops}
                onChange={(e) => setShowStops(e.target.checked)}
                className="w-3.5 h-3.5 text-accent-blue bg-dark-700 border-slate-600 rounded focus:ring-2 focus:ring-accent-blue/50 cursor-pointer"
              />
              <span className="text-sm text-slate-200 font-medium group-hover:text-accent-blue transition-colors">Afficher les arrêts</span>
            </label>
            <label className="flex items-center gap-2.5 cursor-pointer glass-hover px-3 py-2 rounded-lg transition-all group">
              <input
                type="checkbox"
                checked={showVehicles}
                onChange={(e) => setShowVehicles(e.target.checked)}
                className="w-3.5 h-3.5 text-accent-blue bg-dark-700 border-slate-600 rounded focus:ring-2 focus:ring-accent-blue/50 cursor-pointer"
              />
              <span className="text-sm text-slate-200 font-medium group-hover:text-accent-blue transition-colors">Afficher les bus</span>
            </label>
          </div>

          {/* Stats Card - Gradient */}
          <div 
            className="rounded-lg p-4 border border-white/10 animate-slide-up"
            style={{
              background: 'linear-gradient(135deg, rgba(96, 165, 250, 0.1) 0%, rgba(167, 139, 250, 0.1) 100%)',
              animationDelay: '0.2s'
            }}
          >
            <div className="flex items-center gap-2 mb-3">
              <div className="w-1.5 h-1.5 bg-accent-blue rounded-full animate-pulse"></div>
              <p className="text-xs font-bold text-accent-blue uppercase tracking-widest">Statistiques</p>
            </div>
            <div className="space-y-2">
              <div className="flex justify-between items-center">
                <span className="text-xs text-slate-400 font-medium">Bus visibles</span>
                <span className={`text-2xl font-bold ${isLoading ? "text-slate-600" : "bg-gradient-to-r from-accent-blue to-accent-purple bg-clip-text text-transparent"}`}>
                  {isLoading ? "..." : vehicles.length}
                </span>
              </div>
              <div className="flex justify-between items-center pt-2 border-t border-white/10">
                <span className="text-xs text-slate-400 font-medium">Mise à jour</span>
                <span className="text-xs font-semibold text-slate-300">{lastUpdate.toLocaleTimeString("fr-FR")}</span>
              </div>
            </div>
          </div>

          {/* Legend - Modern Card */}
          <div className="glass rounded-lg p-4 border border-white/10 animate-slide-up" style={{ animationDelay: '0.3s' }}>
            <div className="flex items-center gap-2 mb-3">
              <div className="w-1.5 h-1.5 rounded-full bg-slate-400"></div>
              <p className="text-xs font-bold text-slate-400 uppercase tracking-widest">Légende</p>
            </div>
            <div className="space-y-2">
              <div className="flex items-center gap-2.5 px-2 py-1.5 rounded glass-hover transition-all group">
                <div className="w-3.5 h-3.5 bg-emerald-400 rounded-full shadow-md shadow-emerald-400/30 group-hover:scale-110 transition-transform"></div>
                <span className="text-sm text-slate-300 font-medium">Bus à l'heure</span>
              </div>
              <div className="flex items-center gap-2.5 px-2 py-1.5 rounded glass-hover transition-all group">
                <div className="w-3.5 h-3.5 bg-red-400 rounded-full shadow-md shadow-red-400/30 group-hover:scale-110 transition-transform"></div>
                <span className="text-sm text-slate-300 font-medium">Bus en retard</span>
              </div>
              <div className="flex items-center gap-2.5 px-2 py-1.5 rounded glass-hover transition-all group">
                <div className="w-3.5 h-3.5 rounded-full shadow-md shadow-blue-500/30 group-hover:scale-110 transition-transform" style={{ backgroundColor: '#0284c7' }}></div>
                <span className="text-sm text-slate-300 font-medium">Arrêts de bus</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </>
  );
}
