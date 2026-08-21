USE EscuelaAurora;
GO

-- ============================================
-- SP: REGISTRAR USUARIO Y ESTUDIANTE
-- ============================================
CREATE OR ALTER PROCEDURE spRegistrarUsuarioEstudiante
    @nomb VARCHAR(100),
    @primer_apellido VARCHAR(30),
    @segundo_apellido VARCHAR(30) = NULL,
    @identificacion VARCHAR(20),
    @correo VARCHAR(150),
    @telefono VARCHAR(20) = NULL,
    @direccion VARCHAR(250) = NULL,
    @contrasenna VARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Validar si el correo ya está registrado como usuario
        IF EXISTS (
            SELECT 1
            FROM Usuarios
            WHERE correo = @correo
        )
        BEGIN
            ROLLBACK TRANSACTION;

            SELECT
                0 AS Exitoso,
                'El correo electrónico ya se encuentra registrado.' AS Mensaje,
                0 AS IdUsuario,
                0 AS IdEstudiante;

            RETURN;
        END;

        -- Validar identificación o correo repetido en estudiantes
        IF EXISTS (
            SELECT 1
            FROM Estudiantes
            WHERE identificacion = @identificacion
               OR correo = @correo
        )
        BEGIN
            ROLLBACK TRANSACTION;

            SELECT
                0 AS Exitoso,
                'La identificación o el correo ya se encuentra registrado.' AS Mensaje,
                0 AS IdUsuario,
                0 AS IdEstudiante;

            RETURN;
        END;

        -- Registrar usuario con rol Estudiante
        INSERT INTO Usuarios (
            id_rol,
            correo,
            contraseña,
            estado
        )
        VALUES (
            4,
            @correo,
            @contrasenna,
            1
        );

        DECLARE @id_usuario INT;

        SET @id_usuario = SCOPE_IDENTITY();

        -- Registrar estudiante
        INSERT INTO Estudiantes (
            id_usuario,
            nomb,
            primer_apellido,
            segundo_apellido,
            identificacion,
            correo,
            telefono,
            direccion,
            estado
        )
        VALUES (
            @id_usuario,
            @nomb,
            @primer_apellido,
            NULLIF(@segundo_apellido, ''),
            @identificacion,
            @correo,
            NULLIF(@telefono, ''),
            NULLIF(@direccion, ''),
            1
        );

        DECLARE @id_estudiante INT;

        SET @id_estudiante = SCOPE_IDENTITY();

        COMMIT TRANSACTION;

        SELECT
            1 AS Exitoso,
            'El estudiante se registró correctamente.' AS Mensaje,
            @id_usuario AS IdUsuario,
            @id_estudiante AS IdEstudiante;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE spListarEstudiantes
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        id_estudiante
            AS IdEstudiante,

        id_usuario
            AS IdUsuario,

        nomb
            AS Nombre,

        primer_apellido
            AS PrimerApellido,

        segundo_apellido
            AS SegundoApellido,

        identificacion
            AS Identificacion,

        correo
            AS Correo,

        telefono
            AS Telefono,

        direccion
            AS Direccion,

        estado
            AS Estado,

        fecha_registro
            AS FechaRegistro

    FROM Estudiantes

    ORDER BY
        nomb,
        primer_apellido;
END;
GO


CREATE OR ALTER PROCEDURE spConsultarEstudiante
    @id_estudiante INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        id_estudiante
            AS IdEstudiante,

        id_usuario
            AS IdUsuario,

        nomb
            AS Nombre,

        primer_apellido
            AS PrimerApellido,

        segundo_apellido
            AS SegundoApellido,

        identificacion
            AS Identificacion,

        correo
            AS Correo,

        telefono
            AS Telefono,

        direccion
            AS Direccion,

        estado
            AS Estado,

        fecha_registro
            AS FechaRegistro

    FROM Estudiantes

    WHERE id_estudiante =
        @id_estudiante;
END;
GO


