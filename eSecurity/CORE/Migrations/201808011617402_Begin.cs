namespace SECURITY.CORE.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Begin : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        RoleId = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        Name = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        InternalId = c.String(maxLength: 36, storeType: "nvarchar"),
                        CreationDate = c.DateTime(precision: 0),
                        ModificationDate = c.DateTime(precision: 0),
                        IsInternal = c.Boolean(),
                        IsActive = c.Boolean(),
                        Discriminator = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.RoleId)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.UsersRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        RoleId = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.Roles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        InternalId = c.String(nullable: false, maxLength: 36, storeType: "nvarchar"),
                        AudienceId = c.String(nullable: false, maxLength: 36, storeType: "nvarchar"),
                        FirstName = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
                        LastName = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
                        PasswordResetToken = c.String(unicode: false),
                        RefreshToken = c.String(unicode: false),
                        RefreshTokenIssuedUtc = c.DateTime(nullable: false, precision: 0),
                        RefreshTokenExpiresUtc = c.DateTime(nullable: false, precision: 0),
                        CreationDate = c.DateTime(nullable: false, precision: 0),
                        ModificationDate = c.DateTime(precision: 0),
                        IsInternal = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        JoinDate = c.DateTime(nullable: false, precision: 0),
                        Email = c.String(maxLength: 256, storeType: "nvarchar"),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(unicode: false),
                        SecurityStamp = c.String(unicode: false),
                        PhoneNumber = c.String(unicode: false),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(precision: 0),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.Audiences", t => t.AudienceId, cascadeDelete: true)
                .Index(t => new { t.UserName, t.AudienceId }, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.Audiences",
                c => new
                    {
                        AudienceId = c.String(nullable: false, maxLength: 36, storeType: "nvarchar"),
                        InternalId = c.String(nullable: false, maxLength: 36, storeType: "nvarchar"),
                        Name = c.String(maxLength: 128, storeType: "nvarchar"),
                        Secret = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        DaysToExpire = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false, precision: 0),
                        ModificationDate = c.DateTime(precision: 0),
                        ExpirationDate = c.DateTime(precision: 0),
                        IsInternal = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.AudienceId);
            
            CreateTable(
                "dbo.UsersClaims",
                c => new
                    {
                        UserClaimId = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        ClaimType = c.String(unicode: false),
                        ClaimValue = c.String(unicode: false),
                        Type = c.String(unicode: false),
                        Value = c.String(unicode: false),
                        Discriminator = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.UserClaimId)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.UsersLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        ProviderKey = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        UserId = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UsersRoles", "UserId", "dbo.Users");
            DropForeignKey("dbo.UsersLogins", "UserId", "dbo.Users");
            DropForeignKey("dbo.UsersClaims", "UserId", "dbo.Users");
            DropForeignKey("dbo.Users", "AudienceId", "dbo.Audiences");
            DropForeignKey("dbo.UsersRoles", "RoleId", "dbo.Roles");
            DropIndex("dbo.UsersLogins", new[] { "UserId" });
            DropIndex("dbo.UsersClaims", new[] { "UserId" });
            DropIndex("dbo.Users", "UserNameIndex");
            DropIndex("dbo.UsersRoles", new[] { "RoleId" });
            DropIndex("dbo.UsersRoles", new[] { "UserId" });
            DropIndex("dbo.Roles", "RoleNameIndex");
            DropTable("dbo.UsersLogins");
            DropTable("dbo.UsersClaims");
            DropTable("dbo.Audiences");
            DropTable("dbo.Users");
            DropTable("dbo.UsersRoles");
            DropTable("dbo.Roles");
        }
    }
}
