import { useMap } from "react-leaflet";

export function ZoomControls() {
  const map = useMap();

  const handleZoomIn = () => {
    map.zoomIn();
  };

  const handleZoomOut = () => {
    map.zoomOut();
  };

  return (
    <div className="absolute left-4 top-4 z-[400] flex flex-col gap-1 rounded-lg overflow-hidden pointer-events-auto shadow-xl">
      {/* Zoom In Button */}
      <button
        onClick={handleZoomIn}
        className="w-9 h-9 flex items-center justify-center glass glass-hover text-accent-blue font-bold text-xl transition-all active:scale-95 border-b border-white/10"
        title="Zoom avant"
        style={{ background: 'rgba(26, 26, 36, 0.85)' }}
      >
        +
      </button>

      {/* Zoom Out Button */}
      <button
        onClick={handleZoomOut}
        className="w-9 h-9 flex items-center justify-center glass glass-hover text-accent-blue font-bold text-xl transition-all active:scale-95"
        title="Zoom arrière"
        style={{ background: 'rgba(26, 26, 36, 0.85)' }}
      >
        −
      </button>
    </div>
  );
}
