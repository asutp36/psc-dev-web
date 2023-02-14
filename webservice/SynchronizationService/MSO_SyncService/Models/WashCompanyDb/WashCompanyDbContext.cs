using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MSO_SyncService.Models.WashCompanyDb;

public partial class WashCompanyDbContext : DbContext
{
    public WashCompanyDbContext()
    {
    }

    public WashCompanyDbContext(DbContextOptions<WashCompanyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Card> Cards { get; set; }

    public virtual DbSet<Device> Devices { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<EventCollect> EventCollects { get; set; }

    public virtual DbSet<EventIncrease> EventIncreases { get; set; }

    public virtual DbSet<EventMode> EventModes { get; set; }

    public virtual DbSet<EventSimple> EventSimples { get; set; }

    public virtual DbSet<MobileSending> MobileSendings { get; set; }

    public virtual DbSet<Mode> Modes { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<PostSession> PostSessions { get; set; }

    public virtual DbSet<Wash> Washes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=localhost\\SQLEXPRESS;Initial Catalog=WashCompany;User Id=sa; Password=carwash;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Card>(entity =>
        {
            entity.HasKey(e => e.Idcard);

            entity.Property(e => e.Idcard).HasColumnName("IDCard");
            entity.Property(e => e.CardNum).HasMaxLength(20);
            entity.Property(e => e.IdcardStatus).HasColumnName("IDCardStatus");
            entity.Property(e => e.IdcardType).HasColumnName("IDCardType");
            entity.Property(e => e.Idowner).HasColumnName("IDOwner");
            entity.Property(e => e.LocalizedId).HasColumnName("LocalizedID");
        });

        modelBuilder.Entity<Device>(entity =>
        {
            entity.HasKey(e => e.Iddevice);

            entity.ToTable("Device");

            entity.Property(e => e.Iddevice).HasColumnName("IDDevice");
            entity.Property(e => e.Code).HasMaxLength(10);
            entity.Property(e => e.IddeviceType).HasColumnName("IDDeviceType");
            entity.Property(e => e.IpAddress).HasMaxLength(20);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.ServerId).HasColumnName("ServerID");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.Idevent);

            entity.ToTable("Event");

            entity.Property(e => e.Idevent).HasColumnName("IDEvent");
            entity.Property(e => e.Dtime)
                .HasColumnType("datetime")
                .HasColumnName("DTime");
            entity.Property(e => e.IdeventKind).HasColumnName("IDEventKind");
            entity.Property(e => e.IdeventPost).HasColumnName("IDEventPost");
            entity.Property(e => e.Idpost).HasColumnName("IDPost");

            entity.HasOne(d => d.IdpostNavigation).WithMany(p => p.Events)
                .HasForeignKey(d => d.Idpost)
                .HasConstraintName("FK_Event_Point");
        });

        modelBuilder.Entity<EventCollect>(entity =>
        {
            entity.HasKey(e => e.Idevent);

            entity.ToTable("EventCollect");

            entity.Property(e => e.Idevent)
                .ValueGeneratedNever()
                .HasColumnName("IDEvent");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.B10).HasColumnName("b10");
            entity.Property(e => e.B100).HasColumnName("b100");
            entity.Property(e => e.B200).HasColumnName("b200");
            entity.Property(e => e.B50).HasColumnName("b50");
            entity.Property(e => e.M10).HasColumnName("m10");

            entity.HasOne(d => d.IdeventNavigation).WithOne(p => p.EventCollect)
                .HasForeignKey<EventCollect>(d => d.Idevent)
                .HasConstraintName("FK_EventCollect_Event");
        });

