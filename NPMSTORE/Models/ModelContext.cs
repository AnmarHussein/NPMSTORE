using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace NPMSTORE.Models
{
    public partial class ModelContext : DbContext
    {
        public ModelContext()
        {
        }

        public ModelContext(DbContextOptions<ModelContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AboutU> AboutUs { get; set; }
        public virtual DbSet<Caption> Captions { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<ContactU> ContactUs { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<CustomersLogin> CustomersLogins { get; set; }
        public virtual DbSet<Header> Headers { get; set; }
        public virtual DbSet<HomePage> HomePages { get; set; }
        public virtual DbSet<InfoCard> InfoCards { get; set; }
        public virtual DbSet<Link> Links { get; set; }
        public virtual DbSet<Offer> Offers { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderProduct> OrderProducts { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductAttribute> ProductAttributes { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Sidebar> Sidebars { get; set; }
        public virtual DbSet<Stor> Stors { get; set; }
        public virtual DbSet<StoreCategory> StoreCategories { get; set; }
        public virtual DbSet<Testimonial> Testimonials { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseOracle("USER ID=TAH14_USER207;PASSWORD=Hussein2148--;DATA SOURCE=94.56.229.181:3488/traindb");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("TAH14_USER207")
                .HasAnnotation("Relational:Collation", "USING_NLS_COMP");

            modelBuilder.Entity<AboutU>(entity =>
            {
                entity.ToTable("ABOUT_US");

                entity.Property(e => e.Id)
                    .HasColumnType("NUMBER")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.AboutProject)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("ABOUT_PROJECT");

                entity.Property(e => e.Email1)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("EMAIL1");

                entity.Property(e => e.Email2)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("EMAIL2");

                entity.Property(e => e.Fax)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("FAX");

                entity.Property(e => e.HomeId)
                    .HasColumnType("NUMBER")
                    .HasColumnName("HOME_ID");

                entity.Property(e => e.OurMission)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("OUR_MISSION");

                entity.Property(e => e.OurVision)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("OUR_VISION");

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("PHONE_NUMBER");

                entity.Property(e => e.WhyChoess)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("WHY_CHOESS");

                entity.HasOne(d => d.Home)
                    .WithMany(p => p.AboutUs)
                    .HasForeignKey(d => d.HomeId)
                    .HasConstraintName("SYS_C00219106");
            });

            modelBuilder.Entity<Caption>(entity =>
            {
                entity.ToTable("CAPTION");

                entity.Property(e => e.Id)
                    .HasColumnType("NUMBER")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.HomeId)
                    .HasColumnType("NUMBER")
                    .HasColumnName("HOME_ID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("NAME");

                entity.HasOne(d => d.Home)
                    .WithMany(p => p.Captions)
                    .HasForeignKey(d => d.HomeId)
                    .HasConstraintName("SYS_C00219079");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("CATEGORY");

                entity.Property(e => e.Id)
                    .HasColumnType("NUMBER")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(60)
                    .IsUnicode(false)
                    .HasColumnName("NAME");
            });

            modelBuilder.Entity<ContactU>(entity =>
            {
                entity.ToTable("CONTACT_US");

                entity.Property(e => e.Id)
                    .HasColumnType("NUMBER")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Customerid)
                    .HasColumnType("NUMBER")
                    .HasColumnName("CUSTOMERID");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(40)
                    .IsUnicode(false)
                    .HasColumnName("EMAIL");

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(60)
                    .IsUnicode(false)
                    .HasColumnName("FULL_NAME");

                entity.Property(e => e.Message)
                    .IsRequired()
                    .HasMaxLength(300)
                    .IsUnicode(false)
                    .HasColumnName("MESSAGE");

                entity.Property(e => e.Subject)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("SUBJECT");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.ContactUs)
                    .HasForeignKey(d => d.Customerid)
                    .HasConstraintName("SYS_C00219014");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("CUSTOMERS");

                entity.Property(e => e.Id)
                    .HasColumnType("NUMBER")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Address1)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("ADDRESS1");

                entity.Property(e => e.Address2)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("ADDRESS2");

                entity.Property(e => e.Bdate)
                    .HasColumnType("DATE")
                    .HasColumnName("BDATE");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("EMAIL");

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(60)
                    .IsUnicode(false)
                    .HasColumnName("FULL_NAME");

                entity.Property(e => e.Gender)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("GENDER");

                entity.Property(e => e.Image)
                    .HasMaxLength(300)
                    .IsUnicode(false)
                    .HasColumnName("IMAGE");

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("PHONE_NUMBER");
            });

            modelBuilder.Entity<CustomersLogin>(entity =>
            {
                entity.ToTable("CUSTOMERS_LOGIN");

                entity.HasIndex(e => e.Roleid, "SYS_C00219003");

                entity.HasIndex(e => e.Customerid, "SYS_C00219004")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("NUMBER")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Customerid)
                    .HasColumnType("NUMBER")
                    .HasColumnName("CUSTOMERID");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("PASSWORD");

                entity.Property(e => e.Roleid)
                    .HasColumnType("NUMBER")
                    .HasColumnName("ROLEID");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("USER_NAME");

                entity.HasOne(d => d.Customer)
                    .WithOne(p => p.CustomersLogin)
                    .HasForeignKey<CustomersLogin>(d => d.Customerid)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("SYS_C00219006");

                entity.HasOne(d => d.Role)
                    .WithOne(p => p.CustomersLogin)
                    .HasForeignKey<CustomersLogin>(d => d.Roleid)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("SYS_C00219005");
            });

            modelBuilder.Entity<Header>(entity =>
            {
                entity.ToTable("HEADER");

                entity.Property(e => e.Id)
                    .HasColumnType("NUMBER")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.HomeId)
                    .HasColumnType("NUMBER")
                    .HasColumnName("HOME_ID");

                entity.Property(e => e.Logo)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("LOGO");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("NAME");

                entity.HasOne(d => d.Home)
                    .WithMany(p => p.Headers)
                    .HasForeignKey(d => d.HomeId)
                    .HasConstraintName("SYS_C00219112");
            });

            modelBuilder.Entity<HomePage>(entity =>
            {
                entity.ToTable("HOME_PAGE");

                entity.Property(e => e.Id)
                    .HasColumnType("NUMBER")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Customerid)
                    .HasColumnType("NUMBER")
                    .HasColumnName("CUSTOMERID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("NAME");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.HomePages)
                    .HasForeignKey(d => d.Customerid)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("HOMER_PAGE_CUSTOMER_FK");
            });

            modelBuilder.Entity<InfoCard>(entity =>
            {
                entity.ToTable("INFO_CARD");

                entity.HasIndex(e => e.CardNumber, "SYS_C00219117")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("NUMBER")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Balanc)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("BALANC");

                entity.Property(e => e.CardNumber)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("CARD_NUMBER");
            });

            modelBuilder.Entity<Link>(entity =>
            {
                entity.ToTable("LINKS");

                entity.Property(e => e.Id)
                    .HasColumnType("NUMBER")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.HomeId)
                    .HasColumnType("NUMBER")
                    .HasColumnName("HOME_ID");

                entity.Property(e => e.Link1)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("LINK");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("NAME");

                entity.HasOne(d => d.Home)
                    .WithMany(p => p.Links)
                    .HasForeignKey(d => d.HomeId)
                    .HasConstraintName("SYS_C00219074");
            });

            modelBuilder.Entity<Offer>(entity =>
            {
                entity.ToTable("OFFER");

                entity.Property(e => e.Id)
                    .HasColumnType("NUMBER")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Heading1)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("HEADING1");

                entity.Property(e => e.Heading2)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("HEADING2");

                entity.Property(e => e.HomeId)
                    .HasColumnType("NUMBER")
                    .HasColumnName("HOME_ID");

                entity.Property(e => e.Image)
                    .IsRequired()
                    .HasMaxLength(300)
                    .IsUnicode(false)
                    .HasColumnName("IMAGE");

                entity.Property(e => e.Pargraf)
                    .IsRequired()
                    .HasMaxLength(300)
                    .IsUnicode(false)
                    .HasColumnName("PARGRAF");

                entity.HasOne(d => d.Home)
                    .WithMany(p => p.Offers)
                    .HasForeignKey(d => d.HomeId)
                    .HasConstraintName("SYS_C00219087");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("ORDERS");

                entity.Property(e => e.Id)
                    .HasColumnType("NUMBER")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("DATE")
                    .HasColumnName("CREATE_AT");

                entity.Property(e => e.Customerid)
                    .HasColumnType("NUMBER")
                    .HasColumnName("CUSTOMERID");

                entity.Property(e => e.OrderStat)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("ORDER_STAT");

                entity.Property(e => e.Paymentid)
                    .HasColumnType("NUMBER")
                    .HasColumnName("PAYMENTID");

                entity.Property(e => e.TotalCost)
                    .HasColumnType("NUMBER")
                    .HasColumnName("TOTAL_COST");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.Customerid)
                    .HasConstraintName("SYS_C00219039");

                entity.HasOne(d => d.Payment)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.Paymentid)
                    .HasConstraintName("SYS_C00219038");
            });

            modelBuilder.Entity<OrderProduct>(entity =>
            {
                entity.HasKey(e => new { e.Orderid, e.Productid })
                    .HasName("SYS_C00219062");

                entity.ToTable("ORDER_PRODUCT");

                entity.Property(e => e.Orderid)
                    .HasColumnType("NUMBER")
                    .HasColumnName("ORDERID");

                entity.Property(e => e.Productid)
                    .HasColumnType("NUMBER")
                    .HasColumnName("PRODUCTID");

                entity.Property(e => e.Quantity)
                    .HasColumnType("NUMBER")
                    .HasColumnName("QUANTITY");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderProducts)
                    .HasForeignKey(d => d.Orderid)
                    .HasConstraintName("SYS_C00219063");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderProducts)
                    .HasForeignKey(d => d.Productid)
                    .HasConstraintName("SYS_C00219064");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("PAYMENTS");

                entity.Property(e => e.Id)
                    .HasColumnType("NUMBER")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Amount)
                    .HasColumnType("NUMBER")
                    .HasColumnName("AMOUNT");

                entity.Property(e => e.CardName)
                   .IsRequired()
                   .HasMaxLength(100)
                   .IsUnicode(false)
                   .HasColumnName("CARD_NAME");

                entity.Property(e => e.CardNumber)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("CARD_NUMBER");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("DATE")
                    .HasColumnName("CREATE_AT");

                entity.Property(e => e.Customerid)
                    .HasColumnType("NUMBER")
                    .HasColumnName("CUSTOMERID");

                entity.Property(e => e.ExpiryDate)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("EXPIRY_DATE");

                entity.Property(e => e.SecurityCode)
                    .HasColumnType("NUMBER")
                    .HasColumnName("SECURITY_CODE");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(d => d.Customerid)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("SYS_C00218998");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("PRODUCTS");

                entity.Property(e => e.Id)
                    .HasColumnType("NUMBER")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("DATE")
                    .HasColumnName("CREATE_AT");

                entity.Property(e => e.Details)
                    .IsRequired()
                    .HasMaxLength(300)
                    .IsUnicode(false)
                    .HasColumnName("DETAILS");

                entity.Property(e => e.Image)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("IMAGE");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(60)
                    .IsUnicode(false)
                    .HasColumnName("NAME");

                entity.Property(e => e.Price)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("PRICE");

                entity.Property(e => e.Seales)
                    .HasColumnType("NUMBER")
                    .HasColumnName("SEALES")
                    .HasDefaultValueSql("0 ");

                entity.Property(e => e.Stat)
                    .IsRequired()
                    .HasPrecision(1)
                    .HasColumnName("STAT")
                    .HasDefaultValueSql("1 ");
            });

            modelBuilder.Entity<ProductAttribute>(entity =>
            {
                entity.ToTable("PRODUCT_ATTRIBUTES");

                entity.Property(e => e.Id)
                    .HasColumnType("NUMBER")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Describtion)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DESCRIBTION");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("NAME");

                entity.Property(e => e.Productid)
                    .HasColumnType("NUMBER")
                    .HasColumnName("PRODUCTID");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductAttributes)
                    .HasForeignKey(d => d.Productid)
                    .HasConstraintName("SYS_C00219053");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("ROLES");

                entity.Property(e => e.Id)
                    .HasColumnType("NUMBER")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("NAME");
            });

            modelBuilder.Entity<Sidebar>(entity =>
            {
                entity.ToTable("SIDEBAR");

                entity.Property(e => e.Id)
                    .HasColumnType("NUMBER")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Image)
                    .IsRequired()
                    .HasMaxLength(300)
                    .IsUnicode(false)
                    .HasColumnName("IMAGE");

                entity.Property(e => e.Describtion)
                    .IsRequired()
                    .HasMaxLength(300)
                    .IsUnicode(false)
                    .HasColumnName("DESCRIBTION");

                entity.Property(e => e.Heading)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("HEADING");

                entity.Property(e => e.HomeId)
                    .HasColumnType("NUMBER")
                    .HasColumnName("HOME_ID");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("TITLE");

                entity.HasOne(d => d.Home)
                    .WithMany(p => p.Sidebars)
                    .HasForeignKey(d => d.HomeId)
                    .HasConstraintName("SYS_C00219094");
            });

            modelBuilder.Entity<Stor>(entity =>
            {
                entity.ToTable("STORS");

                entity.Property(e => e.Id)
                    .HasColumnType("NUMBER")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Customerid)
                    .HasColumnType("NUMBER")
                    .HasColumnName("CUSTOMERID");

                entity.Property(e => e.Detailes)
                    .IsRequired()
                    .HasMaxLength(300)
                    .IsUnicode(false)
                    .HasColumnName("DETAILES");

                entity.Property(e => e.Image)
                    .IsRequired()
                    .HasMaxLength(300)
                    .IsUnicode(false)
                    .HasColumnName("IMAGE");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(60)
                    .IsUnicode(false)
                    .HasColumnName("NAME");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Stors)
                    .HasForeignKey(d => d.Customerid)
                    .HasConstraintName("SYS_C00219027");
            });

            modelBuilder.Entity<StoreCategory>(entity =>
            {
                entity.ToTable("STORE_CATEGORY");

                entity.Property(e => e.Id)
                    .HasColumnType("NUMBER")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Categoryid)
                    .HasColumnType("NUMBER")
                    .HasColumnName("CATEGORYID");

                entity.Property(e => e.Productsid)
                    .HasColumnType("NUMBER")
                    .HasColumnName("PRODUCTSID");

                entity.Property(e => e.Storeid)
                    .HasColumnType("NUMBER")
                    .HasColumnName("STOREID");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.StoreCategories)
                    .HasForeignKey(d => d.Categoryid)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("CATEGORY_ALL");

                entity.HasOne(d => d.Products)
                    .WithMany(p => p.StoreCategories)
                    .HasForeignKey(d => d.Productsid)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("PRODUCTS_ALL");

                entity.HasOne(d => d.Store)
                    .WithMany(p => p.StoreCategories)
                    .HasForeignKey(d => d.Storeid)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("STORE_ALL");
            });

            modelBuilder.Entity<Testimonial>(entity =>
            {
                entity.ToTable("TESTIMONIALS");

                entity.Property(e => e.Id)
                    .HasColumnType("NUMBER")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Approved)
                    .HasColumnType("NUMBER(38)")
                    .HasColumnName("APPROVED");

                entity.Property(e => e.Customerid)
                    .HasColumnType("NUMBER")
                    .HasColumnName("CUSTOMERID");

                entity.Property(e => e.Details)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("DETAILS");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Testimonials)
                    .HasForeignKey(d => d.Customerid)
                    .HasConstraintName("SYS_C00219020");
            });

            modelBuilder.HasSequence("SEQ_DEPARTMENT");


            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
