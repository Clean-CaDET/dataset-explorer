using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace DataSetExplorer.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Annotators",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    YearsOfExperience = table.Column<int>(type: "integer", nullable: false),
                    Ranking = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Annotators", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CodeSmellDefinitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    SnippetType = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CodeSmellDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DataSets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataSets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HeuristicDefinitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CodeSmellDefinitionId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeuristicDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeuristicDefinitions_CodeSmellDefinitions_CodeSmellDefiniti~",
                        column: x => x.CodeSmellDefinitionId,
                        principalTable: "CodeSmellDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SeverityDefinitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Value = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CodeSmellDefinitionId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeverityDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SeverityDefinitions_CodeSmellDefinitions_CodeSmellDefinitio~",
                        column: x => x.CodeSmellDefinitionId,
                        principalTable: "CodeSmellDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CodeSmells",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    SnippetType = table.Column<string>(type: "text", nullable: false),
                    DataSetId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CodeSmells", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CodeSmells_DataSets_DataSetId",
                        column: x => x.DataSetId,
                        principalTable: "DataSets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DataSetProjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Url = table.Column<string>(type: "text", nullable: true),
                    State = table.Column<int>(type: "integer", nullable: false),
                    DataSetId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataSetProjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataSetProjects_DataSets_DataSetId",
                        column: x => x.DataSetId,
                        principalTable: "DataSets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GraphInstances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CodeSnippetId = table.Column<string>(type: "text", nullable: true),
                    Link = table.Column<string>(type: "text", nullable: true),
                    DataSetProjectId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GraphInstances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GraphInstances_DataSetProjects_DataSetProjectId",
                        column: x => x.DataSetProjectId,
                        principalTable: "DataSetProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SmellCandidateInstances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CodeSmellId = table.Column<int>(type: "integer", nullable: true),
                    DataSetProjectId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmellCandidateInstances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SmellCandidateInstances_CodeSmells_CodeSmellId",
                        column: x => x.CodeSmellId,
                        principalTable: "CodeSmells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SmellCandidateInstances_DataSetProjects_DataSetProjectId",
                        column: x => x.DataSetProjectId,
                        principalTable: "DataSetProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GraphRelatedInstances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CodeSnippetId = table.Column<string>(type: "text", nullable: true),
                    RelationType = table.Column<string>(type: "text", nullable: false),
                    CouplingTypeAndStrength = table.Column<string>(type: "text", nullable: true),
                    Link = table.Column<string>(type: "text", nullable: true),
                    GraphInstanceId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GraphRelatedInstances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GraphRelatedInstances_GraphInstances_GraphInstanceId",
                        column: x => x.GraphInstanceId,
                        principalTable: "GraphInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Instances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CodeSnippetId = table.Column<string>(type: "text", nullable: true),
                    Link = table.Column<string>(type: "text", nullable: true),
                    ProjectLink = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    MetricFeatures = table.Column<string>(type: "text", nullable: true),
                    SmellCandidateInstancesId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Instances_SmellCandidateInstances_SmellCandidateInstancesId",
                        column: x => x.SmellCandidateInstancesId,
                        principalTable: "SmellCandidateInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Annotations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InstanceSmellId = table.Column<int>(type: "integer", nullable: true),
                    Severity = table.Column<string>(type: "text", nullable: true),
                    AnnotatorId = table.Column<int>(type: "integer", nullable: true),
                    Note = table.Column<string>(type: "text", nullable: true),
                    InstanceId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Annotations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Annotations_Annotators_AnnotatorId",
                        column: x => x.AnnotatorId,
                        principalTable: "Annotators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Annotations_CodeSmells_InstanceSmellId",
                        column: x => x.InstanceSmellId,
                        principalTable: "CodeSmells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Annotations_Instances_InstanceId",
                        column: x => x.InstanceId,
                        principalTable: "Instances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Identifiers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: false),
                    InstanceId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Identifiers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Identifiers_Instances_InstanceId",
                        column: x => x.InstanceId,
                        principalTable: "Instances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RelatedInstance",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CodeSnippetId = table.Column<string>(type: "text", nullable: true),
                    Link = table.Column<string>(type: "text", nullable: true),
                    RelationType = table.Column<string>(type: "text", nullable: false),
                    CouplingTypeAndStrength = table.Column<string>(type: "text", nullable: true),
                    InstanceId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelatedInstance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RelatedInstance_Instances_InstanceId",
                        column: x => x.InstanceId,
                        principalTable: "Instances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SmellHeuristics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsApplicable = table.Column<bool>(type: "boolean", nullable: false),
                    ReasonForApplicability = table.Column<string>(type: "text", nullable: true),
                    AnnotationId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmellHeuristics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SmellHeuristics_Annotations_AnnotationId",
                        column: x => x.AnnotationId,
                        principalTable: "Annotations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Annotations_AnnotatorId",
                table: "Annotations",
                column: "AnnotatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Annotations_InstanceId",
                table: "Annotations",
                column: "InstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_Annotations_InstanceSmellId",
                table: "Annotations",
                column: "InstanceSmellId");

            migrationBuilder.CreateIndex(
                name: "IX_Annotators_Email",
                table: "Annotators",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CodeSmellDefinitions_Name",
                table: "CodeSmellDefinitions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CodeSmells_DataSetId",
                table: "CodeSmells",
                column: "DataSetId");

            migrationBuilder.CreateIndex(
                name: "IX_DataSetProjects_DataSetId",
                table: "DataSetProjects",
                column: "DataSetId");

            migrationBuilder.CreateIndex(
                name: "IX_GraphInstances_DataSetProjectId",
                table: "GraphInstances",
                column: "DataSetProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_GraphRelatedInstances_GraphInstanceId",
                table: "GraphRelatedInstances",
                column: "GraphInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_HeuristicDefinitions_CodeSmellDefinitionId",
                table: "HeuristicDefinitions",
                column: "CodeSmellDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_Identifiers_InstanceId",
                table: "Identifiers",
                column: "InstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_Instances_SmellCandidateInstancesId",
                table: "Instances",
                column: "SmellCandidateInstancesId");

            migrationBuilder.CreateIndex(
                name: "IX_RelatedInstance_InstanceId",
                table: "RelatedInstance",
                column: "InstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_SeverityDefinitions_CodeSmellDefinitionId",
                table: "SeverityDefinitions",
                column: "CodeSmellDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_SmellCandidateInstances_CodeSmellId",
                table: "SmellCandidateInstances",
                column: "CodeSmellId");

            migrationBuilder.CreateIndex(
                name: "IX_SmellCandidateInstances_DataSetProjectId",
                table: "SmellCandidateInstances",
                column: "DataSetProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_SmellHeuristics_AnnotationId",
                table: "SmellHeuristics",
                column: "AnnotationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GraphRelatedInstances");

            migrationBuilder.DropTable(
                name: "HeuristicDefinitions");

            migrationBuilder.DropTable(
                name: "Identifiers");

            migrationBuilder.DropTable(
                name: "RelatedInstance");

            migrationBuilder.DropTable(
                name: "SeverityDefinitions");

            migrationBuilder.DropTable(
                name: "SmellHeuristics");

            migrationBuilder.DropTable(
                name: "GraphInstances");

            migrationBuilder.DropTable(
                name: "CodeSmellDefinitions");

            migrationBuilder.DropTable(
                name: "Annotations");

            migrationBuilder.DropTable(
                name: "Annotators");

            migrationBuilder.DropTable(
                name: "Instances");

            migrationBuilder.DropTable(
                name: "SmellCandidateInstances");

            migrationBuilder.DropTable(
                name: "CodeSmells");

            migrationBuilder.DropTable(
                name: "DataSetProjects");

            migrationBuilder.DropTable(
                name: "DataSets");
        }
    }
}
