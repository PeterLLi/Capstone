namespace Capstone.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newmigrationfordatabases : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.FriendsLists", newName: "FriendsList");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.FriendsList", newName: "FriendsLists");
        }
    }
}
