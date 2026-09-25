using System;
using ExpedientesAcademicos.Modelos;
using ExpedientesAcademicos.Creacion;
using ExpedientesAcademicos.Actualizacion;
using ExpedientesAcademicos.Busqueda;
using System.Windows.Forms;

class SistemaExpedientes
{
    static private string? ObtenerRutaArchivo()
    {
        OpenFileDialog dialogo = new OpenFileDialog();

        try
        {
            DialogResult resultado = dialogo.ShowDialog();
            if (resultado == DialogResult.OK)
            {
                return dialogo.FileName;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("[ERROR] No se pudo abrir el explorador de archivos: " + ex.Message);
        }

        return null;
    }

    // ============================================================
    // Métodos auxiliares de lectura validada de números
    // ============================================================

    static int LeerEntero(string mensaje, int minimo = int.MinValue, int maximo = int.MaxValue)
    {
        int valor;
        bool valido = false;

        do
        {
            Console.WriteLine(mensaje);
            string entrada = Console.ReadLine() ?? "";

            if (int.TryParse(entrada, out valor) == false)
            {
                Console.WriteLine("[ERROR] Eso no es un número entero válido. Intente de nuevo.");
            }
            else if (valor < minimo || valor > maximo)
            {
                Console.WriteLine($"[ERROR] El valor debe estar entre {minimo} y {maximo}. Intente de nuevo.");
            }
            else
            {
                valido = true;
            }
        }
        while (valido == false);

        return valor;
    }

    static double LeerDecimal(string mensaje, double minimo = double.MinValue, double maximo = double.MaxValue)
    {
        double valor;
        bool valido = false;

        do
        {
            Console.WriteLine(mensaje);
            string entrada = Console.ReadLine() ?? "";

            if (double.TryParse(entrada, out valor) == false)
            {
                Console.WriteLine("[ERROR] Eso no es un número válido. Intente de nuevo.");
            }
            else if (valor < minimo || valor > maximo)
            {
                Console.WriteLine($"[ERROR] El valor debe estar entre {minimo} y {maximo}. Intente de nuevo.");
            }
            else
            {
                valido = true;
            }
        }
        while (valido == false);

        return valor;
    }

    // ============================================================
    // Métodos de recopilación de datos
    // ============================================================

    static (string codigoEstudiante, string nombreEstudiante, string apellidoEstudiante, string carreraEstudiante, int semestreEstudiante, string correoEstudiante) RecopilarDatosEstudiante(int tipo)
    {
        if (tipo == 1) //Si es 1 es para ingresar nuevos datos
        {
            Console.WriteLine("\nIngrese los datos del estudiante:");
            Console.WriteLine("Código del estudiante:");
            string codigoEstudiante = Console.ReadLine() ?? "";
            Console.WriteLine("Nombre del estudiante:");
            string nombreEstudiante = Console.ReadLine() ?? "";
            Console.WriteLine("Apellido del estudiante:");
            string apellidoEstudiante = Console.ReadLine() ?? "";
            Console.WriteLine("Carrera del estudiante:");
            string carreraEstudiante = Console.ReadLine() ?? "";
            int semestreEstudiante = LeerEntero("Semestre del estudiante (1 hasta 12):", minimo: 1, maximo: 12);
            Console.WriteLine("Correo del estudiante (debe contener '@' y '.'):");
            string correoEstudiante = Console.ReadLine() ?? "";
            return (codigoEstudiante, nombreEstudiante, apellidoEstudiante, carreraEstudiante, semestreEstudiante, correoEstudiante);
        }
        else //Si es otro numero, es para actualizar datos (no se pide codigo)
        {
            Console.WriteLine("\nIngrese los actualizados datos del estudiante:");
            Console.WriteLine("Nombre a actualizar del estudiante:");
            string nombreEstudiante = Console.ReadLine() ?? "";
            Console.WriteLine("Apellido a actualizar del estudiante:");
            string apellidoEstudiante = Console.ReadLine() ?? "";
            Console.WriteLine("Carrera a actulizar del estudiante:");
            string carreraEstudiante = Console.ReadLine() ?? "";
            int semestreEstudiante = LeerEntero("Semestre a actualizar del estudiante (1 hasta 12):", minimo: 1, maximo: 12);
            Console.WriteLine("Correo a actualizar del estudiante (debe contener '@' y '.'):");
            string correoEstudiante = Console.ReadLine() ?? ""; 
            return ("", nombreEstudiante, apellidoEstudiante, carreraEstudiante, semestreEstudiante, correoEstudiante);
        }
    }

    static (string codigoCurso, string nombreCurso, int creditosCurso, double calificacionCurso, string cicloAcademico) RecopilarDatosCurso()
    {
            Console.WriteLine("\nIngrese los datos del curso:");
            Console.WriteLine("Código del curso:");
            string codigoCurso = Console.ReadLine() ?? "";
            Console.WriteLine("Nombre del curso:");
            string nombreCurso = Console.ReadLine() ?? "";

            int creditosCurso = LeerEntero("Créditos del curso (min. 1):", minimo: 1);
            double calificacionCurso = LeerDecimal("Calificación del curso (0-100):", minimo: 0, maximo: 100);

            Console.WriteLine("Ciclo académico (ej. 2026-S1):");
            string cicloAcademico = Console.ReadLine() ?? "";

            return (codigoCurso, nombreCurso, creditosCurso, calificacionCurso, cicloAcademico);
    }

    // ============================================================
    // Cambio de contraseña del sistema
    // ============================================================

    // Vuelve a encriptar los datos ya cargados en memoria con una contraseña
    // nueva. No modifica el archivo original hasta confirmar que la
    // contraseña actual ingresada es correcta y que la nueva coincide
    // en ambos intentos.
    static string? CambiarContrasena(string contrasenaActual, Expedientes expedientesRegistrados, string rutaActual)
    {
        Console.WriteLine("\n | Opción Seleccionada: Cambiar la contraseña del sistema |");
        Console.Write("Para continuar, confirme la contraseña actual: ");
        string confirmacion = Console.ReadLine() ?? "";

        if (confirmacion != contrasenaActual)
        {
            Console.WriteLine("[ERROR] La contraseña ingresada no coincide con la actual. No se realizó ningún cambio.\n");
            return null;
        }

        string nuevaContrasena;
        string repetirContrasena;
        do
        {
            Console.Write("Ingrese la nueva contraseña: ");
            nuevaContrasena = Console.ReadLine() ?? "";
            Console.Write("Confirme la nueva contraseña: ");
            repetirContrasena = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(nuevaContrasena))
            {
                Console.WriteLine("[ERROR] La contraseña no puede estar vacía. Intente de nuevo.");
            }
            else if (nuevaContrasena != repetirContrasena)
            {
                Console.WriteLine("[ERROR] Las contraseñas no coinciden. Intente de nuevo.");
            }
        }
        while (string.IsNullOrWhiteSpace(nuevaContrasena) || nuevaContrasena != repetirContrasena);

        try
        {
            // Se crea un módulo temporal con la nueva contraseña únicamente
            // para volver a encriptar y guardar los datos ya existentes.
            ModuloCreacion moduloConNuevaContrasena = new ModuloCreacion(nuevaContrasena, rutaActual);
            moduloConNuevaContrasena.GuardarArchivo(expedientesRegistrados);

            Console.WriteLine("[OK] Contraseña actualizada correctamente. A partir de ahora use la nueva contraseña para ingresar al sistema.\n");
            return nuevaContrasena;
        }
        catch (Exception ex)
        {
            Console.WriteLine("[ERROR] No se pudo actualizar la contraseña: " + ex.Message);
            return null;
        }
    }

