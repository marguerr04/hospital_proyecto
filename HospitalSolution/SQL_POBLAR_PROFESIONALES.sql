-- =============================================
-- Script para Poblar Tabla PROFESIONAL
-- Hospital Padre Hurtado
-- Fecha: 2025-01-22
-- =============================================

USE HospitalV4;
GO

-- Insertar 20 profesionales médicos realistas
INSERT INTO [dbo].[PROFESIONAL] 
    ([rut], [dv], [primerNombre], [segundoNombre], [primerApellido], [segundoApellido])
VALUES
    -- Cirujanos Generales
    ('11223344', '5', 'Andrés', 'Felipe', 'Moreno', 'Gutiérrez'),
    ('22334455', '6', 'María', 'José', 'González', 'Díaz'),
    ('33445566', '7', 'Francisco', 'Javier', 'Bravo', 'Santana'),
    
    -- Cirujanos Cardiovasculares
    ('44556677', '8', 'Carmen', 'Andrea', 'Fuentes', 'Pérez'),
    ('55667788', '9', 'Roberto', 'Carlos', 'Vargas', 'Silva'),
    
    -- Cirujanos Plásticos
    ('66778899', '0', 'Valentina', 'Paz', 'Campos', 'Núñez'),
    ('77889900', '1', 'Diego', 'Alejandro', 'Muñoz', 'Contreras'),
    
    -- Anestesiólogos
    ('88990011', '2', 'Claudia', 'Isabel', 'Rojas', 'Medina'),
    ('99001122', '3', 'Fernando', 'Luis', 'Soto', 'Valdés'),
    ('10112233', '4', 'Patricia', 'Elena', 'Castro', 'Morales'),
    
    -- Cirujanos Traumatólogos
    ('11223445', '5', 'Sebastián', 'Ignacio', 'Herrera', 'Figueroa'),
    ('12334556', '6', 'Lorena', 'Francisca', 'Espinoza', 'Navarro'),
    
    -- Cirujanos Maxilofaciales
    ('13445667', '7', 'Ricardo', 'Antonio', 'Ortiz', 'Ramírez'),
    ('14556778', '8', 'Daniela', 'Constanza', 'Torres', 'Vega'),
    
    -- Cirujanos Torácicos
    ('15667889', '9', 'Juan', 'Pablo', 'Aravena', 'Lagos'),
    ('16778990', '0', 'Marcela', 'Beatriz', 'Pinto', 'Olivares'),
    
    -- Cirujanos Pediátricos
    ('17889001', '1', 'Ignacio', 'Tomás', 'Reyes', 'Sandoval'),
    ('18990112', '2', 'Carolina', 'Soledad', 'Mendoza', 'Cruz'),
    
    -- Cirujanos Oncológicos
    ('19001223', '3', 'Rodrigo', 'Enrique', 'Sepúlveda', 'Paredes'),
    ('20112334', '4', 'Alejandra', 'Pilar', 'Cortés', 'Maldonado');

GO

-- Verificar la inserción
SELECT 
    Id,
    rut + '-' + dv AS RUT,
    primerNombre + ' ' + ISNULL(segundoNombre + ' ', '') + primerApellido + ' ' + ISNULL(segundoApellido, '') AS NombreCompleto
FROM [dbo].[PROFESIONAL]
ORDER BY Id DESC;

GO

PRINT '? Se insertaron 20 profesionales médicos exitosamente';
