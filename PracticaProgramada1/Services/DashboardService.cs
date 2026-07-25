using Dapper;
using Microsoft.Data.SqlClient;
using PracticaProgramada1.Models;
using System.Data;

namespace PracticaProgramada1.Services
{
    /// <summary>
    /// Servicio para obtener datos de dashboards desde la base de datos
    /// </summary>
    public interface IDashboardService
    {
        Task<DashboardViewModel> ObtenerDashboardPorRol(int idUsuario, int idRol);
        Task<GeneralDashboardViewModel> ObtenerDashboardGeneral();
        Task<DashboardMetricas> ObtenerMetricas(int idUsuario, int idRol);
        Task<List<Notificacion>> ObtenerNotificaciones(int idUsuario, int limite = 10);
        Task<List<ActividadReciente>> ObtenerActividadesRecientes(int idUsuario, int limite = 10);
    }

    public class DashboardService : IDashboardService
    {
        private readonly string _connectionString;

        public DashboardService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("ConnectionString no configurado");
        }

        public async Task<DashboardViewModel> ObtenerDashboardPorRol(int idUsuario, int idRol)
        {
            var dashboard = new DashboardViewModel
            {
                IdUsuario = idUsuario,
                IdRol = idRol,
                Metricas = await ObtenerMetricas(idUsuario, idRol),
                Notificaciones = await ObtenerNotificaciones(idUsuario),
                ActividadesRecientes = await ObtenerActividadesRecientes(idUsuario)
            };

            // Generar widgets según el rol
            dashboard.Widgets = GenerarWidgetsPorRol(idRol, dashboard.Metricas);

            return dashboard;
        }

        public async Task<GeneralDashboardViewModel> ObtenerDashboardGeneral()
        {
            using var connection = new SqlConnection(_connectionString);

            var dashboard = new GeneralDashboardViewModel
            {
                TituloEscuela = "Escuela Aurora",
                Lema = "Educación de Excelencia",
                Resumen = await ObtenerResumenEjecutivo(connection),
                ProximosEventos = await ObtenerProximosEventos(connection),
                AnunciosRecientes = await ObtenerAnuncios(connection),
                MejoresEstudiantes = await ObtenerTopEstudiantes(connection)
            };

            return dashboard;
        }

        public async Task<DashboardMetricas> ObtenerMetricas(int idUsuario, int idRol)
        {
            using var connection = new SqlConnection(_connectionString);

            var metricas = new DashboardMetricas();

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@IdUsuario", idUsuario);
                parameters.Add("@IdRol", idRol);

                // Intentar obtener métricas desde stored procedure
                var resultado = await connection.QueryFirstOrDefaultAsync<DashboardMetricas>(
                    "spObtenerMetricasDashboard",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                if (resultado != null)
                {
                    metricas = resultado;
                }
                else
                {
                    // Si no existe el SP, usar datos de ejemplo
                    metricas = GenerarMetricasEjemplo(idRol);
                }
            }
            catch (SqlException)
            {
                // Si falla, usar datos de ejemplo
                metricas = GenerarMetricasEjemplo(idRol);
            }

            return metricas;
        }

        public async Task<List<Notificacion>> ObtenerNotificaciones(int idUsuario, int limite = 10)
        {
            using var connection = new SqlConnection(_connectionString);

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@IdUsuario", idUsuario);
                parameters.Add("@Limite", limite);

                var notificaciones = await connection.QueryAsync<Notificacion>(
                    "spObtenerNotificacionesUsuario",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return notificaciones.ToList();
            }
            catch
            {
                // Datos de ejemplo si falla
                return GenerarNotificacionesEjemplo(idUsuario);
            }
        }

        public async Task<List<ActividadReciente>> ObtenerActividadesRecientes(int idUsuario, int limite = 10)
        {
            using var connection = new SqlConnection(_connectionString);

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@IdUsuario", idUsuario);
                parameters.Add("@Limite", limite);

                var actividades = await connection.QueryAsync<ActividadReciente>(
                    "spObtenerActividadesRecientes",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return actividades.ToList();
            }
            catch
            {
                // Datos de ejemplo si falla
                return GenerarActividadesEjemplo();
            }
        }

