-- ============================================
-- ELIMINAR BASE DE DATOS SI EXISTE
-- ============================================
USE master;
GO

DROP DATABASE EscuelaAurora;

GO

-- ============================================
-- CREAR BASE DE DATOS
-- ============================================
CREATE DATABASE EscuelaAurora;
GO

USE EscuelaAurora;
GO

-- ============================================
-- TABLA ROLES
-- ============================================
CREATE TABLE Roles (
    id_rol INT IDENTITY(1,1) PRIMARY KEY,
    nomb_rol VARCHAR(50) NOT NULL,
    estado BIT NOT NULL DEFAULT 1
);
GO

-- ============================================
-- TABLA USUARIOS
-- ============================================
CREATE TABLE Usuarios (
    id_usuario INT IDENTITY(1,1) PRIMARY KEY,
    id_rol INT NOT NULL,
    correo VARCHAR(150) NOT NULL UNIQUE,
    contraseña VARCHAR(255) NOT NULL,
    estado BIT NOT NULL DEFAULT 1,
    fecha_registro DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (id_rol) REFERENCES Roles(id_rol)
);
GO

-- ============================================
-- TABLA ESTUDIANTES
-- ============================================
CREATE TABLE Estudiantes (
    id_estudiante INT IDENTITY(1,1) PRIMARY KEY,
    id_usuario INT NOT NULL,
    nomb VARCHAR(100) NOT NULL,
    primer_apellido VARCHAR(30) NOT NULL,
    segundo_apellido VARCHAR(30) NULL,
    identificacion VARCHAR(20) NOT NULL UNIQUE,
    correo VARCHAR(150) NOT NULL UNIQUE,
    telefono VARCHAR(20) NULL,
    direccion VARCHAR(250) NULL,
    estado BIT NOT NULL DEFAULT 1,
    fecha_registro DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (id_usuario) REFERENCES Usuarios(id_usuario)
);
GO

-- ============================================
-- TABLA PROFESORES
-- ============================================
CREATE TABLE Profesores (
    id_profesor INT IDENTITY(1,1) PRIMARY KEY,
    id_usuario INT NOT NULL,
    nomb VARCHAR(100) NOT NULL,
    primer_apellido VARCHAR(30) NOT NULL,
    segundo_apellido VARCHAR(30) NULL,
    identificacion VARCHAR(20) NOT NULL UNIQUE,
    correo VARCHAR(150) NOT NULL UNIQUE,
    telefono VARCHAR(20) NULL,
    especialidad VARCHAR(100) NULL,
    estado BIT NOT NULL DEFAULT 1,
    fecha_registro DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (id_usuario) REFERENCES Usuarios(id_usuario)
);
GO

-- ============================================
-- TABLA ERRORES
-- ============================================
CREATE TABLE Errores (
    id_error INT IDENTITY(1,1) PRIMARY KEY,
    id_usuario INT NULL,
    fecha_error DATETIME NOT NULL DEFAULT GETDATE(),
    mensaje_error VARCHAR(MAX) NOT NULL,
    lugar VARCHAR(250) NOT NULL,
    stack_trace VARCHAR(MAX) NULL,
    FOREIGN KEY (id_usuario) REFERENCES Usuarios(id_usuario)
);
GO

-- ============================================
-- INSERTAR ROLES
-- ============================================
INSERT INTO Roles (nomb_rol, estado) VALUES ('Administrador', 1);
INSERT INTO Roles (nomb_rol, estado) VALUES ('Usuario', 1);
INSERT INTO Roles (nomb_rol, estado) VALUES ('Profesor', 1);
INSERT INTO Roles (nomb_rol, estado) VALUES ('Estudiante', 1);
GO

-- ============================================
-- INSERTAR USUARIO ADMIN (contraseña: 123456)
-- ============================================
INSERT INTO Usuarios (id_rol, correo, contraseña, estado)
VALUES (1, 'admin@escuela.com', '$2a$11$TpEvbCK8sV/HYp6l7Fqk/uR2ZNWlB3qMWsTrXOk7pFpSgUqXL5cTq', 1);
GO

INSERT INTO Estudiantes (
    id_usuario, 
    nomb, 
    primer_apellido, 
    segundo_apellido, 
    identificacion, 
    correo, 
    telefono, 
    direccion
)
VALUES (
    SCOPE_IDENTITY(), 
    'Admin', 
    'Sistema', 
    'Escuela', 
    'ADMIN001', 
    'admin@escuela.com', 
    '8888-8888', 
    'San José'
);
GO

-- ============================================
-- INSERTAR USUARIO TEST (contraseña: 123456)
-- ============================================
INSERT INTO Usuarios (id_rol, correo, contraseña, estado)
VALUES (4, 'test@escuela.com', '$2a$11$TpEvbCK8sV/HYp6l7Fqk/uR2ZNWlB3qMWsTrXOk7pFpSgUqXL5cTq', 1);
GO

INSERT INTO Estudiantes (
    id_usuario, 
    nomb, 
    primer_apellido, 
    segundo_apellido, 
    identificacion, 
    correo, 
    telefono, 
    direccion
)
VALUES (
    SCOPE_IDENTITY(), 
    'Usuario', 
    'Prueba', 
    'Test', 
    'TEST001', 
    'test@escuela.com', 
    '7777-7777', 
    'Cartago'
);
GO