CREATE OR ALTER PROCEDURE spActualizarEstudiante
    @id_estudiante INT,
    @nomb VARCHAR(100),
    @primer_apellido VARCHAR(30),
    @segundo_apellido VARCHAR(30) = NULL,
    @identificacion VARCHAR(20),
    @correo VARCHAR(150),
    @telefono VARCHAR(20),
    @direccion VARCHAR(250)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS
    (
        SELECT 1
        FROM Estudiantes
        WHERE
            (
                identificacion =
                    @identificacion

                OR correo =
                    @correo
            )
            AND id_estudiante <>
                @id_estudiante
    )
    BEGIN
        SELECT 0;
        RETURN;
    END;

    UPDATE Estudiantes

    SET
        nomb =
            @nomb,

        primer_apellido =
            @primer_apellido,

        segundo_apellido =
            @segundo_apellido,

        identificacion =
            @identificacion,

        correo =
            @correo,

        telefono =
            @telefono,

        direccion =
            @direccion

    WHERE id_estudiante =
        @id_estudiante;

    DECLARE @filasAfectadas INT =
        @@ROWCOUNT;

    IF @filasAfectadas > 0
    BEGIN
        UPDATE Usuarios

        SET correo =
            @correo

        WHERE id_usuario =
        (
            SELECT id_usuario
            FROM Estudiantes
            WHERE id_estudiante =
                @id_estudiante
        );
    END;

    SELECT @filasAfectadas;
END;
GO


CREATE OR ALTER PROCEDURE spDesactivarEstudiante
    @id_estudiante INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Estudiantes

    SET estado = 0

    WHERE id_estudiante =
        @id_estudiante;

    DECLARE @filasAfectadas INT =
        @@ROWCOUNT;

    IF @filasAfectadas > 0
    BEGIN
        UPDATE Usuarios

        SET estado = 0

        WHERE id_usuario =
        (
            SELECT id_usuario
            FROM Estudiantes
            WHERE id_estudiante =
                @id_estudiante
        );
    END;

    SELECT @filasAfectadas;
END;
GO


CREATE OR ALTER PROCEDURE spActivarEstudiante
    @id_estudiante INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Estudiantes

    SET estado = 1

    WHERE id_estudiante =
        @id_estudiante;

    DECLARE @filasAfectadas INT =
        @@ROWCOUNT;

    IF @filasAfectadas > 0
    BEGIN
        UPDATE Usuarios

        SET estado = 1

        WHERE id_usuario =
        (
            SELECT id_usuario
            FROM Estudiantes
            WHERE id_estudiante =
                @id_estudiante
        );
    END;

    SELECT @filasAfectadas;
END;
GO


CREATE OR ALTER PROCEDURE spListarProfesores
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM Profesores
    ORDER BY nomb, primer_apellido;
END
GO

CREATE OR ALTER PROCEDURE spConsultarProfesor
    @id_profesor INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM Profesores
    WHERE id_profesor = @id_profesor;
END
GO

CREATE OR ALTER PROCEDURE spActualizarProfesor
    @id_profesor INT,
    @nomb VARCHAR(100),
    @primer_apellido VARCHAR(30),
    @segundo_apellido VARCHAR(30),
    @identificacion VARCHAR(20),
    @correo VARCHAR(150),
    @telefono VARCHAR(20),
    @especialidad VARCHAR(100)
AS
BEGIN
    UPDATE Profesores
    SET nomb=@nomb,
        primer_apellido=@primer_apellido,
        segundo_apellido=@segundo_apellido,
        identificacion=@identificacion,
        correo=@correo,
        telefono=@telefono,
        especialidad=@especialidad
    WHERE id_profesor=@id_profesor;
END
GO

CREATE OR ALTER PROCEDURE spDesactivarProfesor
    @id_profesor INT
AS
BEGIN
    UPDATE Profesores
       SET estado=0
     WHERE id_profesor=@id_profesor;
END
GO

/*==========================================================
TABLA
==========================================================*/

CREATE TABLE dbo.Cursos
(
    id_curso INT IDENTITY(1,1) PRIMARY KEY,
    id_profesor INT NOT NULL,
    nombre_curso VARCHAR(150) NOT NULL,
    descripcion VARCHAR(500) NOT NULL,
    estado BIT NOT NULL DEFAULT(1),
    fecha_registro DATETIME NOT NULL DEFAULT(GETDATE()),

    CONSTRAINT FK_Cursos_Profesores
    FOREIGN KEY(id_profesor)
    REFERENCES dbo.Profesores(id_profesor)
);
GO

/*==========================================================
REGISTRAR
==========================================================*/

CREATE PROCEDURE spRegistrarCurso
(
    @id_profesor INT,
    @nombre_curso VARCHAR(150),
    @descripcion VARCHAR(500)
)
AS
BEGIN

