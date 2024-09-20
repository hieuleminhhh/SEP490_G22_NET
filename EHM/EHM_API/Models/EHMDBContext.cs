using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EHM_API.Models
{
    public partial class EHMDBContext : DbContext
    {
        public EHMDBContext()
        {
        }

        public EHMDBContext(DbContextOptions<EHMDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; } = null!;
        public virtual DbSet<Address> Addresses { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Combo> Combos { get; set; } = null!;
        public virtual DbSet<ComboDetail> ComboDetails { get; set; } = null!;
        public virtual DbSet<Discount> Discounts { get; set; } = null!;
        public virtual DbSet<Dish> Dishes { get; set; } = null!;
        public virtual DbSet<Guest> Guests { get; set; } = null!;
        public virtual DbSet<Ingredient> Ingredients { get; set; } = null!;
        public virtual DbSet<Invoice> Invoices { get; set; } = null!;
        public virtual DbSet<InvoiceLog> InvoiceLogs { get; set; } = null!;
        public virtual DbSet<Material> Materials { get; set; } = null!;
        public virtual DbSet<News> News { get; set; } = null!;
        public virtual DbSet<Notification> Notifications { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public virtual DbSet<OrderTable> OrderTables { get; set; } = null!;
        public virtual DbSet<Reservation> Reservations { get; set; } = null!;
        public virtual DbSet<Setting> Settings { get; set; } = null!;
        public virtual DbSet<Table> Tables { get; set; } = null!;
        public virtual DbSet<TableReservation> TableReservations { get; set; } = null!;
        public virtual DbSet<Wallet> Wallets { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("server =localhost; database = EHMDB;uid=sa;pwd=sa;TrustServerCertificate=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("Account");

                entity.Property(e => e.AccountId).HasColumnName("AccountID");

                entity.Property(e => e.Address).HasMaxLength(50);

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.IsActive).HasDefaultValueSql("((0))");

                entity.Property(e => e.LastName).HasMaxLength(50);

                entity.Property(e => e.Password)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Phone)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.Role)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Username)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Address>(entity =>
            {
                entity.ToTable("Address");

                entity.Property(e => e.AddressId).HasColumnName("AddressID");

                entity.Property(e => e.ConsigneeName).HasMaxLength(50);

                entity.Property(e => e.GuestAddress).HasMaxLength(500);

                entity.Property(e => e.GuestPhone)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.HasOne(d => d.GuestPhoneNavigation)
                    .WithMany(p => p.Addresses)
                    .HasForeignKey(d => d.GuestPhone)
                    .HasConstraintName("FK_Address_Guest");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.CategoryName).HasMaxLength(100);
            });

            modelBuilder.Entity<Combo>(entity =>
            {
                entity.ToTable("Combo");

                entity.Property(e => e.ComboId).HasColumnName("ComboID");

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("ImageURL");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((0))");

                entity.Property(e => e.NameCombo).HasMaxLength(50);

                entity.Property(e => e.Note).HasMaxLength(200);

                entity.Property(e => e.Price).HasColumnType("money");
            });

            modelBuilder.Entity<ComboDetail>()
                               .HasKey(cd => new { cd.ComboId, cd.DishId });

            modelBuilder.Entity<ComboDetail>()
                .HasOne(cd => cd.Combo)
                .WithMany(c => c.ComboDetails)
                .HasForeignKey(cd => cd.ComboId);

            modelBuilder.Entity<ComboDetail>()
                .HasOne(cd => cd.Dish)
                .WithMany(d => d.ComboDetails)
                .HasForeignKey(cd => cd.DishId);

            modelBuilder.Entity<Discount>(entity =>
            {
                entity.ToTable("Discount");

                entity.Property(e => e.DiscountId).HasColumnName("DiscountID");

                entity.Property(e => e.DiscountName).HasMaxLength(200);

                entity.Property(e => e.EndTime).HasColumnType("datetime");

                entity.Property(e => e.Note).HasMaxLength(200);

                entity.Property(e => e.StartTime).HasColumnType("datetime");

                entity.Property(e => e.TotalMoney).HasColumnType("money");
            });

            modelBuilder.Entity<Dish>(entity =>
            {
                entity.ToTable("Dish");

                entity.Property(e => e.DishId).HasColumnName("DishID");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.DiscountId).HasColumnName("DiscountID");

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(100)
                    .HasColumnName("ImageURL");

                entity.Property(e => e.ItemDescription).HasMaxLength(100);

                entity.Property(e => e.ItemName).HasMaxLength(100);

                entity.Property(e => e.Price).HasColumnType("money");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Dishes)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_Dish_Categories");

                entity.HasOne(d => d.Discount)
                    .WithMany(p => p.Dishes)
                    .HasForeignKey(d => d.DiscountId)
                    .HasConstraintName("FK_Dish_Discount");
            });

            modelBuilder.Entity<Guest>(entity =>
            {
                entity.HasKey(e => e.GuestPhone)
                    .HasName("PK_Customer");

                entity.ToTable("Guest");

                entity.Property(e => e.GuestPhone)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Ingredient>(entity =>
            {
                entity.HasKey(e => new { e.DishId, e.MaterialId });

                entity.ToTable("Ingredient");

                entity.Property(e => e.DishId).HasColumnName("DishID");

                entity.Property(e => e.MaterialId).HasColumnName("MaterialID");

                entity.Property(e => e.Quantitative).HasColumnType("decimal(10, 5)");
            });

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.ToTable("Invoice");

                entity.Property(e => e.InvoiceId).HasColumnName("InvoiceID");

                entity.Property(e => e.AccountId).HasColumnName("AccountID");

                entity.Property(e => e.Address).HasMaxLength(100);

                entity.Property(e => e.AmountReceived).HasColumnType("money");

                entity.Property(e => e.CustomerName).HasMaxLength(50);

                entity.Property(e => e.PaymentAmount).HasColumnType("money");

                entity.Property(e => e.PaymentTime).HasColumnType("datetime");

                entity.Property(e => e.Phone)
                    .HasMaxLength(11)
                    .IsUnicode(false);

                entity.Property(e => e.ReturnAmount).HasColumnType("money");

                entity.Property(e => e.Taxcode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Invoices)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK_Invoice_Account");
            });

            modelBuilder.Entity<InvoiceLog>(entity =>
            {
                entity.HasKey(e => e.LogId);

                entity.ToTable("InvoiceLog");

                entity.Property(e => e.LogId).HasColumnName("LogID");

                entity.Property(e => e.Description).HasMaxLength(300);

                entity.Property(e => e.InvoiceId).HasColumnName("InvoiceID");

                entity.HasOne(d => d.Invoice)
                    .WithMany(p => p.InvoiceLogs)
                    .HasForeignKey(d => d.InvoiceId)
                    .HasConstraintName("FK_InvoiceLog_Invoice");
            });

            modelBuilder.Entity<Material>(entity =>
            {
                entity.ToTable("Material");

                entity.Property(e => e.MaterialId).HasColumnName("MaterialID");

                entity.Property(e => e.Category).HasMaxLength(200);

                entity.Property(e => e.Name).HasMaxLength(200);

                entity.Property(e => e.Unit).HasMaxLength(200);
            });

            modelBuilder.Entity<News>(entity =>
            {
                entity.Property(e => e.NewsId).HasColumnName("NewsID");

                entity.Property(e => e.AccountId).HasColumnName("AccountID");

                entity.Property(e => e.NewsDate).HasColumnType("datetime");

                entity.Property(e => e.NewsImage).HasMaxLength(100);

                entity.Property(e => e.NewsTitle).HasMaxLength(50);

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.News)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK_News_Staff");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("Notification");

                entity.Property(e => e.NotificationId).HasColumnName("NotificationID");

                entity.Property(e => e.AccountId).HasColumnName("AccountID");

                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.Property(e => e.Time).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK_Notification_Account1");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK_Notification_Order1");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");

                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.Property(e => e.AccountId).HasColumnName("AccountID");

                entity.Property(e => e.AddressId).HasColumnName("AddressID");

                entity.Property(e => e.CancelBy).HasMaxLength(100);

                entity.Property(e => e.CancelDate).HasColumnType("datetime");

                entity.Property(e => e.CancelationReason).HasMaxLength(200);

                entity.Property(e => e.Deposits)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.DiscountId).HasColumnName("DiscountID");

                entity.Property(e => e.GuestPhone)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.InvoiceId).HasColumnName("InvoiceID");

                entity.Property(e => e.Note).HasMaxLength(200);

                entity.Property(e => e.OrderDate).HasColumnType("datetime");

                entity.Property(e => e.RecevingOrder).HasColumnType("datetime");

                entity.Property(e => e.RefundDate).HasColumnType("datetime");

                entity.Property(e => e.ShipTime).HasColumnType("datetime");

                entity.Property(e => e.StaffId).HasColumnName("StaffID");

                entity.Property(e => e.Status).HasDefaultValueSql("((0))");

                entity.Property(e => e.TotalAmount).HasColumnType("money");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK_Order_Staff");

                entity.HasOne(d => d.Address)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.AddressId)
                    .HasConstraintName("FK_Order_Address");

                entity.HasOne(d => d.Discount)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.DiscountId)
                    .HasConstraintName("FK_Order_Discount");

                entity.HasOne(d => d.GuestPhoneNavigation)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.GuestPhone)
                    .HasConstraintName("FK_Order_Guest");

                entity.HasOne(d => d.Invoice)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.InvoiceId)
                    .HasConstraintName("FK_Order_Invoice");
                modelBuilder.Entity<Order>()
  .HasOne(o => o.Collected)
  .WithMany()
  .HasForeignKey(o => o.CollectedBy)
  .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.ToTable("OrderDetail");

                entity.Property(e => e.OrderDetailId).HasColumnName("OrderDetailID");

                entity.Property(e => e.ComboId).HasColumnName("ComboID");

                entity.Property(e => e.DishId).HasColumnName("DishID");

                entity.Property(e => e.DishesServed).HasDefaultValueSql("((0))");

                entity.Property(e => e.Note).HasMaxLength(200);

                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.Property(e => e.OrderTime).HasColumnType("datetime");

                entity.Property(e => e.UnitPrice).HasColumnType("money");

                entity.HasOne(d => d.Combo)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.ComboId)
                    .HasConstraintName("FK_OrderDetail_Combo");

                entity.HasOne(d => d.Dish)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.DishId)
                    .HasConstraintName("FK_OrderDetail_Dish");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_OrderDetail_Order");
            });

            modelBuilder.Entity<OrderTable>(entity =>
            {
                entity.HasKey(e => new { e.OrderId, e.TableId });

                entity.ToTable("OrderTable");

                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.Property(e => e.TableId).HasColumnName("TableID");

                entity.HasOne(d => d.Order)
                   .WithMany(o => o.OrderTables)
                   .HasForeignKey(d => d.OrderId)
                   .OnDelete(DeleteBehavior.ClientSetNull)
                   .HasConstraintName("FK_OrderTable_Order");

                entity.HasOne(d => d.Table)
                    .WithMany(t => t.OrderTables)
                    .HasForeignKey(d => d.TableId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderTable_Table");
            });

            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.ToTable("Reservation");

                entity.Property(e => e.ReservationId).HasColumnName("ReservationID");

                entity.Property(e => e.AccountId).HasColumnName("AccountID");

                entity.Property(e => e.CancelBy).HasMaxLength(100);

                entity.Property(e => e.Note).HasMaxLength(255);

                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.Property(e => e.ReasonCancel).HasMaxLength(200);

                entity.Property(e => e.ReservationTime).HasColumnType("datetime");

                entity.Property(e => e.TimeIn).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Reservations)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK_Reservation_Account");

                entity.HasOne(d => d.Address)
                    .WithMany(p => p.Reservations)
                    .HasForeignKey(d => d.AddressId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Reservation_Address");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Reservations)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK__Reservati__Order__6FE99F9F");
            });

            modelBuilder.Entity<Setting>(entity =>
            {
                entity.Property(e => e.Address).HasMaxLength(200);

                entity.Property(e => e.EateryName).HasMaxLength(100);

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.LinkContact)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Logo)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Phone)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.Qrcode)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("QRCode");
            });

            modelBuilder.Entity<Table>(entity =>
            {
                entity.ToTable("Table");

                entity.Property(e => e.TableId).HasColumnName("TableID");

                entity.Property(e => e.Floor).HasMaxLength(500);

                entity.Property(e => e.Lable).HasMaxLength(200);
            });

            modelBuilder.Entity<TableReservation>(entity =>
            {
                entity.ToTable("TableReservation");
                entity.HasKey(tr => new { tr.ReservationId, tr.TableId });

                entity.HasOne(tr => tr.Reservation)
                    .WithMany(r => r.TableReservations)
                    .HasForeignKey(tr => tr.ReservationId);

                entity.HasOne(tr => tr.Table)
                    .WithMany(t => t.TableReservations)
                    .HasForeignKey(tr => tr.TableId);
            });

            modelBuilder.Entity<Wallet>(entity =>
            {
                entity.ToTable("Wallet");

                entity.Property(e => e.WalletId).HasColumnName("WalletID");

                entity.Property(e => e.Balance).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.GuestPhone)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.LastUpdated)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.GuestPhoneNavigation)
                    .WithMany(p => p.Wallets)
                    .HasForeignKey(d => d.GuestPhone)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Wallet_GuestPhone");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
