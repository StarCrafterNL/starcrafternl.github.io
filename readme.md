# Portfolio Web API

Een backend API gebouwd met **ASP.NET Core Minimal API**, **Entity Framework Core** en **ASP.NET Core Identity**, georganiseerd volgens de **Vertical Slice Architecture**.

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
Controleer in `appsettings.json` of de `DefaultConnection` overeenkomt met jouw lokale SQL Server-instantie (gebruik geen wachtwoorden of geheime sleutels in de repository):
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=LAPTOP-VIGO-SAM\\SQLEXPRESS;Database=PortfolioDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
}
```

### 3. Database & Identity-migraties toepassen
Voer het volgende commando uit om de database en zowel de domeintabellen (`Projects` en `BlogPosts`) als de ASP.NET Core Identity-tabellen (`AspNetUsers`, `AspNetRoles`, `AspNetUserRoles`) aan te maken:
```bash
dotnet ef database update
```

### 4. Applicatie starten
Start de API vanuit de map `WEBAPI/WEBAPI` met:
```bash
dotnet run
```
Bij het opstarten worden de rollen **`Gast`**, **`Editor`** en **`Admin`** automatisch aangemaakt in de database als deze nog niet bestaan.

### 5. Swagger UI openen
Zodra de applicatie draait, is de API en documentatie beschikbaar via Swagger UI in je browser:
* `https://localhost:7017/swagger`

---

## 🔐 Authenticatie, Rollen & Autorisatie

De API gebruikt **ASP.NET Core Identity** met cookie-authenticatie:
* **Registratie (`POST /api/auth/register`):** Maakt via `UserManager` een nieuw gebruikersaccount aan en kent altijd standaard de rol **`Gast`** toe. Publieke registratie kent nooit `Editor` of `Admin` toe.
* **Login (`POST /api/auth/login`):** Controleert de inloggegevens via `SignInManager` en geeft bij een geldige aanmelding een HTTP-only authenticatiecookie terug.
* **Autorisatieregels:**
  * Alle `GET`-routes voor `/api/projects` en `/api/blogposts` zijn **publiek** toegankelijk.
  * Alle `POST`-, `PUT`- en `DELETE`-routes vereisen expliciet de rol **`Editor`** of **`Admin`**.
  * Zonder aanmelding geeft een schrijfverzoek **`401 Unauthorized`** terug; een aangemelde `Gast` krijgt **`403 Forbidden`**.

### Lokaal testgebruikers (`Gast`, `Editor`, `Admin`) aanmaken
1. Registreer via Swagger UI (`POST /api/auth/register`) drie testaccounts, bijvoorbeeld `gast@portfolio.nl`, `editor@portfolio.nl` en `admin@portfolio.nl`. Alle drie krijgen automatisch de rol `Gast`.
2. Open SQL Server Management Studio (SSMS), selecteer `PortfolioDb` en voer onderstaand lokaal SQL-script uit om `Editor` en `Admin` handmatig toe te kennen:

```sql
-- Verwijder de standaard 'Gast'-rol bij de Editor en Admin
DELETE FROM AspNetUserRoles
WHERE UserId IN (
    SELECT Id FROM AspNetUsers WHERE Email IN ('editor@portfolio.nl', 'admin@portfolio.nl')
);

-- Ken de rol 'Editor' toe
INSERT INTO AspNetUserRoles (UserId, RoleId)
SELECT u.Id, r.Id
FROM AspNetUsers u, AspNetRoles r
WHERE u.Email = 'editor@portfolio.nl' AND r.Name = 'Editor';

-- Ken de rol 'Admin' toe
INSERT INTO AspNetUserRoles (UserId, RoleId)
SELECT u.Id, r.Id
FROM AspNetUsers u, AspNetRoles r
WHERE u.Email = 'admin@portfolio.nl' AND r.Name = 'Admin';
```
3. **Belangrijk:** Log na het wijzigen van een rol opnieuw in via `POST /api/auth/login` zodat het authenticatiecookie wordt vernieuwd met de nieuwe rol.

---

## 🧪 Testen met xUnit

In `WEBAPI/WEBAPI.Tests` staat een afzonderlijk xUnit-testproject dat met `WebApplicationFactory` en een geïsoleerde EF Core InMemory-database (los van SQL Server) alle CRUD- en authenticatieroutes via HTTP test:
* `AUTHTEST.cs` – tests voor registratie (standaardrol `Gast`), geldige login (cookie) en geweigerde login (`401`).
* `PROJECTTEST.cs` – tests voor alle 5 project-routes (`GET`, `POST`, `PUT`, `DELETE`), inclusief `401`, `403`, onbekend ID (`404`), ongeldige invoer (`400`) en database-effect voor `Editor` en `Admin`.
* `BLOGPOSTTEST.cs` – tests voor alle 5 blogpost-routes (`GET`, `POST`, `PUT`, `DELETE`), inclusief autorisatiechecks en database-effect.

### Tests uitvoeren
Ga naar de hoofdmap van de repository (waar het `.sln`-bestand staat) en draai:
```bash
dotnet test
```