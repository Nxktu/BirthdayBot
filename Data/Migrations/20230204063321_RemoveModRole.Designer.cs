﻿// <auto-generated />
using System;
using BirthdayBot.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BirthdayBot.Data.Migrations
{
    [DbContext(typeof(BotDatabaseContext))]
    [Migration("20230204063321_RemoveModRole")]
    partial class RemoveModRole
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("BirthdayBot.Data.GuildConfig", b =>
                {
                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("guild_id");

                    b.Property<string>("AnnounceMessage")
                        .HasColumnType("text")
                        .HasColumnName("announce_message");

                    b.Property<string>("AnnounceMessagePl")
                        .HasColumnType("text")
                        .HasColumnName("announce_message_pl");

                    b.Property<bool>("AnnouncePing")
                        .HasColumnType("boolean")
                        .HasColumnName("announce_ping");

                    b.Property<decimal?>("AnnouncementChannel")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("channel_announce_id");

                    b.Property<decimal?>("BirthdayRole")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("role_id");

                    b.Property<string>("GuildTimeZone")
                        .HasColumnType("text")
                        .HasColumnName("time_zone");

                    b.Property<DateTimeOffset>("LastSeen")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_seen")
                        .HasDefaultValueSql("now()");

                    b.HasKey("GuildId")
                        .HasName("settings_pkey");

                    b.ToTable("settings", (string)null);
                });

            modelBuilder.Entity("BirthdayBot.Data.UserEntry", b =>
                {
                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("guild_id");

                    b.Property<decimal>("UserId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("user_id");

                    b.Property<int>("BirthDay")
                        .HasColumnType("integer")
                        .HasColumnName("birth_day");

                    b.Property<int>("BirthMonth")
                        .HasColumnType("integer")
                        .HasColumnName("birth_month");

                    b.Property<DateTimeOffset>("LastSeen")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_seen")
                        .HasDefaultValueSql("now()");

                    b.Property<string>("TimeZone")
                        .HasColumnType("text")
                        .HasColumnName("time_zone");

                    b.HasKey("GuildId", "UserId")
                        .HasName("user_birthdays_pkey");

                    b.ToTable("user_birthdays", (string)null);
                });

            modelBuilder.Entity("BirthdayBot.Data.UserEntry", b =>
                {
                    b.HasOne("BirthdayBot.Data.GuildConfig", "Guild")
                        .WithMany("UserEntries")
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("user_birthdays_guild_id_fkey");

                    b.Navigation("Guild");
                });

            modelBuilder.Entity("BirthdayBot.Data.GuildConfig", b =>
                {
                    b.Navigation("UserEntries");
                });
#pragma warning restore 612, 618
        }
    }
}
