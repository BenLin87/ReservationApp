# ReservationApp

ReservationApp
	主程式，預設頁面為http://localhost:8081/

Migrations
	存放不同資料庫的Migration，目前支援MS SQL Express與PostgreSql
	參考來源:https://blog.jetbrains.com/dotnet/2022/08/24/entity-framework-core-and-multiple-database-providers/

ReservationApp.Models
	定義DbContext結構(有更動時應以EF Core Code First方式更新欲使用的資料庫)，處理初始化動作及清空資料

EF Migration指令範例，紀錄用
	Add-Migration InitialCreate -Project ReservationApp.LocalDb -Context ReservationApp.Models.ReservationContext -OutputDir ./Migrations
	Add-Migration InitialCreate -Project ReservationApp.PostgreSql -Context ReservationApp.Models.ReservationContext -OutputDir ./Migrations
	Update-Database -Context ReservationApp.Models.ReservationContext