        // Métodos privados auxiliares
        private async Task<ResumenEjecutivo> ObtenerResumenEjecutivo(SqlConnection connection)
        {
            try
            {
                var resumen = await connection.QueryFirstOrDefaultAsync<ResumenEjecutivo>(
                    "spObtenerResumenEjecutivo",
                    commandType: CommandType.StoredProcedure
                );

                return resumen ?? new ResumenEjecutivo
                {
                    TotalEstudiantes = 485,
                    TotalProfesores = 42,
                    TotalCursos = 28,
                    TotalAulas = 24,
                    PromedioGeneralEscuela = 85.5m,
                    TasaAprobacion = 92.3m,
                    TasaAsistencia = 94.7m,
                    EstudiantesNuevosEsteMes = 15
                };
            }
            catch
            {
                return new ResumenEjecutivo
                {
                    TotalEstudiantes = 485,
                    TotalProfesores = 42,
                    TotalCursos = 28,
                    TotalAulas = 24,
                    PromedioGeneralEscuela = 85.5m,
                    TasaAprobacion = 92.3m,
                    TasaAsistencia = 94.7m,
                    EstudiantesNuevosEsteMes = 15
                };
            }
        }

        private async Task<List<EventoEscolar>> ObtenerProximosEventos(SqlConnection connection)
        {
            try
            {
                var eventos = await connection.QueryAsync<EventoEscolar>(
                    "SELECT TOP 5 * FROM EventosEscolares WHERE FechaInicio >= GETDATE() ORDER BY FechaInicio"
                );

                var listaEventos = eventos.ToList();
                if (listaEventos.Count == 0)
                {
                    return GenerarEventosEjemplo();
                }
                return listaEventos;
            }
            catch
            {
                return GenerarEventosEjemplo();
            }
        }

        private async Task<List<Anuncio>> ObtenerAnuncios(SqlConnection connection)
        {
            try
            {
                var anuncios = await connection.QueryAsync<Anuncio>(
                    "SELECT TOP 5 * FROM Anuncios ORDER BY FechaPublicacion DESC"
                );

                var listaAnuncios = anuncios.ToList();
                if (listaAnuncios.Count == 0)
                {
                    return GenerarAnunciosEjemplo();
                }
                return listaAnuncios;
            }
            catch
            {
                return GenerarAnunciosEjemplo();
            }
        }

        private async Task<List<TopEstudiante>> ObtenerTopEstudiantes(SqlConnection connection)
        {
            try
            {
                var estudiantes = await connection.QueryAsync<TopEstudiante>(
                    "spObtenerTopEstudiantes",
                    commandType: CommandType.StoredProcedure
                );

                var lista = estudiantes.ToList();
                if (lista.Count == 0)
                {
                    return GenerarTopEstudiantesEjemplo();
                }
                return lista;
            }
            catch
            {
                return GenerarTopEstudiantesEjemplo();
            }
        }

