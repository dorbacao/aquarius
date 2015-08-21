using System.Data.Entity.Migrations;

namespace Aquarius.Data.Tests.Migrations
{
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Cidade",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NomeCidade = c.String(),
                        Estado_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Estado", t => t.Estado_Id)
                .Index(t => t.Estado_Id);
            
            CreateTable(
                "dbo.Estado",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NomeEstado = c.String(),
                        UF = c.String(),
                        Pais_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Pais", t => t.Pais_Id)
                .Index(t => t.Pais_Id);
            
            CreateTable(
                "dbo.Pais",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NomePais = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Estado", new[] { "Pais_Id" });
            DropIndex("dbo.Cidade", new[] { "Estado_Id" });
            DropForeignKey("dbo.Estado", "Pais_Id", "dbo.Pais");
            DropForeignKey("dbo.Cidade", "Estado_Id", "dbo.Estado");
            DropTable("dbo.Pais");
            DropTable("dbo.Estado");
            DropTable("dbo.Cidade");
        }
    }
}
