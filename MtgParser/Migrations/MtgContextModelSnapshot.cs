﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MtgParser.Context;

#nullable disable

namespace MtgParser.Migrations
{
    [DbContext(typeof(MtgContext))]
    partial class MtgContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("CardKeyword", b =>
                {
                    b.Property<int>("CardsId")
                        .HasColumnType("int");

                    b.Property<int>("KeywordsId")
                        .HasColumnType("int");

                    b.HasKey("CardsId", "KeywordsId");

                    b.HasIndex("KeywordsId");

                    b.ToTable("CardKeyword");
                });

            modelBuilder.Entity("MtgParser.Model.Card", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Cmc")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Color")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Img")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("IsRus")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("NameRus")
                        .HasColumnType("longtext");

                    b.Property<string>("Power")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("TextRus")
                        .HasColumnType("longtext");

                    b.Property<string>("Toughness")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("TypeRus")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Cards");
                });

            modelBuilder.Entity("MtgParser.Model.CardName", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<bool>("IsFoil")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IsParsed")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.Property<string>("NameRus")
                        .HasColumnType("longtext");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<string>("SetShort")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("CardsNames");
                });

            modelBuilder.Entity("MtgParser.Model.CardSet", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("CardId")
                        .HasColumnType("int");

                    b.Property<byte>("IsFoil")
                        .HasColumnType("tinyint unsigned");

                    b.Property<decimal?>("ManualPrice")
                        .HasColumnType("decimal(65,30)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<int>("RarityId")
                        .HasColumnType("int");

                    b.Property<int>("SetId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CardId");

                    b.HasIndex("RarityId");

                    b.HasIndex("SetId");

                    b.ToTable("CardsSets");
                });

            modelBuilder.Entity("MtgParser.Model.Keyword", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("RusName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Keywords");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Deathtouch",
                            RusName = "Смертельное касание"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Defender",
                            RusName = "Защитник"
                        },
                        new
                        {
                            Id = 3,
                            Name = "Double strike",
                            RusName = "Двойной удар"
                        },
                        new
                        {
                            Id = 4,
                            Name = "Enchant",
                            RusName = "Зачаровать"
                        },
                        new
                        {
                            Id = 5,
                            Name = "Equip",
                            RusName = "Снарядить"
                        },
                        new
                        {
                            Id = 6,
                            Name = "First strike",
                            RusName = "Первый удар"
                        },
                        new
                        {
                            Id = 7,
                            Name = "Flash",
                            RusName = "Миг"
                        },
                        new
                        {
                            Id = 8,
                            Name = "Flying",
                            RusName = "Полет"
                        },
                        new
                        {
                            Id = 9,
                            Name = "Haste",
                            RusName = "Ускорение"
                        },
                        new
                        {
                            Id = 10,
                            Name = "Hexproof",
                            RusName = "Порчеустойчивость"
                        },
                        new
                        {
                            Id = 11,
                            Name = "Indestructible",
                            RusName = "Неразрушимость"
                        },
                        new
                        {
                            Id = 12,
                            Name = "Lifelink",
                            RusName = "Цепь жизни"
                        },
                        new
                        {
                            Id = 13,
                            Name = "Menace",
                            RusName = "Угроза"
                        },
                        new
                        {
                            Id = 14,
                            Name = "Protection",
                            RusName = "Защита"
                        },
                        new
                        {
                            Id = 15,
                            Name = "Prowess",
                            RusName = "Искусность"
                        },
                        new
                        {
                            Id = 16,
                            Name = "Reach",
                            RusName = "Захват"
                        },
                        new
                        {
                            Id = 17,
                            Name = "Trample",
                            RusName = "Пробивной удар"
                        },
                        new
                        {
                            Id = 18,
                            Name = "Vigilance",
                            RusName = "Бдительность"
                        });
                });

            modelBuilder.Entity("MtgParser.Model.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Comment")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("ModifyDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Products")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("TrackNumber")
                        .HasColumnType("longtext");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("MtgParser.Model.Permission", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("PermissionStr")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Permissions");
                });

            modelBuilder.Entity("MtgParser.Model.Price", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("CardSetId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<decimal>("Value")
                        .HasColumnType("decimal(65,30)");

                    b.HasKey("Id");

                    b.HasIndex("CardSetId");

                    b.ToTable("Prices");
                });

            modelBuilder.Entity("MtgParser.Model.Rarity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("RusName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Rarities");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Common",
                            RusName = "Обычная"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Uncommon",
                            RusName = "Необычная"
                        },
                        new
                        {
                            Id = 3,
                            Name = "Rare",
                            RusName = "Редкая"
                        },
                        new
                        {
                            Id = 4,
                            Name = "Mythic",
                            RusName = "Раритетная"
                        },
                        new
                        {
                            Id = 5,
                            Name = "Special",
                            RusName = "Специальная"
                        });
                });

            modelBuilder.Entity("MtgParser.Model.Set", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("RusName")
                        .HasColumnType("longtext");

                    b.Property<string>("SearchText")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("SetImg")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("ShortName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Sets");
                });

            modelBuilder.Entity("MtgParser.Model.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Address")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("BanDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("CheckWord")
                        .HasColumnType("longtext");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("RegistrationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Surname")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("CardKeyword", b =>
                {
                    b.HasOne("MtgParser.Model.Card", null)
                        .WithMany()
                        .HasForeignKey("CardsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MtgParser.Model.Keyword", null)
                        .WithMany()
                        .HasForeignKey("KeywordsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MtgParser.Model.CardSet", b =>
                {
                    b.HasOne("MtgParser.Model.Card", "Card")
                        .WithMany()
                        .HasForeignKey("CardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MtgParser.Model.Rarity", "Rarity")
                        .WithMany()
                        .HasForeignKey("RarityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MtgParser.Model.Set", "Set")
                        .WithMany()
                        .HasForeignKey("SetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Card");

                    b.Navigation("Rarity");

                    b.Navigation("Set");
                });

            modelBuilder.Entity("MtgParser.Model.Order", b =>
                {
                    b.HasOne("MtgParser.Model.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("MtgParser.Model.Permission", b =>
                {
                    b.HasOne("MtgParser.Model.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("MtgParser.Model.Price", b =>
                {
                    b.HasOne("MtgParser.Model.CardSet", "CardSet")
                        .WithMany("Prices")
                        .HasForeignKey("CardSetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CardSet");
                });

            modelBuilder.Entity("MtgParser.Model.CardSet", b =>
                {
                    b.Navigation("Prices");
                });
#pragma warning restore 612, 618
        }
    }
}
