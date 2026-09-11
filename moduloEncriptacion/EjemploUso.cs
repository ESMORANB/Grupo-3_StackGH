using Encriptacion;
// ============================================================
// PRUEBAS DEL MÓDULO DE ENCRIPTACIÓN
// ============================================================
// Este archivo no es parte del sistema final: sirve para que
// cualquiera del equipo confirme, antes de usar Encriptador.cs
// en sus funciones, que el módulo realmente funciona.
//
// Para correrlo: se pone este archivo junto a Encriptador.cs en
// un proyecto de consola y se ejecuta con "dotnet run".
// ============================================================


string carpetaPruebas = "pruebas_encriptador";
if (Directory.Exists(carpetaPruebas))
{
    Directory.Delete(carpetaPruebas, recursive: true);
}
Directory.CreateDirectory(carpetaPruebas);

string rutaEncriptado = Path.Combine(carpetaPruebas, "prueba.xml.enc");
string xmlDePrueba = "<expedientes><expediente><carnet>1088825</carnet><nombre>Fatima Lopez</nombre></expediente></expedientes>";

int total = 0;
int exitosas = 0;

void Verificar(string nombrePrueba, bool condicion)
{
    total++;
    if (condicion)
    {
        exitosas++;
        Console.WriteLine("[OK]    " + nombrePrueba);
    }
    else
    {
        Console.WriteLine("[FALLÓ] " + nombrePrueba);
    }
}

Console.WriteLine("=== Pruebas del módulo Encriptador ===\n");

// Prueba 1: encriptar un texto y confirmar que el archivo generado
// ya no contiene el XML legible.
Encriptador encriptador = new Encriptador("claveDePrueba123");
encriptador.EncriptarTexto(xmlDePrueba, rutaEncriptado);

bool archivoExiste = File.Exists(rutaEncriptado);
string contenidoCrudo = File.ReadAllText(rutaEncriptado, System.Text.Encoding.Latin1); // ← AQUI ESTA EL XML ENCRIPTADO
bool noEsLegible = !contenidoCrudo.Contains("Fatima") && !contenidoCrudo.Contains("<expediente>");
Verificar("1. Encriptar genera un archivo y su contenido no es XML legible", archivoExiste && noEsLegible);

// Prueba 2: desencriptar con la misma contraseña debe devolver
// exactamente el mismo texto original.
string xmlRecuperado = encriptador.DesencriptarArchivo(rutaEncriptado); // ← AQUI ESTA EL XML DESCRIPTADO
Verificar("2. Desencriptar devuelve el mismo texto que se encriptó", xmlRecuperado == xmlDePrueba);
Console.WriteLine(xmlRecuperado);

// Prueba 3: una instancia con una contraseña distinta no debe poder
// desencriptar el archivo.
Encriptador encriptadorClaveIncorrecta = new Encriptador("claveEquivocada");
bool rechazoContrasenaIncorrecta = false;
try
{
    encriptadorClaveIncorrecta.DesencriptarArchivo(rutaEncriptado);
}
catch (Exception)
{
    rechazoContrasenaIncorrecta = true;
}
Verificar("3. Una contraseña incorrecta lanza una excepción al desencriptar", rechazoContrasenaIncorrecta);

// Prueba 4: EncriptarArchivo (a partir de un archivo XML plano en disco,
// no de un texto en memoria) también debe funcionar.
string rutaXmlPlano = Path.Combine(carpetaPruebas, "prueba_original.xml");
string rutaEncriptadoDesdeArchivo = Path.Combine(carpetaPruebas, "prueba_desde_archivo.xml.enc");
File.WriteAllText(rutaXmlPlano, xmlDePrueba);

encriptador.EncriptarArchivo(rutaXmlPlano, rutaEncriptadoDesdeArchivo);
string xmlRecuperadoDesdeArchivo = encriptador.DesencriptarArchivo(rutaEncriptadoDesdeArchivo);
Verificar("4. EncriptarArchivo + DesencriptarArchivo también recuperan el XML original",
    xmlRecuperadoDesdeArchivo == xmlDePrueba);

// Prueba 5: intentar desencriptar un archivo que no existe debe avisar
// con una excepción clara, no con un error confuso.
bool avisoArchivoInexistente = false;
try
{
    encriptador.DesencriptarArchivo(Path.Combine(carpetaPruebas, "no_existe.xml.enc"));
}
catch (FileNotFoundException)
{
    avisoArchivoInexistente = true;
}
Verificar("5. Desencriptar un archivo inexistente lanza FileNotFoundException", avisoArchivoInexistente);

Console.WriteLine();
Console.WriteLine($"Resultado final: {exitosas}/{total} pruebas exitosas.");