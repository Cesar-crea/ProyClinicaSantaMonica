using Microsoft.Data.SqlClient;
using ProyectoSantaMonica_Cesar.Models;
using ProyectoSantaMonica_Cesar.Models.enums;
using System.Data;

namespace ProyectoSantaMonica_Cesar.Repository
{
    public class CitaRepository
    {
        private readonly string _connection;

        public CitaRepository(IConfiguration config)
        {
            _connection = config.GetConnectionString("CibertecConnection");
        }

        // =========================
        // MAPEO
        // =========================

        private Cita MapearBase(SqlDataReader dr)
        {
            return new Cita
            {
                Id_Cita = dr.GetInt32(0),
                Id_Medico = dr.GetInt32(1),
                Id_Paciente = dr.GetInt32(2),
                Id_Usuario = dr.GetInt64(3),
                Fecha = dr.GetDateTime(4),
                Hora = dr.GetTimeSpan(5),
                Motivo = dr.IsDBNull(6) ? "" : dr.GetString(6),
                Estado = Enum.Parse<EstadoCita>(dr.GetString(7))
            };
        }
        private Cita MapearCompleto(SqlDataReader dr)
        {
            return new Cita
            {
                Id_Cita = dr.GetInt32(0),
                Id_Medico = dr.GetInt32(1),
                Id_Paciente = dr.GetInt32(2),
                Id_Usuario = dr.GetInt64(3),
                Fecha = dr.GetDateTime(4),
                Hora = dr.GetTimeSpan(5),
                Motivo = dr.IsDBNull(6) ? "" : dr.GetString(6),
                Estado = Enum.Parse<EstadoCita>(dr.GetString(7)),

                Paciente = new Paciente
                {
                    Nombres = ExisteColumna(dr, "PacienteNombre")
                        ? dr["PacienteNombre"]?.ToString()
                        : "",

                    Apellidos = ExisteColumna(dr, "PacienteApellido")
                        ? dr["PacienteApellido"]?.ToString()
                        : "",

                    Dni = ExisteColumna(dr, "PacienteDni")
                        ? dr["PacienteDni"]?.ToString()
                        : ""
                },

                Medico = new Medico
                {
                    Nombres = ExisteColumna(dr, "MedicoNombre")
                        ? dr["MedicoNombre"]?.ToString()
                        : "",

                    Apellidos = ExisteColumna(dr, "MedicoApellido")
                        ? dr["MedicoApellido"]?.ToString()
                        : "",

                    Especialidad = ExisteColumna(dr, "Especialidad")
                        ? dr["Especialidad"]?.ToString()
                        : ""
                },

                Usuario = new Usuario
                {
                    Nombres = ExisteColumna(dr, "UsuarioNombre")
                        ? dr["UsuarioNombre"]?.ToString()
                        : "",

                    Apellidos = ExisteColumna(dr, "UsuarioApellido")
                        ? dr["UsuarioApellido"]?.ToString()
                        : ""
                }
            };
        }

