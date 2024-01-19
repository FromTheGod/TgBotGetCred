namespace TgBotFunVersion.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.People",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        SecondName = c.String(),
                        Birthday = c.String(),
                        Number = c.String(),
                        SecondNumber = c.String(),
                        CreditSize = c.Decimal(nullable: false, precision: 18, scale: 2),
                        InterestRate = c.String(),
                        Debt = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.People");
        }
    }
}
