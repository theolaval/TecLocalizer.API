import { MapContainer, TileLayer, useMap } from "react-leaflet";
import type { StopDto, VehicleDto } from "../types/api";
import { Province } from "../types/api";
import { useVehicles, useStops } from "../hooks/useApi";
import { useFilters } from "../context/FilterContext";
import { createBusIcon, createStopIcon } from "../utils/iconUtils";
import { createStopPopupContent, createVehiclePopupContent } from "../utils/popupContent";
import { ZoomControls } from "./ZoomControls";
import * as leaflet from "leaflet";
import "leaflet/dist/leaflet.css";
import { useEffect } from "react";

const WALLONIA_CENTER: [number, number] = [50.5, 4.7];
const WALLONIA_ZOOM = 8;
const WALLONIA_BOUNDS: [[number, number], [number, number]] = [
  [49.4, 3.2],
  [50.9, 6.6],
];

// Component to setup map bounds
function MapSetup() {
  const map = useMap();
  useEffect(() => {
    map.setMaxBounds(WALLONIA_BOUNDS);
  }, [map]);
  return null;
}

// Component to render markers on the map
function MapMarkers() {
  const map = useMap();
  const { selectedProvinces, showStops, showVehicles } = useFilters();
  
  // Fetch data for all provinces
  const { data: allVehicles = [] } = useVehicles(Province.All);
  const { data: allStops = [] } = useStops(Province.All);

  // Filter vehicles and stops based on selected provinces
  const vehicles = allVehicles.filter((vehicle: VehicleDto) => 
    selectedProvinces.has(vehicle.province as Province)
  );
  
  const stops = allStops.filter((stop: StopDto) => 
    selectedProvinces.has(stop.province as Province)
  );

  useEffect(() => {
    const L = leaflet as any;
    
    // Clear all markers
    map.eachLayer((layer: any) => {
      if (layer instanceof L.Marker) {
        map.removeLayer(layer);
      }
    });

    // Add stop markers
    if (showStops) {
      stops.forEach((stop: StopDto) => {
        const iconData = createStopIcon();
        const icon = L.icon(iconData);
        const popupContent = createStopPopupContent(stop);

        L.marker([stop.latitude, stop.longitude], { icon, title: stop.name })
          .bindPopup(popupContent)
          .addTo(map);
      });
    }

    // Add vehicle markers
    if (showVehicles) {
      vehicles.forEach((vehicle: VehicleDto) => {
        const isLate = vehicle.delayMinutes > 5;
        const iconData = createBusIcon(isLate);
        const icon = L.icon(iconData);
        const popupContent = createVehiclePopupContent(vehicle);

        L.marker(
          [vehicle.latitude, vehicle.longitude],
          { icon, title: `Ligne ${vehicle.routeShortName}` }
        )
          .bindPopup(popupContent)
          .addTo(map);
      });
    }
  }, [map, stops, vehicles, showStops, showVehicles]);

  return null;
}

export function BusMap() {
  const mapProps = {
    center: WALLONIA_CENTER,
    zoom: WALLONIA_ZOOM,
    zoomControl: false, // Disable default zoom controls
    style: { width: "100%", height: "100%" },
  } as any;

  return (
    <MapContainer {...mapProps}>
      <MapSetup />
      <MapMarkers />
      <ZoomControls />
      {/* Dark themed map tiles */}
      <TileLayer 
        url="https://{s}.basemaps.cartocdn.com/dark_all/{z}/{x}/{y}{r}.png"
      />
    </MapContainer>
  );
}