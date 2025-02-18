# InlamningsUppgift_EF

# **Projektadministrationssystem**

Detta är en **projektadministrationsapplikation** byggd i .NET, med **Entity Framework Core**, **ASP.NET Core MVC** och **SQL Server**. Applikationen hanterar projekt, kunder, tjänster och beställningar via en **databasdriven lösning**.

## **Projektstruktur**

Projektet är uppdelat i tre huvudsakliga lager:

1. **Presentation Layer (ASP.NET Core MVC)**

   - **Hantera UI och interaktion mellan användaren och systemet.**
   - Razor Views (`.cshtml`) används för att rendera UI.
   - Controllers tar emot HTTP-anrop och skickar data mellan vyer och tjänstelagret.

2. **Business Layer (Affärslogik)**

   - **Hanterar all affärslogik, validering och tjänster.**
   - DTOs (`Data Transfer Objects`) används för att skicka data mellan backend och frontend.
   - Service-lagret kommunicerar med databasen via repositories.

3. **Data Layer (Databas och Repository Pattern)**
   - **Hanterar all datalagring och databaslogik.**
   - Entity Framework Core används som ORM.
   - Repository Pattern används för att separera databasåtkomst från affärslogiken.

## **Huvudfunktioner**

- **Projekt**

  - Skapa och hantera projekt med start- och slutdatum.
  - Tilldela en projektledare och hantera status.
  - Generera projektnummer automatiskt.
  - Koppla samman beställningar och sammanställningar.

- **Projektledare**

  - Registrera och hantera projektledare.

- **Kunder**

  - Lagra kunduppgifter såsom namn, organisationsnummer och adress.
  - Möjlighet att ange kundspecifika rabatter.

- **Tjänster**

  - Hantera tjänster som kan ingå i beställningar.

- **Beställningar**

  - Koppla kunder, projekt och tjänster.
  - Hantera timmar och priser samt beräkna totalpris.

- **Sammanställning**
  - Varje projekt kan ha en sammanfattning med totalt antal timmar och kostnader.

## **Arkitektur**

### **1. Presentation Layer (ASP.NET Core MVC)**

Projektet har ett **webbgränssnitt** byggt med **Razor Views** och **Bootstrap-stylad CSS**:

- **Controllers**

  - `AdminPageController` – Hanterar adminfunktioner, såsom att skapa och ta bort kunder, tjänster och projektledare.
  - `CreatePageController` – Används för att skapa nya projekt, kunder och beställningar.
  - `EditPageController` – Hanterar redigering av befintliga projekt och beställningar.
  - `HomeController` – Huvudsida som visar alla projekt.
  - `ProjectPageController` – Detaljvy för ett enskilt projekt.

- **ViewModels**

  - `ProjectViewModel` – Representerar projekt med dess beställningar och sammanfattning.
  - `OrderViewModel` – Hanterar information om kundbeställningar.
  - `CustomerViewModel` – Representerar kunder.
  - `AdminPageViewModel` – Innehåller en lista över kunder, tjänster och projektledare.

- **Konfigurationsfiler**
  - `_Layout.cshtml` – Grundläggande layout för alla sidor.
  - `_ValidationScriptsPartial.cshtml` – Hanterar klientvalidering.
  - `_Layout.cshtml.css` – Stilar för UI.

### **2. Business Layer (Tjänstelager)**

- **Tjänster (`Services`)** hanterar affärslogik och anrop till databasen via repositories.
- **DTOs (`Data Transfer Objects`)** används för att skicka data mellan backend och frontend.

### **3. Data Layer (Databas och Repository Pattern)**

- **Entity Framework Core** används för att hantera databasen.
- **Repositories** implementerar databaslogik:
  - **`BaseRepository<TEntity>`** – Generisk CRUD-hantering med transaktionsstöd.
  - **Specifika repositories** för **kunder, projekt, projektledare och tjänster**.
- **Migrations** används för att uppdatera databasschemat.

### **4. Programkonfiguration (`Program.cs`)**

I `Program.cs` registreras:

- Dependency Injection för tjänster och repositories.
- Konfiguration av Entity Framework Core och databasanvändning (`DefaultConnection` i `appsettings.json`).
- Routning och hantering av HTTP-request pipeline.

## **Databasförändringar och Migrationer**

Projektet använder **Entity Framework Core** för att hantera databasen. Nedan är några viktiga migrationer som genomförts:

- **`InitialCreate`**: Skapar grundläggande tabeller för kunder, projekt, tjänster och beställningar.
- **`RemoveHourlyRateFromServices`**: Tar bort `HourlyRate` från tjänstetabellen.
- **`RenameServiceNameColumn`**: Byter namn på `Name` i tjänstetabellen till `ServiceName`.
- **`ChangeToDecimalFoRHOurs`**: Ändrar `TotalHours` i sammanställningar från `int` till `decimal`.
- **`FixOrdersPrimaryKey`**: Justerar primärnyckeln för beställningar till att inkludera **ProjectID, CustomerID och ServiceID**.
- **`FixTruncateDecimal`**: Förbättrar decimalhantering vid lagring av värden.

## **Databasstruktur**

- **Projekt** (`Projects`)
- **Projektledare** (`ProjectLeaders`)
- **Kunder** (`Customers`)
- **Tjänster** (`Services`)
- **Beställningar** (`Orders`) – En order kopplar samman ett **projekt**, en **kund** och en **tjänst**.
- **Sammanställning** (`Summaries`) – Innehåller summering av timmar och totalpris för ett projekt.

## **Teknologier**

- **.NET Core**
- **ASP.NET Core MVC**
- **Entity Framework Core**
- **SQL Server**
- **Razor Views**
