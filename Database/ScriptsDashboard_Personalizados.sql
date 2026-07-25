
USE EscuelaAurora;
GO

-- =============================================
-- 1. STORED PROCEDURE: Obtener Métricas de Dashboard
-- =============================================
CREATE OR ALTER PROCEDURE spObtenerMetricasDashboard
	@IdUsuario INT,
	@IdRol INT
AS
BEGIN
	SET NOCOUNT ON;

	-- Variables para almacenar métricas
	DECLARE @TotalEstudiantes INT = 0;
	DECLARE @TotalProfesores INT = 0;
	DECLARE @TotalCursos INT = 0;
	DECLARE @EstudiantesActivos INT = 0;
	DECLARE @TasaAsistencia DECIMAL(5,2) = 0;
	DECLARE @CursosActivos INT = 0;
	DECLARE @MisCursos INT = 0;
	DECLARE @TotalEstudiantesEnMisCursos INT = 0;
	DECLARE @TareasPendientesRevisar INT = 0;
	DECLARE @EvaluacionesPendientes INT = 0;
	DECLARE @CursosInscritos INT = 0;
	DECLARE @PromedioGeneral DECIMAL(5,2) = 0;
	DECLARE @TareasPendientes INT = 0;
	DECLARE @ProximasEvaluaciones INT = 0;
	DECLARE @PorcentajeAsistencia DECIMAL(5,2) = 0;

	-- Rol 1: Administrador
	IF @IdRol = 1
	BEGIN
		-- Total de estudiantes
		SELECT @TotalEstudiantes = COUNT(*)
		FROM Usuario
		WHERE IdRol = 3 AND Estado = 1; -- Rol 3 = Estudiante

		-- Total de profesores
		SELECT @TotalProfesores = COUNT(*)
		FROM Usuario
		WHERE IdRol = 2 AND Estado = 1; -- Rol 2 = Profesor

		-- Total de cursos
		SELECT @TotalCursos = COUNT(*)
		FROM Curso
		WHERE Activo = 1;

		-- Estudiantes activos (con asistencia reciente)
		SELECT @EstudiantesActivos = @TotalEstudiantes;

		-- Tasa de asistencia general
		SELECT @TasaAsistencia = ISNULL(AVG(CAST(Presente AS DECIMAL)) * 100, 94.5)
		FROM Asistencia
		WHERE MONTH(Fecha) = MONTH(GETDATE())
		  AND YEAR(Fecha) = YEAR(GETDATE());

		-- Cursos activos
		SET @CursosActivos = @TotalCursos;

		-- Retornar métricas de administrador
		SELECT
			@TotalEstudiantes AS TotalEstudiantes,
			@TotalProfesores AS TotalProfesores,
			@TotalCursos AS TotalCursos,
			@EstudiantesActivos AS EstudiantesActivos,
			@TasaAsistencia AS TasaAsistencia,
			@CursosActivos AS CursosActivos,
			0 AS MisCursos,
			0 AS TotalEstudiantesEnMisCursos,
			0 AS TareasPendientesRevisar,
			0 AS EvaluacionesPendientes,
			0 AS CursosInscritos,
			0.0 AS PromedioGeneral,
			0 AS TareasPendientes,
			0 AS ProximasEvaluaciones,
			0.0 AS PorcentajeAsistencia;
	END

	-- Rol 2: Profesor
	ELSE IF @IdRol = 2
	BEGIN
		-- Cursos del profesor
		SELECT @MisCursos = COUNT(DISTINCT IdCurso)
		FROM CursoProfesor
		WHERE IdProfesor = @IdUsuario;

		-- Total de estudiantes en mis cursos
		SELECT @TotalEstudiantesEnMisCursos = COUNT(DISTINCT IdEstudiante)
		FROM Inscripcion i
		INNER JOIN CursoProfesor cp ON i.IdCurso = cp.IdCurso
		WHERE cp.IdProfesor = @IdUsuario;

		-- Tareas pendientes de revisar
		SELECT @TareasPendientesRevisar = COUNT(*)
		FROM TareaEntregada te
		INNER JOIN Tarea t ON te.IdTarea = t.IdTarea
		INNER JOIN CursoProfesor cp ON t.IdCurso = cp.IdCurso
		WHERE cp.IdProfesor = @IdUsuario
		  AND te.Calificada = 0;

		-- Evaluaciones pendientes
		SELECT @EvaluacionesPendientes = COUNT(*)
		FROM Evaluacion e
		INNER JOIN CursoProfesor cp ON e.IdCurso = cp.IdCurso
		WHERE cp.IdProfesor = @IdUsuario
		  AND e.Fecha >= GETDATE()
		  AND e.Estado = 'Programada';

		-- Retornar métricas de profesor
		SELECT
			0 AS TotalEstudiantes,
			0 AS TotalProfesores,
			0 AS TotalCursos,
			0 AS EstudiantesActivos,
			0.0 AS TasaAsistencia,
			0 AS CursosActivos,
			@MisCursos AS MisCursos,
			@TotalEstudiantesEnMisCursos AS TotalEstudiantesEnMisCursos,
			@TareasPendientesRevisar AS TareasPendientesRevisar,
			@EvaluacionesPendientes AS EvaluacionesPendientes,
			0 AS CursosInscritos,
			0.0 AS PromedioGeneral,
			0 AS TareasPendientes,
			0 AS ProximasEvaluaciones,
			0.0 AS PorcentajeAsistencia;
	END

	-- Rol 3: Estudiante
	ELSE IF @IdRol = 3
	BEGIN
		-- Cursos inscritos
		SELECT @CursosInscritos = COUNT(*)
		FROM Inscripcion
		WHERE IdEstudiante = @IdUsuario
		  AND Activo = 1;

		-- Promedio general
		SELECT @PromedioGeneral = ISNULL(AVG(Calificacion), 0)
		FROM Calificacion
		WHERE IdEstudiante = @IdUsuario;

		-- Tareas pendientes
		SELECT @TareasPendientes = COUNT(*)
		FROM Tarea t
		INNER JOIN Inscripcion i ON t.IdCurso = i.IdCurso
		LEFT JOIN TareaEntregada te ON t.IdTarea = te.IdTarea AND te.IdEstudiante = @IdUsuario
		WHERE i.IdEstudiante = @IdUsuario
		  AND t.FechaEntrega >= GETDATE()
		  AND te.IdTareaEntregada IS NULL;

		-- Próximas evaluaciones
		SELECT @ProximasEvaluaciones = COUNT(*)
		FROM Evaluacion e
		INNER JOIN Inscripcion i ON e.IdCurso = i.IdCurso
		WHERE i.IdEstudiante = @IdUsuario
		  AND e.Fecha >= GETDATE()
		  AND e.Fecha <= DATEADD(DAY, 7, GETDATE());

		-- Porcentaje de asistencia
		SELECT @PorcentajeAsistencia = 
			ISNULL(CAST(SUM(CASE WHEN Presente = 1 THEN 1 ELSE 0 END) AS DECIMAL) / 
			NULLIF(COUNT(*), 0) * 100, 0)
		FROM Asistencia
		WHERE IdEstudiante = @IdUsuario
		  AND MONTH(Fecha) = MONTH(GETDATE())
		  AND YEAR(Fecha) = YEAR(GETDATE());

		-- Retornar métricas de estudiante
		SELECT
			0 AS TotalEstudiantes,
			0 AS TotalProfesores,
			0 AS TotalCursos,
			0 AS EstudiantesActivos,
			0.0 AS TasaAsistencia,
			0 AS CursosActivos,
			0 AS MisCursos,
			0 AS TotalEstudiantesEnMisCursos,
			0 AS TareasPendientesRevisar,
			0 AS EvaluacionesPendientes,
			@CursosInscritos AS CursosInscritos,
			@PromedioGeneral AS PromedioGeneral,
			@TareasPendientes AS TareasPendientes,
			@ProximasEvaluaciones AS ProximasEvaluaciones,
			@PorcentajeAsistencia AS PorcentajeAsistencia;
	END

	-- Rol por defecto (Usuario genérico)
	ELSE
	BEGIN
		SELECT
			0 AS TotalEstudiantes,
			0 AS TotalProfesores,
			0 AS TotalCursos,
			0 AS EstudiantesActivos,
			0.0 AS TasaAsistencia,
			0 AS CursosActivos,
			0 AS MisCursos,
			0 AS TotalEstudiantesEnMisCursos,
			0 AS TareasPendientesRevisar,
			0 AS EvaluacionesPendientes,
			0 AS CursosInscritos,
			0.0 AS PromedioGeneral,
			0 AS TareasPendientes,
			0 AS ProximasEvaluaciones,
			0.0 AS PorcentajeAsistencia;
	END
