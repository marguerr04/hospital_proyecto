# Hospital Management System - Documentación

**Versión**: 1.0.0  
**Actualizado**: Noviembre 2025  
**Tecnologías**: .NET 8, Blazor Server, SQL Server  
**Equipo**: Hospital Management Team

Links: 

Base de datos: https://drive.google.com/drive/folders/1WhhBhWMPAcMCnQ0uzjMOeLzcqTOprjSa?usp=sharing
Github: https://github.com/marguerr04/hospital_proyecto



---

## 1. Descripción General

Hospital Management System digitaliza el flujo quirúrgico: gestión de pacientes, solicitudes de procedimientos, priorización automática y análisis de datos. Desarrollado en .NET 8 con arquitectura limpia, enfocado en escalabilidad y mantenibilidad.

### Stack Tecnológico
- Frontend: Blazor Server + MudBlazor + Chart.js + SweetAlert2
- Backend: ASP.NET Core REST API
- Base de Datos: SQL Server 2019+
- ORM: Entity Framework Core 8

---

## 2. Conceptos Clave de la Arquitectura

### Entities (Modelos de BD)
Representan las tablas en la base de datos. Se definen en `Hospital.Api/Data/Entities/`. Cada Entity tiene propiedades que mapean a columnas de BD.

### DTOs (Data Transfer Objects)
Son modelos ligeros para transferir datos entre capas. Se usan en peticiones HTTP en lugar de Entities para:
- Proteger la estructura interna de BD
- Transferir solo datos necesarios
- Validar entrada de usuario

Se definen en `Hospital.Api/DTOs/`.


### Services (Lógica de Negocio)
Contienen la lógica de aplicación, validaciones y consultas a BD. Se definen en `Hospital.Api/Data/Services/`.

Responsabilidades:
- Validar datos
- Ejecutar transacciones
- Mapear Entities a DTOs
- Consultar base de datos



### Controllers (Endpoints REST)
Reciben peticiones HTTP, llaman a Services y retornan respuestas. Se definen en `Hospital.Api/Controllers/`.

Responsabilidades:
- Validar entrada (parámetros, headers)
- Llamar a Service
- Retornar DTOs en JSON
- Manejar códigos de estado HTTP



### ApiServices (Frontend)
Consumidores de API REST. Se definen en `proyecto_hospital_version_1/Services/`.

Responsabilidades:
- Hacer peticiones HTTP al backend
- Manejar deserialization de JSON
- Retornar DTOs al componente



### Components (Blazor)
Componentes visuales que renderean UI. Se definen en `proyecto_hospital_version_1/Components/`.

Responsabilidades:
- Renderizar interfaz
- Capturar entrada de usuario
- Llamar a ApiServices
- Actualizar estado local



---

## 3. Funcionalidades Principales

Gestión de Pacientes: Búsqueda por RUT, ficha clínica, historial y alertas.

Solicitudes Quirúrgicas: Flujo guiado en 3 pasos, consentimiento PDF automático, seguimiento del estado.

Priorización: Criterios GES/PRAIS/SENAME/logística, cálculo automático (1, 2, 3), historial.

Dashboard: Percentil 75, análisis de egresos, contactabilidad, gráficos interactivos.

---

## 4. Requisitos del Sistema

Software: Visual Studio 2022, .NET 8 SDK, SQL Server 2019 Express+

---

## 5. Estructura del Proyecto

```
HospitalSolution/
├── Hospital.Api/
│   ├── Controllers/
│   │   ├── PacienteController.cs
│   │   ├── SolicitudController.cs
│   │   ├── PriorizacionController.cs
│   │   └── ...
│   ├── Data/
│   │   ├── HospitalDbContext.cs
│   │   ├── Entities/
│   │   │   ├── Paciente.cs
│   │   │   ├── SolicitudQuirurgicaReal.cs
│   │   │   └── ...
│   │   └── Services/
│   │       ├── ISolicitudQuirurgicaService.cs
│   │       ├── SolicitudQuirurgicaRealService.cs
│   │       └── ...
│   ├── DTOs/
│   │   ├── PacienteDto.cs
│   │   ├── SolicitudMedicoDto.cs
│   │   └── ...
│   └── Program.cs
│
└── proyecto_hospital_version_1/
    ├── Components/
    │   ├── Pages/
    │   │   ├── Dashboard.razor
    │   │   ├── PriorizarSolicitud.razor
    │   │   └── ...
    │   ├── Shared/
    │   │   ├── BuscadorPaciente.razor
    │   │   ├── InfoPacienteCard.razor
    │   │   └── ...
    │   └── Layout/
    ├── Services/
    │   ├── PacienteApiService.cs
    │   ├── SolicitudQuirurgicaApiService.cs
    │   └── ...
    └── Program.cs
```

---

## 6. Arquitectura 

```
Capa 1: front (Blazor Components)
        ↓
Capa 2: Servicios Cliente (ApiServices)
        ↓
Capa 3: API REST (Controllers)
        ↓
Capa 4: Lógica de Negocio (Services)
        ↓
Capa 5: Datos (EF Core + SQL Server)
```

# Credenciales para ingresar 
medico : 

nombre: medico1

contraseña: medico

rol: medico




administrador:


nombre: admin

contraseña: admin

rol: adminitrador



---
