using Microsoft.JSInterop;
using System.Timers;
using Microsoft.AspNetCore.Components;

namespace proyecto_hospital_version_1.Services;

public class AuthStateService
{
    private readonly IJSRuntime _js;
    private readonly NavigationManager _nav;
    private Timer? _inactivityTimer;
    private DotNetObjectReference<AuthStateService>? _dotNetRef;
    private const int InactivityMs = 5 * 60 * 1000; // 5 minutos

    public string? Token { get; private set; }
    public string? Username { get; private set; }
    public string? Role { get; private set; }

    public AuthStateService(IJSRuntime js, NavigationManager nav)
    {
        _js = js;
        _nav = nav;
    }

    public async Task SetTokenAsync(string token, string username, string role, DateTime expiresAt)
    {
        Token = token;
        Username = username;
        Role = role;

        await _js.InvokeVoidAsync("localStorage.setItem", "auth_token", token);
        await _js.InvokeVoidAsync("localStorage.setItem", "auth_username", username);
        await _js.InvokeVoidAsync("localStorage.setItem", "auth_role", role);

        StartInactivityWatcher();
    }

    public async Task LoadFromStorageAsync()
    {
        Token = await _js.InvokeAsync<string?>("localStorage.getItem", "auth_token");
        Username = await _js.InvokeAsync<string?>("localStorage.getItem", "auth_username");
        Role = await _js.InvokeAsync<string?>("localStorage.getItem", "auth_role");
        if (!string.IsNullOrEmpty(Token))
            StartInactivityWatcher();
    }

    public async Task LogoutAsync()
    {
        Token = null;
        Username = null;
        Role = null;
        StopInactivityWatcher();
        await _js.InvokeVoidAsync("localStorage.removeItem", "auth_token");
        await _js.InvokeVoidAsync("localStorage.removeItem", "auth_username");
        await _js.InvokeVoidAsync("localStorage.removeItem", "auth_role");
        _nav.NavigateTo("/login", true);
    }

    private void StartInactivityWatcher()
    {
        StopInactivityWatcher();

        _inactivityTimer = new Timer(InactivityMs);
        _inactivityTimer.Elapsed += async (_, __) =>
        {
            await LogoutAsync();
        };
        _inactivityTimer.AutoReset = false;
        _inactivityTimer.Start();

        // registrar para recibir eventos desde JS
        _dotNetRef = DotNetObjectReference.Create(this);
        _ = _js.InvokeVoidAsync("sessionManager.registerDotNet", _dotNetRef);
        _ = _js.InvokeVoidAsync("sessionManager.startListening");
    }

    private void StopInactivityWatcher()
    {
        try
        {
            _inactivityTimer?.Stop();
            _inactivityTimer?.Dispose();
            _inactivityTimer = null;
        }
        catch { }
        try
        {
            if (_dotNetRef != null)
            {
                _dotNetRef.Dispose();
                _dotNetRef = null;
            }
            _ = _js.InvokeVoidAsync("sessionManager.stopListening");
        }
        catch { }
    }

    [JSInvokable("ResetInactivity")]
    public void ResetInactivity()
    {
        if (_inactivityTimer != null)
        {
            _inactivityTimer.Stop();
            _inactivityTimer.Start();
        }
    }
}
