﻿// <auto-generated />
using System;
using DataSetExplorer.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace DataSetExplorer.Migrations
{
    [DbContext(typeof(DataSetExplorerContext))]
    [Migration("20220410002913_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.6")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("DataSetExplorer.Core.Annotations.Model.Annotation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int?>("AnnotatorId")
                        .HasColumnType("integer");

                    b.Property<int?>("InstanceId")
                        .HasColumnType("integer");

                    b.Property<int?>("InstanceSmellId")
                        .HasColumnType("integer");

                    b.Property<string>("Note")
                        .HasColumnType("text");

                    b.Property<int>("Severity")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("AnnotatorId");

                    b.HasIndex("InstanceId");

                    b.HasIndex("InstanceSmellId");

                    b.ToTable("DataSetAnnotations");
                });

            modelBuilder.Entity("DataSetExplorer.Core.Annotations.Model.Annotator", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("Ranking")
                        .HasColumnType("integer");

                    b.Property<int>("YearsOfExperience")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Annotators");
                });

            modelBuilder.Entity("DataSetExplorer.Core.Annotations.Model.CodeSmell", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int?>("DataSetId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("DataSetId");

                    b.ToTable("CodeSmells");
                });

            modelBuilder.Entity("DataSetExplorer.Core.Annotations.Model.SmellHeuristic", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int?>("AnnotationId")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<bool>("IsApplicable")
                        .HasColumnType("boolean");

                    b.Property<string>("ReasonForApplicability")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("AnnotationId");

                    b.ToTable("SmellHeuristics");
                });

            modelBuilder.Entity("DataSetExplorer.Core.DataSets.Model.DataSet", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("DataSets");
                });

            modelBuilder.Entity("DataSetExplorer.Core.DataSets.Model.DataSetProject", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int?>("DataSetId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int>("State")
                        .HasColumnType("integer");

                    b.Property<string>("Url")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("DataSetId");

                    b.ToTable("DataSetProjects");
                });

            modelBuilder.Entity("DataSetExplorer.Core.DataSets.Model.Instance", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("CodeSnippetId")
                        .HasColumnType("text");

                    b.Property<string>("Link")
                        .HasColumnType("text");

                    b.Property<string>("MetricFeatures")
                        .HasColumnType("text");

                    b.Property<string>("ProjectLink")
                        .HasColumnType("text");

                    b.Property<int?>("SmellCandidateInstancesId")
                        .HasColumnType("integer");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("SmellCandidateInstancesId");

                    b.ToTable("DataSetInstances");
                });

            modelBuilder.Entity("DataSetExplorer.Core.DataSets.Model.RelatedInstance", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("CodeSnippetId")
                        .HasColumnType("text");

                    b.Property<string>("CouplingTypeAndStrength")
                        .HasColumnType("text");

                    b.Property<int?>("InstanceId")
                        .HasColumnType("integer");

                    b.Property<string>("Link")
                        .HasColumnType("text");

                    b.Property<string>("RelationType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("InstanceId");

                    b.ToTable("RelatedInstance");
                });

            modelBuilder.Entity("DataSetExplorer.Core.DataSets.Model.SmellCandidateInstances", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int?>("CodeSmellId")
                        .HasColumnType("integer");

                    b.Property<int?>("DataSetProjectId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("CodeSmellId");

                    b.HasIndex("DataSetProjectId");

                    b.ToTable("SmellCandidateInstances");
                });

            modelBuilder.Entity("DataSetExplorer.Core.Annotations.Model.Annotation", b =>
                {
                    b.HasOne("DataSetExplorer.Core.Annotations.Model.Annotator", "Annotator")
                        .WithMany()
                        .HasForeignKey("AnnotatorId");

                    b.HasOne("DataSetExplorer.Core.DataSets.Model.Instance", null)
                        .WithMany("Annotations")
                        .HasForeignKey("InstanceId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DataSetExplorer.Core.Annotations.Model.CodeSmell", "InstanceSmell")
                        .WithMany()
                        .HasForeignKey("InstanceSmellId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Annotator");

                    b.Navigation("InstanceSmell");
                });

            modelBuilder.Entity("DataSetExplorer.Core.Annotations.Model.CodeSmell", b =>
                {
                    b.HasOne("DataSetExplorer.Core.DataSets.Model.DataSet", null)
                        .WithMany("SupportedCodeSmells")
                        .HasForeignKey("DataSetId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DataSetExplorer.Core.Annotations.Model.SmellHeuristic", b =>
                {
                    b.HasOne("DataSetExplorer.Core.Annotations.Model.Annotation", null)
                        .WithMany("ApplicableHeuristics")
                        .HasForeignKey("AnnotationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DataSetExplorer.Core.DataSets.Model.DataSetProject", b =>
                {
                    b.HasOne("DataSetExplorer.Core.DataSets.Model.DataSet", null)
                        .WithMany("Projects")
                        .HasForeignKey("DataSetId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DataSetExplorer.Core.DataSets.Model.Instance", b =>
                {
                    b.HasOne("DataSetExplorer.Core.DataSets.Model.SmellCandidateInstances", null)
                        .WithMany("Instances")
                        .HasForeignKey("SmellCandidateInstancesId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DataSetExplorer.Core.DataSets.Model.RelatedInstance", b =>
                {
                    b.HasOne("DataSetExplorer.Core.DataSets.Model.Instance", null)
                        .WithMany("RelatedInstances")
                        .HasForeignKey("InstanceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DataSetExplorer.Core.DataSets.Model.SmellCandidateInstances", b =>
                {
                    b.HasOne("DataSetExplorer.Core.Annotations.Model.CodeSmell", "CodeSmell")
                        .WithMany()
                        .HasForeignKey("CodeSmellId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DataSetExplorer.Core.DataSets.Model.DataSetProject", null)
                        .WithMany("CandidateInstances")
                        .HasForeignKey("DataSetProjectId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("CodeSmell");
                });

            modelBuilder.Entity("DataSetExplorer.Core.Annotations.Model.Annotation", b =>
                {
                    b.Navigation("ApplicableHeuristics");
                });

            modelBuilder.Entity("DataSetExplorer.Core.DataSets.Model.DataSet", b =>
                {
                    b.Navigation("Projects");

                    b.Navigation("SupportedCodeSmells");
                });

            modelBuilder.Entity("DataSetExplorer.Core.DataSets.Model.DataSetProject", b =>
                {
                    b.Navigation("CandidateInstances");
                });

            modelBuilder.Entity("DataSetExplorer.Core.DataSets.Model.Instance", b =>
                {
                    b.Navigation("Annotations");

                    b.Navigation("RelatedInstances");
                });

            modelBuilder.Entity("DataSetExplorer.Core.DataSets.Model.SmellCandidateInstances", b =>
                {
                    b.Navigation("Instances");
                });
#pragma warning restore 612, 618
        }
    }
}
