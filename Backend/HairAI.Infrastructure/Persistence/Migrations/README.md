# Migrations

This folder contains the Entity Framework Core migrations for the HairAI application.

## Generating Migrations

To generate a new migration, run the following command from the Backend directory:

```bash
dotnet ef migrations add MigrationName --project HairAI.Infrastructure --startup-project HairAI.Api
```

## Applying Migrations

To apply migrations to the database, run:

```bash
dotnet ef database update --project HairAI.Infrastructure --startup-project HairAI.Api
```