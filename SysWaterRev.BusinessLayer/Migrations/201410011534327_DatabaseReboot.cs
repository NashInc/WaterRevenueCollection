namespace SysWaterRev.BusinessLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DatabaseReboot : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Charges",
                c => new
                    {
                        ChargeId = c.Guid(nullable: false),
                        StartRange = c.Double(nullable: false),
                        EndRange = c.Double(nullable: false),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ChargeScheduleId = c.Guid(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false),
                        LastEditedBy = c.String(),
                        LastEditDate = c.DateTime(),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.ChargeId)
                .ForeignKey("dbo.ChargeSchedules", t => t.ChargeScheduleId, cascadeDelete: true)
                .Index(t => t.ChargeScheduleId);
            
            CreateTable(
                "dbo.ChargeSchedules",
                c => new
                    {
                        ChargeScheduleId = c.Guid(nullable: false),
                        ChargeScheduleName = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        ActivatedBy = c.String(),
                        DateActivated = c.DateTime(),
                        EffectiveDate = c.DateTime(),
                        DateCreated = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false),
                        LastEditedBy = c.String(),
                        LastEditDate = c.DateTime(),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.ChargeScheduleId);
            
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        CustomerId = c.Guid(nullable: false),
                        FirstName = c.String(nullable: false, maxLength: 20),
                        MiddleName = c.String(nullable: false, maxLength: 20),
                        Surname = c.String(nullable: false, maxLength: 20),
                        PhoneNumber = c.String(nullable: false, maxLength: 20),
                        Identification = c.String(nullable: false),
                        EmailAddress = c.String(nullable: false),
                        UserGender = c.Int(nullable: false),
                        CustomerNumber = c.String(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false),
                        LastEditedBy = c.String(),
                        LastEditDate = c.DateTime(),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.CustomerId);
            
            CreateTable(
                "dbo.Meters",
                c => new
                    {
                        MeterId = c.Guid(nullable: false),
                        MeterSerialNumber = c.String(nullable: false),
                        MeterNumber = c.String(nullable: false),
                        CustomerId = c.Guid(),
                        DateCreated = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false),
                        LastEditedBy = c.String(),
                        LastEditDate = c.DateTime(),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.MeterId)
                .ForeignKey("dbo.Customers", t => t.CustomerId)
                .Index(t => t.CustomerId);
            
            CreateTable(
                "dbo.Readings",
                c => new
                    {
                        ReadingId = c.Guid(nullable: false),
                        Latitude = c.Double(nullable: false),
                        Longitude = c.Double(nullable: false),
                        ReadingValue = c.Int(nullable: false),
                        IsConfirmed = c.Boolean(),
                        ConfirmedBy = c.String(),
                        CorrectionValue = c.Int(nullable: false),
                        CorrectedBy = c.String(),
                        Accuracy = c.Double(nullable: false),
                        Altitude = c.Double(nullable: false),
                        Speed = c.Double(nullable: false),
                        LocationDateTime = c.DateTime(nullable: false),
                        Heading = c.Double(nullable: false),
                        AltitudeAccuracy = c.Double(nullable: false),
                        EmployeeId = c.Guid(nullable: false),
                        MeterId = c.Guid(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false),
                        LastEditedBy = c.String(),
                        LastEditDate = c.DateTime(),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        PreviousReading_ReadingId = c.Guid(),
                    })
                .PrimaryKey(t => t.ReadingId)
                .ForeignKey("dbo.Meters", t => t.MeterId, cascadeDelete: true)
                .ForeignKey("dbo.Readings", t => t.PreviousReading_ReadingId)
                .ForeignKey("dbo.Employees", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.MeterId)
                .Index(t => t.PreviousReading_ReadingId);
            
            CreateTable(
                "dbo.Employees",
                c => new
                    {
                        EmployeeId = c.Guid(nullable: false),
                        FirstName = c.String(nullable: false),
                        MiddleName = c.String(nullable: false),
                        Surname = c.String(nullable: false),
                        PhoneNumber = c.String(nullable: false),
                        EmailAddress = c.String(nullable: false),
                        Identification = c.String(nullable: false),
                        EmployeeNumber = c.String(nullable: false),
                        EmployeeGender = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false),
                        LastEditedBy = c.String(),
                        LastEditDate = c.DateTime(),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.EmployeeId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                        Description = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.SystemSettings",
                c => new
                    {
                        SystemSettingId = c.Guid(nullable: false),
                        ChargeScheduleId = c.Guid(nullable: false),
                        SetBy = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false),
                        LastEditedBy = c.String(),
                        LastEditDate = c.DateTime(),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.SystemSettingId)
                .ForeignKey("dbo.ChargeSchedules", t => t.ChargeScheduleId, cascadeDelete: true)
                .Index(t => t.ChargeScheduleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        IsActive = c.Boolean(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                        CustomerDetails_CustomerId = c.Guid(),
                        EmployeeDetails_EmployeeId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Customers", t => t.CustomerDetails_CustomerId)
                .ForeignKey("dbo.Employees", t => t.EmployeeDetails_EmployeeId)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex")
                .Index(t => t.CustomerDetails_CustomerId)
                .Index(t => t.EmployeeDetails_EmployeeId);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "EmployeeDetails_EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.AspNetUsers", "CustomerDetails_CustomerId", "dbo.Customers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.SystemSettings", "ChargeScheduleId", "dbo.ChargeSchedules");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Meters", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.Readings", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.Readings", "PreviousReading_ReadingId", "dbo.Readings");
            DropForeignKey("dbo.Readings", "MeterId", "dbo.Meters");
            DropForeignKey("dbo.Charges", "ChargeScheduleId", "dbo.ChargeSchedules");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", new[] { "EmployeeDetails_EmployeeId" });
            DropIndex("dbo.AspNetUsers", new[] { "CustomerDetails_CustomerId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.SystemSettings", new[] { "ChargeScheduleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Readings", new[] { "PreviousReading_ReadingId" });
            DropIndex("dbo.Readings", new[] { "MeterId" });
            DropIndex("dbo.Readings", new[] { "EmployeeId" });
            DropIndex("dbo.Meters", new[] { "CustomerId" });
            DropIndex("dbo.Charges", new[] { "ChargeScheduleId" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.SystemSettings");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Employees");
            DropTable("dbo.Readings");
            DropTable("dbo.Meters");
            DropTable("dbo.Customers");
            DropTable("dbo.ChargeSchedules");
            DropTable("dbo.Charges");
        }
    }
}
