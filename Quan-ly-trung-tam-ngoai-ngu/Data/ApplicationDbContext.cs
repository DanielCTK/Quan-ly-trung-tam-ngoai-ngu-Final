using Microsoft.EntityFrameworkCore;

namespace Quan_ly_trung_tam_ngoai_ngu.Data;

public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<AccountEntity> Accounts => Set<AccountEntity>();
    public DbSet<StudentEntity> Students => Set<StudentEntity>();
    public DbSet<TeacherEntity> Teachers => Set<TeacherEntity>();
    public DbSet<CourseEntity> Courses => Set<CourseEntity>();
    public DbSet<ClassEntity> Classes => Set<ClassEntity>();
    public DbSet<EnrollmentEntity> Enrollments => Set<EnrollmentEntity>();
    public DbSet<ReceiptEntity> Receipts => Set<ReceiptEntity>();
    public DbSet<ClassSessionEntity> ClassSessions => Set<ClassSessionEntity>();
    public DbSet<AttendanceEntity> Attendances => Set<AttendanceEntity>();
    public DbSet<ExamEntity> Exams => Set<ExamEntity>();
    public DbSet<ExamResultEntity> ExamResults => Set<ExamResultEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccountEntity>(entity =>
        {
            entity.ToTable("Accounts");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Username).HasMaxLength(50).IsRequired();
            entity.Property(x => x.PasswordHash).HasMaxLength(255).IsRequired();
            entity.Property(x => x.FullName).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(100);
            entity.Property(x => x.Phone).HasMaxLength(20);
            entity.Property(x => x.Role).HasMaxLength(20).IsRequired();
            entity.HasIndex(x => x.Username).IsUnique();
            entity.HasIndex(x => x.Email).IsUnique();
        });

        modelBuilder.Entity<StudentEntity>(entity =>
        {
            entity.ToTable("Students");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.StudentCode).HasMaxLength(20).IsRequired();
            entity.Property(x => x.FullName).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Gender).HasMaxLength(10);
            entity.Property(x => x.Phone).HasMaxLength(20);
            entity.Property(x => x.Email).HasMaxLength(100);
            entity.Property(x => x.Address).HasMaxLength(255);
            entity.Property(x => x.AvatarUrl).HasMaxLength(255);
            entity.HasIndex(x => x.StudentCode).IsUnique();
            entity.HasIndex(x => x.Email).IsUnique();
        });

        modelBuilder.Entity<TeacherEntity>(entity =>
        {
            entity.ToTable("Teachers");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.TeacherCode).HasMaxLength(20).IsRequired();
            entity.Property(x => x.FullName).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Phone).HasMaxLength(20);
            entity.Property(x => x.Email).HasMaxLength(100);
            entity.Property(x => x.Specialization).HasMaxLength(100);
            entity.Property(x => x.AvatarUrl).HasMaxLength(255);
            entity.HasIndex(x => x.TeacherCode).IsUnique();
            entity.HasIndex(x => x.Email).IsUnique();
        });

        modelBuilder.Entity<CourseEntity>(entity =>
        {
            entity.ToTable("Courses");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CourseCode).HasMaxLength(20).IsRequired();
            entity.Property(x => x.CourseName).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(500);
            entity.Property(x => x.TuitionFee).HasColumnType("decimal(18,2)");
            entity.HasIndex(x => x.CourseCode).IsUnique();
        });

        modelBuilder.Entity<ClassEntity>(entity =>
        {
            entity.ToTable("Classes");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.ClassCode).HasMaxLength(20).IsRequired();
            entity.Property(x => x.ClassName).HasMaxLength(100).IsRequired();
            entity.Property(x => x.ScheduleText).HasMaxLength(255);
            entity.HasIndex(x => x.ClassCode).IsUnique();

            entity.HasOne(x => x.Course)
                .WithMany(x => x.Classes)
                .HasForeignKey(x => x.CourseId);

            entity.HasOne(x => x.Teacher)
                .WithMany(x => x.Classes)
                .HasForeignKey(x => x.TeacherId);
        });

        modelBuilder.Entity<EnrollmentEntity>(entity =>
        {
            entity.ToTable("Enrollments");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Status).HasMaxLength(20).IsRequired();
            entity.Property(x => x.TotalFee).HasColumnType("decimal(18,2)");
            entity.Property(x => x.DiscountAmount).HasColumnType("decimal(18,2)");
            entity.Property(x => x.FinalFee)
                .HasColumnType("decimal(18,2)")
                .HasComputedColumnSql("[TotalFee]-[DiscountAmount]", stored: true);
            entity.Property(x => x.Note).HasMaxLength(255);
            entity.HasIndex(x => new { x.StudentId, x.ClassId }).IsUnique();

            entity.HasOne(x => x.Student)
                .WithMany(x => x.Enrollments)
                .HasForeignKey(x => x.StudentId);

            entity.HasOne(x => x.Class)
                .WithMany(x => x.Enrollments)
                .HasForeignKey(x => x.ClassId);
        });

        modelBuilder.Entity<ReceiptEntity>(entity =>
        {
            entity.ToTable("Receipts");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.ReceiptCode).HasMaxLength(20).IsRequired();
            entity.Property(x => x.Amount).HasColumnType("decimal(18,2)");
            entity.Property(x => x.PaymentMethod).HasMaxLength(20).IsRequired();
            entity.Property(x => x.Note).HasMaxLength(255);
            entity.HasIndex(x => x.ReceiptCode).IsUnique();

            entity.HasOne(x => x.Enrollment)
                .WithMany(x => x.Receipts)
                .HasForeignKey(x => x.EnrollmentId);
        });

        modelBuilder.Entity<ClassSessionEntity>(entity =>
        {
            entity.ToTable("ClassSessions");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Topic).HasMaxLength(255);
            entity.Property(x => x.Note).HasMaxLength(255);

            entity.HasOne(x => x.Class)
                .WithMany(x => x.ClassSessions)
                .HasForeignKey(x => x.ClassId);
        });

        modelBuilder.Entity<AttendanceEntity>(entity =>
        {
            entity.ToTable("Attendances");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.AttendanceStatus).HasMaxLength(20).IsRequired();
            entity.Property(x => x.Note).HasMaxLength(255);
            entity.HasIndex(x => new { x.EnrollmentId, x.ClassSessionId }).IsUnique();

            entity.HasOne(x => x.Enrollment)
                .WithMany(x => x.Attendances)
                .HasForeignKey(x => x.EnrollmentId);

            entity.HasOne(x => x.ClassSession)
                .WithMany(x => x.Attendances)
                .HasForeignKey(x => x.ClassSessionId);
        });

        modelBuilder.Entity<ExamEntity>(entity =>
        {
            entity.ToTable("Exams");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.ExamName).HasMaxLength(100).IsRequired();
            entity.Property(x => x.ExamType).HasMaxLength(20).IsRequired();
            entity.Property(x => x.MaxScore).HasColumnType("decimal(5,2)");

            entity.HasOne(x => x.Class)
                .WithMany(x => x.Exams)
                .HasForeignKey(x => x.ClassId);
        });

        modelBuilder.Entity<ExamResultEntity>(entity =>
        {
            entity.ToTable("ExamResults");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Score).HasColumnType("decimal(5,2)");
            entity.Property(x => x.ResultStatus).HasMaxLength(20);
            entity.Property(x => x.Note).HasMaxLength(255);
            entity.HasIndex(x => new { x.ExamId, x.EnrollmentId }).IsUnique();

            entity.HasOne(x => x.Exam)
                .WithMany(x => x.Results)
                .HasForeignKey(x => x.ExamId);

            entity.HasOne(x => x.Enrollment)
                .WithMany(x => x.ExamResults)
                .HasForeignKey(x => x.EnrollmentId);
        });
    }
}

