using Quan_ly_trung_tam_ngoai_ngu.Models;
using Quan_ly_trung_tam_ngoai_ngu.Services.Interfaces;

namespace Quan_ly_trung_tam_ngoai_ngu.Services.Mocks;

public class DemoAuthService : IDemoAuthService
{
    private readonly IMockDataService _mockDataService;

    public DemoAuthService(IMockDataService mockDataService)
    {
        _mockDataService = mockDataService;
    }

    public IReadOnlyList<DemoAccount> GetDemoAccounts()
    {
        return _mockDataService.GetAccounts();
    }

    public DemoAccount? ValidateLogin(string email, string password)
    {
        return _mockDataService
            .GetAccounts()
            .FirstOrDefault(x =>
                x.Email.Equals(email, StringComparison.OrdinalIgnoreCase) &&
                x.Password == password);
    }
}
