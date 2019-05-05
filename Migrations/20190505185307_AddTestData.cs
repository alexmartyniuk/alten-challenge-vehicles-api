using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace VehiclesAPI.Migrations
{
    public partial class AddTestData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData("Customers", new[] { "Id", "FullName", "Address" }, new object[] { 1, "Kalles Grustransporter AB", "Cementvägen 8, 111 11 Södertälje" });
            migrationBuilder.InsertData("Vehicles", new[] { "Id", "VehicleId", "RegistrationNumber", "CustomerId", "ConnectUpdated" }, new object[] { 1, "YS2R4X20005399401", "ABC123", 1, DateTime.UtcNow });
            migrationBuilder.InsertData("Vehicles", new[] { "Id", "VehicleId", "RegistrationNumber", "CustomerId", "ConnectUpdated" }, new object[] { 2, "VLUR4X20009093588", "DEF456", 1, DateTime.UtcNow });
            migrationBuilder.InsertData("Vehicles", new[] { "Id", "VehicleId", "RegistrationNumber", "CustomerId", "ConnectUpdated" }, new object[] { 3, "VLUR4X20009048066", "GHI789", 1, DateTime.UtcNow });

            migrationBuilder.InsertData("Customers", new[] { "Id", "FullName", "Address" }, new object[] { 2, "Johans Bulk AB", "Balkvägen 12, 222 22 Stockholm" });
            migrationBuilder.InsertData("Vehicles", new[] { "Id", "VehicleId", "RegistrationNumber", "CustomerId", "ConnectUpdated" }, new object[] { 4, "YS2R4X20005388011", "JKL012", 2, DateTime.UtcNow });
            migrationBuilder.InsertData("Vehicles", new[] { "Id", "VehicleId", "RegistrationNumber", "CustomerId", "ConnectUpdated" }, new object[] { 5, "YS2R4X20005387949", "MNO345", 2, DateTime.UtcNow });

            migrationBuilder.InsertData("Customers", new[] { "Id", "FullName", "Address" }, new object[] { 3, "Haralds Värdetransporter AB", "Budgetvägen 1, 333 33 Uppsala" });
            migrationBuilder.InsertData("Vehicles", new[] { "Id", "VehicleId", "RegistrationNumber", "CustomerId", "ConnectUpdated" }, new object[] { 6, "VLUR4X20009048066", "PQR678", 3, DateTime.UtcNow });
            migrationBuilder.InsertData("Vehicles", new[] { "Id", "VehicleId", "RegistrationNumber", "CustomerId", "ConnectUpdated" }, new object[] { 7, "YS2R4X20005387055", "STU901", 3, DateTime.UtcNow });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData("Vehicles", "Id", 1);
            migrationBuilder.DeleteData("Vehicles", "Id", 2);
            migrationBuilder.DeleteData("Vehicles", "Id", 3);
            migrationBuilder.DeleteData("Customers", "Id", 1);

            migrationBuilder.DeleteData("Vehicles", "Id", 4);
            migrationBuilder.DeleteData("Vehicles", "Id", 5);
            migrationBuilder.DeleteData("Vehicles", "Id", 2);

            migrationBuilder.DeleteData("Vehicles", "Id", 6);
            migrationBuilder.DeleteData("Vehicles", "Id", 7);
            migrationBuilder.DeleteData("Vehicles", "Id", 3);
        }
    }
}