        // =========================
        // LISTAR
        // =========================
        public async Task<List<Cita>> ListarAsync()
        {
            var lista = new List<Cita>();

            using var cn = new SqlConnection(_connection);
            using var cmd = new SqlCommand("sp_listarCitas", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            await cn.OpenAsync();
            var dr = await cmd.ExecuteReaderAsync();

            while (await dr.ReadAsync())
                lista.Add(MapearCompleto(dr));

            return lista;
        }


        // =========================
        // BUSCAR POR PACIENTE
        // =========================
        public async Task<List<Cita>> BuscarPorPacienteAsync(string texto)
        {
            var lista = new List<Cita>();

            using var cn = new SqlConnection(_connection);
            using var cmd = new SqlCommand("sp_buscarCitasPaciente", cn)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("@Texto", texto);

            await cn.OpenAsync();
            using var dr = await cmd.ExecuteReaderAsync();

            while (await dr.ReadAsync())
                lista.Add(MapearCompleto(dr));

            return lista;
        }

        // =========================
        // VALIDACION DEL HORARIO Y DIA MEDICO
        // =========================

        public async Task<bool> MedicoAtiendeEnHorarioAsync(
       int idMedico,
       DateTime fecha,
       TimeSpan hora)
        {
            // =========================
            // VALIDACIONES BASICAS
            // =========================

            if (idMedico <= 0)
                return false;

            if (fecha.Date < DateTime.Today)
                return false;

            if (hora == TimeSpan.Zero)
                return false;

            using var cn = new SqlConnection(_connection);

            // =========================
            // OBTENER DIA SEMANA
            // =========================

            string diaSemana = fecha.DayOfWeek switch
            {
                DayOfWeek.Monday => "Lunes",
                DayOfWeek.Tuesday => "Martes",
                DayOfWeek.Wednesday => "Miercoles",
                DayOfWeek.Thursday => "Jueves",
                DayOfWeek.Friday => "Viernes",
                DayOfWeek.Saturday => "Sabado",
                DayOfWeek.Sunday => "Domingo",
                _ => ""
            };

            // =========================
            // VALIDAR SI EL MEDICO
            // ATIENDE ESE DIA Y HORA
            // =========================

            string sql = @"
        SELECT COUNT(*)
        FROM Horarios_Atencion
        WHERE Id_Medico = @Id_Medico
        AND Dia_Semana = @Dia_Semana
        AND @Hora >= Horario_Entrada
        AND @Hora < Horario_Salida
    ";

            using var cmd = new SqlCommand(sql, cn);

            cmd.Parameters.AddWithValue("@Id_Medico", idMedico);
            cmd.Parameters.AddWithValue("@Dia_Semana", diaSemana);
            cmd.Parameters.AddWithValue("@Hora", hora);

            await cn.OpenAsync();

            int cantidad = Convert.ToInt32(await cmd.ExecuteScalarAsync());

            // =========================
            // SI NO TIENE HORARIO
            // =========================

            if (cantidad <= 0)
                return false;

            // =========================
            // VALIDAR DOBLE CITA
            // =========================

            string sqlCita = @"
        SELECT COUNT(*)
        FROM Cita
        WHERE Id_Medico = @Id_Medico
        AND Fecha = @Fecha
        AND Hora = @Hora
        AND Estado <> 'CANCELADO'
    ";

            using var cmdCita = new SqlCommand(sqlCita, cn);

            cmdCita.Parameters.AddWithValue("@Id_Medico", idMedico);
            cmdCita.Parameters.AddWithValue("@Fecha", fecha.Date);
            cmdCita.Parameters.AddWithValue("@Hora", hora);

            int citas = Convert.ToInt32(await cmdCita.ExecuteScalarAsync());

            // =========================
            // SI YA EXISTE CITA
            // =========================

            if (citas > 0)
                return false;

            return true;
        }


        // =========================
        // INSERTAR
        // =========================
        public async Task<string> RegistrarAsync(Cita c)
        {
            using var cn = new SqlConnection(_connection);
            using var cmd = new SqlCommand("sp_insertarCita", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Id_Medico", c.Id_Medico);
            cmd.Parameters.AddWithValue("@Id_Paciente", c.Id_Paciente);
            cmd.Parameters.AddWithValue("@Id_Usuario", c.Id_Usuario);
            cmd.Parameters.AddWithValue("@Fecha", c.Fecha);
            cmd.Parameters.AddWithValue("@Hora", c.Hora);
            cmd.Parameters.AddWithValue("@Motivo", (object?)c.Motivo ?? DBNull.Value);

            await cn.OpenAsync();
            return (await cmd.ExecuteScalarAsync())?.ToString();
        }

        // =========================
        // ACTUALIZAR
        // =========================
        public async Task<string> ActualizarAsync(Cita c)
        {
            using var cn = new SqlConnection(_connection);
            using var cmd = new SqlCommand("sp_actualizarCita", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Id_Cita", c.Id_Cita);
            cmd.Parameters.AddWithValue("@Fecha", c.Fecha);
            cmd.Parameters.AddWithValue("@Hora", c.Hora);
            cmd.Parameters.AddWithValue("@Motivo", (object?)c.Motivo ?? DBNull.Value);

            await cn.OpenAsync();
            return (await cmd.ExecuteScalarAsync())?.ToString();
        }

        // =========================
        // ELIMINAR
        // =========================
        public async Task<string> EliminarAsync(int id)
        {
            using var cn = new SqlConnection(_connection);
            using var cmd = new SqlCommand("sp_eliminarCita", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Id_Cita", id);

            await cn.OpenAsync();
            return (await cmd.ExecuteScalarAsync())?.ToString();
        }

        // =========================
        // OBTENER POR ID
        // =========================
        public async Task<Cita?> ObtenerPorIdAsync(int id)
        {
            using var cn = new SqlConnection(_connection);
            using var cmd = new SqlCommand("sp_obtenerCitaPorId", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Id_Cita", id);

            await cn.OpenAsync();
            var dr = await cmd.ExecuteReaderAsync();

            if (await dr.ReadAsync())
                return MapearCompleto(dr);

            return null;
        }

        // =========================
        // HORARIOS OCUPADOS
        // =========================
        public async Task<List<Cita>> HorariosOcupadosAsync(int idMedico, DateTime fecha)
        {
            var lista = new List<Cita>();

            using var cn = new SqlConnection(_connection);
            using var cmd = new SqlCommand("sp_horariosOcupados", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Id_Medico", idMedico);
            cmd.Parameters.AddWithValue("@Fecha", fecha);

            await cn.OpenAsync();
            var dr = await cmd.ExecuteReaderAsync();

            while (await dr.ReadAsync())
            {
                lista.Add(new Cita
                {
                    Hora = dr["Hora"] != DBNull.Value ? (TimeSpan)dr["Hora"] : TimeSpan.Zero,
                    Motivo = dr["Motivo"]?.ToString(),

                    Paciente = new Paciente
                    {
                        Nombres = dr["Nombres"]?.ToString(),
                        Apellidos = dr["Apellidos"]?.ToString()
                    }
                });
            }

            return lista;
        }




        // =========================
        // LISTAR POR ESTADO
        // ========================
        public async Task<List<Cita>> ListarPorEstadoAsync(string estado)
        {
            var lista = new List<Cita>();

            using var cn = new SqlConnection(_connection);
            using var cmd = new SqlCommand("sp_listarCitasPorEstado", cn)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("@Estado", estado);

            await cn.OpenAsync();
            using var dr = await cmd.ExecuteReaderAsync();

            while (await dr.ReadAsync())
                lista.Add(MapearCompleto(dr));

            return lista;
        }

        // =========================
        // PENDIENTES POR PACIENTE
        // =========================

        public async Task<List<Cita>> PendientesPorPacienteAsync(string dato)
        {
            var lista = new List<Cita>();

            using var cn = new SqlConnection(_connection);
            using var cmd = new SqlCommand("sp_buscarPendientesPaciente", cn)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("@Dato", dato);

            await cn.OpenAsync();
            using var dr = await cmd.ExecuteReaderAsync();

            while (await dr.ReadAsync())
                lista.Add(MapearCompleto(dr));

            return lista;
        }

        // =========================
        // ACTUALIZAR ESTADO
        // =========================
        public async Task<string> ActualizarEstadoAsync(int idCita, EstadoCita estado)
        {
            using var cn = new SqlConnection(_connection);
            using var cmd = new SqlCommand("sp_actualizarEstadoCita", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Id_Cita", idCita);
            cmd.Parameters.AddWithValue("@Estado", estado.ToString());

            await cn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();


            return result?.ToString() ?? "Error";
        }

        // =========================
        // ACTUALIZAR ESTADO AUTOMATICO
        // =========================

        public async Task ActualizarEstadosAutomaticosAsync()
        {
            using var cn = new SqlConnection(_connection);
            using var cmd = new SqlCommand("sp_actualizarEstadosCitasCompleto", cn)
            { CommandType = CommandType.StoredProcedure };

            await cn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        //Metodo auxiliar para ignorar las columnos si el metodo no lo usa
        private bool ExisteColumna(SqlDataReader dr, string columna)
        {
            for (int i = 0; i < dr.FieldCount; i++)
            {
                if (dr.GetName(i).Equals(columna, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }


    }
}