INSERT INTO Cursos
(
id_profesor,
nombre_curso,
descripcion,
estado,
fecha_registro
)

VALUES
(
@id_profesor,
@nombre_curso,
@descripcion,
1,
GETDATE()
)

END
GO

/*==========================================================
LISTAR
==========================================================*/

CREATE OR ALTER PROCEDURE spListarCursos
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        C.id_curso,
        C.id_profesor,
        P.nomb + ' ' + P.primer_apellido AS Profesor,
        C.nombre_curso,
        C.descripcion,
        C.estado,
        C.fecha_registro
    FROM Cursos C
    INNER JOIN Profesores P
        ON C.id_profesor = P.id_profesor
    ORDER BY C.id_curso DESC;
END
GO

/*==========================================================
CONSULTAR
==========================================================*/

CREATE PROCEDURE spConsultarCurso
(
@id_curso INT
)
AS
BEGIN

SELECT *

FROM Cursos

WHERE id_curso=@id_curso

END
GO

/*==========================================================
ACTUALIZAR
==========================================================*/

CREATE PROCEDURE spActualizarCurso
(
@id_curso INT,
@id_profesor INT,
@nombre_curso VARCHAR(150),
@descripcion VARCHAR(500),
@estado BIT
)
AS
BEGIN

UPDATE Cursos

SET

id_profesor=@id_profesor,
nombre_curso=@nombre_curso,
descripcion=@descripcion,
estado=@estado

WHERE id_curso=@id_curso

END
GO

/*==========================================================
DESACTIVAR
==========================================================*/

CREATE PROCEDURE spDesactivarCurso
(
@id_curso INT
)
AS
BEGIN

UPDATE Cursos

SET estado=0

WHERE id_curso=@id_curso

END
GO

/*==============================================================
                    TABLA MATRICULAS
==============================================================*/

IF OBJECT_ID('dbo.Matriculas','U') IS NULL
BEGIN

CREATE TABLE dbo.Matriculas
(
    id_matricula INT IDENTITY(1,1) PRIMARY KEY,
    id_estudiante INT NOT NULL,
    id_curso INT NOT NULL,
    fecha_matricula DATETIME NOT NULL DEFAULT(GETDATE()),
    estado BIT NOT NULL DEFAULT(1),

    CONSTRAINT FK_Matriculas_Estudiantes
        FOREIGN KEY(id_estudiante)
        REFERENCES dbo.Estudiantes(id_estudiante),

    CONSTRAINT FK_Matriculas_Cursos
        FOREIGN KEY(id_curso)
        REFERENCES dbo.Cursos(id_curso)
);

END
GO

/*==============================================================
REGISTRAR MATRICULA
==============================================================*/

CREATE OR ALTER PROCEDURE spRegistrarMatricula
(
    @id_estudiante INT,
    @id_curso INT
)
AS
BEGIN

    IF EXISTS
    (
        SELECT 1
        FROM Matriculas
        WHERE id_estudiante=@id_estudiante
        AND id_curso=@id_curso
        AND estado=1
    )
    BEGIN
        RAISERROR('El estudiante ya está matriculado en este curso.',16,1)
        RETURN
    END

    INSERT INTO Matriculas
    (
        id_estudiante,
        id_curso,
        fecha_matricula,
        estado
    )
    VALUES
    (
        @id_estudiante,
        @id_curso,
        GETDATE(),
        1
    )

END
GO

/*==============================================================
LISTAR MATRICULAS
==============================================================*/

CREATE OR ALTER PROCEDURE spListarMatriculas
AS
BEGIN

SELECT

M.id_matricula,
M.id_estudiante,
E.nomb + ' ' + E.primer_apellido AS Estudiante,
M.id_curso,
C.nombre_curso,
M.fecha_matricula,
M.estado

FROM Matriculas M

INNER JOIN Estudiantes E
ON E.id_estudiante=M.id_estudiante

INNER JOIN Cursos C
ON C.id_curso=M.id_curso

ORDER BY M.id_matricula DESC

END
GO

/*==============================================================
CONSULTAR MATRICULA
==============================================================*/

CREATE OR ALTER PROCEDURE spConsultarMatricula
(
    @id_matricula INT
)
AS
BEGIN

SELECT *

FROM Matriculas

WHERE id_matricula=@id_matricula

