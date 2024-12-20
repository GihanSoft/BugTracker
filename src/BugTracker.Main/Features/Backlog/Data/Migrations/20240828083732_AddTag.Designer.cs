﻿// <auto-generated />
using System;
using BugTracker.Main.Features.Backlog.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BugTracker.Main.Features.Backlog.Data.Migrations
{
    [DbContext(typeof(BacklogDbContext))]
    [Migration("20240828083732_AddTag")]
    partial class AddTag
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("BugTracker.Main.Features.Backlog.PbiTag", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityAlwaysColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("CreationMoment")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("creation_moment");

                    b.Property<long>("PbiId")
                        .HasColumnType("bigint")
                        .HasColumnName("pbi_id");

                    b.Property<long>("TagId")
                        .HasColumnType("bigint")
                        .HasColumnName("tag_id");

                    b.HasKey("Id")
                        .HasName("pk_pbi_tag");

                    b.HasIndex("TagId")
                        .HasDatabaseName("ix_pbi_tag_tag_id");

                    b.HasIndex("PbiId", "TagId")
                        .IsUnique()
                        .HasDatabaseName("ix_pbi_tag_pbi_id_tag_id");

                    b.ToTable("pbi_tag", "backlog");
                });

            modelBuilder.Entity("BugTracker.Main.Features.Backlog.ProductBacklogItem", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityAlwaysColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("CreationMoment")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("creation_moment");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<int>("ProjectId")
                        .HasColumnType("integer")
                        .HasColumnName("project_id");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(1024)
                        .HasColumnType("character varying(1024)")
                        .HasColumnName("title");

                    b.HasKey("Id")
                        .HasName("pk_product_backlog_item");

                    b.HasIndex("ProjectId")
                        .HasDatabaseName("ix_product_backlog_item_project_id");

                    b.ToTable("product_backlog_item", "backlog");
                });

            modelBuilder.Entity("BugTracker.Main.Features.Backlog.Project", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityAlwaysColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreationMoment")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("creation_moment");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("key");

                    b.Property<string>("OwnerKey")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("owner_key");

                    b.HasKey("Id")
                        .HasName("pk_project");

                    b.HasIndex("OwnerKey", "Key")
                        .IsUnique()
                        .HasDatabaseName("ix_project_owner_key_key");

                    b.ToTable("project", "backlog");
                });

            modelBuilder.Entity("BugTracker.Main.Features.Backlog.Tag", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityAlwaysColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("CreationMoment")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("creation_moment");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("key");

                    b.Property<int>("ProjectId")
                        .HasColumnType("integer")
                        .HasColumnName("project_id");

                    b.HasKey("Id")
                        .HasName("pk_tag");

                    b.HasIndex("ProjectId")
                        .HasDatabaseName("ix_tag_project_id");

                    b.HasIndex("Key", "ProjectId")
                        .IsUnique()
                        .HasDatabaseName("ix_tag_key_project_id");

                    b.ToTable("tag", "backlog");
                });

            modelBuilder.Entity("BugTracker.Main.Features.Backlog.PbiTag", b =>
                {
                    b.HasOne("BugTracker.Main.Features.Backlog.ProductBacklogItem", "Pbi")
                        .WithMany("Tags")
                        .HasForeignKey("PbiId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_pbi_tag_product_backlog_item_pbi_id");

                    b.HasOne("BugTracker.Main.Features.Backlog.Tag", "Tag")
                        .WithMany("Tags")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_pbi_tag_tag_tag_id");

                    b.Navigation("Pbi");

                    b.Navigation("Tag");
                });

            modelBuilder.Entity("BugTracker.Main.Features.Backlog.ProductBacklogItem", b =>
                {
                    b.HasOne("BugTracker.Main.Features.Backlog.Project", "Project")
                        .WithMany("BacklogItems")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_product_backlog_item_project_project_id");

                    b.Navigation("Project");
                });

            modelBuilder.Entity("BugTracker.Main.Features.Backlog.Tag", b =>
                {
                    b.HasOne("BugTracker.Main.Features.Backlog.Project", "Project")
                        .WithMany("Tags")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_tag_project_project_id");

                    b.Navigation("Project");
                });

            modelBuilder.Entity("BugTracker.Main.Features.Backlog.ProductBacklogItem", b =>
                {
                    b.Navigation("Tags");
                });

            modelBuilder.Entity("BugTracker.Main.Features.Backlog.Project", b =>
                {
                    b.Navigation("BacklogItems");

                    b.Navigation("Tags");
                });

            modelBuilder.Entity("BugTracker.Main.Features.Backlog.Tag", b =>
                {
                    b.Navigation("Tags");
                });
#pragma warning restore 612, 618
        }
    }
}
