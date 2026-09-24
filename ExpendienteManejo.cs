using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Proyecto1.Modelos;
namespace Proyecto1.Servicios
{
    public class ExpedienteManejo
    {
        private readonly string datos = "Datos";
        private readonly string rutaArchivo = Path.Combine("Datos", "expedientes.xml");
        public void CrearArchivo()
        {
            if(Directory.Exists(datos) == false)
            {
                Directory.CreateDirectory(datos);
            }

            if(File.Exists(rutaArchivo) == false)
            {
               Expedientes datosIniciales = new Expedientes();

                XmlSerializer serializador =new XmlSerializer(typeof(Expedientes));

                using FileStream archivo =new FileStream(rutaArchivo,FileMode.Create,FileAccess.Write);

                serializador.Serialize(archivo, datosIniciales);
            }
        }

        public Expedientes? LeerArchivo()
        {
            if(File.Exists(rutaArchivo) == false)
            {
                Console.WriteLine("La ruta del archivo que ingreso no existe.");
                return null;
            }
            XmlSerializer serializer = new XmlSerializer(typeof(Expedientes));
            using (FileStream archivo = new FileStream(rutaArchivo, FileMode.Open))
            {
                return serializer.Deserialize(archivo) as Expedientes;
            }

        }
        
        public void GuardarArchivo(Expedientes expedientes)
        {
            if(Directory.Exists(datos) == false)
            {
                Directory.CreateDirectory(datos);
            }

            XmlSerializer serializador = new XmlSerializer(typeof(Expedientes));
            using (FileStream archivo = new FileStream(rutaArchivo, FileMode.Create, FileAccess.Write))
            {
                serializador.Serialize(archivo, expedientes);
            }
        }

        public bool RegistrarExpediente(Expedientes expedientes, Expediente expedienteNuevo)
        {
            bool validacionExpediente = expedienteNuevo.EsValido(out string mensajeError);
            if(validacionExpediente == false)
            {
                Console.WriteLine(mensajeError);
                return false;
            }
            else
            {
                string codigoIngresado = expedienteNuevo.CodigoEstudiante; 
                if(expedientes.ExisteCodigo(codigoIngresado) == true)
                {
                    Console.WriteLine("El código del estudiante previamente ingresado ya existe en el registro. No se puede volver a registrar.");
                    return false;
                }
                else
                {
                    expedientes.AgregarExpediente(expedienteNuevo);
                    GuardarArchivo(expedientes);
                    return true;
                }
            }
          
        }

        public Expediente? BuscarExpedienteCodigo( string codigoIngresado, Expedientes expedientes)
        {
            Expediente? expedienteEncontrado = expedientes.BuscarExpediente(codigoIngresado);
            if(expedienteEncontrado == null)
            {
                Console.WriteLine("El código del estudiante ingresado no existe en el registro.");
                return null;
            }
            else
            {
                return expedienteEncontrado;
            }
        }
        
    }
}