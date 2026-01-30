import type { StopDto, VehicleDto } from "../types/api";

export function createStopPopupContent(stop: StopDto): string {
  return `
    <div class="stop-popup">
      <div class="popup-header">
        <h3>${stop.name}</h3>
        <p class="popup-subtitle">Arrêt de bus</p>
      </div>
      <div class="popup-content">
        <div class="popup-row">
          <span class="popup-label">📍 Code:</span>
          <span class="popup-value">${stop.code}</span>
        </div>
        <div class="popup-row">
          <span class="popup-label">🗺️ Province:</span>
          <span class="popup-value">${stop.province}</span>
        </div>
        <div class="popup-row">
          <span class="popup-label">📎 Coord:</span>
          <span class="popup-value">${stop.latitude.toFixed(4)}, ${stop.longitude.toFixed(4)}</span>
        </div>
      </div>
    </div>
  `;
}

export function createVehiclePopupContent(vehicle: VehicleDto): string {
  const isLate = vehicle.delayMinutes > 5;
  const statusIcon = isLate ? "🔴" : "🟢";
  const statusText = isLate ? "EN RETARD" : "À L'HEURE";
  const delaySign = vehicle.delayMinutes > 0 ? "+" : "";

  return `
    <div class="vehicle-popup">
      <div class="popup-header vehicle-${isLate ? "late" : "ontime"}">
        <h3>Ligne ${vehicle.routeShortName}</h3>
        <p class="popup-subtitle">${statusIcon} ${statusText}</p>
      </div>
      <div class="popup-content">
        <div class="popup-row">
          <span class="popup-label">⏱️ Retard:</span>
          <span class="popup-value ${isLate ? "text-red-600" : "text-green-600"}">${delaySign}${vehicle.delayMinutes} min</span>
        </div>
        <div class="popup-row">
          <span class="popup-label">🚗 Vitesse:</span>
          <span class="popup-value">${vehicle.speed.toFixed(1)} km/h</span>
        </div>
        <div class="popup-row">
          <span class="popup-label">🗺️ Province:</span>
          <span class="popup-value">${vehicle.province}</span>
        </div>
        <div class="popup-row">
          <span class="popup-label">📍 Coord:</span>
          <span class="popup-value text-xs">${vehicle.latitude.toFixed(4)}, ${vehicle.longitude.toFixed(4)}</span>
        </div>
      </div>
    </div>
  `;
}
