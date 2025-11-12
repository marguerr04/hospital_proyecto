// En proyecto_hospital_version_1/Services/IConsentimientoInformadoService.cs
using ApiEntities = Hospital.Api.Data.Entities;
public interface IConsentimientoInformadoService
{
    // AHORA ESPERA ApiEntities.ConsentimientoInformadoReal
    Task<int> CrearConsentimientoAsync(ApiEntities.ConsentimientoInformadoReal consentimiento);
}