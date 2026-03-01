Fullstack Auktionslösning Beskrivning

Detta projekt är en fullstack-applikation bestående av:

Frontend: React (Vite)

Backend: ASP.NET Core Web API

Databas: SQL Server

Arkitektur: React klient ↔ Web API ↔ SQL Server

Applikationen är en auktionssajt där användare kan registrera sig, logga in, skapa auktioner och lägga bud. All data hämtas via Web API-anrop och lagras i en SQL Server-databas.

Teknisk Struktur
Backend (ASP.NET Core Web API)

Projektet innehåller:

Controllers

AuthController

AuctionsController

BidsController

AdminController

Models

User

Auction

Bid

DTOs

RegisterRequestDto

LoginRequestDto

CreateAuctionRequestDto

UpdateAuctionRequestDto

AuctionDetailsDto

AuctionListItemDto

AuthResponseDto

Data

AuctionDbContext

Entity Framework Core

Migrations

Databasen är kopplad via appsettings.json.

Frontend (React)

Frontend är byggd med:

React

React Router

Context API

Fetch-baserad API-service (api.js)

Struktur:

pages

LoginPage

RegisterPage

AuctionsPage

context

AuthContext

api.js
Användare

Registrera användare (namn, email, lösenord)

Logga in

Spara användardata i databas

🔨 Auktioner

Skapa auktion (titel, beskrivning, startpris, startdatum, slutdatum)

Auktion är öppen om slutdatum > aktuellt datum

Endast inloggad användare kan skapa auktion

Visa alla öppna auktioner

Söka på auktion via titel (endast öppna auktioner visas)

💰 Bud

Visa budhistorik på en auktion

Lägga bud om:

Auktionen är öppen

Användaren inte äger auktionen

Budet är högre än tidigare högsta bud

Felmeddelande visas om budet är för lågt

Ägaren kan inte lägga bud på sin egen auktion

🎨 Design

Strukturerad layout

Tydlig navigering

Responsiv design

Kod enligt kursens riktlinjer

⭐ Extra Funktionalitet (Väl Godkänd – VG)

JWT-autentisering

Endast inloggade användare kan:

Skapa auktioner

Uppdatera auktioner

Lägga bud

Möjlighet att:

Uppdatera eget lösenord

Söka även avslutade auktioner

Visa endast vinnande bud på avslutad auktion

Ta bort senaste bud (om auktionen inte är avslutad)

Uppdatera auktion (ej ändra pris om bud finns)

- Admin

Admin-inloggning

Inaktivera auktion (syns ej i sökningar)

Inaktivera användare (kan ej logga in)

 Autentisering

JWT-token används (för VG)

Token sparas i frontend

Skyddade endpoints kräver Authorization-header

 Databas

SQL Server

Entity Framework Core

Code First + Migrations

Skapa databasen:

Update-Database
 Hur man kör projektet
 Starta Backend

Öppna projektet i Visual Studio

Kontrollera connection string i appsettings.json

Kör migrations

Starta projektet

### Backend URL

När projektet körs lokalt startar backend på:

http://localhost:5220

Swagger UI:
http://localhost:5220/swagger
Frontend: http://localhost:5173

Obs: Porten kan variera beroende på launch profile i Visual Studio.
Kontrollera launchSettings.json om porten skiljer sig.
