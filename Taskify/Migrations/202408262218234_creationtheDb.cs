namespace Taskify.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class creationtheDb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Duties",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DutyName = c.String(),
                        Description = c.String(),
                        Status = c.Int(nullable: false),
                        DueDate = c.DateTime(),
                        Active = c.Boolean(nullable: false),
                        Archived = c.Boolean(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        UserSurname = c.String(),
                        Email = c.String(),
                        Password = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Users");
            DropTable("dbo.Duties");
        }
    }
}
