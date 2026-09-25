using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace ExpedientesAcademicos.Modelos
{
    /// Representa el conjunto de expedientes académicos almacenados en el archivo XML
    [XmlRoot("expedientes")]
    public class Expedientes
    {
        private List<Expediente> expedientes;

        /// Constructor por defecto
        public Expedientes()
        {
            expedientes = new List<Expediente>();
        }

        /// Lista de expedientes registrados
        [XmlElement("expediente")]
        public List<Expediente> Lista
        {
            get => expedientes;
            set => expedientes = value ?? new List<Expediente>();
        }

        /// Verifica si ya existe un expediente con el código indicado
        public bool ExisteCodigo(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return false;

            return Lista.Any(e =>
                e.CodigoEstudiante.Equals(
                    codigo.Trim(),
                    StringComparison.OrdinalIgnoreCase));
        }

        /// Agrega un expediente verificando que el código del estudiante no esté repetido
        public void AgregarExpediente(Expediente expediente)
        {
            if (expediente == null)
            {
                throw new ArgumentNullException(
                    nameof(expediente),
                    "El expediente no puede ser nulo.");
            }

            if (ExisteCodigo(expediente.CodigoEstudiante))
            {
                throw new InvalidOperationException(
                    $"El código {expediente.CodigoEstudiante} ya existe.");
            }

            Lista.Add(expediente);
        }

        /// Busca un expediente utilizando el código del estudiante
        public Expediente? BuscarExpediente(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return null;

            return Lista.FirstOrDefault(e =>
                e.CodigoEstudiante.Equals(
                    codigo.Trim(),
                    StringComparison.OrdinalIgnoreCase));
        }
    }
}