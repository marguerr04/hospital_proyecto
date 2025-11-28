using Microsoft.JSInterop;
using System.Timers;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace proyecto_hospital_version_1.Services;

public class AuthStateService
{
    private readonly IJSRuntime _js;
    private readonly NavigationManager _nav;
    private readonly HttpClient _http;
    private System.Timers.Timer? _inactivityTimer;
    private System.Timers.Timer? _warningTimer;
    private DotNetObjectReference<AuthStateService>? _dotNetRef;
    private const int InactivityMs = 5 * 60 * 1000; // 5 minutos
    private const int WarningMs = 4 * 60 * 1000;   // 4 minutos (1 minuto antes)
    private bool _warningShown = false;

    public string? Token { get; private set; }
    public string? Username { get; private set; }
    public string? Role { get; private set; }

    public bool IsAuthenticated => !string.IsNullOrEmpty(Token);

    public AuthStateService(IJSRuntime js, NavigationManager nav, HttpClient http)
    {
        _js = js;
        _nav = nav;
        _http = http;
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        try
        {
            var loginData = new
            {
                username = username,
                password = password
            };

            Console.WriteLine($"🔐 [AuthStateService] Enviando login...");
            Console.WriteLine($"   Username: {username}");
            Console.WriteLine($"   URL: {_http.BaseAddress}api/Auth/login");

            var response = await _http.PostAsJsonAsync("api/Auth/login", loginData);

            Console.WriteLine($"📡 [AuthStateService] Status: {response.StatusCode}");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

                if (result != null && !string.IsNullOrEmpty(result.Token))
                {
                    Console.WriteLine($"✅ [AuthStateService] Token recibido:");
                    Console.WriteLine($"   Username: {result.Username}");
                    Console.WriteLine($"   Role: {result.Role}");
                    Console.WriteLine($"   Token (primeros 20 chars): {result.Token.Substring(0, 20)}...");
                    Console.WriteLine($"   ExpiresAt: {result.ExpiresAt}");

                    await SetTokenAsync(result.Token, result.Username ?? username, result.Role ?? "Usuario", result.ExpiresAt);

                    Console.WriteLine($"💾 [AuthStateService] Token guardado en localStorage");
                    Console.WriteLine($"   IsAuthenticated: {IsAuthenticated}");

                    return true;
                }
                else
                {
                    Console.WriteLine($"❌ [AuthStateService] Respuesta exitosa pero sin token");
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ [AuthStateService] Error: {errorContent}");
            }

            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"💥 [AuthStateService] Excepción: {ex.Message}");
            return false;
        }
    }

    public async Task SetTokenAsync(string token, string username, string role, DateTime expiresAt)
    {
        Token = token;
        Username = username;
        Role = role;

        try
        {
            Console.WriteLine($"💾 [SetTokenAsync] Guardando token...");
            
            await _js.InvokeVoidAsync("localStorage.setItem", "auth_token", token);
            await _js.InvokeVoidAsync("localStorage.setItem", "auth_username", username);
            await _js.InvokeVoidAsync("localStorage.setItem", "auth_role", role);

            Console.WriteLine($"✅ [SetTokenAsync] Token guardado en localStorage");

            // ✅ Registrar dotnet ref en JS para poder llamar ResetInactivity
            _dotNetRef = DotNetObjectReference.Create(this);
            try
            {
                await _js.InvokeVoidAsync("sessionManager.registerDotNet", _dotNetRef);
                Console.WriteLine($"✅ [SetTokenAsync] DotNet ref registrado en sessionManager");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ [SetTokenAsync] Error registrando DotNet ref: {ex.Message}");
            }

            // ✅ HABILITAR LISTENERS DE ACTIVIDAD Y TIMERS
            await StartActivityListeners();
            StartInactivityWatcher();
        }
        catch (JSDisconnectedException ex)
        {
            Console.WriteLine($"⚠️ [SetTokenAsync] Circuito cerrado: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ [SetTokenAsync] Error: {ex.Message}");
            throw;
        }
    }

    public async Task LoadFromStorageAsync()
    {
        try
        {
            Token = await _js.InvokeAsync<string?>("localStorage.getItem", "auth_token");
            Username = await _js.InvokeAsync<string?>("localStorage.getItem", "auth_username");
            Role = await _js.InvokeAsync<string?>("localStorage.getItem", "auth_role");
            
            Console.WriteLine($"📂 [LoadFromStorageAsync] Cargado desde localStorage:");
            Console.WriteLine($"   Token existe: {!string.IsNullOrEmpty(Token)}");
            Console.WriteLine($"   Username: {Username}");
            Console.WriteLine($"   Role: {Role}");

            if (!string.IsNullOrEmpty(Token))
            {
                // ✅ Registrar DotNet ref para actividad
                _dotNetRef = DotNetObjectReference.Create(this);
                try
                {
                    await _js.InvokeVoidAsync("sessionManager.registerDotNet", _dotNetRef);
                    Console.WriteLine($"✅ [LoadFromStorageAsync] DotNet ref registrado en sessionManager");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ [LoadFromStorageAsync] Error registrando DotNet ref: {ex.Message}");
                }

                // ✅ REINICIAR LISTENERS Y TIMERS
                await StartActivityListeners();
                StartInactivityWatcher();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ [LoadFromStorageAsync] Error: {ex.Message}");
        }
    }

    public async Task LogoutAsync()
    {
        Console.WriteLine($"🚪 [LogoutAsync] Iniciando logout...");
        Token = null;
        Username = null;
        Role = null;
        _warningShown = false;
        StopInactivityWatcher();
        
        try
        {
            await _js.InvokeVoidAsync("localStorage.removeItem", "auth_token");
            await _js.InvokeVoidAsync("localStorage.removeItem", "auth_username");
            await _js.InvokeVoidAsync("localStorage.removeItem", "auth_role");
            await _js.InvokeVoidAsync("sessionManager.stopListening");
        }
        catch (TaskCanceledException ex)
        {
            Console.WriteLine($"⚠️ [LogoutAsync] Operación cancelada (circuito cerrado): {ex.Message}");
        }
        catch (JSDisconnectedException ex)
        {
            Console.WriteLine($"⚠️ [LogoutAsync] Circuito desconectado: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ [LogoutAsync] Error al limpiar: {ex.Message}");
        }
        finally
        {
            try
            {
                _nav.NavigateTo("/login", true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ [LogoutAsync] Error en navegación: {ex.Message}");
            }
        }
    }

    private async Task StartActivityListeners()
    {
        try
        {
            await _js.InvokeVoidAsync("sessionManager.startListening");
            Console.WriteLine($"📡 [StartActivityListeners] Listeners de actividad iniciados");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ [StartActivityListeners] Error: {ex.Message}");
        }
    }

    private void StartInactivityWatcher()
    {
        StopInactivityWatcher();
        _warningShown = false;

        try
        {
            // ⚠️ PRIMER TIMER: Mostrar advertencia a los 4 minutos
            _warningTimer = new System.Timers.Timer(WarningMs);
            _warningTimer.Elapsed += async (_, __) =>
            {
                Console.WriteLine($"⚠️ [WarningTimer] Se alcanzaron 4 minutos sin actividad");
                try
                {
                    if (!_warningShown)
                    {
                        _warningShown = true;
                        await _js.InvokeVoidAsync("sessionManager.showSessionWarning");
                        Console.WriteLine($"✅ [WarningTimer] Alerta mostrada");
                    }
                }
                catch (TaskCanceledException)
                {
                    Console.WriteLine($"⚠️ [WarningTimer] Operación cancelada");
                }
                catch (JSDisconnectedException)
                {
                    Console.WriteLine($"⚠️ [WarningTimer] Circuito cerrado");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ [WarningTimer] Error: {ex.Message}");
                }
            };
            _warningTimer.AutoReset = false;
            _warningTimer.Start();

            // ✅ SEGUNDO TIMER: Logout a los 5 minutos
            _inactivityTimer = new System.Timers.Timer(InactivityMs);
            _inactivityTimer.Elapsed += async (_, __) =>
            {
                Console.WriteLine($"⏰ [LogoutTimer] Se alcanzaron 5 minutos sin actividad - Logout");
                try
                {
                    await LogoutAsync();
                }
                catch (TaskCanceledException)
                {
                    Console.WriteLine($"⚠️ [LogoutTimer] Operación cancelada");
                }
                catch (JSDisconnectedException)
                {
                    Console.WriteLine($"⚠️ [LogoutTimer] Circuito cerrado");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ [LogoutTimer] Error: {ex.Message}");
                }
            };
            _inactivityTimer.AutoReset = false;
            _inactivityTimer.Start();

            Console.WriteLine($"⏰ [StartInactivityWatcher] Timers iniciados (alerta 4min, logout 5min)");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ [StartInactivityWatcher] Error: {ex.Message}");
        }
    }

    private void StopInactivityWatcher()
    {
        try
        {
            _warningTimer?.Stop();
            _warningTimer?.Dispose();
            _warningTimer = null;

            _inactivityTimer?.Stop();
            _inactivityTimer?.Dispose();
            _inactivityTimer = null;
            
            Console.WriteLine($"⏹️ [StopInactivityWatcher] Timers detenidos");
        }
        catch { }
    }

    [JSInvokable("ResetInactivity")]
    public void ResetInactivity()
    {
        Console.WriteLine($"🔄 [ResetInactivity] Usuario activo - Reiniciando timers");
        _warningShown = false;
        StartInactivityWatcher();
    }

    private class LoginResponse
    {
        public string? Token { get; set; }
        public string? Username { get; set; }
        public string? Role { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}