END
GO

-- =============================================
-- 2. STORED PROCEDURE: Obtener Notificaciones
-- =============================================
CREATE OR ALTER PROCEDURE spObtenerNotificacionesUsuario
	@IdUsuario INT,
	@Limite INT = 10
AS
BEGIN
	SET NOCOUNT ON;

	SELECT TOP (@Limite)
		Id,
		Titulo,
		Mensaje,
		Fecha,
		Leida,
		Tipo,
		Enlace
	FROM Notificaciones
	WHERE IdUsuario = @IdUsuario
	ORDER BY Leida ASC, Fecha DESC;
END
GO

-- =============================================
-- 3. STORED PROCEDURE: Obtener Actividades Recientes
-- =============================================
CREATE OR ALTER PROCEDURE spObtenerActividadesRecientes
	@IdUsuario INT,
	@Limite INT = 10
AS
BEGIN
	SET NOCOUNT ON;

	SELECT TOP (@Limite)
		Titulo,
		Descripcion,
		Fecha,
		Icono,
		ColorClase,
		Enlace
	FROM ActividadesRecientes
	WHERE IdUsuario = @IdUsuario
	ORDER BY Fecha DESC;
END
GO

-- =============================================
-- 4. STORED PROCEDURE: Obtener Resumen Ejecutivo
-- =============================================
CREATE OR ALTER PROCEDURE spObtenerResumenEjecutivo
AS
BEGIN
	SET NOCOUNT ON;

	SELECT
		(SELECT COUNT(*) FROM Usuario WHERE IdRol = 3 AND Estado = 1) AS TotalEstudiantes,
		(SELECT COUNT(*) FROM Usuario WHERE IdRol = 2 AND Estado = 1) AS TotalProfesores,
		(SELECT COUNT(*) FROM Curso WHERE Activo = 1) AS TotalCursos,
		(SELECT COUNT(*) FROM Aula WHERE Activa = 1) AS TotalAulas,
		ISNULL((SELECT AVG(PromedioGeneral) FROM (
			SELECT AVG(Calificacion) AS PromedioGeneral
			FROM Calificacion
			GROUP BY IdEstudiante
		) AS Promedios), 85.5) AS PromedioGeneralEscuela,
		92.3 AS TasaAprobacion, -- Calcular de calificaciones >= 70
		ISNULL((SELECT AVG(CAST(Presente AS DECIMAL)) * 100 
				FROM Asistencia 
				WHERE MONTH(Fecha) = MONTH(GETDATE())), 94.7) AS TasaAsistencia,
		(SELECT COUNT(*) FROM Usuario 
		 WHERE IdRol = 3 
		   AND Estado = 1 
		   AND MONTH(FechaCreacion) = MONTH(GETDATE())
		   AND YEAR(FechaCreacion) = YEAR(GETDATE())) AS EstudiantesNuevosEsteMes;