-- ============================================
-- PROCEDIMIENTOS ALMACENADOS
-- ============================================

-- SP: Iniciar Sesión
CREATE PROCEDURE spIniciarSesionUsuario
    @correo VARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        u.id_usuario AS IdUsuario,
        u.correo AS Correo,
        u.contraseña AS Contrasenna,
        u.estado AS Estado,
        u.id_rol AS IdRol,
        r.nomb_rol AS NombreRol,
        ISNULL(e.nomb, p.nomb) AS Nombre,
        ISNULL(e.primer_apellido, p.primer_apellido) AS PrimerApellido,
        ISNULL(e.identificacion, p.identificacion) AS Identificacion,
        CASE 
            WHEN e.id_estudiante IS NOT NULL THEN 'Estudiante'
            WHEN p.id_profesor IS NOT NULL THEN 'Profesor'
            ELSE 'Usuario'
        END AS TipoUsuario
    FROM Usuarios u
    INNER JOIN Roles r ON u.id_rol = r.id_rol
    LEFT JOIN Estudiantes e ON u.id_usuario = e.id_usuario AND e.estado = 1
    LEFT JOIN Profesores p ON u.id_usuario = p.id_usuario AND p.estado = 1
    WHERE u.correo = @correo
      AND u.estado = 1;
END
GO

-- SP: Registrar Error
CREATE PROCEDURE spRegistrarError
    @mensaje_error VARCHAR(MAX),
    @lugar VARCHAR(250),
    @stack_trace VARCHAR(MAX),
    @id_usuario INT
AS
BEGIN
    INSERT INTO Errores (id_usuario, mensaje_error, lugar, stack_trace)
    VALUES (@id_usuario, @mensaje_error, @lugar, @stack_trace);
END
GO

-- SP: Registrar Usuario
CREATE PROCEDURE spRegistrarUsuario
    @correo VARCHAR(150),
    @contraseña VARCHAR(255),
    @id_rol INT
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE correo = @correo)
    BEGIN
        INSERT INTO Usuarios (id_rol, correo, contraseña, estado)
        VALUES (@id_rol, @correo, @contraseña, 1);
        
        SELECT SCOPE_IDENTITY() AS IdUsuario;
    END
    ELSE
    BEGIN
        SELECT -1 AS IdUsuario;
    END
END
GO

-- SP: Registrar Estudiante
CREATE PROCEDURE spRegistrarEstudiante
    @id_usuario INT,
    @nomb VARCHAR(100),
    @primer_apellido VARCHAR(30),
    @segundo_apellido VARCHAR(30),
    @identificacion VARCHAR(20),
    @correo VARCHAR(150),
    @telefono VARCHAR(20),
    @direccion VARCHAR(250)
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Estudiantes WHERE identificacion = @identificacion OR correo = @correo)
    BEGIN
        INSERT INTO Estudiantes (
            id_usuario, nomb, primer_apellido, segundo_apellido,
            identificacion, correo, telefono, direccion, estado
        ) VALUES (
            @id_usuario, @nomb, @primer_apellido, @segundo_apellido,
            @identificacion, @correo, @telefono, @direccion, 1
        );
        
        SELECT SCOPE_IDENTITY() AS IdEstudiante;
    END
    ELSE
    BEGIN
        SELECT -1 AS IdEstudiante;
    END
END
GO

-- ============================================
-- VERIFICAR DATOS (SIN ALIAS VACIOS)
-- ============================================
PRINT '=== USUARIOS ===';
SELECT id_usuario, id_rol, correo, estado FROM Usuarios;
GO

PRINT '=== ESTUDIANTES ===';
SELECT id_estudiante, id_usuario, nomb, primer_apellido, correo FROM Estudiantes;
GO

PRINT '=== PROBAR SP ===';
EXEC spIniciarSesionUsuario 'admin@escuela.com';
GO

USE EscuelaAurora;
GO

-- Actualizar admin con el hash CORRECTO
UPDATE Usuarios 
SET contraseña = '$2a$11$dUmKzo753u0eXVTsXhJx.ee7VSPco6n.EPyEtKtuxig6.ayZwknzK'
WHERE correo = 'admin@escuela.com';
GO

-- Actualizar test con el hash CORRECTO
UPDATE Usuarios 
SET contraseña = '$2a$11$dUmKzo753u0eXVTsXhJx.ee7VSPco6n.EPyEtKtuxig6.ayZwknzK'
WHERE correo = 'test@escuela.com';
GO

-- Verificar que la longitud sea 60
SELECT correo, LEN(contraseña) AS LongitudHash FROM Usuarios;
GO

-- Probar el SP
EXEC spIniciarSesionUsuario 'admin@escuela.com';
GO

USE EscuelaAurora;
GO

-- Ver el hash actual
SELECT correo, contraseña, LEN(contraseña) AS Longitud 
FROM Usuarios 
WHERE correo = 'admin@escuela.com';
GO