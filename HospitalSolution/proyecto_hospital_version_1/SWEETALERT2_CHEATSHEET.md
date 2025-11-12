<!-- ============================================
     ?? CHEAT SHEET - SWEETALERT2 EN BLAZOR
     Guía rápida para usar alertas modernas
     ============================================ -->

# ?? Cómo Usar SweetAlert2 en Blazor - Cheat Sheet

## 1?? CONFIGURACIÓN INICIAL (Una sola vez por componente)

```csharp
@inject SweetAlertService SweetAlert
```

---

## 2?? ALERTAS BÁSICAS

### ? Éxito (Auto-cierra en 3 segundos)
```csharp
await SweetAlert.SuccessAsync("¡Guardado!", "Los datos se guardaron correctamente");
```

### ? Error
```csharp
await SweetAlert.ErrorAsync("Error", "No se pudo completar la operación");
```

### ?? Advertencia
```csharp
await SweetAlert.WarningAsync("Advertencia", "Debe completar todos los campos");
```

### ?? Información
```csharp
await SweetAlert.InfoAsync("Información", "Los datos fueron actualizados");
```

---

## 3?? ALERTA DE CONFIRMACIÓN

```csharp
var confirmar = await SweetAlert.ConfirmAsync(
    "¿Está seguro?", 
    "Esta acción eliminará los datos permanentemente",
    "Sí, eliminar",    // Texto botón confirmar (opcional)
    "Cancelar"           // Texto botón cancelar (opcional)
);

if (confirmar)
{
    // Usuario confirmó - Ejecutar acción
    await EliminarDatos();
    await SweetAlert.SuccessAsync("¡Eliminado!", "Los datos fueron eliminados");
}
else
{
    // Usuario canceló
    await SweetAlert.InfoAsync("Cancelado", "No se eliminó nada");
}
```

---

## 4?? ALERTA DE VALIDACIÓN (Múltiples errores)

```csharp
var errores = new List<string>();

if (string.IsNullOrEmpty(nombre))
    errores.Add("El campo 'Nombre' es requerido");

if (string.IsNullOrEmpty(email))
    errores.Add("El campo 'Email' es requerido");

if (edad < 18)
  errores.Add("Debe ser mayor de 18 años");

if (errores.Any())
{
    await SweetAlert.ValidationErrorAsync("Errores de validación", errores);
    return;
}
```

---

## 5?? ALERTA DE CARGA (Para operaciones largas)

```csharp
try
{
    // 1. Mostrar alerta de carga
    await SweetAlert.LoadingAsync("Guardando datos...", "Por favor espere");
    
    // 2. Ejecutar operación larga
    await Task.Delay(2000); // Simular operación
    await _service.GuardarAsync(datos);
    
    // 3. Cerrar alerta de carga
    await SweetAlert.CloseAsync();
    
    // 4. Mostrar resultado
    await SweetAlert.SuccessAsync("¡Completado!", "Datos guardados correctamente");
}
catch (Exception ex)
{
    // Cerrar carga en caso de error
    await SweetAlert.CloseAsync();

    // Mostrar error
    await SweetAlert.ErrorAsync("Error", ex.Message);
}
```

---

## 6?? TOAST NOTIFICATIONS (Pequeñas, en esquina)

```csharp
// Toast de éxito
await SweetAlert.SuccessToastAsync("Datos guardados");

// Toast de error
await SweetAlert.ErrorToastAsync("Error al guardar");
```

---

## 7?? EJEMPLOS COMPLETOS DE USO

### Ejemplo 1: Guardar Formulario con Validación

```csharp
private async Task GuardarFormulario()
{
    // Validar
    var errores = new List<string>();
    
    if (string.IsNullOrEmpty(_nombre))
        errores.Add("El nombre es requerido");
    
    if (string.IsNullOrEmpty(_email))
        errores.Add("El email es requerido");
    
    if (errores.Any())
    {
    await SweetAlert.ValidationErrorAsync("Complete los siguientes campos:", errores);
        return;
    }
    
    // Guardar con indicador de carga
    await SweetAlert.LoadingAsync("Guardando...", "Por favor espere");
    
    try
    {
        await _service.GuardarAsync(new Formulario 
 { 
    Nombre = _nombre, 
         Email = _email 
        });
        
     await SweetAlert.CloseAsync();
        await SweetAlert.SuccessAsync("¡Guardado!", "Los datos fueron guardados correctamente");
        
        // Limpiar formulario
        _nombre = "";
  _email = "";
    }
    catch (Exception ex)
    {
        await SweetAlert.CloseAsync();
        await SweetAlert.ErrorAsync("Error al guardar", ex.Message);
    }
}
```

### Ejemplo 2: Eliminar con Confirmación

```csharp
private async Task EliminarRegistro(int id)
{
    var confirmar = await SweetAlert.ConfirmAsync(
        "¿Eliminar registro?",
        "Esta acción no se puede deshacer",
        "Sí, eliminar",
        "Cancelar"
    );
    
    if (!confirmar)
  return;
    
    await SweetAlert.LoadingAsync("Eliminando...");
    
    try
 {
 await _service.EliminarAsync(id);
     await SweetAlert.CloseAsync();
      await SweetAlert.SuccessAsync("¡Eliminado!", "El registro fue eliminado");
        
        // Recargar lista
        await CargarDatos();
    }
    catch (Exception ex)
    {
        await SweetAlert.CloseAsync();
        await SweetAlert.ErrorAsync("Error", $"No se pudo eliminar: {ex.Message}");
    }
}
```

