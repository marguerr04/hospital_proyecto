namespace proyecto_hospital_version_1.Services;

public class AuthStateService
{
    private bool _isAuthenticated;
    private string? _username;
    private string? _role;

    public bool IsAuthenticated => _isAuthenticated;
    public string? Username => _username;
    public string? Role => _role;

    public event Action? OnChange;

    public void Login(string username, string role)
    {
        Console.WriteLine($"[AuthStateService] Login llamado: User={username}, Role={role}");
        _isAuthenticated = true;
        _username = username;
        _role = role;
        NotifyStateChanged();
    }

    public void Logout()
    {
        Console.WriteLine($"[AuthStateService] Logout llamado");
        _isAuthenticated = false;
        _username = null;
        _role = null;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
