using Microsoft.Data.SqlClient;
using ProyectoSantaMonica_Cesar.Models;
using ProyectoSantaMonica_Cesar.Models.enums;
using System.Data;

namespace ProyectoSantaMonica_Cesar.Repository
{
    public class ComprobanteDePagoRepository
    {
        private readonly string cadena;

        public ComprobanteDePagoRepository(IConfiguration config)
        {
            cadena = config.GetConnectionString("CibertecConnection");
        }

        public async Task InsertarAsync(ComprobanteDePago c)
        {
            using SqlConnection cn = new SqlConnection(cadena);
            using SqlCommand cmd = new SqlCommand("sp_InsertarComprobante", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Id_Cita", c.Id_Cita);
            cmd.Parameters.AddWithValue("@Nombre_Pagador", c.Nombre_Pagador);
            cmd.Parameters.AddWithValue("@Apellidos_Pagador", c.Apellidos_Pagador);
            cmd.Parameters.AddWithValue("@Dni_Pagador", (object?)c.Dni_Pagador ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Contacto_Pagador", (object?)c.Contacto_Pagador ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Monto", c.Monto);
            cmd.Parameters.AddWithValue("@Metodo_Pago", c.Metodo_Pago);

            await cn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<ComprobanteDePago>> ListarAsync()
        {
            List<ComprobanteDePago> lista = new();

            using SqlConnection cn = new SqlConnection(cadena);
            using SqlCommand cmd = new SqlCommand("sp_ListarComprobantesDetallado", cn);

            await cn.OpenAsync();
            SqlDataReader dr = await cmd.ExecuteReaderAsync();

            while (await dr.ReadAsync())
            {
                lista.Add(Mapear1(dr));
            }

            return lista;
        }

        public async Task<List<ComprobanteDePago>> BuscarAsync(string dato)
        {
            List<ComprobanteDePago> lista = new();

            using SqlConnection cn = new SqlConnection(cadena);
            using SqlCommand cmd = new SqlCommand("sp_BuscarComprobante", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@dato", dato);

            await cn.OpenAsync();
            SqlDataReader dr = await cmd.ExecuteReaderAsync();

            while (await dr.ReadAsync())
            {
                lista.Add(Mapear(dr));
            }

            return lista;
        }

        public async Task<ComprobanteDePago?> BuscarPorIdAsync(int id)
        {
            using SqlConnection cn = new SqlConnection(cadena);
            using SqlCommand cmd = new SqlCommand("sp_BuscarComprobantePorId1", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Id", id);

            await cn.OpenAsync();
            SqlDataReader dr = await cmd.ExecuteReaderAsync();

            if (await dr.ReadAsync())
                return MapearDetalle(dr);

            return null;
        }

        public async Task AnularAsync(int id)
        {
            using SqlConnection cn = new SqlConnection(cadena);
            using SqlCommand cmd = new SqlCommand("sp_AnularComprobante", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Id", id);

            await cn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        private ComprobanteDePago Mapear(SqlDataReader dr)
        {
            return new ComprobanteDePago
            {
                Id_Comprobante = (int)dr["Id_Comprobante"],
                Id_Cita = (int)dr["Id_Cita"],
                Nombre_Pagador = dr["Nombre_Pagador"].ToString(),
                Apellidos_Pagador = dr["Apellidos_Pagador"].ToString(),
                Dni_Pagador = dr["Dni_Pagador"]?.ToString(),
                Contacto_Pagador = dr["Contacto_Pagador"]?.ToString(),
                Fecha_Emision = (DateTime)dr["Fecha_Emision"],
                Monto = (decimal)dr["Monto"],
                Metodo_Pago = dr["Metodo_Pago"].ToString(),
                Estado = Enum.Parse<EstadoComprobante>(dr["Estado"].ToString())
            };

        }


        private ComprobanteDePago Mapear1(SqlDataReader dr)
        {
            return new ComprobanteDePago
            {
                Id_Comprobante = (int)dr["Id_Comprobante"],
                Id_Cita = (int)dr["Id_Cita"],
                Nombre_Pagador = dr["Nombre_Pagador"].ToString(),
                Apellidos_Pagador = dr["Apellidos_Pagador"].ToString(),
                Dni_Pagador = dr["Dni_Pagador"]?.ToString(),
                Contacto_Pagador = dr["Contacto_Pagador"]?.ToString(),
                Fecha_Emision = (DateTime)dr["Fecha_Emision"],
                Monto = (decimal)dr["Monto"],
                Metodo_Pago = dr["Metodo_Pago"].ToString(),
                Estado = Enum.Parse<EstadoComprobante>(dr["Estado"].ToString()),

                // 🔥 AGREGAR OBJETO CITA
                Cita = new Cita
                {
                    Id_Cita = (int)dr["Id_Cita"],
                    Fecha = (DateTime)dr["Fecha_Cita"],
                    Hora = (TimeSpan)dr["Hora"],
                    Motivo = dr["Motivo"].ToString(),

                    Paciente = new Paciente
                    {
                        Nombres = dr["NombrePaciente"].ToString(),
                        Apellidos = dr["ApellidoPaciente"].ToString()
                    },

                    Medico = new Medico
                    {
                        Nombres = dr["NombreMedico"].ToString(),
                        Apellidos = dr["ApellidoMedico"].ToString()
                    }
                }
            };
        }

        public async Task<List<ComprobanteDePago>> BuscarPorPacienteAsync(string dato)
        {
            List<ComprobanteDePago> lista = new();

            using SqlConnection cn = new SqlConnection(cadena);
            using SqlCommand cmd = new SqlCommand("sp_BuscarComprobantePorPaciente", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@dato", dato);

            await cn.OpenAsync();
            using SqlDataReader dr = await cmd.ExecuteReaderAsync();

            while (await dr.ReadAsync())
                lista.Add(Mapear(dr));

            return lista;
        }

        private ComprobanteDePago MapearDetalle(SqlDataReader dr)
        {
            return new ComprobanteDePago
            {
                Id_Comprobante = (int)dr["Id_Comprobante"],
                Id_Cita = (int)dr["Id_Cita"],
                Nombre_Pagador = dr["Nombre_Pagador"].ToString(),
                Apellidos_Pagador = dr["Apellidos_Pagador"].ToString(),
                Dni_Pagador = dr["Dni_Pagador"]?.ToString(),
                Contacto_Pagador = dr["Contacto_Pagador"]?.ToString(),
                Fecha_Emision = (DateTime)dr["Fecha_Emision"],
                Monto = (decimal)dr["Monto"],
                Metodo_Pago = dr["Metodo_Pago"].ToString(),
                Estado = Enum.Parse<EstadoComprobante>(dr["Estado"].ToString()),

                Cita = new Cita
                {
                    Id_Cita = (int)dr["Id_Cita"],
                    Fecha = (DateTime)dr["Fecha_Cita"],
                    Hora = (TimeSpan)dr["Hora"],
                    Motivo = dr["Motivo"].ToString(),

                    Paciente = new Paciente
                    {
                        Nombres = dr["NombrePaciente"].ToString(),
                        Apellidos = dr["ApellidoPaciente"].ToString()
                    },

                    Medico = new Medico
                    {
                        Nombres = dr["NombreMedico"].ToString(),
                        Apellidos = dr["ApellidoMedico"].ToString()
                    },

                    Usuario = new Usuario
                    {
                        Nombres = dr["NombreUsuario"].ToString(),
                        Apellidos = dr["ApellidoUsuario"].ToString()
                    }
                }
            };
        }

    }
}