END
GO

/*==============================================================
ACTUALIZAR MATRICULA
==============================================================*/

CREATE OR ALTER PROCEDURE spActualizarMatricula
(
    @id_matricula INT,
    @id_estudiante INT,
    @id_curso INT,
    @estado BIT
)
AS
BEGIN

UPDATE Matriculas

SET

id_estudiante=@id_estudiante,
id_curso=@id_curso,
estado=@estado

WHERE id_matricula=@id_matricula

END
GO

/*==============================================================
DESACTIVAR MATRICULA
==============================================================*/

CREATE OR ALTER PROCEDURE spDesactivarMatricula
(
    @id_matricula INT
)
AS
BEGIN

UPDATE Matriculas

SET estado=0

WHERE id_matricula=@id_matricula

END
GO


/*==============================================================
                    TABLA CALIFICACIONES
==============================================================*/

IF OBJECT_ID('dbo.Calificaciones','U') IS NULL
BEGIN

CREATE TABLE dbo.Calificaciones
(
    id_calificacion INT IDENTITY(1,1) PRIMARY KEY,
    id_matricula INT NOT NULL,
    nota DECIMAL(5,2) NOT NULL,
    fecha_registro DATETIME NOT NULL DEFAULT(GETDATE()),
    fecha_modificacion DATETIME NULL,

    CONSTRAINT FK_Calificaciones_Matriculas
        FOREIGN KEY(id_matricula)
        REFERENCES dbo.Matriculas(id_matricula)
);

END
GO

/*==============================================================
REGISTRAR CALIFICACION
==============================================================*/

CREATE OR ALTER PROCEDURE spRegistrarCalificacion
(
    @id_matricula INT,
    @nota DECIMAL(5,2)
)
AS
BEGIN

    IF @nota < 0 OR @nota > 100
    BEGIN
        RAISERROR('La nota debe estar entre 0 y 100.',16,1)
        RETURN
    END

    INSERT INTO Calificaciones
    (
        id_matricula,
        nota,
        fecha_registro
    )

    VALUES
    (
        @id_matricula,
        @nota,
        GETDATE()
    )

END
GO

/*==============================================================
LISTAR CALIFICACIONES
==============================================================*/

CREATE OR ALTER PROCEDURE spListarCalificaciones
AS
BEGIN

SELECT

C.id_calificacion,
C.id_matricula,
E.nomb + ' ' + E.primer_apellido AS Estudiante,
CU.nombre_curso,
C.nota,
C.fecha_registro,
C.fecha_modificacion

FROM Calificaciones C

INNER JOIN Matriculas M
ON C.id_matricula = M.id_matricula

INNER JOIN Estudiantes E
ON E.id_estudiante = M.id_estudiante

INNER JOIN Cursos CU
ON CU.id_curso = M.id_curso

ORDER BY C.id_calificacion DESC

END
GO

/*==============================================================
CONSULTAR CALIFICACION
==============================================================*/

CREATE OR ALTER PROCEDURE spConsultarCalificacion
(
    @id_calificacion INT
)
AS
BEGIN

SELECT *

FROM Calificaciones

WHERE id_calificacion=@id_calificacion

END
GO

/*==============================================================
ACTUALIZAR CALIFICACION
==============================================================*/

CREATE OR ALTER PROCEDURE spActualizarCalificacion
(
    @id_calificacion INT,
    @id_matricula INT,
    @nota DECIMAL(5,2)
)
AS
BEGIN

    IF @nota < 0 OR @nota > 100
    BEGIN
        RAISERROR('La nota debe estar entre 0 y 100.',16,1)
        RETURN
    END

    UPDATE Calificaciones

    SET

        id_matricula=@id_matricula,
        nota=@nota,
        fecha_modificacion=GETDATE()

    WHERE id_calificacion=@id_calificacion

END
GO

/*==============================================================
DESACTIVAR / ELIMINAR CALIFICACION
==============================================================*/

CREATE OR ALTER PROCEDURE spEliminarCalificacion
(
    @id_calificacion INT
)
AS
BEGIN

DELETE FROM Calificaciones

WHERE id_calificacion=@id_calificacion

END
GO


/*==============================================================
                    TABLA ASISTENCIAS
==============================================================*/

IF OBJECT_ID('dbo.Asistencias','U') IS NULL
BEGIN

