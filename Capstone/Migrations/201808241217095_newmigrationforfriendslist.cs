namespace Capstone.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newmigrationforfriendslist : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.FriendsList", newName: "FriendsLists");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.FriendsLists", newName: "FriendsList");
        }
    }
}
