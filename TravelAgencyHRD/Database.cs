using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;

namespace TravelAgencyHRD
{
    public class LogJournalConfiguration : IEntityTypeConfiguration<LogJournal>
    {
        public void Configure(EntityTypeBuilder<LogJournal> builder)
        {
            builder.ToTable("LogJournal");
            builder
                .Property(x => x.Initials)
                .HasColumnType("varchar")
                .HasMaxLength(30);
            builder
                .Property(x => x.Password)
                .HasColumnType("varchar")
                .HasMaxLength(16);
            builder
               .Property(x => x.Role)
               .HasColumnType("varchar")
               .HasMaxLength(50);
            builder.HasData(
                new LogJournal[]
                {
                    new LogJournal() { Id = 1, Initials = "admin", Password = "admin", Role = "admin"},
                    new LogJournal() { Id = 2, Initials = "user", Password = "user", Role = "user" },
                    new LogJournal() { Id = 3, Initials = "worker1", Password = "worker", Role = "worker" }
                });
            builder.HasQueryFilter(p => p.IsDeleted == false);
        }
    }
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.ToTable("Post");
            builder
                .Property(x => x.Name)
                .HasColumnType("varchar")
                .HasMaxLength(150);
            builder.HasIndex(p => new { p.Name, p.IsDeleted}).IsUnique(true);
            builder.HasQueryFilter(p => p.IsDeleted == false);
        }
    }
    public class CabinetConfiguration : IEntityTypeConfiguration<Cabinet>
    {
        public void Configure(EntityTypeBuilder<Cabinet> builder)
        {
            builder.ToTable("Cabinet");
            builder
                .Property(x => x.Name)
                .HasColumnType("varchar")
                .HasMaxLength(100);
            builder
                .HasCheckConstraint("CabinetNumber", "CabinetNumber > 0")
                .HasCheckConstraint("Floor", "Floor > -1");
            builder.HasQueryFilter(p => p.IsDeleted == false);
        }
    }
    public class SeatsConfiguration : IEntityTypeConfiguration<Seats>
    {
        public void Configure(EntityTypeBuilder<Seats> builder)
        {
            builder.ToTable("Seats");
            builder
                .HasOne(s => s.Post)
                .WithMany(p => p.seats)
                .HasForeignKey(s => s.PostId)
                .HasPrincipalKey(p => p.Id)
                .OnDelete(DeleteBehavior.SetNull);
            builder
                .HasOne(s => s.Cabinet)
                .WithMany(c => c.Seats)
                .HasForeignKey(s => s.CabinetId)
                .HasPrincipalKey(c => c.Id)
                .OnDelete(DeleteBehavior.SetNull);
            builder.HasCheckConstraint("PostCount", "PostCount < 10");
            builder.HasCheckConstraint("Salary", "Salary > 0");
            builder.HasQueryFilter(p => p.IsDeleted == false);
        }
    }
    public class AppointmentsConfiguration : IEntityTypeConfiguration<Appointments>
    {
        public void Configure(EntityTypeBuilder<Appointments> builder)
        {
            builder.ToTable("Appointments");

            builder
            .HasOne(sp => sp.Seat)
                .WithMany(s => s.Appointments)
                .HasForeignKey(sp => sp.SeatsID)
                .HasPrincipalKey(s => s.Id)
                .OnDelete(DeleteBehavior.Restrict);
            builder
            .HasOne(sp => sp.Person)
                .WithMany(p => p.ServicePlacements)
                .HasForeignKey(sp => sp.PersonId)
                .HasPrincipalKey(p => p.Id)
                .OnDelete(DeleteBehavior.Restrict);
            builder
                .Property(sp => sp.DismissalReason)
                .HasColumnType("varchar")
                .HasMaxLength(300);
            builder.HasCheckConstraint("Stake", "Stake > 0");
            builder.HasQueryFilter(p => p.IsDeleted == false);
        }
    }
    public class PersonsEducationConfiguration : IEntityTypeConfiguration<PersonsEducation>
    {
        public void Configure(EntityTypeBuilder<PersonsEducation> builder)
        {
            builder.ToTable("PersonsEducation");
            builder
                .HasMany(ps => ps.Persons)
                .WithOne(p => p.Education)
                .HasForeignKey(p => p.EducationId)
                .HasPrincipalKey(ps => ps.Id)
                .OnDelete(DeleteBehavior.SetNull);
            builder
                .Property(ps => ps.Profession)
                .HasColumnType("varchar")
                .HasMaxLength(200);
            builder
                .Property(ps => ps.LanguageSkills)
                .HasColumnType("varchar")
                .HasMaxLength(200);
            builder
                .Property(ps => ps.ACDegree)
                .HasColumnType("varchar")
                .HasMaxLength(100);
            builder.HasQueryFilter(p => p.IsDeleted == false);
        }
    }
    public class PersonConfiguration : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.ToTable("Person");
            /*builder
                .HasMany(p => p.ServicePlacements)
                .WithOne(sp => sp.Person)
                .HasForeignKey(sp => sp.PersonId)
                .HasPrincipalKey(p => p.Id)
                .OnDelete(DeleteBehavior.Cascade);*/
            builder
                .Property(p => p.Initials)
                .HasColumnType("varchar")
                .HasMaxLength(200);
            builder
                .Property(p => p.Nationality)
                .HasColumnType("varchar")
                .HasMaxLength(100);
            builder
                .Property(p => p.FamilyStatus)
                .HasColumnType("varchar")
                .HasMaxLength(20);
            builder
                .Property(p => p.PhoneNumber)
                .HasColumnType("char")
                .HasMaxLength(13);
            builder
               .Property(p => p.Email)
               .HasColumnType("varchar")
               .HasMaxLength(30);
            builder.HasIndex(p => new { p.PhoneNumber, p.IsDeleted }).IsUnique(true);
            builder.HasQueryFilter(p => p.IsDeleted == false);
        }
    }
    public class PersonInformationConfiguration : IEntityTypeConfiguration<PersonInformation>
    {
        public void Configure(EntityTypeBuilder<PersonInformation> builder)
        {
            builder.ToTable("PersonsInformation");
            builder
                .Property(pi => pi.Initials)
                .HasColumnType("varchar")
                .HasMaxLength(200);
            builder
                .Property(pi => pi.Information)
                .HasColumnType("varchar")
                .HasMaxLength(300);
            builder
                .Property(pi => pi.Operation)
                .HasColumnType("varchar")
                .HasMaxLength(100);
            builder.HasQueryFilter(pi => pi.IsDeleted == false);
        }
    }
    public class SeatsInformationConfiguration : IEntityTypeConfiguration<SeatsInformation>
    {
        public void Configure(EntityTypeBuilder<SeatsInformation> builder)
        {
            builder.ToTable("SeatsInformation");
            builder
                .Property(pi => pi.Information)
                .HasColumnType("varchar")
                .HasMaxLength(300);
            builder
                .Property(pi => pi.Operation)
                .HasColumnType("varchar")
                .HasMaxLength(100);
            builder
                .Property(si => si.Post)
                .HasColumnType("varchar")
                .HasMaxLength(100);
            builder
                .Property(si => si.Cabinet)
                .HasColumnType("varchar")
                .HasMaxLength(100);
            builder.HasQueryFilter(pi => pi.IsDeleted == false);
        }
    }
    public class AppointmentsInformationConfiguration : IEntityTypeConfiguration<AppointmentsInformation>
    {
        public void Configure(EntityTypeBuilder<AppointmentsInformation> builder)
        {
            builder.ToTable("AppointmentsInformation");
            builder
                .Property(pi => pi.Information)
                .HasColumnType("varchar")
                .HasMaxLength(300);
            builder
                .Property(pi => pi.Operation)
                .HasColumnType("varchar")
                .HasMaxLength(100);
            builder
                .Property(si => si.Person)
                .HasColumnType("varchar")
                .HasMaxLength(200);
            builder
                .Property(si => si.Stake)
                .HasColumnType("varchar")
                .HasMaxLength(20);
            builder.HasQueryFilter(pi => pi.IsDeleted == false);
        }
    }
    public class ApplicationDbContext : DbContext
    {
        public DbSet<LogJournal> LogJournal { get; set; }
        public DbSet<Cabinet> Cabinet { get; set; }
        public DbSet<Post> Post { get; set; }
        public DbSet<Seats> Seats { get; set; }
        public DbSet<Appointments> Appointments { get; set; }
        public DbSet<PersonsEducation> PersonsEducation { get; set; }
        public DbSet<Person> Person { get; set; }
        public DbSet<SeatsInformation> SeatsInformation { get; set; }
        public DbSet<PersonInformation> PersonInformation { get; set; }
        public DbSet<AppointmentsInformation> AppointmentsInformation { get; set; }
        public ApplicationDbContext()
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer(
                    @"Server=(localdb)\mssqllocaldb;Database=TAHED;Trusted_Connection=True;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new LogJournalConfiguration());
            modelBuilder.ApplyConfiguration(new PostConfiguration());
            modelBuilder.ApplyConfiguration(new CabinetConfiguration());
            modelBuilder.ApplyConfiguration(new SeatsConfiguration());
            modelBuilder.ApplyConfiguration(new AppointmentsConfiguration());
            modelBuilder.ApplyConfiguration(new PersonsEducationConfiguration());
            modelBuilder.ApplyConfiguration(new PersonConfiguration());
            modelBuilder.ApplyConfiguration(new PersonInformationConfiguration());
            modelBuilder.ApplyConfiguration(new SeatsInformationConfiguration());
            modelBuilder.ApplyConfiguration(new AppointmentsInformationConfiguration());

        }
        public void GenerateValues()
            {
            ValuePadding.ValuesForEducation(this);
            ValuePadding.ValuesForPerson(this);
            ValuePadding.ValuesForPost(this);
            ValuePadding.ValuesForCabinet(this);
            ValuePadding.ValuesForSeats(this);
            ValuePadding.ValuesForAppointments(this);
        }
        public override int SaveChanges()
        {
            UpdateSoftDeleteStatuses();
            return base.SaveChanges();
        }
        private void UpdateSoftDeleteStatuses()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.CurrentValues["IsDeleted"] = false;
                        break;
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.CurrentValues["IsDeleted"] = true;
                        break;
                }
            }
        }
    }
}