CREATE TABLE dbo.Asistencias
(
    id_asistencia INT IDENTITY(1,1) PRIMARY KEY,
    id_matricula INT NOT NULL,
    fecha DATE NOT NULL,
    estado VARCHAR(20) NOT NULL,
    fecha_registro DATETIME NOT NULL DEFAULT(GETDATE()),
    fecha_modificacion DATETIME NULL,

    CONSTRAINT FK_Asistencias_Matriculas
        FOREIGN KEY(id_matricula)
        REFERENCES dbo.Matriculas(id_matricula)
);

END
GO


/*==============================================================
REGISTRAR ASISTENCIA
==============================================================*/

CREATE OR ALTER PROCEDURE spRegistrarAsistencia
(
    @id_matricula INT,
    @fecha DATE,
    @estado VARCHAR(20)
)
AS
BEGIN

    IF EXISTS
    (
        SELECT 1
        FROM Asistencias
        WHERE id_matricula=@id_matricula
        AND fecha=@fecha
    )
    BEGIN
        RAISERROR('Ya existe una asistencia para ese estudiante en esa fecha.',16,1)
        RETURN
    END

    INSERT INTO Asistencias
    (
        id_matricula,
        fecha,
        estado,
        fecha_registro
    )

    VALUES
    (
        @id_matricula,
        @fecha,
        @estado,
        GETDATE()
    )

END
GO


/*==============================================================
LISTAR ASISTENCIAS
==============================================================*/

CREATE OR ALTER PROCEDURE spListarAsistencias
AS
BEGIN

SELECT

A.id_asistencia,
A.id_matricula,
E.nomb + ' ' + E.primer_apellido AS Estudiante,
C.nombre_curso,
A.fecha,
A.estado,
A.fecha_registro,
A.fecha_modificacion

FROM Asistencias A

INNER JOIN Matriculas M
ON A.id_matricula=M.id_matricula

INNER JOIN Estudiantes E
ON E.id_estudiante=M.id_estudiante

INNER JOIN Cursos C
ON C.id_curso=M.id_curso

ORDER BY
A.fecha DESC,
E.nomb

END
GO


/*==============================================================
CONSULTAR ASISTENCIA
==============================================================*/

CREATE OR ALTER PROCEDURE spConsultarAsistencia
(
    @id_asistencia INT
)
AS
BEGIN

SELECT *

FROM Asistencias

WHERE id_asistencia=@id_asistencia

END
GO


/*==============================================================
ACTUALIZAR ASISTENCIA
==============================================================*/

CREATE OR ALTER PROCEDURE spActualizarAsistencia
(
    @id_asistencia INT,
    @id_matricula INT,
    @fecha DATE,
    @estado VARCHAR(20)
)
AS
BEGIN

UPDATE Asistencias

SET

id_matricula=@id_matricula,
fecha=@fecha,
estado=@estado,
fecha_modificacion=GETDATE()

WHERE id_asistencia=@id_asistencia

END
GO


/*==============================================================
ELIMINAR ASISTENCIA
==============================================================*/

CREATE OR ALTER PROCEDURE spEliminarAsistencia
(
    @id_asistencia INT
)
AS
BEGIN

DELETE FROM Asistencias

WHERE id_asistencia=@id_asistencia

END
GO


/*==============================================================
                    TABLA EVENTOS
==============================================================*/

IF OBJECT_ID('dbo.Eventos','U') IS NULL
BEGIN

CREATE TABLE dbo.Eventos
(
    id_evento INT IDENTITY(1,1) PRIMARY KEY,
    id_curso INT NOT NULL,
    titulo VARCHAR(150) NOT NULL,
    descripcion VARCHAR(500) NOT NULL,
    fecha_evento DATETIME NOT NULL,
    lugar VARCHAR(200) NOT NULL,
    estado BIT NOT NULL DEFAULT(1),
    fecha_registro DATETIME NOT NULL DEFAULT(GETDATE()),
    fecha_modificacion DATETIME NULL,

    CONSTRAINT FK_Eventos_Cursos
        FOREIGN KEY(id_curso)
        REFERENCES dbo.Cursos(id_curso)

);

END
GO


/*==============================================================
REGISTRAR EVENTO
==============================================================*/

CREATE OR ALTER PROCEDURE spRegistrarEvento
(
    @id_curso INT,
    @titulo VARCHAR(150),
    @descripcion VARCHAR(500),
    @fecha_evento DATETIME,
    @lugar VARCHAR(200)
)
AS
BEGIN

