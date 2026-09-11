
using System.Security.Cryptography;
using System.Text;

namespace ExpedientesSimple
{
    public class Encriptador
    {
        // Convierte la contraseña en una clave de 32 bytes usando SHA-256,
        // porque AES necesita una clave de un tamaño exacto (256 bits).
        private static byte[] ObtenerClaveDesdeContrasena(string contrasena)
        {
            using SHA256 sha256 = SHA256.Create();
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(contrasena));
        }

        // Encripta el archivo XML de rutaOrigen y guarda el resultado en rutaDestino.
        public void EncriptarArchivo(string rutaOrigen, string rutaDestino, string contrasena)
        {
            // Leemos el contenido del archivo original como texto.
            string textoPlano = File.ReadAllText(rutaOrigen);
            byte[] datosPlanos = Encoding.UTF8.GetBytes(textoPlano);

            byte[] clave = ObtenerClaveDesdeContrasena(contrasena);

            using Aes aes = Aes.Create();
            aes.Key = clave;
            aes.GenerateIV(); // IV aleatorio: hace que el mismo texto no siempre encripte igual

            using ICryptoTransform encriptador = aes.CreateEncryptor();
            byte[] datosEncriptados = encriptador.TransformFinalBlock(datosPlanos, 0, datosPlanos.Length);

            // Guardamos primero el IV (no es secreto) y luego los datos encriptados.
            using FileStream archivoSalida = new FileStream(rutaDestino, FileMode.Create);
            archivoSalida.Write(aes.IV, 0, aes.IV.Length);
            archivoSalida.Write(datosEncriptados, 0, datosEncriptados.Length);
        }

        // Desencripta un archivo generado por EncriptarArchivo y devuelve el XML original.
        public string DesencriptarArchivo(string rutaArchivoEncriptado, string contrasena)
        {
            byte[] contenidoCompleto = File.ReadAllBytes(rutaArchivoEncriptado);

            byte[] clave = ObtenerClaveDesdeContrasena(contrasena);

            using Aes aes = Aes.Create();
            aes.Key = clave;

            // El IV mide siempre 16 bytes en AES, lo separamos del resto del archivo.
            int tamanioIV = aes.BlockSize / 8;
            byte[] iv = new byte[tamanioIV];
            Array.Copy(contenidoCompleto, 0, iv, 0, tamanioIV);
            aes.IV = iv;

            int tamanioDatos = contenidoCompleto.Length - tamanioIV;
            byte[] datosEncriptados = new byte[tamanioDatos];
            Array.Copy(contenidoCompleto, tamanioIV, datosEncriptados, 0, tamanioDatos);

            try
            {
                using ICryptoTransform desencriptador = aes.CreateDecryptor();
                byte[] datosPlanos = desencriptador.TransformFinalBlock(datosEncriptados, 0, datosEncriptados.Length);
                return Encoding.UTF8.GetString(datosPlanos);
            }
            catch (CryptographicException)
            {
                // Si la contraseña es incorrecta, AES suele fallar aquí porque
                // el padding del texto desencriptado no tiene sentido.
                throw new Exception("No se pudo desencriptar el archivo. Verifique que la contraseña sea correcta.");
            }
        }
    }

    internal class Program
    {
        private static void Main()
        {
            // 1. Creamos un archivo XML de prueba, como si fuera un expediente académico.
            string rutaOriginal = "expediente.xml";
            string rutaEncriptado = "expediente.xml.enc";
            string rutaDesencriptado = "expediente_recuperado.xml";

            string xmlDePrueba =
                @"<expediente>
                <carnet>20231234</carnet>
                <nombre>María José Pérez</nombre>
                <carrera>Ingeniería en Sistemas</carrera>
                </expediente>";

            File.WriteAllText(rutaOriginal, xmlDePrueba);
            Console.WriteLine("1) Archivo original creado: " + rutaOriginal);
            Console.WriteLine(xmlDePrueba);

            // 2. Encriptamos el archivo con una contraseña.
            string contrasena = "miClaveSecreta123";
            Encriptador encriptador = new Encriptador();
            encriptador.EncriptarArchivo(rutaOriginal, rutaEncriptado, contrasena);
            Console.WriteLine("\n2) Archivo encriptado creado: " + rutaEncriptado);

            // Mostramos que el archivo encriptado ya NO se puede leer como texto normal.
            byte[] bytesEncriptados = File.ReadAllBytes(rutaEncriptado);
            Console.WriteLine("   Tamaño del archivo encriptado: " + bytesEncriptados.Length + " bytes (ilegible como texto)");

            // 3. Desencriptamos el archivo con la misma contraseña.
            string xmlRecuperado = encriptador.DesencriptarArchivo(rutaEncriptado, contrasena);
            File.WriteAllText(rutaDesencriptado, xmlRecuperado);
            Console.WriteLine("\n3) Archivo desencriptado:");
            Console.WriteLine(xmlRecuperado);

            // 4. Verificamos que el contenido recuperado es igual al original.
            if (xmlRecuperado == xmlDePrueba)
            {
                Console.WriteLine("\nPRUEBA EXITOSA: el contenido desencriptado es igual al original.");
            }
            else
            {
                Console.WriteLine("\nPRUEBA FALLIDA: el contenido no coincide.");
            }

            // 5. Probamos qué pasa si alguien usa una contraseña incorrecta.
            Console.WriteLine("\n4) Probando con una contraseña incorrecta...");
            try
            {
                encriptador.DesencriptarArchivo(rutaEncriptado, "contrasenaEquivocada");
                Console.WriteLine("Esto no debería pasar: se desencriptó sin la contraseña correcta.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("PRUEBA EXITOSA: se rechazó la contraseña incorrecta.");
                Console.WriteLine("   Mensaje: " + ex.Message);
            }
        }
    }
}