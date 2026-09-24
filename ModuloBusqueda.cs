// Modulo 3 - Busqueda y listado. Usa el modelo real (Proyecto1.Modelos)
// y el Encriptador de persona 5. El XML en disco siempre esta encriptado;
// aqui solo se desencripta para leer, nunca se reescribe.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using ExpedientesSimple;
using Proyecto1.Modelos;

namespace ExpedientesAcademicos
{
    public class ModuloBusqueda
    {
        private readonly string rutaXmlEncriptado;
        private readonly string contrasena;
        private readonly Encriptador encriptador;

        private Expedientes datos;

        // Indice en memoria: codigoEstudiante -> Expediente, para busqueda O(1)
        private Dictionary<string, Expediente> indicePorCodigo;

        public ModuloBusqueda(string rutaXmlEncriptado, string contrasena)
        {
            this.rutaXmlEncriptado = rutaXmlEncriptado;
            this.contrasena = contrasena;
            encriptador = new Encriptador();
            indicePorCodigo = new Dictionary<string, Expediente>(StringComparer.OrdinalIgnoreCase);
        }

        public void CargarDesdeXml()
        {
            indicePorCodigo.Clear();
            try
            {
                if (!File.Exists(rutaXmlEncriptado))
                    throw new FileNotFoundException($"No se encontro el archivo: {rutaXmlEncriptado}");

                string xmlTexto = encriptador.DesencriptarArchivo(rutaXmlEncriptado, contrasena);
                XmlSerializer serializer = new XmlSerializer(typeof(Expedientes));
                using StringReader reader = new StringReader(xmlTexto);
                datos = (Expedientes)serializer.Deserialize(reader);

                foreach (Expediente exp in datos.Lista)
                {
                    if (!indicePorCodigo.ContainsKey(exp.CodigoEstudiante))
                        indicePorCodigo.Add(exp.CodigoEstudiante, exp);
                }
            }
            catch (Exception ex)
            {
                // Cubre: archivo no encontrado, contrasena incorrecta, XML mal formado.
                Console.WriteLine("[ERROR] No se pudo cargar el XML: " + ex.Message);
                datos = new Expedientes();
            }
        }

        public Expediente BuscarPorCodigo(string codigo)
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

        public void ImprimirExpediente(Expediente exp)
        {
            if (exp == null) return;
            Console.WriteLine("==================================================");
            Console.WriteLine(exp.ToString());
            foreach (Curso c in exp.Cursos)
                Console.WriteLine("   - " + c);
            Console.WriteLine("==================================================");
        }
    }

    // Main de prueba desactivado: el proyecto integrado solo puede tener UN Main
    // (el del menu general de persona 5). Descomentar solo para probar este
    // modulo por separado.
    /*
    class Program
    {
        static void Main(string[] args)
        {
            string rutaPlano = "expedientes.xml";
            string rutaEncriptado = "expedientes.xml.enc";
            string contrasena = "claveDelEquipo123";

            if (!File.Exists(rutaEncriptado))
            {
                Expedientes datos = new Expedientes();
                Expediente e1 = new Expediente("1119452", "Eduardo", "Moran", "Ingenieria en Informatica", 8, "eduardo@url.edu.gt");
                e1.AgregarCurso(new Curso("MIA101", "Manejo e Implementacion de Archivos", 3, 90, "2026-S2"));
                e1.AgregarCurso(new Curso("EDD202", "Estructura de Datos II", 4, 55, "2025-S1"));
                datos.AgregarExpediente(e1);

                XmlSerializer serializer = new XmlSerializer(typeof(Expedientes));
                using (StreamWriter writer = new StreamWriter(rutaPlano))
                    serializer.Serialize(writer, datos);

                new Encriptador().EncriptarArchivo(rutaPlano, rutaEncriptado, contrasena);
                File.Delete(rutaPlano);
                Console.WriteLine("[INFO] XML de prueba generado y encriptado.");
            }

            ModuloBusqueda modulo = new ModuloBusqueda(rutaEncriptado, contrasena);
            modulo.CargarDesdeXml();

            Console.Write("Codigo a buscar: ");
            string codigo = Console.ReadLine();
            modulo.ImprimirExpediente(modulo.BuscarPorCodigo(codigo));

            Console.WriteLine("\nTodos los expedientes:");
            foreach (var e in modulo.ListarTodos()) modulo.ImprimirExpediente(e);
        }
    }
    */
}