        private List<WidgetCard> GenerarWidgetsPorRol(int idRol, DashboardMetricas metricas)
        {
            var widgets = new List<WidgetCard>();

            switch (idRol)
            {
                case 1: // Administrador
                    widgets.Add(new WidgetCard
                    {
                        Titulo = "Total Estudiantes",
                        Valor = metricas.TotalEstudiantes.ToString(),
                        Descripcion = "estudiantes activos",
                        Icono = "fas fa-user-graduate",
                        ColorClase = "primary",
                        Tendencia = "up",
                        PorcentajeCambio = 5.2m
                    });
                    widgets.Add(new WidgetCard
                    {
                        Titulo = "Total Profesores",
                        Valor = metricas.TotalProfesores.ToString(),
                        Descripcion = "docentes activos",
                        Icono = "fas fa-chalkboard-teacher",
                        ColorClase = "success",
                        Tendencia = "neutral",
                        PorcentajeCambio = 0
                    });
                    widgets.Add(new WidgetCard
                    {
                        Titulo = "Cursos Activos",
                        Valor = metricas.CursosActivos.ToString(),
                        Descripcion = "en este período",
                        Icono = "fas fa-book",
                        ColorClase = "info",
                        Tendencia = "up",
                        PorcentajeCambio = 2.1m
                    });
                    widgets.Add(new WidgetCard
                    {
                        Titulo = "Tasa Asistencia",
                        Valor = metricas.TasaAsistencia.ToString("0.0") + "%",
                        Descripcion = "promedio general",
                        Icono = "fas fa-calendar-check",
                        ColorClase = "warning",
                        Tendencia = "up",
                        PorcentajeCambio = 3.5m
                    });
                    break;

                case 2: // Profesor
                    widgets.Add(new WidgetCard
                    {
                        Titulo = "Mis Cursos",
                        Valor = metricas.MisCursos.ToString(),
                        Descripcion = "cursos asignados",
                        Icono = "fas fa-chalkboard",
                        ColorClase = "primary"
                    });
                    widgets.Add(new WidgetCard
                    {
                        Titulo = "Total Estudiantes",
                        Valor = metricas.TotalEstudiantesEnMisCursos.ToString(),
                        Descripcion = "en mis cursos",
                        Icono = "fas fa-users",
                        ColorClase = "success"
                    });
                    widgets.Add(new WidgetCard
                    {
                        Titulo = "Tareas Pendientes",
                        Valor = metricas.TareasPendientesRevisar.ToString(),
                        Descripcion = "por revisar",
                        Icono = "fas fa-tasks",
                        ColorClase = "warning"
                    });
                    widgets.Add(new WidgetCard
                    {
                        Titulo = "Evaluaciones",
                        Valor = metricas.EvaluacionesPendientes.ToString(),
                        Descripcion = "pendientes",
                        Icono = "fas fa-clipboard-check",
                        ColorClase = "danger"
                    });
                    break;

                case 3: // Estudiante
                    widgets.Add(new WidgetCard
                    {
                        Titulo = "Promedio General",
                        Valor = metricas.PromedioGeneral.ToString("0.00"),
                        Descripcion = "mi promedio actual",
                        Icono = "fas fa-graduation-cap",
                        ColorClase = "primary",
                        Tendencia = "up",
                        PorcentajeCambio = 2.5m
                    });
                    widgets.Add(new WidgetCard
                    {
                        Titulo = "Cursos Inscritos",
                        Valor = metricas.CursosInscritos.ToString(),
                        Descripcion = "materias activas",
                        Icono = "fas fa-book-open",
                        ColorClase = "success"
                    });
                    widgets.Add(new WidgetCard
                    {
                        Titulo = "Tareas Pendientes",
                        Valor = metricas.TareasPendientes.ToString(),
                        Descripcion = "por entregar",
                        Icono = "fas fa-clipboard-list",
                        ColorClase = "warning"
                    });
                    widgets.Add(new WidgetCard
                    {
                        Titulo = "Asistencia",
                        Valor = metricas.PorcentajeAsistencia.ToString("0.0") + "%",
                        Descripcion = "este mes",
                        Icono = "fas fa-calendar-check",
                        ColorClase = "info"
                    });
                    break;

                default: // Usuario genérico
                    widgets.Add(new WidgetCard
                    {
                        Titulo = "Bienvenido",
                        Valor = "ES",
                        Descripcion = "Escuela Aurora",
                        Icono = "fas fa-school",
                        ColorClase = "primary"
                    });
                    break;
            }

            return widgets;
        }

        // Métodos para generar datos de ejemplo
        private DashboardMetricas GenerarMetricasEjemplo(int idRol)
        {
            var metricas = new DashboardMetricas();

            switch (idRol)
            {
                case 1: // Administrador
                    metricas.TotalEstudiantes = 485;
                    metricas.TotalProfesores = 42;
                    metricas.TotalCursos = 28;
                    metricas.EstudiantesActivos = 465;
                    metricas.TasaAsistencia = 94.5m;
                    metricas.CursosActivos = 28;
                    break;

                case 2: // Profesor
                    metricas.MisCursos = 3;
                    metricas.TotalEstudiantesEnMisCursos = 85;
                    metricas.TareasPendientesRevisar = 26;
                    metricas.EvaluacionesPendientes = 2;
                    break;

                case 3: // Estudiante
                    metricas.CursosInscritos = 6;
                    metricas.PromedioGeneral = 87.5m;
                    metricas.TareasPendientes = 3;
                    metricas.ProximasEvaluaciones = 2;
                    metricas.PorcentajeAsistencia = 96.2m;
                    break;
            }

            return metricas;
        }