public sealed class AccountEntity
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public byte Status { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public sealed class StudentEntity
{
    public int Id { get; set; }
    public string StudentCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? AvatarUrl { get; set; }
    public byte Status { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public ICollection<EnrollmentEntity> Enrollments { get; set; } = new List<EnrollmentEntity>();
}

public sealed class TeacherEntity
{
    public int Id { get; set; }
    public string TeacherCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Specialization { get; set; }
    public string? AvatarUrl { get; set; }
    public byte Status { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public ICollection<ClassEntity> Classes { get; set; } = new List<ClassEntity>();
}

public sealed class CourseEntity
{
    public int Id { get; set; }
    public string CourseCode { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DurationHours { get; set; }
    public decimal TuitionFee { get; set; }
    public byte Status { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public ICollection<ClassEntity> Classes { get; set; } = new List<ClassEntity>();
}

public sealed class ClassEntity
{
    public int Id { get; set; }
    public string ClassCode { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public int CourseId { get; set; }
    public int? TeacherId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? ScheduleText { get; set; }
    public int Capacity { get; set; }
    public byte Status { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public CourseEntity Course { get; set; } = null!;
    public TeacherEntity? Teacher { get; set; }
    public ICollection<EnrollmentEntity> Enrollments { get; set; } = new List<EnrollmentEntity>();
    public ICollection<ClassSessionEntity> ClassSessions { get; set; } = new List<ClassSessionEntity>();
    public ICollection<ExamEntity> Exams { get; set; } = new List<ExamEntity>();
}

public sealed class EnrollmentEntity
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int ClassId { get; set; }
    public DateTime EnrollDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalFee { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal? FinalFee { get; set; }
    public string? Note { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public StudentEntity Student { get; set; } = null!;
    public ClassEntity Class { get; set; } = null!;
    public ICollection<ReceiptEntity> Receipts { get; set; } = new List<ReceiptEntity>();
    public ICollection<AttendanceEntity> Attendances { get; set; } = new List<AttendanceEntity>();
    public ICollection<ExamResultEntity> ExamResults { get; set; } = new List<ExamResultEntity>();
}

public sealed class ReceiptEntity
{
    public int Id { get; set; }
    public string ReceiptCode { get; set; } = string.Empty;
    public int EnrollmentId { get; set; }
    public DateTime PaymentDate { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }

    public EnrollmentEntity Enrollment { get; set; } = null!;
}

public sealed class ClassSessionEntity
{
    public int Id { get; set; }
    public int ClassId { get; set; }
    public DateTime SessionDate { get; set; }
    public string? Topic { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }

    public ClassEntity Class { get; set; } = null!;
    public ICollection<AttendanceEntity> Attendances { get; set; } = new List<AttendanceEntity>();
}

public sealed class AttendanceEntity
{
    public int Id { get; set; }
    public int EnrollmentId { get; set; }
    public int ClassSessionId { get; set; }
    public string AttendanceStatus { get; set; } = string.Empty;
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }

    public EnrollmentEntity Enrollment { get; set; } = null!;
    public ClassSessionEntity ClassSession { get; set; } = null!;
}

public sealed class ExamEntity
{
    public int Id { get; set; }
    public int ClassId { get; set; }
    public string ExamName { get; set; } = string.Empty;
    public string ExamType { get; set; } = string.Empty;
    public DateTime ExamDate { get; set; }
    public decimal MaxScore { get; set; }
    public DateTime CreatedAt { get; set; }

    public ClassEntity Class { get; set; } = null!;
    public ICollection<ExamResultEntity> Results { get; set; } = new List<ExamResultEntity>();
}

public sealed class ExamResultEntity
{
    public int Id { get; set; }
    public int ExamId { get; set; }
    public int EnrollmentId { get; set; }
    public decimal Score { get; set; }
    public string? ResultStatus { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }

    public ExamEntity Exam { get; set; } = null!;
    public EnrollmentEntity Enrollment { get; set; } = null!;
}
