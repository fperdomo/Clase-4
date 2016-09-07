namespace Tresana.Data.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Priorities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Weight = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Projects",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        StartDate = c.DateTime(nullable: false),
                        Duedate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        LastName = c.String(),
                        UserName = c.String(),
                        Mail = c.String(),
                        Project_Id = c.Int(),
                        Task_Id = c.Int(),
                        Team_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Projects", t => t.Project_Id)
                .ForeignKey("dbo.Tasks", t => t.Task_Id)
                .ForeignKey("dbo.Teams", t => t.Team_Id)
                .Index(t => t.Project_Id)
                .Index(t => t.Task_Id)
                .Index(t => t.Team_Id);
            
            CreateTable(
                "dbo.Status",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Ordinal = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Tasks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        StartDate = c.DateTime(nullable: false),
                        FinishDate = c.DateTime(nullable: false),
                        Estimation = c.Int(),
                        CreationDate = c.DateTime(nullable: false),
                        DueDate = c.DateTime(nullable: false),
                        Creator_Id = c.Int(),
                        Priority_Id = c.Int(),
                        Status_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.Creator_Id)
                .ForeignKey("dbo.Priorities", t => t.Priority_Id)
                .ForeignKey("dbo.Status", t => t.Status_Id)
                .Index(t => t.Creator_Id)
                .Index(t => t.Priority_Id)
                .Index(t => t.Status_Id);
            
            CreateTable(
                "dbo.Teams",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Url = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "Team_Id", "dbo.Teams");
            DropForeignKey("dbo.Tasks", "Status_Id", "dbo.Status");
            DropForeignKey("dbo.Users", "Task_Id", "dbo.Tasks");
            DropForeignKey("dbo.Tasks", "Priority_Id", "dbo.Priorities");
            DropForeignKey("dbo.Tasks", "Creator_Id", "dbo.Users");
            DropForeignKey("dbo.Users", "Project_Id", "dbo.Projects");
            DropIndex("dbo.Tasks", new[] { "Status_Id" });
            DropIndex("dbo.Tasks", new[] { "Priority_Id" });
            DropIndex("dbo.Tasks", new[] { "Creator_Id" });
            DropIndex("dbo.Users", new[] { "Team_Id" });
            DropIndex("dbo.Users", new[] { "Task_Id" });
            DropIndex("dbo.Users", new[] { "Project_Id" });
            DropTable("dbo.Teams");
            DropTable("dbo.Tasks");
            DropTable("dbo.Status");
            DropTable("dbo.Users");
            DropTable("dbo.Projects");
            DropTable("dbo.Priorities");
        }
    }
}