        private List<Notificacion> GenerarNotificacionesEjemplo(int idUsuario)
        {
            return new List<Notificacion>
            {
                new Notificacion
                {
                    Id = 1,
                    Titulo = "Nueva tarea asignada",
                    Mensaje = "Se ha asignado una nueva tarea en Matemáticas",
                    Fecha = DateTime.Now.AddHours(-2),
                    Leida = false,
                    Tipo = "warning"
                },
                new Notificacion
                {
                    Id = 2,
                    Titulo = "Recordatorio de evaluación",
                    Mensaje = "Examen de Ciencias programado para mañana",
                    Fecha = DateTime.Now.AddHours(-5),
                    Leida = false,
                    Tipo = "danger"
                },
                new Notificacion
                {
                    Id = 3,
                    Titulo = "Calificación publicada",
                    Mensaje = "Nueva calificación disponible en Sociales",
                    Fecha = DateTime.Now.AddDays(-1),
                    Leida = true,
                    Tipo = "success"
                }
            };
        }

        private List<ActividadReciente> GenerarActividadesEjemplo()
        {
            return new List<ActividadReciente>
            {
                new ActividadReciente
                {
                    Titulo = "Tarea completada",
                    Descripcion = "Entregaste la tarea de Matemáticas Cap. 5",
                    Fecha = DateTime.Now.AddHours(-1),
                    Icono = "fas fa-check-circle",
                    ColorClase = "success"
                },
                new ActividadReciente
                {
                    Titulo = "Asistencia registrada",
                    Descripcion = "Presente en clase de Ciencias Naturales",
                    Fecha = DateTime.Now.AddHours(-3),
                    Icono = "fas fa-user-check",
                    ColorClase = "info"
                },
                new ActividadReciente
                {
                    Titulo = "Calificación obtenida",
                    Descripcion = "Calificación de 92 en Quiz de Sociales",
                    Fecha = DateTime.Now.AddDays(-1),
                    Icono = "fas fa-star",
                    ColorClase = "warning"
                }
            };
        }

        private List<EventoEscolar> GenerarEventosEjemplo()
        {
            return new List<EventoEscolar>
            {
                new EventoEscolar
                {
                    Id = 1,
                    Titulo = "Inicio de Clases",
                    Descripcion = "Inicio del período lectivo 2024",
                    FechaInicio = DateTime.Now.AddDays(7),
                    Tipo = "general",
                    ColorEvento = "#3788d8"
                },
                new EventoEscolar
                {
                    Id = 2,
                    Titulo = "Feria de Ciencias",
                    Descripcion = "Exhibición de proyectos científicos",
                    FechaInicio = DateTime.Now.AddDays(30),
                    Tipo = "evento",
                    ColorEvento = "#28a745"
                }
            };
        }

        private List<Anuncio> GenerarAnunciosEjemplo()
        {
            return new List<Anuncio>
            {
                new Anuncio
                {
                    Id = 1,
                    Titulo = "Proceso de Matrícula 2024",
                    Contenido = "Recordamos que el proceso de matrícula finaliza el 25 de enero",
                    FechaPublicacion = DateTime.Now.AddDays(-2),
                    Autor = "Administración",
                    Importante = true,
                    Categoria = "Académico"
                }
            };
        }

        private List<TopEstudiante> GenerarTopEstudiantesEjemplo()
        {
            return new List<TopEstudiante>
            {
                new TopEstudiante { IdEstudiante = 1, NombreCompleto = "Ana García Pérez", PromedioGeneral = 98.5m, Posicion = 1, Grado = "11mo" },
                new TopEstudiante { IdEstudiante = 2, NombreCompleto = "Carlos Rodríguez López", PromedioGeneral = 96.8m, Posicion = 2, Grado = "10mo" },
                new TopEstudiante { IdEstudiante = 3, NombreCompleto = "María Fernández Castro", PromedioGeneral = 95.2m, Posicion = 3, Grado = "11mo" },
                new TopEstudiante { IdEstudiante = 4, NombreCompleto = "José Martínez Salas", PromedioGeneral = 94.5m, Posicion = 4, Grado = "9no" },
                new TopEstudiante { IdEstudiante = 5, NombreCompleto = "Laura Jiménez Mora", PromedioGeneral = 93.8m, Posicion = 5, Grado = "10mo" }
            };
        }
    }
}
