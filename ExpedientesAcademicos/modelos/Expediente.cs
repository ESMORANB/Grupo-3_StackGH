using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace ExpedientesAcademicos.Modelos
{
    [XmlType("expediente")]
    public class Expediente
    {
        private string codigoEstudiante;
        private string nombre;
        private string apellido;
        private string carrera;
        private int semestre;
        private string correo;
        private List<Curso> cursos;

        public Expediente()
        {
            codigoEstudiante = string.Empty;
            nombre = string.Empty;
            apellido = string.Empty;
            carrera = string.Empty;
            correo = string.Empty;
            cursos = new List<Curso>();

            FechaIngreso = DateTime.Today;
            EstadoEstudiante = "Activo";
        }

        public Expediente(
            string codigoEstudiante,
            string nombre,
            string apellido,
            string carrera,
            int semestre,
            string correo) : this()
        {
            CodigoEstudiante = codigoEstudiante;
            Nombre = nombre;
            Apellido = apellido;
            Carrera = carrera;
            Semestre = semestre;
            Correo = correo;
        }

        /// Código del estudiante
        [XmlElement("codigoEstudiante")]
        public string CodigoEstudiante
        {
            get => codigoEstudiante;
            set => codigoEstudiante = string.IsNullOrWhiteSpace(value)
                ? throw new ArgumentException(
                    "El código de estudiante es obligatorio.")
                : value.Trim();
        }

        /// Nombre del estudiante
        [XmlElement("nombre")]
        public string Nombre
        {
            get => nombre;
            set => nombre = string.IsNullOrWhiteSpace(value)
                ? throw new ArgumentException(
                    "El nombre es obligatorio.")
                : value.Trim();
        }

        /// Apellido del estudiante
        [XmlElement("apellido")]
        public string Apellido
        {
            get => apellido;
            set => apellido = string.IsNullOrWhiteSpace(value)
                ? throw new ArgumentException(
                    "El apellido es obligatorio.")
                : value.Trim();
        }

        /// Carrera que estudia el estudiante
        [XmlElement("carrera")]
        public string Carrera
        {
            get => carrera;
            set => carrera = string.IsNullOrWhiteSpace(value)
                ? throw new ArgumentException(
                    "La carrera es obligatoria.")
                : value.Trim();
        }

        /// Semestre actual del estudiante
        [XmlElement("semestre")]
        public int Semestre
        {
            get => semestre;
            set => semestre = (value >= 1 && value <= 12)
                ? value
                : throw new ArgumentOutOfRangeException(
                    nameof(Semestre),
                    "El semestre debe estar entre 1 y 12.");
        }

        /// Correo del estudiante
        [XmlElement("correo")]
        public string Correo
        {
            get => correo;
            set
            {
                if (string.IsNullOrWhiteSpace(value) ||
                    !value.Contains("@") ||
                    !value.Contains("."))
                {
                    throw new ArgumentException(
                        "El correo no tiene un formato válido.");
                }

                correo = value.Trim().ToLower();
            }
        }

        /// Fecha en que el estudiante ingresó a la universidad
        [XmlElement("fechaIngreso", DataType = "date")]
        public DateTime FechaIngreso { get; set; }

        /// Estado actual del estudiante
        [XmlElement("estado")]
        public string EstadoEstudiante { get; set; }

        /// Lista de cursos que forman parte del expediente
        [XmlArray("cursos")]
        [XmlArrayItem("curso")]
        public List<Curso> Cursos
        {
            get => cursos;
            set => cursos = value ?? new List<Curso>();
        }

        /// Nombre completo del estudiante
        /// Este valor no se guarda en el XML porque se puede obtener uniendo el nombre y apellido
        [XmlIgnore]
        public string NombreCompleto
        {
            get => $"{Nombre} {Apellido}";
        }

        /// Calcula el promedio del estudiante tomando en cuenta los créditos de cada curso
        [XmlIgnore]
        public double Promedio
        {
            get
            {
                if (Cursos == null || Cursos.Count == 0)
                    return 0;

                int totalCreditos = Cursos.Sum(c => c.Creditos);

                if (totalCreditos == 0)
                    return 0;

                return Cursos.Sum(c => c.Nota * c.Creditos)
                       / totalCreditos;
            }
        }

        /// Agrega un curso al expediente
        /// No permite repetir el codigo de un curso
        public void AgregarCurso(Curso curso)
        {
            if (curso == null)
            {
                throw new ArgumentNullException(
                    nameof(curso),
                    "El curso no puede ser nulo.");
            }

            if (Cursos.Any(c =>
                c.Codigo.Equals(
                    curso.Codigo,
                    StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException(
                    $"El curso {curso.Codigo} ya existe en el expediente.");
            }

            Cursos.Add(curso);
        }

        /// Busca un curso por su codigo
        public Curso? BuscarCurso(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return null;

            return Cursos.FirstOrDefault(c =>
                c.Codigo.Equals(
                    codigo.Trim(),
                    StringComparison.OrdinalIgnoreCase));
        }

        /// Comprueba que el expediente tenga sus datos principales
        public bool EsValido(out string mensajeError)
        {
            if (string.IsNullOrWhiteSpace(CodigoEstudiante))
            {
                mensajeError = "Falta el código del estudiante.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Nombre))
            {
                mensajeError = "Falta el nombre.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Apellido))
            {
                mensajeError = "Falta el apellido.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Carrera))
            {
                mensajeError = "Falta la carrera.";
                return false;
            }

            if (Semestre < 1 || Semestre > 12)
            {
                mensajeError = "El semestre no es válido.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Correo))
            {
                mensajeError = "Falta el correo.";
                return false;
            }

            mensajeError = string.Empty;
            return true;
        }

        /// Muestra la informacion principal del expediente
        public override string ToString()
        {
            return $"[{CodigoEstudiante}] {NombreCompleto} | {Carrera} | " +
                   $"Semestre {Semestre} | Cursos: {Cursos.Count} | " +
                   $"Promedio: {Promedio:0.00}";
        }
    }
}