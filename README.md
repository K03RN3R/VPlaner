# VPlaner

Webanwendung zur Terminplanung und Anwesenheitsverwaltung eines Karnevalsvereins.

## Was sie macht

- Importiert die jährliche Terminliste des Vereins aus einer PDF
- Verwaltet Abteilungen und deren Mitglieder
- Erfasst pro Termin die Zu- und Absagen jeder Person (Status: Ausstehend, Zusage, Absage)
- Filtert Termine nach Abteilung (ein Termin kann mehrere Abteilungen betreffen)
- Sperrt vergangene Termine automatisch
- Behält beim Re-Import einer aktualisierten PDF alle eingetragenen Statuswerte

## Wie sie es macht

- **Blazor Server** rendert die UI serverseitig und synchronisiert sie per SignalR
  mit dem Browser — keine Client-Logik, kein JavaScript nötig
- **Entity Framework Core** auf **SQLite** als Persistenzschicht, Schema-Evolution
  über Code-First Migrations
- **PdfPig** liest die Terminliste spaltenweise aus und erkennt pro Zeile Datum,
  Veranstaltung, Ort und zugeordnete Abteilungen
- **M:N-Beziehungen** über explizite Junction-Entities: Personen können in mehreren
  Abteilungen aktiv sein, Termine mehrere Abteilungen betreffen
- **Rollenbasierte Zugriffskontrolle** via ASP.NET Core Identity (Admin / User)
- **Schichtenarchitektur** mit drei Projekten: `Core` (Models), `Infrastructure`
  (DbContext, Services, PDF-Parser), `VPlaner` (UI)

## Tech-Stack

- .NET 10 (LTS)
- Blazor Server
- Entity Framework Core 10
- SQLite
- ASP.NET Core Identity
- PdfPig
- C# 13

## Status

In aktiver Entwicklung als Lern- und Portfolio-Projekt.
