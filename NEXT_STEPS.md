// NEXT_STEPS.md - Prochaines Étapes Immédiates

# 🎯 Prochaines Étapes - TEC Localizer

## ✅ Objectif : Passer du Prototype aux Vraies Données

### Étape 1 : Valider le Prototype (Aujourd'hui - 30 min)

```bash
# 1. Démarrer l'application
./start.sh  # ou start.cmd sur Windows

# 2. Vérifier dans navigateur
http://localhost:5173
# Vous devez voir :
# - Carte Wallonie
# - Arrêts bleus
# - Bus verts/rouges
# - Sidebar avec filtres

# 3. Tester API manuellement
curl http://localhost:5000/api/stops | jq
curl http://localhost:5000/api/vehicles | jq

# 4. Consulter Swagger
https://localhost:5001/swagger
```

**Si tout fonctionne** ✅ → Continuer

**Si erreurs** ❌ → Consulter INSTALL.md

---

### Étape 2 : Intégrer Vraies Données GTFS (1-2 jours)

#### 2.1 Télécharger GTFS Statique

```bash
# Option A : Télécharger ZIP manuellement
# https://transportdata.be/dataset/tec-gtfs
# Fichier : gtfs-tec.zip (~5MB)

# Option B : Script de téléchargement
mkdir -p data/gtfs
cd data/gtfs
# Télécharger et extraire gtfs-tec.zip
unzip gtfs-tec.zip
```

**Fichiers GTFS à traiter** :
- `stops.txt` - Arrêts avec coordonnées GPS
- `routes.txt` - Lignes de bus
- `trips.txt` - Trajets
- `stop_times.txt` - Horaires

#### 2.2 Parser GTFS en .NET

Créer service GTFS parser :

```csharp
// TecLocalizer.Infrastructure/GtfsParser.cs
namespace TecLocalizer.Infrastructure.Gtfs;

public class GtfsParser
{
    public List<Stop> ParseStops(string gtfsZipPath)
    {
        // 1. Unzip gtfs-tec.zip
        // 2. Lire stops.txt (CSV)
        // 3. Mapper Stop model
        // 4. Associer Province via coordonnées GPS
        
        var stops = new List<Stop>();
        using (var zip = ZipFile.OpenRead(gtfsZipPath))
        {
            var stopsFile = zip.GetEntry("stops.txt");
            using (var reader = new StreamReader(stopsFile.Open()))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<dynamic>();
                foreach (var record in records)
                {
                    var stop = new Stop
                    {
                        StopId = record.stop_id,
                        Name = record.stop_name,
                        Latitude = decimal.Parse(record.stop_lat),
                        Longitude = decimal.Parse(record.stop_lon),
                        Province = DetermineProvince(
                            decimal.Parse(record.stop_lat),
                            decimal.Parse(record.stop_lon)
                        )
                    };
                    stops.Add(stop);
                }
            }
        }
        return stops;
    }
    
    private Province DetermineProvince(decimal lat, decimal lng)
    {
        // Mapper coordonnées GPS → Province
        // Liège: 50.4-50.8, 5.2-5.8
        // Namur: 50.2-50.6, 4.6-5.0
        // Hainaut: 50.0-50.7, 3.5-4.5
        // Brabant Wallon: 50.5-50.9, 4.3-4.9
        // Luxembourg: 49.4-50.2, 5.5-6.2
        
        return lat switch
        {
            >= 50.4m and <= 50.8m when lng >= 5.2m and lng <= 5.8m => Province.Liege,
            >= 50.2m and <= 50.6m when lng >= 4.6m and lng <= 5.0m => Province.Namur,
            >= 50.0m and <= 50.7m when lng >= 3.5m and lng <= 4.5m => Province.Hainaut,
            >= 50.5m and <= 50.9m when lng >= 4.3m and lng <= 4.9m => Province.BrabantWallon,
            >= 49.4m and <= 50.2m when lng >= 5.5m and lng <= 6.2m => Province.Luxembourg,
            _ => Province.All
        };
    }
}
```

#### 2.3 Charger dans BD