INSERT INTO Eventos
(
    id_curso,
    titulo,
    descripcion,
    fecha_evento,
    lugar,
    estado,
    fecha_registro
)

VALUES
(
    @id_curso,
    @titulo,
    @descripcion,
    @fecha_evento,
    @lugar,
    1,
    GETDATE()
)

END
GO


/*==============================================================
LISTAR EVENTOS
==============================================================*/

CREATE OR ALTER PROCEDURE spListarEventos
AS
BEGIN

SELECT

E.id_evento,
E.id_curso,
C.nombre_curso,
E.titulo,
E.descripcion,
E.fecha_evento,
E.lugar,
E.estado,
E.fecha_registro,
E.fecha_modificacion

FROM Eventos E

INNER JOIN Cursos C
ON E.id_curso=C.id_curso

ORDER BY E.fecha_evento DESC

END
GO


/*==============================================================
CONSULTAR EVENTO
==============================================================*/

CREATE OR ALTER PROCEDURE spConsultarEvento
(
    @id_evento INT
)
AS
BEGIN

SELECT *

FROM Eventos

WHERE id_evento=@id_evento

END
GO


/*==============================================================
ACTUALIZAR EVENTO
==============================================================*/

CREATE OR ALTER PROCEDURE spActualizarEvento
(
    @id_evento INT,
    @id_curso INT,
    @titulo VARCHAR(150),
    @descripcion VARCHAR(500),
    @fecha_evento DATETIME,
    @lugar VARCHAR(200),
    @estado BIT
)
AS
BEGIN

UPDATE Eventos

SET

id_curso=@id_curso,
titulo=@titulo,
descripcion=@descripcion,
fecha_evento=@fecha_evento,
lugar=@lugar,
estado=@estado,
fecha_modificacion=GETDATE()

WHERE id_evento=@id_evento

END
GO


/*==============================================================
DESACTIVAR EVENTO
==============================================================*/

CREATE OR ALTER PROCEDURE spDesactivarEvento
(
    @id_evento INT
)
AS
BEGIN

UPDATE Eventos

SET

estado=0,
fecha_modificacion=GETDATE()

WHERE id_evento=@id_evento

END
GO

/*==============================================================
                    TABLA NOTIFICACIONES
==============================================================*/

IF OBJECT_ID('dbo.Notificaciones','U') IS NULL
BEGIN

CREATE TABLE dbo.Notificaciones
(
    id_notificacion INT IDENTITY(1,1) PRIMARY KEY,
    id_usuario INT NOT NULL,
    asunto VARCHAR(150) NOT NULL,
    mensaje VARCHAR(MAX) NOT NULL,
    leida BIT NOT NULL DEFAULT(0),
    fecha_envio DATETIME NOT NULL DEFAULT(GETDATE()),
    fecha_lectura DATETIME NULL,

    CONSTRAINT FK_Notificaciones_Usuarios
        FOREIGN KEY(id_usuario)
        REFERENCES dbo.Usuarios(id_usuario)

);

END
GO


/*==============================================================
REGISTRAR NOTIFICACION
==============================================================*/

CREATE OR ALTER PROCEDURE spRegistrarNotificacion
(
    @id_usuario INT,
    @asunto VARCHAR(150),
    @mensaje VARCHAR(MAX)
)
AS
BEGIN

INSERT INTO Notificaciones
(
    id_usuario,
    asunto,
    mensaje,
    leida,
    fecha_envio
)

VALUES
(
    @id_usuario,
    @asunto,
    @mensaje,
    0,
    GETDATE()
)

END
GO


/*==============================================================
LISTAR NOTIFICACIONES
==============================================================*/

CREATE OR ALTER PROCEDURE spListarNotificaciones
AS
BEGIN

SELECT

N.id_notificacion,
N.id_usuario,
U.correo,
N.asunto,
N.mensaje,
N.leida,
N.fecha_envio,
N.fecha_lectura

FROM Notificaciones N

INNER JOIN Usuarios U
ON U.id_usuario = N.id_usuario

ORDER BY N.fecha_envio DESC

END
GO


/*==============================================================
CONSULTAR NOTIFICACION
==============================================================*/

CREATE OR ALTER PROCEDURE spConsultarNotificacion
(
    @id_notificacion INT
)
AS
BEGIN