### Ejemplo 3: Proceso con Múltiples Pasos

```csharp
private async Task ProcesarMultipasoAsync()
{
    try
    {
        // Paso 1
        await SweetAlert.LoadingAsync("Paso 1 de 3", "Validando datos...");
     await Task.Delay(1000);
        var validacion = await ValidarDatos();
        
        if (!validacion.Exitoso)
        {
            await SweetAlert.CloseAsync();
       await SweetAlert.ErrorAsync("Error de validación", validacion.Mensaje);
       return;
      }
   
        // Paso 2
        await SweetAlert.LoadingAsync("Paso 2 de 3", "Guardando en base de datos...");
        await Task.Delay(1000);
     await GuardarEnBD();
        
        // Paso 3
        await SweetAlert.LoadingAsync("Paso 3 de 3", "Enviando notificaciones...");
        await Task.Delay(1000);
        await EnviarNotificaciones();
        
     // Completado
        await SweetAlert.CloseAsync();
        await SweetAlert.SuccessAsync("¡Proceso completado!", "Todos los pasos fueron ejecutados correctamente");
    }
    catch (Exception ex)
    {
        await SweetAlert.CloseAsync();
        await SweetAlert.ErrorAsync("Error en el proceso", ex.Message);
    }
}
```

---

## 8?? TIPS Y MEJORES PRÁCTICAS

### ? DO (Buenas prácticas)

```csharp
// ? Usa títulos cortos y descriptivos
await SweetAlert.SuccessAsync("¡Guardado!", "Los datos fueron guardados");

// ? Cierra alertas de carga en caso de error
try {
    await SweetAlert.LoadingAsync("Procesando...");
    await Procesar();
    await SweetAlert.CloseAsync();
} catch {
    await SweetAlert.CloseAsync(); // ? Importante!
    await SweetAlert.ErrorAsync("Error");
}

// ? Usa ValidationErrorAsync para múltiples errores
var errores = new List<string> { "Error 1", "Error 2" };
await SweetAlert.ValidationErrorAsync("Errores:", errores);

// ? Usa ConfirmAsync para acciones destructivas
if (await SweetAlert.ConfirmAsync("¿Eliminar?", "Acción irreversible"))
    await Eliminar();
```

### ? DON'T (Malas prácticas)

```csharp
// ? No uses múltiples SuccessAsync seguidos
await SweetAlert.SuccessAsync("Paso 1");
await SweetAlert.SuccessAsync("Paso 2"); // Se solapan

// ? No olvides cerrar LoadingAsync
await SweetAlert.LoadingAsync("Cargando...");
// ... olvidas cerrar con CloseAsync() ?

// ? No uses mensajes muy largos
await SweetAlert.ErrorAsync("Error", "Un mensaje de 500 caracteres..."); // ?

// ? No uses alert() nativo
await JsRuntime.InvokeVoidAsync("alert", "Mensaje"); // ? Obsoleto
```

---

## 9?? REFERENCIA RÁPIDA DE MÉTODOS

| Método | Cuándo usar | Auto-cierre |
|--------|-------------|-------------|
| `SuccessAsync()` | Operación exitosa | ? 3 seg |
| `ErrorAsync()` | Error o fallo | ? No |
| `WarningAsync()` | Advertencia o precaución | ? No |
| `InfoAsync()` | Información general | ? No |
| `ConfirmAsync()` | Confirmación de acción | ? No |
| `ValidationErrorAsync()` | Múltiples errores de validación | ? No |
| `LoadingAsync()` | Operación larga en progreso | ? No (usar `CloseAsync()`) |
| `CloseAsync()` | Cerrar alerta actual | - |
| `SuccessToastAsync()` | Notificación rápida de éxito | ? 3 seg |
| `ErrorToastAsync()` | Notificación rápida de error | ? 3 seg |

---

## ?? PÁGINA DE DEMOSTRACIÓN

Para ver todos los tipos de alertas en acción, navega a:

```
/ejemplo-sweetalert
```

---

## ?? RECURSOS ADICIONALES

- **Guía completa:** `GUIA_MIGRACION_SWEETALERT2.md`
- **Resumen de migración:** `MIGRACION_SWEETALERT2_COMPLETADA.md`
- **Componente de ejemplo:** `Components/Pages/EjemploSweetAlert.razor`
- **Documentación oficial:** https://sweetalert2.github.io/

---

## ?? SOLUCIÓN RÁPIDA DE PROBLEMAS

| Problema | Solución |
|----------|----------|
| "SweetAlert is not defined" | Verifica que `App.razor` tenga `<script src="js/sweetAlertInterop.js"></script>` |
| Servicio no disponible | Verifica `Program.cs`: `builder.Services.AddScoped<SweetAlertService>();` |
| Alertas no se ven | Verifica CSS en `App.razor`: `<link href="...sweetalert2.min.css"...>` |
| Alerta de carga no se cierra | Asegúrate de llamar `await SweetAlert.CloseAsync();` |

---

**¡Listo para usar!** ??

Copia y pega los ejemplos según lo necesites.