```csharp
// TecLocalizer.API/Services/GtfsService.cs
public class GtfsService : IGtfsService
{
    private readonly TecDbContext _context;
    
    public async Task InitializeAsync()
    {
        // 1. Parser GTFS
        var parser = new GtfsParser();
        var stops = parser.ParseStops("data/gtfs/gtfs-tec.zip");
        var routes = parser.ParseRoutes("data/gtfs/gtfs-tec.zip");
        
        // 2. Charger en BD
        await _context.Stops.AddRangeAsync(stops);
        await _context.Routes.AddRangeAsync(routes);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Loaded {StopCount} stops and {RouteCount} routes", 
            stops.Count, routes.Count);
    }
}
```

#### 2.4 Migration EF Core

```bash
cd TecLocalizer.API
dotnet ef migrations add AddGtfsData --project TecLocalizer.DAL
dotnet ef database update
```

**Vérifier** :
```bash
# Requête directe
curl http://localhost:5000/api/stops | jq '.[] | .name' | head -10
# Doit lister vrais noms d'arrêts TEC
```

---

### Étape 3 : Intégrer GTFS Real-Time (Optional mais Important)

#### 3.1 Récupérer GTFS-RT (Protobuf)

```bash
# Source: https://beltac.tec-wl.be/api/v1/gtfs-rt/vehicle-positions
# Format: Protobuf (binaire)
```

#### 3.2 Parser Protobuf en .NET

```bash
# Installer
dotnet add package Google.Protobuf

# Définir message Protobuf (ou télécharger)
# transit_realtime.proto à partir de GTFS-RT spec
```

```csharp
// Parser GTFS-RT
var gtfsRtBytes = await FetchFromUrl("https://beltac.tec-wl.be/...");
var feed = transit_realtime.FeedMessage.Parser.ParseFrom(gtfsRtBytes);

foreach (var entity in feed.Entity)
{
    if (entity.Vehicle != null)
    {
        var position = new VehiclePosition
        {
            VehicleId = entity.Vehicle.Vehicle.Id,
            Latitude = entity.Vehicle.Position.Latitude,
            Longitude = entity.Vehicle.Position.Longitude,
            RouteId = entity.Vehicle.TripDescriptor.RouteId,
            UpdatedAt = DateTime.UtcNow
        };
        // Mapper province, délai, etc.
    }
}
```

#### 3.3 Polling Real-Time

Remplacer `GenerateMockVehiclePositions()` par :

```csharp
private async Task PollVehiclePositionsAsync(CancellationToken cancellationToken)
{
    while (!cancellationToken.IsCancellationRequested)
    {
        try
        {
            // Récupérer RT
            var positions = await FetchGtfsRt();
            
            // Mettre en cache
            lock (_cacheLock)
            {
                _cachedVehicles = positions;
                _lastUpdateTime = DateTime.UtcNow;
            }
            
            _logger.LogDebug("Updated {Count} vehicles", positions.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error polling GTFS-RT");
        }
        
        await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
    }
}
```

---

### Étape 4 : Ajouter Tests (1 jour)

#### 4.1 Setup Backend Tests

```bash
cd TecLocalizer.API
dotnet new xunit -n TecLocalizer.Tests
cd TecLocalizer.Tests
dotnet add reference ../TecLocalizer.BLL
dotnet add package Moq
```

#### 4.2 Écrire Tests

```csharp
// TecLocalizer.Tests/Services/GtfsServiceTests.cs
public class GtfsServiceTests
{
    [Fact]
    public async Task GetAllStopsAsync_ReturnsStops()
    {
        // Arrange
        var service = new GtfsService(_mockLogger);
        await service.InitializeAsync();
        
        // Act
        var stops = await service.GetAllStopsAsync();
        
        // Assert
        Assert.NotEmpty(stops);
        Assert.All(stops, s => Assert.NotNull(s.Name));
    }
    
    [Theory]
    [InlineData(Province.Liege)]
    [InlineData(Province.Namur)]
    public async Task GetAllStopsAsync_FilteredByProvince_ReturnsOnlyProvincialStops(Province province)
    {
        // Arrange
        var service = new GtfsService(_mockLogger);
        
        // Act
        var stops = await service.GetAllStopsAsync(province);
        
        // Assert
        Assert.All(stops, s => Assert.Equal(province.ToString(), s.Province));
    }
}
```

---

### Étape 5 : Déployer en Production (1-2 jours)

#### 5.1 Choix Cloud

- **Heroku** (gratuit pour prototype)
- **Azure App Service** (avec .NET support)
- **AWS** (complexe mais scalable)
- **DigitalOcean App Platform** (simple et abordable)

