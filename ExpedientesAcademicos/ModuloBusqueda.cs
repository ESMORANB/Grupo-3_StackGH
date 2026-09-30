// Modulo 3 - Busqueda y listado. Usa el modelo real (Proyecto1.Modelos)
// y el Encriptador de persona 5. El XML en disco siempre esta encriptado;
// aqui solo se desencripta para leer, nunca se reescribe.

using System.Xml.Serialization;
using ExpedientesAcademicos.Encriptacion;
using ExpedientesAcademicos.Modelos;

namespace ExpedientesAcademicos.Busqueda
{
    public class ModuloBusqueda
    {
        private readonly string rutaXmlEncriptado;
        private readonly string contrasena;
        private readonly Encriptador encriptador;

        private Expedientes datos = new Expedientes();

        // Indice en memoria: codigoEstudiante -> Expediente, para busqueda O(1)
        private Dictionary<string, Expediente> indicePorCodigo;

        public ModuloBusqueda(string rutaXmlEncriptado, string contrasena)
        {
            this.rutaXmlEncriptado = rutaXmlEncriptado;
            this.contrasena = contrasena;
            encriptador = new Encriptador(contrasena);
            indicePorCodigo = new Dictionary<string, Expediente>(StringComparer.OrdinalIgnoreCase);
        }

        public void CargarDesdeXml()
        {
            indicePorCodigo.Clear();
            try
            {
                if (!File.Exists(rutaXmlEncriptado))
                    throw new FileNotFoundException($"No se encontro el archivo: {rutaXmlEncriptado}");

                string xmlTexto = encriptador.DesencriptarArchivo(rutaXmlEncriptado);
                XmlSerializer serializer = new XmlSerializer(typeof(Expedientes));
                using StringReader reader = new StringReader(xmlTexto);
                datos = serializer.Deserialize(reader) as Expedientes ?? new Expedientes();

                foreach (Expediente exp in datos.Lista)
                {
                    if (!indicePorCodigo.ContainsKey(exp.CodigoEstudiante))
                        indicePorCodigo.Add(exp.CodigoEstudiante, exp);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] No se pudo cargar el XML: " + ex.Message);
                datos = new Expedientes();
            }
        }

        public Expediente? BuscarPorCodigo(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
            {
                Console.WriteLine("[ERROR] Debe indicar un codigo.");
                return null;
            }

            if (indicePorCodigo.TryGetValue(codigo, out Expediente encontrado))
                return encontrado;

            Console.WriteLine($"[INFO] No existe expediente con codigo {codigo}.");
            return null;
        }

        public void ImprimirExpediente(Expediente? exp)
        {
            if (exp == null) return;
            Console.WriteLine("==================================================");
            Console.WriteLine(exp.ToString());
            foreach (Curso c in exp.Cursos)
                Console.WriteLine("   - " + c);
            Console.WriteLine("==================================================");
}

        public List<Expediente> ListarTodos() => datos.Lista;

        public List<Expediente> ListarPorCurso(string codigoCurso)
        {
            return datos.Lista.Where(e => e.BuscarCurso(codigoCurso) != null).ToList();
        }

        // estado: "Aprobado" o "Reprobado" (se calcula por nota, no se guarda en XML)
        public List<Expediente> ListarPorEstadoCurso(string estado)
        {
            return datos.Lista
                .Where(e => e.Cursos.Any(c => c.Estado.Equals(estado, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }

    }
}
