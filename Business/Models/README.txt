📌 REGLER FÖR DTOs OCH MODELS
---------------------------------------
1. DTOs (Data Transfer Objects) finns i Business/Dtos.
   - De används för att skicka data mellan lager.
   - Ingen affärslogik får finnas i en DTO.

2. När du behöver lägga till affärslogik i en DTO:
   - Skapa en motsvarande Model i Business/Models.
   - Kopiera över egenskaperna från DTO.
   - Lägg till affärslogik i Model.

3. Exempel:
   - ProductDto.cs (DTO, endast data)
   - ProductModel.cs (Model, innehåller affärslogik som "IsExpensive => Price > 1000")

🚀 Tänk på: En Model kan existera **utan en DTO**, men en DTO får aldrig innehålla affärslogik!
