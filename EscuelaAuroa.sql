-- Crear la base de datos
CREATE DATABASE EscuelaAurora;
GO

USE EscuelaAurora;
GO

-- Tabla Usuarios
CREATE TABLE Usuarios (
    IdUsuario INT PRIMARY KEY IDENTITY(1,1),
    Correo VARCHAR(100) NOT NULL,
    Contrasenna VARCHAR(100) NOT NULL,
    Estado VARCHAR(20) NOT NULL
);
GO

-- Tabla Roles
CREATE TABLE Roles (
    IdRol INT PRIMARY KEY IDENTITY(1,1),
    IdUsuario INT NOT NULL,
    NombreRol VARCHAR(50) NOT NULL,
    FOREIGN KEY (IdUsuario) REFERENCES Usuarios(IdUsuario)
);
GO

-- Tabla Estudiantes
CREATE TABLE Estudiantes (
    IdEstudiante INT PRIMARY KEY IDENTITY(1,1),
    Nombre VARCHAR(100),
    Correo VARCHAR(100),
    Telefono VARCHAR(20),
    Direccion VARCHAR(200)
);
GO

-- Tabla Profesores
CREATE TABLE Profesores (
    IdProfesor INT PRIMARY KEY IDENTITY(1,1),
    Nombre VARCHAR(100),
    Telefono VARCHAR(20),
    Especialidad VARCHAR(100)
);
GO

-- Tabla Cursos
CREATE TABLE Cursos (
    IdCurso INT PRIMARY KEY IDENTITY(1,1),
    IdProfesor INT NOT NULL,
    NombreCurso VARCHAR(100),
    Descripcion TEXT,
    FOREIGN KEY (IdProfesor) REFERENCES Profesores(IdProfesor)
);
GO

-- Tabla Eventos
CREATE TABLE Eventos (
    IdEvento INT PRIMARY KEY IDENTITY(1,1),
    IdCurso INT NOT NULL,
    Titulo VARCHAR(100),
    Fecha DATE,
    Descripcion TEXT,
    FOREIGN KEY (IdCurso) REFERENCES Cursos(IdCurso)
);
GO

-- Tabla Matriculas
CREATE TABLE Matriculas (
    IdMatricula INT PRIMARY KEY IDENTITY(1,1),
    IdEstudiante INT NOT NULL,
    IdCurso INT NOT NULL,
    FechaMatricula DATE,
    Estado VARCHAR(20),
    FOREIGN KEY (IdEstudiante) REFERENCES Estudiantes(IdEstudiante),
    FOREIGN KEY (IdCurso) REFERENCES Cursos(IdCurso)
);
GO

-- Tabla Asistencias
CREATE TABLE Asistencias (
    IdAsistencia INT PRIMARY KEY IDENTITY(1,1),
    IdMatricula INT NOT NULL,
    Fecha DATE,
    Estado VARCHAR(20),
    FOREIGN KEY (IdMatricula) REFERENCES Matriculas(IdMatricula)
);
GO

-- Tabla Calificaciones
CREATE TABLE Calificaciones (
    IdCalificacion INT PRIMARY KEY IDENTITY(1,1),
    IdMatricula INT NOT NULL,
    Nota DECIMAL(5,2),
    FechaRegistro DATE,
    FOREIGN KEY (IdMatricula) REFERENCES Matriculas(IdMatricula)
);
GO

-- Tabla Notificaciones
CREATE TABLE Notificaciones (
    IdNotificacion INT PRIMARY KEY IDENTITY(1,1),
    IdUsuario INT NOT NULL,
    Mensaje TEXT,
    FechaEnvio DATE,
    FOREIGN KEY (IdUsuario) REFERENCES Usuarios(IdUsuario)
);
GO

-- Stored Procedure: Login de usuario
CREATE PROCEDURE spIniciarSesionUsuario
    @Correo VARCHAR(100),
    @Contrasenna VARCHAR(100)
AS
BEGIN
    SELECT IdUsuario, Correo, Estado
    FROM Usuarios
    WHERE Correo = @Correo
      AND Contrasenna = @Contrasenna
      AND Estado = 'Activo';
END
GO

-- Stored Procedure: Insertar estudiante
CREATE PROCEDURE spInsertarEstudiante
    @Nombre VARCHAR(100),
    @Correo VARCHAR(100),
    @Telefono VARCHAR(20),
    @Direccion VARCHAR(200)
AS
BEGIN
    INSERT INTO Estudiantes (Nombre, Correo, Telefono, Direccion)
    VALUES (@Nombre, @Correo, @Telefono, @Direccion);
END
GO

-- Stored Procedure: Insertar curso
CREATE PROCEDURE spInsertarCurso
    @IdProfesor INT,
    @NombreCurso VARCHAR(100),
    @Descripcion TEXT
AS
BEGIN
    INSERT INTO Cursos (IdProfesor, NombreCurso, Descripcion)
    VALUES (@IdProfesor, @NombreCurso, @Descripcion);
END
GO

-- Inserts de prueba
INSERT INTO Usuarios (Correo, Contrasenna, Estado)
VALUES ('admin@escuela.com', '1234', 'Activo'),
       ('profesor@escuela.com', 'abcd', 'Activo');
GO

INSERT INTO Roles (IdUsuario, NombreRol)
VALUES (1, 'Administrador'),
       (2, 'Profesor');
GO

INSERT INTO Profesores (Nombre, Telefono, Especialidad)
VALUES ('Carlos Pérez', '8888-8888', 'Matemáticas');
GO

INSERT INTO Estudiantes (Nombre, Correo, Telefono, Direccion)
VALUES ('Ana Gómez', 'ana@correo.com', '7777-7777', 'Cartago');
GO

INSERT INTO Cursos (IdProfesor, NombreCurso, Descripcion)
VALUES (1, 'Álgebra I', 'Curso básico de álgebra');
GO
