    using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MusicStoreManager.Entities;

namespace MusicStoreManager.Services
{
    public partial class mvcMusicStoreContext : IdentityDbContext<IdentityUser>
    {
        public mvcMusicStoreContext()
        {
            
        }

        public mvcMusicStoreContext(DbContextOptions<mvcMusicStoreContext> options)
            : base(options)
        {
            
        }

        public virtual DbSet<Album> Album { get; set; }
        public virtual DbSet<Artist> Artist { get; set; }
        public virtual DbSet<Cart> Cart { get; set; }
        public virtual DbSet<Genre> Genre { get; set; }
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<OrderDetail> OrderDetail { get; set; }
        public virtual DbSet<Feedback> Feedbacks { get; set; }
        public virtual DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Album>(entity =>
            {
                entity.HasIndex(e => e.AlbumId)
                    .HasName("IPK_ProductItem");

                entity.HasIndex(e => e.ArtistId)
                    .HasName("IFK_Artist_Album");

                entity.Property(e => e.AlbumArtUrl)
                    .HasMaxLength(1024)
                    .HasDefaultValueSql("(N'/AlbumArt/placeholder.jpg')");

                entity.Property(e => e.Price).HasColumnType("numeric(10, 2)");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(160);

                entity.HasOne(d => d.Artist)
                    .WithMany(p => p.Albums)
                    .HasForeignKey(d => d.ArtistId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__Album__ArtistId__276EDEB3");

                entity.HasOne(d => d.Genre)
                    .WithMany(p => p.Album)
                    .HasForeignKey(d => d.GenreId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Album_Genre");
            });

            modelBuilder.Entity<Artist>(entity =>
            {
                entity.HasIndex(e => e.ArtistId)
                    .HasName("IPK_Artist");

                entity.Property(e => e.Name).HasMaxLength(120);
            });

            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasKey(e => e.RecordId);

                entity.Property(e => e.CartId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.HasOne(d => d.Album)
                    .WithMany(p => p.Cart)
                    .HasForeignKey(d => d.AlbumId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Cart_Album");
            });

            modelBuilder.Entity<Genre>(entity =>
            {
                entity.HasIndex(e => e.GenreId)
                    .HasName("IPK_Genre");

                entity.Property(e => e.Description).HasMaxLength(4000);

                entity.Property(e => e.Name).HasMaxLength(120);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasIndex(e => e.OrderId)
                    .HasName("IPK_Invoice");

                entity.Property(e => e.Address).HasMaxLength(70);

                entity.Property(e => e.City).HasMaxLength(40);

                entity.Property(e => e.Country).HasMaxLength(40);

                entity.Property(e => e.Email).HasMaxLength(160);

                entity.Property(e => e.FirstName).HasMaxLength(160);

                entity.Property(e => e.LastName).HasMaxLength(160);

                entity.Property(e => e.OrderDate).HasColumnType("datetime");

                entity.Property(e => e.Phone).HasMaxLength(24);

                entity.Property(e => e.PostalCode).HasMaxLength(10);

                entity.Property(e => e.State).HasMaxLength(40);

                entity.Property(e => e.Total).HasColumnType("numeric(10, 2)");

                entity.Property(e => e.Username).HasMaxLength(256);
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasIndex(e => e.OrderDetailId)
                    .HasName("IPK_InvoiceLine");

                entity.HasIndex(e => e.OrderId)
                    .HasName("IFK_Invoice_InvoiceLine");

                entity.Property(e => e.UnitPrice).HasColumnType("numeric(10, 2)");

                entity.HasOne(d => d.Album)
                    .WithMany(p => p.OrderDetail)
                    .HasForeignKey(d => d.AlbumId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_InvoiceLine_Album");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetail)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__InvoiceLi__Invoi__2F10007B");
            });
        }
    }
}