SELECT *

FROM Notificaciones

WHERE id_notificacion=@id_notificacion

END
GO


/*==============================================================
ACTUALIZAR NOTIFICACION
==============================================================*/

CREATE OR ALTER PROCEDURE spActualizarNotificacion
(
    @id_notificacion INT,
    @id_usuario INT,
    @asunto VARCHAR(150),
    @mensaje VARCHAR(MAX)
)
AS
BEGIN

UPDATE Notificaciones

SET

id_usuario=@id_usuario,
asunto=@asunto,
mensaje=@mensaje

WHERE id_notificacion=@id_notificacion

END
GO


/*==============================================================
MARCAR COMO LEIDA
==============================================================*/

CREATE OR ALTER PROCEDURE spMarcarNotificacionLeida
(
    @id_notificacion INT
)
AS
BEGIN

UPDATE Notificaciones

SET

leida=1,
fecha_lectura=GETDATE()

WHERE id_notificacion=@id_notificacion

END
GO


/*==============================================================
ELIMINAR NOTIFICACION
==============================================================*/

CREATE OR ALTER PROCEDURE spEliminarNotificacion
(
    @id_notificacion INT
)
AS
BEGIN

DELETE FROM Notificaciones

WHERE id_notificacion=@id_notificacion

END
GO


-- Notificacion

-- SP: Validar que el correo exista (para recuperar acceso)
CREATE PROCEDURE spValidarCorreoUsuario
    @correo VARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        u.id_usuario AS IdUsuario,
        u.correo AS Correo,
        u.id_rol AS IdRol,
        ISNULL(e.nomb, p.nomb) AS Nombre
    FROM Usuarios u
    LEFT JOIN Estudiantes e ON u.id_usuario = e.id_usuario AND e.estado = 1
    LEFT JOIN Profesores p ON u.id_usuario = p.id_usuario AND p.estado = 1
    WHERE u.correo = @correo
      AND u.estado = 1;
END
GO

-- SP: Actualizar contraseña (temporal o definitiva)
CREATE PROCEDURE spActualizarContrasennaUsuario
    @id_usuario INT,
    @contraseña VARCHAR(255)
AS
BEGIN
    UPDATE Usuarios
    SET contraseña = @contraseña
    WHERE id_usuario = @id_usuario;

    SELECT @@ROWCOUNT AS Filas;
END
GO


-- ============================================
-- PASO 1: Agregar columna para marcar contraseña temporal
-- ============================================
ALTER TABLE Usuarios
ADD indicador_temp BIT NOT NULL DEFAULT 0;
GO

-- ============================================
-- PASO 2: Actualizar spIniciarSesionUsuario para
-- que también devuelva si la contraseña es temporal
-- ============================================
ALTER PROCEDURE spIniciarSesionUsuario
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
        u.indicador_temp AS IndicadorTemp,
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

-- ============================================
-- PASO 3: Actualizar spActualizarContrasennaUsuario
-- para que marque la contraseña como temporal cuando
-- viene de "recuperar acceso"
-- ============================================
ALTER PROCEDURE spActualizarContrasennaUsuario
    @id_usuario INT,
    @contraseña VARCHAR(255)
AS
BEGIN
    UPDATE Usuarios
    SET contraseña = @contraseña,
        indicador_temp = 1
    WHERE id_usuario = @id_usuario;

    SELECT @@ROWCOUNT AS Filas;
END
GO

-- ============================================
-- PASO 4: Nuevo SP para obtener el hash actual
-- (necesario para validar la contraseña actual
-- antes de dejar cambiarla)
-- ============================================
CREATE PROCEDURE spObtenerContrasenaActual
    @id_usuario INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT contraseña AS Contrasenna
    FROM Usuarios
    WHERE id_usuario = @id_usuario
      AND estado = 1;
END
GO

-- ============================================
-- PASO 5: Nuevo SP para establecer la contraseña
-- definitiva y quitar la marca de temporal
-- ============================================
CREATE PROCEDURE spCambiarContrasena
    @id_usuario INT,
    @contraseña VARCHAR(255)
AS
BEGIN
    UPDATE Usuarios
    SET contraseña = @contraseña,
        indicador_temp = 0
    WHERE id_usuario = @id_usuario;

    SELECT @@ROWCOUNT AS Filas;
END
GO