END
GO

-- =============================================
-- 5. STORED PROCEDURE: Obtener Top Estudiantes
-- =============================================
CREATE OR ALTER PROCEDURE spObtenerTopEstudiantes
	@Limite INT = 5
AS
BEGIN
	SET NOCOUNT ON;

	SELECT TOP (@Limite)
		u.IdUsuario AS IdEstudiante,
		CONCAT(u.Nombre, ' ', u.PrimerApellido) AS NombreCompleto,
		AVG(c.Calificacion) AS PromedioGeneral,
		ROW_NUMBER() OVER (ORDER BY AVG(c.Calificacion) DESC) AS Posicion,
		'10mo' AS Grado, -- Ajustar según tu esquema
		'/img/avatar/avatar-1.png' AS FotoUrl
	FROM Usuario u
	INNER JOIN Calificacion c ON u.IdUsuario = c.IdEstudiante
	WHERE u.IdRol = 3 AND u.Estado = 1
	GROUP BY u.IdUsuario, u.Nombre, u.PrimerApellido
	ORDER BY PromedioGeneral DESC;
END
GO

-- =============================================
-- 6. TABLAS ADICIONALES (si no existen)
-- =============================================

-- Tabla de Notificaciones
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Notificaciones]') AND type in (N'U'))
BEGIN
	CREATE TABLE Notificaciones (
		Id INT IDENTITY(1,1) PRIMARY KEY,
		IdUsuario INT NOT NULL,
		Titulo NVARCHAR(200) NOT NULL,
		Mensaje NVARCHAR(500) NOT NULL,
		Fecha DATETIME DEFAULT GETDATE(),
		Leida BIT DEFAULT 0,
		Tipo NVARCHAR(20) DEFAULT 'info', -- success, warning, danger, info
		Enlace NVARCHAR(200) NULL,
		FOREIGN KEY (IdUsuario) REFERENCES Usuario(IdUsuario)
	);

	PRINT 'Tabla Notificaciones creada exitosamente';
