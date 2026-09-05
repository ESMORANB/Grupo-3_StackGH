// ============================================================
// MODULO 3 - BUSQUEDA Y LISTADO
// Proyecto: Sistema de Expedientes Academicos (XML)
// Curso: Manejo e Implementacion de Archivos - 2026S2
// ============================================================
// Responsabilidad de este modulo:
//   - Cargar el XML de expedientes en memoria.
//   - Construir un indice (Dictionary) por carne para busqueda O(1),
//     en vez de recorrer todo el XML linealmente cada vez.
//   - Listar todos los expedientes.
//   - Listar filtrando por curso o por estado de curso.
//
// IMPORTANTE: Las clases Expediente y Curso deben coincidir EXACTO
// con las que use la persona 1 (modelo de datos). Si su estructura
// cambia, ajustar aqui tambien.
// ============================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ExpedientesAcademicos
{
    // ---------------------------------------------------------
    // MODELO (debe ser el mismo que usa el resto del equipo)
    // ---------------------------------------------------------
    public class Curso
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public int Nota { get; set; }
        public string Estado { get; set; } // "Aprobado", "Reprobado", "En curso", etc.
    }

    public class Expediente
    {
        public string Carne { get; set; }
        public string Nombre { get; set; }
        public string Carrera { get; set; }
        public List<Curso> Cursos { get; set; } = new List<Curso>();
    }

    // ---------------------------------------------------------
    // MODULO DE BUSQUEDA
    // ---------------------------------------------------------
    public class ModuloBusqueda
    {
        private readonly string rutaXml;

        // El indice es el corazon de este modulo: carne -> Expediente.
        // Esto es lo que le va a gustar al catedratico cuando pregunte
        // "y donde esta el concepto de indices para recuperacion".
        private Dictionary<string, Expediente> indicePorCarne;
        private List<Expediente> expedientes;

        public ModuloBusqueda(string rutaXml)
        {
            this.rutaXml = rutaXml;
            this.expedientes = new List<Expediente>();
            this.indicePorCarne = new Dictionary<string, Expediente>();
        }

        // -----------------------------------------------------
        // Carga el XML completo a memoria y construye el indice.
        // Se debe llamar antes de cualquier busqueda/listado.
        // -----------------------------------------------------
        public void CargarDesdeXml()
        {
            expedientes.Clear();
            indicePorCarne.Clear();

            try
            {
                if (!System.IO.File.Exists(rutaXml))
                {
                    throw new System.IO.FileNotFoundException(
                        $"No se encontro el archivo XML en la ruta: {rutaXml}");
                }

                XDocument doc = XDocument.Load(rutaXml);

                foreach (XElement expEl in doc.Root.Elements("expediente"))
                {
                    Expediente exp = new Expediente
                    {
                        Carne = (string)expEl.Element("carne"),
                        Nombre = (string)expEl.Element("nombre"),
                        Carrera = (string)expEl.Element("carrera")
                    };

                    XElement cursosEl = expEl.Element("cursos");
                    if (cursosEl != null)
                    {
                        foreach (XElement cursoEl in cursosEl.Elements("curso"))
                        {
                            exp.Cursos.Add(new Curso
                            {
                                Codigo = (string)cursoEl.Element("codigo"),
                                Nombre = (string)cursoEl.Element("nombre"),
                                Nota = (int?)cursoEl.Element("nota") ?? 0,
                                Estado = (string)cursoEl.Element("estado")
                            });
                        }
                    }

                    expedientes.Add(exp);

                    // Si hay carnes duplicados en el XML, algo esta mal con
                    // la validacion del modulo de registro. Avisamos pero
                    // no tronamos el programa.
                    if (!indicePorCarne.ContainsKey(exp.Carne))
                    {
                        indicePorCarne.Add(exp.Carne, exp);
                    }
                    else
                    {
                        Console.WriteLine($"[AVISO] Carne duplicado detectado en el XML: {exp.Carne}");
                    }
                }
            }
            catch (System.Xml.XmlException ex)
            {
                Console.WriteLine("[ERROR] El archivo XML esta mal formado: " + ex.Message);
            }
            catch (System.IO.FileNotFoundException ex)
            {
                Console.WriteLine("[ERROR] " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] Ocurrio un problema leyendo el XML: " + ex.Message);
            }
        }

        // -----------------------------------------------------
        // Busqueda por identificador unico (carne). O(1) gracias
        // al diccionario en vez de recorrer la lista completa.
        // -----------------------------------------------------
        public Expediente BuscarPorCarne(string carne)
        {
            if (string.IsNullOrWhiteSpace(carne))
            {
                Console.WriteLine("[ERROR] Debe indicar un carne para buscar.");
                return null;
            }

            if (indicePorCarne.TryGetValue(carne, out Expediente encontrado))
            {
                return encontrado;
            }

            Console.WriteLine($"[INFO] No existe ningun expediente con carne {carne}.");
            return null;
        }

        // -----------------------------------------------------
        // Listado completo de expedientes.
        // -----------------------------------------------------
        public List<Expediente> ListarTodos()
        {
            return expedientes;
        }

        // -----------------------------------------------------
        // Listado filtrado: expedientes que tengan un curso
        // especifico (por codigo de curso).
        // -----------------------------------------------------
        public List<Expediente> ListarPorCurso(string codigoCurso)
        {
            return expedientes
                .Where(e => e.Cursos.Any(c => c.Codigo.Equals(codigoCurso, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }

        // -----------------------------------------------------
        // Listado filtrado: expedientes que tengan al menos un
        // curso en un estado especifico (ej. "Reprobado").
        // -----------------------------------------------------
        public List<Expediente> ListarPorEstado(string estado)
        {
            return expedientes
                .Where(e => e.Cursos.Any(c => c.Estado.Equals(estado, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }

        // -----------------------------------------------------
        // Utilidad para mostrar un expediente bonito en consola.
        // -----------------------------------------------------
        public void ImprimirExpediente(Expediente exp)
        {
            if (exp == null) return;

            Console.WriteLine("==================================================");
            Console.WriteLine($"Carne:   {exp.Carne}");
            Console.WriteLine($"Nombre:  {exp.Nombre}");
            Console.WriteLine($"Carrera: {exp.Carrera}");
            Console.WriteLine("Cursos:");
            foreach (Curso c in exp.Cursos)
            {
                Console.WriteLine($"   - [{c.Codigo}] {c.Nombre} | Nota: {c.Nota} | Estado: {c.Estado}");
            }
            Console.WriteLine("==================================================");
        }
    }

    // ---------------------------------------------------------
    // PROGRAMA DE PRUEBA STANDALONE
    // Esto es solo para probar el modulo por separado. Cuando se
    // integre con el equipo, este Main se reemplaza por el menu
    // general del sistema (persona 5).
    // ---------------------------------------------------------
    class Program
    {
        static void Main(string[] args)
        {
            string ruta = "expedientes.xml";

            // Si no existe el XML todavia (porque el modulo de registro
            // no ha corrido), generamos uno de prueba para no depender
            // de nadie mas mientras probamos este modulo.
            if (!System.IO.File.Exists(ruta))
            {
                GenerarXmlDePrueba(ruta);
                Console.WriteLine("[INFO] No existia XML, se genero uno de prueba: " + ruta);
            }

            ModuloBusqueda modulo = new ModuloBusqueda(ruta);
            modulo.CargarDesdeXml();

            bool salir = false;
            while (!salir)
            {
                Console.WriteLine("\n---- MODULO DE BUSQUEDA Y LISTADO ----");
                Console.WriteLine("1. Buscar por carne");
                Console.WriteLine("2. Listar todos los expedientes");
                Console.WriteLine("3. Listar por curso (codigo)");
                Console.WriteLine("4. Listar por estado de curso");
                Console.WriteLine("5. Recargar XML");
                Console.WriteLine("0. Salir");
                Console.Write("Opcion: ");
                string opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1":
                        Console.Write("Carne a buscar: ");
                        string carne = Console.ReadLine();
                        Expediente exp = modulo.BuscarPorCarne(carne);
                        modulo.ImprimirExpediente(exp);
                        break;

                    case "2":
                        var todos = modulo.ListarTodos();
                        Console.WriteLine($"Total de expedientes: {todos.Count}");
                        foreach (var e in todos) modulo.ImprimirExpediente(e);
                        break;

                    case "3":
                        Console.Write("Codigo de curso: ");
                        string cod = Console.ReadLine();
                        var porCurso = modulo.ListarPorCurso(cod);
                        Console.WriteLine($"Expedientes con el curso {cod}: {porCurso.Count}");
                        foreach (var e in porCurso) modulo.ImprimirExpediente(e);
                        break;

                    case "4":
                        Console.Write("Estado a buscar (Aprobado/Reprobado/En curso): ");
                        string estado = Console.ReadLine();
                        var porEstado = modulo.ListarPorEstado(estado);
                        Console.WriteLine($"Expedientes con curso en estado '{estado}': {porEstado.Count}");
                        foreach (var e in porEstado) modulo.ImprimirExpediente(e);
                        break;

                    case "5":
                        modulo.CargarDesdeXml();
                        Console.WriteLine("[INFO] XML recargado.");
                        break;

                    case "0":
                        salir = true;
                        break;

                    default:
                        Console.WriteLine("Opcion invalida.");
                        break;
                }
            }
        }

        // Genera un XML de prueba con 3 expedientes para poder
        // correr este modulo sin depender de los demas todavia.
        static void GenerarXmlDePrueba(string ruta)
        {
            XDocument doc = new XDocument(
                new XElement("expedientes",
                    new XElement("expediente",
                        new XElement("carne", "1119452"),
                        new XElement("nombre", "Eduardo Test"),
                        new XElement("carrera", "Ingenieria en Informatica"),
                        new XElement("cursos",
                            new XElement("curso",
                                new XElement("codigo", "MIA101"),
                                new XElement("nombre", "Manejo e Implementacion de Archivos"),
                                new XElement("nota", 90),
                                new XElement("estado", "Aprobado")),
                            new XElement("curso",
                                new XElement("codigo", "EDD202"),
                                new XElement("nombre", "Estructura de Datos II"),
                                new XElement("nota", 55),
                                new XElement("estado", "Reprobado")))),
                    new XElement("expediente",
                        new XElement("carne", "1119999"),
                        new XElement("nombre", "Ana Ejemplo"),
                        new XElement("carrera", "Ingenieria Industrial"),
                        new XElement("cursos",
                            new XElement("curso",
                                new XElement("codigo", "MIA101"),
                                new XElement("nombre", "Manejo e Implementacion de Archivos"),
                                new XElement("nota", 75),
                                new XElement("estado", "Aprobado"))))
                )
            );

            doc.Save(ruta);
        }
    }
}
