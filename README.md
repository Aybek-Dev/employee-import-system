# Employee Import System

## Description
**Employee Import System** is a **ASP.NET MVC** web application designed to integrate with external systems by importing employee data from a **CSV file** into a **SQL Server** database.

The application is built using **Clean Architecture**, which ensures:
- **Modularity** – easily extendable code structure.
- **Testability** – support for unit testing.
- **Flexibility** – adaptable for different business needs.

---

## Technology Stack
- **Language:** C# 11  
- **Platform:** .NET 8  
- **Database:** SQL Server (in Docker)  
- **ORM:** Entity Framework Core (Code First)  

---

## Features
✔ Upload **CSV file** via web interface.  
✔ **Automatic parsing** and data validation.  
✔ **Import** data into **SQL Server**.  
✔ **Report** on the number of successfully processed records.  
✔ **Display** imported employees in an interactive table.  
✔ Support for **sorting, searching, and editing** data.  
✔ Utilization of a **third-party library** for table rendering.  

---

## Architecture
The application follows the principles of **Clean Architecture**:
- Separation into **layers (Web, Application, Domain, Infrastructure)**.
- Use of **Dependency Injection**.
- Interaction with the database via **Entity Framework Core**.

---

## Deployment
### 1. Start SQL Server in Docker:
```sh
docker-compose up -d
```

### 2. Configure connection in `appsettings.json`
Edit `ConnectionStrings:DefaultConnection` to specify SQL Server parameters.

### 3. Run tests:
```sh
dotnet test
```
