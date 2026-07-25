namespace PracticaProgramada1.Models
{
    public class DashboardViewModel
    {
        public string NombreUsuario { get; set; } = string.Empty;
        public string RolUsuario { get; set; } = string.Empty;
        public int IdRol { get; set; }
        public int IdUsuario { get; set; }

        // Métricas Generales
        public DashboardMetricas Metricas { get; set; } = new DashboardMetricas();

        // Widgets específicos por rol
        public List<WidgetCard> Widgets { get; set; } = new List<WidgetCard>();

        // Actividades recientes
        public List<ActividadReciente> ActividadesRecientes { get; set; } = new List<ActividadReciente>();

        // Notificaciones
        public List<Notificacion> Notificaciones { get; set; } = new List<Notificacion>();
    }

    public class DashboardMetricas
    {
        // Métricas para Administrador
        public int TotalEstudiantes { get; set; }
        public int TotalProfesores { get; set; }
        public int TotalCursos { get; set; }
        public int EstudiantesActivos { get; set; }
        public decimal TasaAsistencia { get; set; }
        public int CursosActivos { get; set; }

        // Métricas para Profesor
        public int MisCursos { get; set; }
        public int TotalEstudiantesEnMisCursos { get; set; }
        public int TareasPendientesRevisar { get; set; }
        public int EvaluacionesPendientes { get; set; }

        // Métricas para Estudiante
        public int CursosInscritos { get; set; }
        public decimal PromedioGeneral { get; set; }
        public int TareasPendientes { get; set; }
        public int ProximasEvaluaciones { get; set; }
        public decimal PorcentajeAsistencia { get; set; }

        // Estadísticas para gráficos
        public List<EstadisticaMensual> InscripcionesMensuales { get; set; } = new List<EstadisticaMensual>();
        public List<RendimientoAcademico> RendimientoPorMateria { get; set; } = new List<RendimientoAcademico>();
        public List<AsistenciaSemanal> AsistenciaSemanal { get; set; } = new List<AsistenciaSemanal>();
    }

    public class WidgetCard
    {
        public string Titulo { get; set; } = string.Empty;
        public string Icono { get; set; } = string.Empty;
        public string Valor { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string ColorClase { get; set; } = "primary";
        public string Enlace { get; set; } = "#";
        public string Tendencia { get; set; } = ""; // "up", "down", "neutral"
        public decimal PorcentajeCambio { get; set; }
    }

    public class ActividadReciente
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string Icono { get; set; } = "fas fa-circle";
        public string ColorClase { get; set; } = "info";
        public string Enlace { get; set; } = "#";
    }

    public class Notificacion
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public bool Leida { get; set; }
        public string Tipo { get; set; } = "info"; // success, warning, danger, info
        public string Enlace { get; set; } = "#";
    }

    public class EstadisticaMensual
    {
        public string Mes { get; set; } = string.Empty;
        public int Cantidad { get; set; }
    }

    public class RendimientoAcademico
    {
        public string Materia { get; set; } = string.Empty;
        public decimal Promedio { get; set; }
        public int TotalEstudiantes { get; set; }
    }

    public class AsistenciaSemanal
    {
        public string Dia { get; set; } = string.Empty;
        public decimal PorcentajeAsistencia { get; set; }
    }

    public class GeneralDashboardViewModel
    {
        public string TituloEscuela { get; set; } = "Escuela Aurora";
        public string Lema { get; set; } = "Educación de Excelencia";

        // Resumen ejecutivo
        public ResumenEjecutivo Resumen { get; set; } = new ResumenEjecutivo();

        // Calendario de eventos
        public List<EventoEscolar> ProximosEventos { get; set; } = new List<EventoEscolar>();

        // Anuncios
        public List<Anuncio> AnunciosRecientes { get; set; } = new List<Anuncio>();

        // TOP Estudiantes
        public List<TopEstudiante> MejoresEstudiantes { get; set; } = new List<TopEstudiante>();
    }

    public class ResumenEjecutivo
    {
        public int TotalEstudiantes { get; set; }
        public int TotalProfesores { get; set; }
        public int TotalCursos { get; set; }
        public int TotalAulas { get; set; }
        public decimal PromedioGeneralEscuela { get; set; }
        public decimal TasaAprobacion { get; set; }
        public decimal TasaAsistencia { get; set; }
        public int EstudiantesNuevosEsteMes { get; set; }
    }

    public class EventoEscolar
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string Tipo { get; set; } = "general"; // examen, reunion, evento, festivo
        public string ColorEvento { get; set; } = "#3788d8";
    }

    public class Anuncio
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Contenido { get; set; } = string.Empty;
        public DateTime FechaPublicacion { get; set; }
        public string Autor { get; set; } = string.Empty;
        public bool Importante { get; set; }
        public string Categoria { get; set; } = "General";
    }

    public class TopEstudiante
    {
        public int IdEstudiante { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public decimal PromedioGeneral { get; set; }
        public int Posicion { get; set; }
        public string Grado { get; set; } = string.Empty;
        public string FotoUrl { get; set; } = "/img/avatar/avatar-1.png";
    }
}