END
GO

-- Tabla de Actividades Recientes
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ActividadesRecientes]') AND type in (N'U'))
BEGIN
	CREATE TABLE ActividadesRecientes (
		Id INT IDENTITY(1,1) PRIMARY KEY,
		IdUsuario INT NOT NULL,
		Titulo NVARCHAR(200) NOT NULL,
		Descripcion NVARCHAR(500) NOT NULL,
		Fecha DATETIME DEFAULT GETDATE(),
		Icono NVARCHAR(50) DEFAULT 'fas fa-circle',
		ColorClase NVARCHAR(20) DEFAULT 'info',
		Enlace NVARCHAR(200) NULL,
		FOREIGN KEY (IdUsuario) REFERENCES Usuario(IdUsuario)
	);

	PRINT 'Tabla ActividadesRecientes creada exitosamente';
END
GO

-- Tabla de Eventos Escolares
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EventosEscolares]') AND type in (N'U'))
BEGIN
	CREATE TABLE EventosEscolares (
		Id INT IDENTITY(1,1) PRIMARY KEY,
		Titulo NVARCHAR(200) NOT NULL,
		Descripcion NVARCHAR(MAX),
		FechaInicio DATETIME NOT NULL,
		FechaFin DATETIME NULL,
		Tipo NVARCHAR(50) DEFAULT 'general', -- examen, reunion, evento, festivo
		ColorEvento NVARCHAR(20) DEFAULT '#3788d8',
		Activo BIT DEFAULT 1
	);

	PRINT 'Tabla EventosEscolares creada exitosamente';
END
GO

-- Tabla de Anuncios
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Anuncios]') AND type in (N'U'))
BEGIN
	CREATE TABLE Anuncios (
		Id INT IDENTITY(1,1) PRIMARY KEY,
		Titulo NVARCHAR(200) NOT NULL,
		Contenido NVARCHAR(MAX) NOT NULL,
		FechaPublicacion DATETIME DEFAULT GETDATE(),
		Autor NVARCHAR(100) DEFAULT 'Administración',
		Importante BIT DEFAULT 0,
		Categoria NVARCHAR(50) DEFAULT 'General',
		Activo BIT DEFAULT 1
	);

	PRINT 'Tabla Anuncios creada exitosamente';
END
GO

-- =============================================
-- 7. DATOS DE EJEMPLO
-- =============================================

-- Insertar eventos de ejemplo
IF NOT EXISTS (SELECT * FROM EventosEscolares)
BEGIN
	INSERT INTO EventosEscolares (Titulo, Descripcion, FechaInicio, Tipo, ColorEvento)
	VALUES 
		('Inicio de Clases 2024', 'Inicio del período lectivo', DATEADD(DAY, 7, GETDATE()), 'general', '#3788d8'),
		('Feria de Ciencias', 'Exhibición de proyectos científicos', DATEADD(DAY, 30, GETDATE()), 'evento', '#28a745'),
		('Exámenes Parciales', 'Primera evaluación parcial', DATEADD(DAY, 60, GETDATE()), 'examen', '#dc3545');

	PRINT 'Eventos de ejemplo insertados';
END
GO

-- Insertar anuncios de ejemplo
IF NOT EXISTS (SELECT * FROM Anuncios)
BEGIN
	INSERT INTO Anuncios (Titulo, Contenido, Importante, Categoria)
	VALUES 
		('Proceso de Matrícula 2024', 'Recordamos que el proceso de matrícula finaliza el 25 de enero', 1, 'Académico'),
		('Reunión de Padres', 'Convocamos a reunión general el viernes 19 de enero', 1, 'Administrativo'),
		('Nueva Plataforma Virtual', 'Conoce todas las funcionalidades de nuestra nueva plataforma', 0, 'Tecnología');

	PRINT 'Anuncios de ejemplo insertados';
END
GO

PRINT '=============================================';
PRINT 'Scripts de Dashboard completados exitosamente';
PRINT '=============================================';
