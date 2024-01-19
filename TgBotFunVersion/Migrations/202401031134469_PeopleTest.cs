namespace TgBotFunVersion.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class PeopleTest : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.People",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IdTelegram = c.String(),
                        Name = c.String(),
                        SecondName = c.String(),
                        Birthday = c.String(),
                        Number = c.String(),
                        SecondNumber = c.String(),
                        CreditSize = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TimeCredMounth = c.Int(nullable: false),
                        DatePayment = c.DateTime(nullable: false),
                        InterestRate = c.String(),
                        Debt = c.Int(nullable: false),
                        SizePayment = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.People");
        }
    }
}
