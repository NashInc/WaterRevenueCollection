namespace SysWaterRev.BusinessLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AnonEdit : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Charges", "PreviousCharge_ChargeId", c => c.Guid());
            CreateIndex("dbo.Charges", "PreviousCharge_ChargeId");
            AddForeignKey("dbo.Charges", "PreviousCharge_ChargeId", "dbo.Charges", "ChargeId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Charges", "PreviousCharge_ChargeId", "dbo.Charges");
            DropIndex("dbo.Charges", new[] { "PreviousCharge_ChargeId" });
            DropColumn("dbo.Charges", "PreviousCharge_ChargeId");
        }
    }
}
