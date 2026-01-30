// Create SVG icons as data URLs for custom markers

export const createBusIcon = (isLate: boolean) => {
  const color = isLate ? "#ef4444" : "#10b981"; // red if late, green if ontime
  
  const svgString = `
    <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="${color}" width="32" height="32">
      <!-- Bus body -->
      <rect x="4" y="4" width="16" height="14" rx="2" fill="${color}"/>
      
      <!-- Bus windows -->
      <rect x="6" y="6" width="4" height="3" fill="white" rx="1"/>
      <rect x="12" y="6" width="4" height="3" fill="white" rx="1"/>
      
      <!-- Door -->
      <rect x="6" y="10" width="3" height="5" fill="white" opacity="0.6" rx="1"/>
      
      <!-- Wheels -->
      <circle cx="8" cy="19" r="1.5" fill="#333"/>
      <circle cx="16" cy="19" r="1.5" fill="#333"/>
    </svg>
  `;
  
  return {
    iconUrl: `data:image/svg+xml;base64,${btoa(svgString)}`,
    iconSize: [32, 32] as [number, number],
    iconAnchor: [16, 32] as [number, number],
    popupAnchor: [0, -32] as [number, number],
    className: "bus-icon"
  };
};

export const createStopIcon = () => {
  const svgString = `
    <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="#0284c7" stroke-width="2" width="32" height="32">
      <!-- Stop post -->
      <rect x="7" y="6" width="10" height="14" rx="2" fill="#0284c7" stroke="#0284c7"/>
      
      <!-- Stop label area -->
      <rect x="9" y="8" width="6" height="6" fill="white" rx="1"/>
      
      <!-- Pin point at bottom -->
      <path d="M 12 20 L 8 24 L 16 24 Z" fill="#0284c7"/>
    </svg>
  `;
  
  return {
    iconUrl: `data:image/svg+xml;base64,${btoa(svgString)}`,
    iconSize: [32, 32] as [number, number],
    iconAnchor: [16, 32] as [number, number],
    popupAnchor: [0, -32] as [number, number],
    className: "stop-icon"
  };
};
