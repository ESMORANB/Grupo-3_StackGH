
using System.Security.Cryptography;
using System.Text;

namespace ExpedientesAcademicos.Encriptacion
{
    // ============================================================
    // MÓDULO DE ENCRIPTACIÓN - guía rápida para el equipo
    // ============================================================
    //
    // El flujo de encriptación funciona de la siguiente manera: se almacena
    // el archivo XML en dos ubicaciones: en disco (archivos encriptados)
    // y en memoria (archivos desencriptados). Toda accion del sistema utiliza
    // como entrada el XML en memoria (desencriptado). Cada vez que se realiza 
    // una modificación (acciones de registrar, modificar o eliminar), 
    // se deben guardar los cambios en el XML en disco (encriptado) para que
    // ambos registros estén en sincronía. 
    //
    // Esta clase encripta y desencripta el archivo XML del proyecto
    // usando AES-256. Cada quien la usa creando su propia instancia
    // con la contraseña del sistema, así:
    //
    //   Encriptador encriptador = new Encriptador("laContraseñaDelSistema");
    //
    // Una vez creada la instancia, se usan sus 3 métodos según lo que
    // se necesite hacer:
    //
    //   - EncriptarTexto(texto, ruta)     -> cuando ya se tiene el XML como
    //                                        string en memoria (por ejemplo,
    //                                        después de armar un XDocument
    //                                        y hacer .ToString()).
    //
    //   - EncriptarArchivo(origen, destino) -> cuando tenés un archivo XML
    //                                          plano ya guardado en disco
    //                                          y querés generar su versión
    //                                          encriptada.
    //
    //   - DesencriptarArchivo(ruta)       -> devuelve el contenido XML
    //                                        (como string) para poder
    //                                        parsearlo, por ejemplo con
    //                                        XDocument.Parse(...).
    //
    // Ejemplo de uso típico en el flujo de "guardar cambios":
    //
    //   Encriptador encriptador = new Encriptador(contrasena);
    //   string xml = documento.ToString();
    //   encriptador.EncriptarTexto(xml, "expedientes.xml.enc");
    //
    // Ejemplo de uso típico en el flujo de "cargar datos al iniciar":
    //
    //   Encriptador encriptador = new Encriptador(contrasena);
    //   string xml = encriptador.DesencriptarArchivo("expedientes.xml.enc");
    //   XDocument documento = XDocument.Parse(xml);
    //
    // Si la contraseña usada para desencriptar es incorrecta, o el archivo
    // fue corrompido, DesencriptarArchivo lanza una Exception normal con
    // un mensaje explicando el problema (revisar el bloque try/catch más
    // abajo). Conviene que quien la use envuelva su llamado en un try/catch
    // para mostrarle un mensaje claro al usuario final.
    // ============================================================
    public class Encriptador
    {
        // La contraseña se guarda una sola vez, al crear la instancia.
        // Por eso los métodos de esta clase no piden la contraseña de nuevo:
        // ya quedó asociada al objeto "encriptador" que se creó.
        private readonly string contrasena;

        // Constructor: así es como el equipo crea una instancia de esta clase.
        public Encriptador(string contrasena)
        {
            if (string.IsNullOrWhiteSpace(contrasena))
            {
                throw new ArgumentException("La contraseña no puede estar vacía.");
            }
            this.contrasena = contrasena;
        }

        // Convierte la contraseña en una clave de 32 bytes usando SHA-256,
        // porque AES necesita una clave de un tamaño exacto (256 bits).
        // Es un método privado: nadie fuera de esta clase lo necesita llamar.
        private byte[] ObtenerClave()
        {
            using SHA256 sha256 = SHA256.Create();
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(contrasena));
        }

        // Encripta un texto que ya se tiene en memoria (por ejemplo, un XML
        // armado con XDocument) y lo guarda en rutaDestino.
        // Útil cuando se está trabajando con los datos en memoria y solo
        // se quiere persistir el resultado final, sin pasar por un archivo
        // plano intermedio.
        public void EncriptarTexto(string texto, string rutaDestino)
        {
            byte[] datosPlanos = Encoding.UTF8.GetBytes(texto);
            byte[] clave = ObtenerClave();

            using Aes aes = Aes.Create();
            aes.Key = clave;
            aes.GenerateIV(); // IV aleatorio: hace que el mismo texto no siempre encripte igual

            using ICryptoTransform encriptador = aes.CreateEncryptor();
            byte[] datosEncriptados = encriptador.TransformFinalBlock(datosPlanos, 0, datosPlanos.Length);

            // Se guarda primero el IV (no es secreto) y luego los datos encriptados,
            // uno después del otro, en el mismo archivo.
            using FileStream archivoSalida = new FileStream(rutaDestino, FileMode.Create);
            archivoSalida.Write(aes.IV, 0, aes.IV.Length);
            archivoSalida.Write(datosEncriptados, 0, datosEncriptados.Length);
        }

        // Encripta un archivo XML plano que ya existe en disco (rutaOrigen)
        // y guarda el resultado en rutaDestino. Internamente solo lee el
        // archivo y delega el trabajo a EncriptarTexto.
        public void EncriptarArchivo(string rutaOrigen, string rutaDestino)
        {
            if (!File.Exists(rutaOrigen))
            {
                throw new FileNotFoundException("El archivo a encriptar no existe.", rutaOrigen);
            }

            string textoPlano = File.ReadAllText(rutaOrigen);
            EncriptarTexto(textoPlano, rutaDestino); // ← AQUI ESTA EL XML ENCRIPTADO, SE GUARDA EN DISCO DESPUÉS DE CUALQUIER MODIFICACIÓN
        }

        // Desencripta un archivo generado por EncriptarTexto o EncriptarArchivo
        // y devuelve el XML original como string, listo para parsear.
        public string DesencriptarArchivo(string rutaArchivoEncriptado)
        {
            if (!File.Exists(rutaArchivoEncriptado))
            {
                throw new FileNotFoundException("El archivo encriptado no existe.", rutaArchivoEncriptado);
            }

            byte[] contenidoCompleto = File.ReadAllBytes(rutaArchivoEncriptado);
            byte[] clave = ObtenerClave();

            using Aes aes = Aes.Create();
            aes.Key = clave;

            // El IV mide siempre 16 bytes en AES, lo separamos del resto del archivo.
            int tamanioIV = aes.BlockSize / 8;

            if (contenidoCompleto.Length < tamanioIV)
            {
                throw new Exception("El archivo encriptado está incompleto o corrupto.");
            }

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
                return Encoding.UTF8.GetString(datosPlanos); // ← AQUI ESTA EL XML RECUPERADO
            }
            catch (CryptographicException)
            {
                // Si la contraseña es incorrecta, AES suele fallar aquí porque
                // el padding del texto desencriptado no tiene sentido.
                throw new Exception("No se pudo desencriptar el archivo. Verifique que la contraseña sea correcta.");
            }
        }
    }
}