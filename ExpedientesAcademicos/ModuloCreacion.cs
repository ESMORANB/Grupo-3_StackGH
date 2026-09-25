using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using ExpedientesAcademicos.Modelos;
using ExpedientesAcademicos.Encriptacion;

namespace ExpedientesAcademicos.Creacion
{
    public class ModuloCreacion
    {
        private readonly string datos = "Datos";
        // El archivo ahora se guarda con extensión .xml.enc porque su
        // contenido ya no es XML legible, sino XML encriptado.

        private readonly string contrasena;

        private readonly string rutaArchivo;
        private readonly Encriptador encriptador;

        public ModuloCreacion(string contrasena, string ruta)
        {
            this.contrasena = contrasena;
            encriptador = new Encriptador(contrasena);
            this.rutaArchivo = ruta;
        }

        public void CrearArchivo()
        {
            if(Directory.Exists(datos) == false)
            {
                Directory.CreateDirectory(datos);
            }

            if(File.Exists(rutaArchivo) == false)
            {
               Expedientes datosIniciales = new Expedientes();
               GuardarArchivo(datosIniciales);
            }
        }

        public Expedientes? LeerArchivo()
        {
            if(File.Exists(rutaArchivo) == false)
            {
                Console.WriteLine("La ruta del archivo que ingreso no existe.");
                return null;
            }

            string xml;
            try
            {
                xml = encriptador.DesencriptarArchivo(rutaArchivo);
            }
            catch (Exception ex)
            {
                Console.WriteLine("No se pudo desencriptar el archivo: " + ex.Message);
                return null;
            }

            XmlSerializer serializer = new XmlSerializer(typeof(Expedientes));
            using StringReader lectorTexto = new StringReader(xml);
            return serializer.Deserialize(lectorTexto) as Expedientes;
        }
        
        public void GuardarArchivo(Expedientes expedientes)
        {
            if(Directory.Exists(datos) == false)
            {
                Directory.CreateDirectory(datos);
            }

            string temp = rutaArchivo + ".tmp";
            try
            {
                XmlSerializer serializador = new XmlSerializer(typeof(Expedientes));
                using (StreamWriter writer = new StreamWriter(temp))
                    serializador.Serialize(writer, expedientes);

                encriptador.EncriptarArchivo(temp, rutaArchivo);
            }
            finally
            {
                if (File.Exists(temp)) File.Delete(temp);
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

        public Expediente? BuscarExpedienteCodigo(string codigoIngresado, Expedientes expedientes)
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

        public void PrecargarArchivoVacio()
        {
            if (Directory.Exists(datos) == false)
            {
                Directory.CreateDirectory(datos);
            }

            Expedientes expedientesVacios = new Expedientes();
            GuardarArchivo(expedientesVacios);

            Console.WriteLine("Archivo de expedientes vacío generado correctamente en: " + rutaArchivo);
        }
        
    }
}