        modelBuilder.Entity<EventIncrease>(entity =>
        {
            entity.HasKey(e => e.Idevent).HasName("PK_EventCash");

            entity.ToTable("EventIncrease");

            entity.Property(e => e.Idevent)
                .ValueGeneratedNever()
                .HasColumnName("IDEvent");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.B10).HasColumnName("b10");
            entity.Property(e => e.B100).HasColumnName("b100");
            entity.Property(e => e.B200).HasColumnName("b200");
            entity.Property(e => e.B50).HasColumnName("b50");
            entity.Property(e => e.Balance).HasColumnName("balance");
            entity.Property(e => e.IdpostSession).HasColumnName("IDPostSession");
            entity.Property(e => e.M10).HasColumnName("m10");

            entity.HasOne(d => d.IdeventNavigation).WithOne(p => p.EventIncrease)
                .HasForeignKey<EventIncrease>(d => d.Idevent)
                .HasConstraintName("FK_EventCash_Event");

            entity.HasOne(d => d.IdpostSessionNavigation).WithMany(p => p.EventIncreases)
                .HasForeignKey(d => d.IdpostSession)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_EventIncrease_PostSession");
        });

        modelBuilder.Entity<EventMode>(entity =>
        {
            entity.HasKey(e => e.Idevent);

            entity.ToTable("EventMode");

            entity.Property(e => e.Idevent)
                .ValueGeneratedNever()
                .HasColumnName("IDEvent");
            entity.Property(e => e.CardNum).HasMaxLength(20);
            entity.Property(e => e.CardTypeCode).HasMaxLength(20);
            entity.Property(e => e.Cost).HasColumnType("numeric(18, 0)");
            entity.Property(e => e.DtimeFinish)
                .HasColumnType("datetime")
                .HasColumnName("DTimeFinish");
            entity.Property(e => e.DtimeStart)
                .HasColumnType("datetime")
                .HasColumnName("DTimeStart");
            entity.Property(e => e.Idmode).HasColumnName("IDMode");

            entity.HasOne(d => d.IdeventNavigation).WithOne(p => p.EventMode)
                .HasForeignKey<EventMode>(d => d.Idevent)
                .HasConstraintName("FK_EventMode_Event");

            entity.HasOne(d => d.IdmodeNavigation).WithMany(p => p.EventModes)
                .HasForeignKey(d => d.Idmode)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_EventMode_Mode");
        });

        modelBuilder.Entity<EventSimple>(entity =>
        {
            entity.HasKey(e => e.Idevent);

            entity.ToTable("EventSimple");

            entity.Property(e => e.Idevent)
                .ValueGeneratedNever()
                .HasColumnName("IDEvent");

            entity.HasOne(d => d.IdeventNavigation).WithOne(p => p.EventSimple)
                .HasForeignKey<EventSimple>(d => d.Idevent)
                .HasConstraintName("FK_EventSimple_Event");
        });

        modelBuilder.Entity<MobileSending>(entity =>
        {
            entity.HasKey(e => e.IdmobileSending);

            entity.HasIndex(e => new { e.Idcard, e.Idpost, e.DtimeStart }, "UC_MobileSending").IsUnique();

            entity.Property(e => e.IdmobileSending).HasColumnName("IDMobileSending");
            entity.Property(e => e.DtimeEnd)
                .HasColumnType("datetime")
                .HasColumnName("DTimeEnd");
            entity.Property(e => e.DtimeStart)
                .HasColumnType("datetime")
                .HasColumnName("DTimeStart");
            entity.Property(e => e.Guid)
                .HasMaxLength(36)
                .IsFixedLength();
            entity.Property(e => e.Idcard).HasColumnName("IDCard");
            entity.Property(e => e.Idpost).HasColumnName("IDPost");
            entity.Property(e => e.ResultMessage).HasMaxLength(80);

            entity.HasOne(d => d.IdcardNavigation).WithMany(p => p.MobileSendings)
                .HasForeignKey(d => d.Idcard)
                .HasConstraintName("FK_MobileSendings_Cards");

            entity.HasOne(d => d.IdpostNavigation).WithMany(p => p.MobileSendings)
                .HasForeignKey(d => d.Idpost)
                .HasConstraintName("FK_MobileSendings_Posts");
        });

        modelBuilder.Entity<Mode>(entity =>
        {
            entity.HasKey(e => e.Idmode);

            entity.ToTable("Mode");

            entity.Property(e => e.Idmode).HasColumnName("IDMode");
            entity.Property(e => e.Code).HasMaxLength(20);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.Idpost);

            entity.Property(e => e.Idpost).HasColumnName("IDPost");
            entity.Property(e => e.Code).HasMaxLength(20);
            entity.Property(e => e.Iddevice).HasColumnName("IDDevice");
            entity.Property(e => e.Idwash).HasColumnName("IDWash");
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Qrcode)
                .HasMaxLength(20)
                .HasColumnName("QRCode");

            entity.HasOne(d => d.IddeviceNavigation).WithMany(p => p.Posts)
                .HasForeignKey(d => d.Iddevice)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Posts_Device");

            entity.HasOne(d => d.IdwashNavigation).WithMany(p => p.Posts)
                .HasForeignKey(d => d.Idwash)
                .HasConstraintName("FK_Posts_Wash");
        });

        modelBuilder.Entity<PostSession>(entity =>
        {
            entity.HasKey(e => e.IdpostSession);

            entity.ToTable("PostSession");

            entity.Property(e => e.IdpostSession).HasColumnName("IDPostSession");
            entity.Property(e => e.FiscalError).HasMaxLength(150);
            entity.Property(e => e.Idpost).HasColumnName("IDPost");
            entity.Property(e => e.IdsessionOnPost).HasColumnName("IDSessionOnPost");
            entity.Property(e => e.Qr)
                .HasMaxLength(100)
                .HasColumnName("QR");
            entity.Property(e => e.StartDtime)
                .HasColumnType("datetime")
                .HasColumnName("StartDTime");
            entity.Property(e => e.StopDtime)
                .HasColumnType("datetime")
                .HasColumnName("StopDTime");
        });

        modelBuilder.Entity<Wash>(entity =>
        {
            entity.HasKey(e => e.Idwash);

            entity.ToTable("Wash");

            entity.Property(e => e.Idwash).HasColumnName("IDWash");
            entity.Property(e => e.Address).HasMaxLength(100);
            entity.Property(e => e.Code).HasMaxLength(5);
            entity.Property(e => e.Idregion).HasColumnName("IDRegion");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
