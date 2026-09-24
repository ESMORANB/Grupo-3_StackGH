// Modulo 4 - Actualizacion, eliminacion y validaciones. Usa el modelo real
// (Proyecto1.Modelos) y el Encriptador de persona 5. Los setters del modelo
// ya validan formato (lanzan ArgumentException/ArgumentOutOfRangeException);
// aqui solo se valida que el registro exista antes de tocarlo.

using System;
using System.IO;
using System.Xml.Serialization;
using ExpedientesSimple;
using Proyecto1.Modelos;

namespace ExpedientesAcademicos
{
    public class RegistroNoExisteException : Exception
    {
        public RegistroNoExisteException(string mensaje) : base(mensaje) { }
    }

    public class ModuloActualizacionEliminacion
    {
        private readonly string rutaXmlEncriptado;
        private readonly string contrasena;
        private readonly Encriptador encriptador;
        private Expedientes datos;

        public ModuloActualizacionEliminacion(string rutaXmlEncriptado, string contrasena)
        {
            this.rutaXmlEncriptado = rutaXmlEncriptado;
            this.contrasena = contrasena;
            encriptador = new Encriptador();
        }

        public void CargarDesdeXml()
        {
            try
            {
                if (!File.Exists(rutaXmlEncriptado))
                    throw new FileNotFoundException($"No se encontro el archivo: {rutaXmlEncriptado}");

                string xmlTexto = encriptador.DesencriptarArchivo(rutaXmlEncriptado, contrasena);
                XmlSerializer serializer = new XmlSerializer(typeof(Expedientes));
                using StringReader reader = new StringReader(xmlTexto);
                datos = (Expedientes)serializer.Deserialize(reader);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] No se pudo cargar el XML: " + ex.Message);
                datos = new Expedientes();
            }
        }

        // Guarda en un temporal en texto plano y lo vuelve a encriptar sobre la
        // ruta final. El temporal se borra siempre, incluso si algo falla.
        private void GuardarXml()
        {
            string temp = rutaXmlEncriptado + ".tmp";
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Expedientes));
                using (StreamWriter writer = new StreamWriter(temp))
                    serializer.Serialize(writer, datos);

                encriptador.EncriptarArchivo(temp, rutaXmlEncriptado, contrasena);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] No se pudo guardar el XML: " + ex.Message);
            }
            finally
            {
                if (File.Exists(temp)) File.Delete(temp);
            }
        }

        private Expediente ObtenerExpedienteOTronar(string codigo)
        {
            Expediente exp = datos.BuscarExpediente(codigo);
            if (exp == null)
                throw new RegistroNoExisteException($"No existe expediente con codigo {codigo}.");
            return exp;
        }

        public bool ActualizarExpediente(string codigo, string nombre, string apellido, string carrera, int semestre, string correo)
        {
            try
            {
                Expediente exp = ObtenerExpedienteOTronar(codigo);
                exp.Nombre = nombre;
                exp.Apellido = apellido;
                exp.Carrera = carrera;
                exp.Semestre = semestre;
                exp.Correo = correo;
                GuardarXml();
                Console.WriteLine($"[OK] Expediente {codigo} actualizado.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] " + ex.Message);
                return false;
            }
        }

        // Actualiza el curso si ya existe en el expediente; si no, lo agrega.
        public bool ActualizarCurso(string codigo, string codigoCurso, string nombreCurso, int creditos, double nota, string ciclo)
        {
            try
            {
                Expediente exp = ObtenerExpedienteOTronar(codigo);
                Curso curso = exp.BuscarCurso(codigoCurso);

                if (curso != null)
                {
                    curso.Nombre = nombreCurso;
                    curso.Creditos = creditos;
                    curso.Nota = nota;
                    curso.Ciclo = ciclo;
                    Console.WriteLine($"[OK] Curso {codigoCurso} actualizado en {codigo}.");
                }
                else
                {
                    exp.AgregarCurso(new Curso(codigoCurso, nombreCurso, creditos, nota, ciclo));
                    Console.WriteLine($"[OK] Curso {codigoCurso} agregado a {codigo}.");
                }

                GuardarXml();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] " + ex.Message);
                return false;
            }
        }

        public bool EliminarExpediente(string codigo)
        {
            try
            {
                Expediente exp = ObtenerExpedienteOTronar(codigo);
                datos.Lista.Remove(exp);
                GuardarXml();
                Console.WriteLine($"[OK] Expediente {codigo} eliminado.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] " + ex.Message);
                return false;
            }
        }

        public bool EliminarCurso(string codigo, string codigoCurso)
        {
            try
            {
                Expediente exp = ObtenerExpedienteOTronar(codigo);
                Curso curso = exp.BuscarCurso(codigoCurso);
                if (curso == null)
                {
                    Console.WriteLine($"[ERROR] {codigo} no tiene el curso {codigoCurso}.");
                    return false;
                }
                exp.Cursos.Remove(curso);
                GuardarXml();
                Console.WriteLine($"[OK] Curso {codigoCurso} eliminado de {codigo}.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] " + ex.Message);
                return false;
            }
        }
    }

    // Main de prueba desactivado: el proyecto integrado solo puede tener UN Main.
    /*
    class Program
    {
        static void Main(string[] args)
        {
            string contrasena = "claveDelEquipo123";
            string rutaEncriptado = "expedientes.xml.enc";

            ModuloActualizacionEliminacion modulo = new ModuloActualizacionEliminacion(rutaEncriptado, contrasena);
            modulo.CargarDesdeXml();

            Console.Write("Codigo del expediente a actualizar: ");
            string codigo = Console.ReadLine();
            modulo.ActualizarCurso(codigo, "MIA101", "Manejo e Implementacion de Archivos", 3, 95, "2026-S2");
        }
    }
    */
}
