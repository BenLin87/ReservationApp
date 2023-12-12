ReservationApp
===
###### Temporary development record, last updated : 2023/12

## Development environment
- Visual Studio 2022
- .Net 6.0
## Components
- EntityFrameworkCore 7.1.4
- EntityFrameworkCore.SqlServer 7.0.9
- EntityFrameworkCore.PostgreSQL 7.0.4
- JQuery 3.7.0
## Project description
- ReservationApp  
	Main program, the default page is http://localhost:8081/  
	The login page currently only checks the account/password as admin/admin. After logging in, you can perform modifications and deletions on reservation orders.
- ReservationApp.Models  
    Stores migrations for different databases. Support MS SQL Server Express (local) and PostgreSql (connection string should be configured in the appsettings.json of the ReservationApp project).  
	Reference : https://blog.jetbrains.com/dotnet/2022/08/24/entity-framework-core-and-multiple-database-providers/
- Migrations  
    Defines the structure of DbContext. When there are changes, using the EF Core Code First to update the desired database.

