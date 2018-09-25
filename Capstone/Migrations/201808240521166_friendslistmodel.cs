namespace Capstone.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class friendslistmodel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FriendsLists",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MyId = c.String(),
                        FriendId = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.FriendsLists");
        }
    }
}
