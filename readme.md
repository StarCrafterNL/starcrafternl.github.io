# Portfolio Web API

Een backend API gebouwd met **ASP.NET Core Minimal API** en **Entity Framework Core**, georganiseerd volgens de **Vertical Slice Architecture**.

---

## 🚀 Benodigdheden

Om dit project lokaal uit te voeren heb je het volgende nodig:
* [.NET 10 SDK](https://dotnet.microsoft.com/) (of .NET 8/9)
* [SQL Server Express](https://www.microsoft.com/sql-server) of SQL Server Management Studio (SSMS)
* Visual Studio Code of Visual Studio

---

## 🛠️ Installatie & Uitvoering

Volg onderstaande stappen om het project lokaal te draaien:

### 1. Repository klonen
```bash
git clone https://github.com/StarCrafterNL/starcrafternl.github.io.git
cd starcrafternl.github.io/WEBAPI/WEBAPI
```

### 2. Connection String controleren
Controleer in `appsettings.json` of de `DefaultConnection` overeenkomt met jouw lokale SQL Server-instantie:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=LAPTOP-VIGO-SAM\\SQLEXPRESS;Database=PortfolioDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
}
```

### 3. Database & Migraties toepassen
Voer het volgende commando uit om de database en de benodigde tabellen (`Projects` en `Blogposts`) aan te maken:
```bash
dotnet ef database update
```

### 4. Applicatie starten
Start de API met:
```bash
dotnet run
```

### 5. Swagger UI openen
Zodra de applicatie draait, is de API en documentatie beschikbaar via Swagger UI in je browser:
* `https://localhost:<POORT>/swagger`

---