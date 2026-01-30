import { useEffect, useState, useCallback } from 'react';
import { MapContainer, TileLayer, Marker, Popup } from 'react-leaflet';
import * as L from 'leaflet';
import * as signalR from '@microsoft/signalr';
import 'leaflet/dist/leaflet.css';

delete ((L as any).Icon.Default.prototype as any)._getIconUrl;
(L as any).Icon.Default.mergeOptions({
  iconRetinaUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-icon-2x.png',
  iconUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-icon.png',
  shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-shadow.png',
});

interface VehicleDto {
  vehicleId: string;
  routeShortName: string;
  latitude: number;
  longitude: number;
  speed: number;
}

function App() {
  const [vehicles, setVehicles] = useState<VehicleDto[]>([]);
  const [connection, setConnection] = useState<signalR.HubConnection | null>(null);

  const connectToSignalR = useCallback(async () => {
    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:5001/hubs/vehicles')
      .withAutomaticReconnect()
      .build();

    newConnection.on('VehiclesUpdated', (data: VehicleDto[]) => {
      console.log('Vehicles updated:', data);
      setVehicles(data);
    });

    try {
      await newConnection.start();
      console.log('SignalR Connected');
      setConnection(newConnection);
    } catch (err) {
      console.error('SignalR Connection Error: ', err);
    }
  }, []);

  useEffect(() => {
    connectToSignalR();

    return () => {
      connection?.stop();
    };
  }, [connectToSignalR]);

  const position: [number, number] = [50.633, 5.567];

  return (
    <div style={{ height: '100vh', width: '100vw' }}>
      <div style={{ 
        position: 'absolute', 
        top: 10, 
        left: 10, 
        background: 'white', 
        padding: 10, 
        borderRadius: 5,
        zIndex: 1000 
      }}>
        <div>Bus en direct: {vehicles.length}</div>
      </div>
      
      <MapContainer
        {...({ center: position, zoom: 13, style: { height: '100%', width: '100%' } } as any)}
      >
        <TileLayer
          {...({ url: 'https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>' } as any)}
        />
        
        {vehicles.map((vehicle) => (
          <Marker 
            key={vehicle.vehicleId} 
            position={[vehicle.latitude, vehicle.longitude]}
          >
            <Popup>
              <strong>Bus {vehicle.vehicleId}</strong><br />
              Ligne: <span style={{color: 'blue'}}>{vehicle.routeShortName}</span><br />
              Vitesse: <strong>{vehicle.speed.toFixed(1)} km/h</strong>
            </Popup>
          </Marker>
        ))}
      </MapContainer>
    </div>
  );
}

export default App;