#### 5.2 Docker Build & Push

```bash
# Build image
docker build -t teclocalizer-api:1.0.0 .

# Tag pour registry
docker tag teclocalizer-api:1.0.0 myregistry/teclocalizer-api:1.0.0

# Push
docker push myregistry/teclocalizer-api:1.0.0
```

#### 5.3 Heroku Deploy (exemple simple)

```bash
# Install Heroku CLI
# https://devcenter.heroku.com/articles/heroku-cli

# Login
heroku login

# Create app
heroku create teclocalizer-api

# Add PostgreSQL addon
heroku addons:create heroku-postgresql:hobby-dev

# Deploy
git push heroku main

# View logs
heroku logs --tail
```

---

## 📋 Checklist Rapide (Copier-Coller)

```markdown
# Phase 2 - Vraies Données

## Setup Données GTFS
- [ ] Télécharger gtfs-tec.zip de transportdata.be
- [ ] Créer TecLocalizer.Infrastructure projet
- [ ] Implémenter GtfsParser (stops.txt, routes.txt)
- [ ] Tester parsing avec fichiers locaux
- [ ] Vérifier Province mapping par GPS

## Base de Données
- [ ] Créer migrations EF Core (Stop, Route tables)
- [ ] Configurer PostgreSQL (local ou Heroku)
- [ ] Charger données GTFS en BD
- [ ] Vérifier ~10k+ arrêts chargés

## API Update
- [ ] Modifier GtfsService pour lire de la BD
- [ ] Ajouter caching (Redis ou In-Memory)
- [ ] Tester endpoints avec vraies données
- [ ] Swagger génère bon schéma

## Tests
- [ ] Créer TecLocalizer.Tests projet
- [ ] GtfsService tests
- [ ] Controller integration tests
- [ ] Target couverture >80%

## Frontend Update
- [ ] Vérifier affichage réel des arrêts
- [ ] Tester filtres provinciaux
- [ ] Checker performance (clustering si besoin)

## Deployment
- [ ] Build Docker image
- [ ] Tester localement avec docker-compose
- [ ] Chose provider (Heroku/Azure/AWS)
- [ ] Créer CI/CD pipeline (GitHub Actions)
- [ ] First production deploy!

## Documentation
- [ ] Mettre à jour README avec vraies données
- [ ] Architecture diagrams
- [ ] API docs (Swagger)
```

---

## 🎓 Ressources Utiles

### GTFS
- **GTFS Spec** : https://gtfs.org/
- **GTFS Validators** : https://github.com/MobilityData/gtfs-validator
- **TEC Data** : https://transportdata.be/dataset/tec-gtfs

### Real-Time (GTFS-RT)
- **GTFS-RT Spec** : https://gtfs.org/en/realtime/
- **Proto Files** : https://github.com/google/transit/tree/master/realtime
- **Protobuf .NET** : https://github.com/protocolbuffers/protobuf/releases

### .NET EF Core
- **Migrations Guide** : https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/
- **DbContext Docs** : https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/

### Deployment
- **Heroku .NET** : https://devcenter.heroku.com/articles/getting-started-with-dotnet
- **Azure App Service** : https://azure.microsoft.com/en-us/services/app-service/
- **Docker .NET** : https://hub.docker.com/_/microsoft-dotnet

---

## 💬 Questions Courantes

**Q: Combien de temps prendra GTFS integration ?**  
A: 1-2 jours si vous suivez ce guide (parsing + tests + verification)

**Q: Avez-vous accès à GTFS-RT ?**  
A: Contacter opendata@letec.be ou tester avec données publiques Letec

**Q: Faut-il PostgreSQL ou autre BD ?**  
A: PostgreSQL recommandé pour production. SQLite OK pour dev.

**Q: Comment gérer volumes de données ?**  
A: Indexer stop_id, route_id. Paginer requêtes. Caching Redis.

**Q: Et après GTFS ?**  
A: Tests, clustering markers, heatmaps, mobile app, etc.

---

## 🚀 Résumé du Plan

```
Jour 1: Valider prototype + commencer GTFS parsing
Jour 2-3: Charger BD + tester vraies données
Jour 4: Tests + fixes bugs
Jour 5: Déploiement production

Total: ~5 jours de travail pour MVP complet
```

---

**Bonne chance ! Envoyez-moi les mises à jour !** 🎉

Pour questions : opendata@letec.be