    // ============================================================
    // Punto de entrada
    // ============================================================

    [STAThread]
    static void Main()
    {
        Console.WriteLine("========== SISTEMA DE GESTIÓN DE EXPEDIENTES ACADÉMICOS ==========\n");
        Console.Write("Ingrese la contraseña del sistema: ");
        string contrasena = Console.ReadLine() ?? "";

        Console.WriteLine("\nSeleccione el archivo de expedientes (.xml.enc) a utilizar:");
        string ruta = ObtenerRutaArchivo() ?? "";

        if (string.IsNullOrWhiteSpace(ruta))
        {
            Console.WriteLine("[ERROR] No se seleccionó ningún archivo. El sistema no puede continuar sin un archivo de datos.");
            return;
        }

        ModuloCreacion moduloCreacion = new ModuloCreacion(contrasena, ruta);
        ModuloBusqueda moduloBusqueda = new ModuloBusqueda(ruta, contrasena);
        ModuloActualizacionEliminacion moduloActualizacion = new ModuloActualizacionEliminacion(ruta, contrasena);

        // Carga de los expedientes, una sola vez, con manejo de errores.
        // Si algo falla aquí (contraseña incorrecta, archivo corrupto),
        Expedientes? expedientesRegistrados = null;
        try
        {
            expedientesRegistrados = moduloCreacion.LeerArchivo();

            if (expedientesRegistrados == null)
            {
                Console.WriteLine("[ERROR] No se pudo cargar el archivo de expedientes. Verifique que la contraseña y el archivo seleccionado sean correctos.");
                return;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("[ERROR] Ocurrió un problema al cargar los datos: " + ex.Message);
            return;
        }

        Console.WriteLine("[OK] Datos cargados correctamente.\n");

        EjecutarMenu(expedientesRegistrados, ruta, contrasena, moduloBusqueda, moduloCreacion, moduloActualizacion);
    }

    // ============================================================
    // Menú principal
    // ============================================================

    static void EjecutarMenu(Expedientes expedientesRegistrados, string ruta, string contrasena, ModuloBusqueda moduloBusqueda, ModuloCreacion moduloCreacion, ModuloActualizacionEliminacion moduloActualizacion)
    {
        int opcionIngresada = 0;
        while (opcionIngresada != 9)
        {
            Console.WriteLine(" ========== SISTEMA DE GESTIÓN DE EXPEDIENTES ACADÉMICOS ========== ");
            Console.WriteLine(" -------------- MENU PRINCIPAL DE OPCIONES -------------- ");
            Console.WriteLine("[1]| Registrar un nuevo expediente académico |");
            Console.WriteLine("[2]| Consultar un expediente académico por código de estudiante |");
            Console.WriteLine("[3]| Consultar todos los expedientes académicos registrados |");
            Console.WriteLine("[4]| Actualizar un expediente académico existente |");
            Console.WriteLine("[5]| Actualizar/Agregar un curso de un expediente académico |");
            Console.WriteLine("[6]| Eliminar un expediente académico por número |");
            Console.WriteLine("[7]| Eliminar un curso de un expediente académico |");
            Console.WriteLine("[8]| Cambiar la contraseña del sistema |");
            Console.WriteLine("[9]| Salir del sistema |");

            opcionIngresada = LeerEntero("Seleccione una opción:", minimo: 1, maximo: 9);
            Console.WriteLine();

            switch (opcionIngresada)
            {
                case 1:
                    Console.WriteLine(" | Opción Seleccionada: Registrar un nuevo expediente académico |");

                    Expediente? nuevoExpediente = null;
                    while (nuevoExpediente == null)
                    {
                        var datosEstudiante = RecopilarDatosEstudiante(1);
                        try
                        {
                            nuevoExpediente = new Expediente(
                                datosEstudiante.codigoEstudiante,
                                datosEstudiante.nombreEstudiante,
                                datosEstudiante.apellidoEstudiante,
                                datosEstudiante.carreraEstudiante,
                                datosEstudiante.semestreEstudiante,
                                datosEstudiante.correoEstudiante);
                        }
                        catch (ArgumentException ex)
                        {
                            Console.WriteLine("[ERROR] " + ex.Message);
                            Console.WriteLine("Por favor, ingrese los datos del estudiante nuevamente.");
                        }
                    }

                    bool agregarMasCursos = true;
                    while (agregarMasCursos) {
                        var datosCurso = RecopilarDatosCurso();
                        Curso cursoNuevo = new Curso(
                            datosCurso.codigoCurso,
                            datosCurso.nombreCurso,
                            datosCurso.creditosCurso,
                            datosCurso.calificacionCurso,
                            datosCurso.cicloAcademico);

                        nuevoExpediente.AgregarCurso(cursoNuevo);
                        Console.WriteLine($"[OK] Curso {datosCurso.codigoCurso} agregado al expediente.\n");

                        int opcionAgregarMas = LeerEntero("¿Desea agregar otro curso?\n[1]| Sí\n[2]| No", minimo: 1, maximo: 2);
                        agregarMasCursos = opcionAgregarMas == 1;

                    }
                    bool registrado = moduloCreacion.RegistrarExpediente(expedientesRegistrados, nuevoExpediente);
                    if (registrado)
                    {
                        Console.WriteLine($"[OK] Expediente de {nuevoExpediente.CodigoEstudiante} registrado exitosamente.\n");
                    }
                    break;

                case 2:
                    Console.WriteLine(" | Opción Seleccionada: Consultar un expediente académico por código de estudiante |");
                    moduloBusqueda.CargarDesdeXml();
                    Console.Write("Código a buscar: ");
                    string codigoBuscar = Console.ReadLine() ?? "";
                    Expediente? encontrado = moduloBusqueda.BuscarPorCodigo(codigoBuscar);
                    if (encontrado != null)
                    {
                        moduloBusqueda.ImprimirExpediente(encontrado);
                    }
                    break;

                case 3:
                    Console.WriteLine(" | Opción Seleccionada: Consultar todos los expedientes académicos registrados |");
                    moduloBusqueda.CargarDesdeXml();
                    var todos = moduloBusqueda.ListarTodos();
                    if (todos.Count == 0)
                    {
                        Console.WriteLine("Aún no hay expedientes registrados en el sistema.\n");
                    }
                    else
                    {
                        Console.WriteLine($"\nSe encontraron {todos.Count} expediente(s):");
                        foreach (var e in todos)
                            moduloBusqueda.ImprimirExpediente(e);
                    }
                    break;

                case 4:
                    Console.WriteLine(" | Opción Seleccionada: Actualizar un expediente académico existente |");
                    moduloActualizacion.CargarDesdeXml();
                    Console.Write("Ingrese el código del expediente a actualizar: ");
                    string codigoActualizar = Console.ReadLine() ?? "";

                    var nuevosDatos = RecopilarDatosEstudiante(2);
                    moduloActualizacion.ActualizarExpediente(
                        codigoActualizar,
                        nuevosDatos.nombreEstudiante,
                        nuevosDatos.apellidoEstudiante,
                        nuevosDatos.carreraEstudiante,
                        nuevosDatos.semestreEstudiante,
                        nuevosDatos.correoEstudiante);
                    break;

                case 5:
                    Console.WriteLine(" | Opción Seleccionada: Actualizar/Agregar un curso de un expediente académico |");
                    moduloActualizacion.CargarDesdeXml();
                    Console.Write("Ingrese el código del expediente al que pertenece el curso: ");
                    string codigoExpedienteCurso = Console.ReadLine() ?? "";

                    var datosCursoActualizar = RecopilarDatosCurso();
                    try {
                    moduloActualizacion.ActualizarCurso(
                        codigoExpedienteCurso,
                        datosCursoActualizar.codigoCurso,
                        datosCursoActualizar.nombreCurso,
                        datosCursoActualizar.creditosCurso,
                        datosCursoActualizar.calificacionCurso,
                        datosCursoActualizar.cicloAcademico);
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine("[ERROR] " + ex.Message);
                        Console.WriteLine("Por favor, ingrese los datos del curso nuevamente.");
                    }
                    break;

                case 6:
                    Console.WriteLine(" | Opción Seleccionada: Eliminar un expediente académico |");
                    moduloActualizacion.CargarDesdeXml();
                    Console.Write("Ingrese el código del expediente a eliminar: ");
                    string codigoEliminar = Console.ReadLine() ?? "";
                    moduloActualizacion.EliminarExpediente(codigoEliminar);
                    break;

                case 7:
                    Console.WriteLine(" | Opción Seleccionada: Eliminar un curso de un expediente académico |");
                    moduloActualizacion.CargarDesdeXml();
                    Console.Write("Ingrese el código del expediente: ");
                    string codigoExpedienteEliminarCurso = Console.ReadLine() ?? "";
                    Console.Write("Ingrese el código del curso a eliminar: ");
                    string codigoCursoEliminar = Console.ReadLine() ?? "";
                    moduloActualizacion.EliminarCurso(codigoExpedienteEliminarCurso, codigoCursoEliminar);
                    break;

                case 8:
                    string? contrasenaNueva = CambiarContrasena(contrasena, expedientesRegistrados, ruta);
                    if (contrasenaNueva != null)
                    {
                        // Se actualiza la contraseña en uso y se recrean los módulos
                        // que dependen de ella, para que las siguientes operaciones
                        // (buscar, actualizar, eliminar) usen la contraseña nueva.
                        contrasena = contrasenaNueva;
                        moduloCreacion = new ModuloCreacion(contrasena, ruta);
                        moduloBusqueda = new ModuloBusqueda(ruta, contrasena);
                        moduloActualizacion = new ModuloActualizacionEliminacion(ruta, contrasena);
                    }
                    break;

                case 9:
                    Console.WriteLine("Gracias por usar el sistema. Saliendo...");
                    break;
            }

            Console.WriteLine();
        }
    }
}