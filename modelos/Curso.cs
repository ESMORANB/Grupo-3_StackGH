using System;
using System.Xml.Serialization;

namespace Proyecto1.Modelos
{
    [XmlType("curso")]
    public class Curso
    {
        private string codigo;
        private string nombre;
        private int creditos;
        private double nota;
        private string ciclo;

        public Curso()
        {
            codigo = string.Empty;
            nombre = string.Empty;
            ciclo = string.Empty;
        }

        public Curso(string codigo, string nombre, int creditos, double nota, string ciclo)
        {
            Codigo = codigo;
            Nombre = nombre;
            Creditos = creditos;
            Nota = nota;
            Ciclo = ciclo;
        }

        /// Código del curso
        [XmlElement("codigo")]
        public string Codigo
        {
            get => codigo;
            set => codigo = string.IsNullOrWhiteSpace(value)
                ? throw new ArgumentException("El código del curso es obligatorio.")
                : value.Trim().ToUpper();
        }

        /// Nombre del curso
        [XmlElement("nombre")]
        public string Nombre
        {
            get => nombre;
            set => nombre = string.IsNullOrWhiteSpace(value)
                ? throw new ArgumentException("El nombre del curso es obligatorio.")
                : value.Trim();
        }

        /// Cantidad de creditos del curso
        [XmlElement("creditos")]
        public int Creditos
        {
            get => creditos;
            set => creditos = value > 0
                ? value
                : throw new ArgumentOutOfRangeException(
                    nameof(Creditos),
                    "Los créditos deben ser mayores que cero.");
        }

        /// Nota obtenida en el curso. Debe estar entre 0 y 100
        [XmlElement("nota")]
        public double Nota
        {
            get => nota;
            set => nota = (value >= 0 && value <= 100)
                ? value
                : throw new ArgumentOutOfRangeException(
                    nameof(Nota),
                    "La nota debe estar entre 0 y 100.");
        }

        /// Ciclo academico en el que se cursó
        [XmlElement("ciclo")]
        public string Ciclo
        {
            get => ciclo;
            set => ciclo = value?.Trim() ?? string.Empty;
        }

        /// Indica si el estudiante aprobo o reprobo el curso
        /// Este dato se calcula y no se guarda en el XML
        [XmlIgnore]
        public string Estado
        {
            get
            {
                if (Nota >= 61)
                    return "Aprobado";

                return "Reprobado";
            }
        }

        /// Muestra la informacion del curso
        public override string ToString()
        {
            return $"{Codigo} - {Nombre} | Créditos: {Creditos} | Nota: {Nota:0.##} ({Estado})";
        }
    }
}