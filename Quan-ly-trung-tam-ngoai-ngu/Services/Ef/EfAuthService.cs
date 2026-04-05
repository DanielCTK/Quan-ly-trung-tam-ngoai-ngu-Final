using Quan_ly_trung_tam_ngoai_ngu.Data;
using Quan_ly_trung_tam_ngoai_ngu.Models;
using Quan_ly_trung_tam_ngoai_ngu.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Quan_ly_trung_tam_ngoai_ngu.Services.Ef;

public sealed class EfAuthService : IDemoAuthService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<EfAuthService> _logger;

    public EfAuthService(ApplicationDbContext dbContext, ILogger<EfAuthService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public IReadOnlyList<DemoAccount> GetDemoAccounts()
    {
        var teacherSpecializations = _dbContext.Teachers
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.Email != null)
            .ToDictionary(x => x.Email!, x => x.Specialization ?? string.Empty, StringComparer.OrdinalIgnoreCase);

        return _dbContext.Accounts
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Id)
            .Select(x => new DemoAccount
            {
                Id = x.Id,
                Username = x.Username,
                FullName = x.FullName,
                Email = x.Email ?? string.Empty,
                Phone = x.Phone ?? string.Empty,
                Role = x.Role,
                Department = ResolveDepartment(x.Role, x.Email, teacherSpecializations),
                Status = EfServiceMapper.MapAccountStatus(x.IsActive, x.Status),
                Password = string.IsNullOrWhiteSpace(x.PasswordHash) ? string.Empty : "********",
                PasswordHash = x.PasswordHash
            })
            .ToList();
    }

    public DemoAccount? ValidateLogin(string email, string password)
    {
        var login = email.Trim();
        var normalizedLogin = login.ToLower();

        var account = _dbContext.Accounts
            .AsNoTracking()
            .FirstOrDefault(x =>
                !x.IsDeleted &&
                ((x.Email != null && x.Email.ToLower() == normalizedLogin) ||
                 x.Username.ToLower() == normalizedLogin));

        if (account is null)
        {
            return null;
        }

        if (!account.IsActive || account.Status != 1)
        {
            return null;
        }

        if (!string.Equals(account.PasswordHash, password, StringComparison.Ordinal))
        {
            return null;
        }

        var teacherSpecialization = _dbContext.Teachers
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.Email == account.Email)
            .Select(x => x.Specialization)
            .FirstOrDefault();

        return new DemoAccount
        {
            Id = account.Id,
            Username = account.Username,
            FullName = account.FullName,
            Email = account.Email ?? string.Empty,
            Phone = account.Phone ?? string.Empty,
            Role = account.Role,
            Department = ResolveDepartment(account.Role, account.Email, teacherSpecialization),
            Status = EfServiceMapper.MapAccountStatus(account.IsActive, account.Status),
            Password = "********",
            PasswordHash = account.PasswordHash
        };
    }

    public StudentRegistrationResult RegisterStudent(string fullName, string email, string phone, string password)
    {
        var normalizedName = fullName.Trim();
        var normalizedEmail = email.Trim();
        var normalizedPhone = phone.Trim();

        if (string.IsNullOrWhiteSpace(normalizedName))
        {
            return StudentRegistrationResult.Fail("Họ và tên là bắt buộc.");
        }

        if (string.IsNullOrWhiteSpace(normalizedEmail))
        {
            return StudentRegistrationResult.Fail("Email là bắt buộc.");
        }

        var emailExists = _dbContext.Students.Any(x => !x.IsDeleted && x.Email == normalizedEmail) ||
                          _dbContext.Accounts.Any(x => !x.IsDeleted && x.Email == normalizedEmail);

        if (emailExists)
        {
            return StudentRegistrationResult.Fail("Email này đã tồn tại trong cơ sở dữ liệu.");
        }

        try
        {
            var maxStudentCode = _dbContext.Students
                .AsNoTracking()
                .Where(x => x.StudentCode.StartsWith("S"))
                .Select(x => x.StudentCode)
                .ToList()
                .Select(code => int.TryParse(code[1..], out var value) ? value : 0)
                .DefaultIfEmpty(0)
                .Max();

            var nextStudentCode = $"S{maxStudentCode + 1:000}";

            _dbContext.Students.Add(new StudentEntity
            {
                StudentCode = nextStudentCode,
                FullName = normalizedName,
                Email = normalizedEmail,
                Phone = string.IsNullOrWhiteSpace(normalizedPhone) ? null : normalizedPhone,
                Status = 1,
                IsDeleted = false,
                CreatedAt = DateTime.Now
            });

            _dbContext.SaveChanges();
            return StudentRegistrationResult.Success($"Đăng ký thành công. Học viên {nextStudentCode} đã được tạo trong cơ sở dữ liệu.", nextStudentCode);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Could not register student with email {Email}.", normalizedEmail);
            return StudentRegistrationResult.Fail("Không thể tạo học viên mới trong cơ sở dữ liệu.");
        }
    }

    private static string ResolveDepartment(string role, string? email, IReadOnlyDictionary<string, string> teacherSpecializations)
    {
        if (!string.IsNullOrWhiteSpace(email) && teacherSpecializations.TryGetValue(email, out var specialization) && !string.IsNullOrWhiteSpace(specialization))
        {
            return specialization;
        }

        return role switch
        {
            "Admin" => "Ban quản trị",
            "Staff" => "Giáo vụ",
            "Teacher" => "Giảng viên",
            _ => "Hệ thống"
        };
    }

    private static string ResolveDepartment(string role, string? email, string? teacherSpecialization)
    {
        if (!string.IsNullOrWhiteSpace(teacherSpecialization))
        {
            return teacherSpecialization;
        }

        return ResolveDepartment(role, email, new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
    }
}
