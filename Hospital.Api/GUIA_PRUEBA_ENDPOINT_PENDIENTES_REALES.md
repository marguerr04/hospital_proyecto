# ?? GUÍA DE PRUEBA - ENDPOINT NUEVO

## ? IMPLEMENTACIÓN COMPLETADA

Se agregó el **nuevo endpoint** al API sin romper funcionalidades:

```
GET /api/Solicitud/pendientes-reales
```

---

## ?? QUÉ HACE

Obtiene todas las solicitudes quirúrgicas con sus datos de paciente correctamente relacionados a través de CONSENTIMIENTO_INFORMADO:

```
SOLICITUD_QUIRURGICA 
    ? CONSENTIMIENTO_INFORMADO_id 
    ? CONSENTIMIENTO_INFORMADO 
    ? PACIENTE_id 
    ? PACIENTE (nombre, RUT)
```

---

## ?? PASO 1: PROBAR EN SWAGGER

1. **Inicia la API**
   - Ejecuta `Hospital.Api`
   - Espera a que se inicie

2. **Abre Swagger**
   - Navega a `https://localhost:7032/swagger`

3. **Busca el endpoint**
   - Sección: `Solicitud`
   - Endpoint: `GET /api/Solicitud/pendientes-reales`

4. **Presiona "Try it out"**
   - Luego "Execute"

5. **Deberías ver un resultado como:**

```json
[
  {
    "id": 1,
    "nombrePaciente": "Juan Pérez",
    "rut": "12345678-9",
    "diagnostico": "Fractura de cadera",
    "procedimiento": "",
    "estado": "GES",
    "fechaCreacion": "2025-11-25T13:45:00",
    "fechaProgramada": null,
    "diasRestantes": null,
    "contactabilidad": "Por Contactar",
    "prioridad": 1
  }
]
```

---

## ?? PASO 2: PROBAR EN SQL PRIMERO

Si no ves datos en Swagger, verifica que haya datos en BD:

```sql
-- Ver solicitudes con pacientes
SELECT 
    sq.idSolicitud AS Id,
    p.primerNombre + ' ' + p.apellidoPaterno AS NombrePaciente,
    p.rut + '-' + p.dv AS Rut,
    d.nombre AS Diagnostico,
    sq.validacionGES AS EsGes,
    sq.fechaCreacion AS FechaCreacion
FROM SOLICITUD_QUIRURGICA sq
INNER JOIN CONSENTIMIENTO_INFORMADO ci ON sq.CONSENTIMIENTO_INFORMADO_id = ci.id
INNER JOIN PACIENTE p ON ci.PACIENTE_id = p.id
INNER JOIN DIAGNOSTICO d ON sq.DIAGNOSTICO_id = d.id
WHERE sq.validacionGES IS NOT NULL
ORDER BY sq.fechaCreacion DESC;
```

**Si NO hay datos:**
- Necesitas crear consentimientos + solicitudes

---

## ?? DATOS QUE RETORNA

| Campo | Fuente | Descripción |
|-------|--------|------------|
| `id` | `sq.idSolicitud` | ID de solicitud |
| `nombrePaciente` | `p.primerNombre + p.apellidoPaterno` | Nombre del paciente |
| `rut` | `p.rut + p.dv` | RUT formateado |
| `diagnostico` | `d.nombre` | Diagnóstico |
| `estado` | `sq.validacionGES` | "GES" o "NO GES" |
| `fechaCreacion` | `sq.fechaCreacion` | Fecha de creación |
| `prioridad` | `sq.validacionGES` | 1 si GES, 0 si NO GES |

---

## ? VERIFICACIÓN DE NO RUPTURA

Se verificó que **NO se rompió nada**:

- ? Compilación exitosa
- ? Método agregado sin modificar existentes
- ? Endpoint nuevo sin conflictos
- ? Estructura de datos compatible

---

## ?? PRÓXIMO PASO

Una vez confirmado que el endpoint funciona en Swagger:
1. Integrar en Blazor
2. Crear componente `TablaActualizacionesSolicitudes.razor`
3. Agregar dropdowns dinámicos por estado

