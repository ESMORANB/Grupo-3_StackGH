using System;
using ExpedientesAcademicos.Configuracion;
using ExpedientesAcademicos.Modelos;
using ExpedientesAcademicos.Creacion;
using ExpedientesAcademicos.Actualizacion;
using ExpedientesAcademicos.Busqueda;


class SistemaExpedientes
{
    static private string? ObtenerRutaArchivo()
    {
        Console.Write("Ruta del archivo XML: ");
        string? entrada = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(entrada))
        {
            return "";
        }

        // Quita espacios, comillas dobles y comillas simples de los extremos
        return entrada.Trim().Trim('"', '\'').Trim();
       
    }

    // ============================================================
    // Metodos auxiliares de lectura validada de numeros
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
                Console.WriteLine("[ERROR] Eso no es un numero entero valido. Intente de nuevo.");
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
                Console.WriteLine("[ERROR] Eso no es un numero valido. Intente de nuevo.");
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
    // Metodos de recopilacion de datos
    // ============================================================

    static (string codigoEstudiante, string nombreEstudiante, string apellidoEstudiante, string carreraEstudiante, int semestreEstudiante, string correoEstudiante) RecopilarDatosEstudiante(int tipo)
    {
        if (tipo == 1) //Si es 1 es para ingresar nuevos datos
        {
            Console.WriteLine("\nIngrese los datos del estudiante:");
            Console.WriteLine("codigo del estudiante:");
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
            Console.WriteLine("codigo del curso:");
            string codigoCurso = Console.ReadLine() ?? "";
            Console.WriteLine("Nombre del curso:");
            string nombreCurso = Console.ReadLine() ?? "";

            int creditosCurso = LeerEntero("Creditos del curso (min. 1):", minimo: 1);
            double calificacionCurso = LeerDecimal("Calificacion del curso (0-100):", minimo: 0, maximo: 100);

            Console.WriteLine("Ciclo academico (ej. 2026-S1):");
            string cicloAcademico = Console.ReadLine() ?? "";

            return (codigoCurso, nombreCurso, creditosCurso, calificacionCurso, cicloAcademico);
    }

    // ============================================================
    // Cambio de contrasenia del sistema
    // ============================================================

    // Vuelve a encriptar los datos ya cargados en memoria con una contrasenia
    // nueva. No modifica el archivo original hasta confirmar que la
    // contrasenia actual ingresada es correcta y que la nueva coincide
    // en ambos intentos.
    static string? CambiarContrasena(string contrasenaActual, Expedientes expedientesRegistrados, string rutaActual)
    {
        Console.WriteLine("\n | Opcion Seleccionada: Cambiar la contrasenia del sistema |");
        Console.Write("Para continuar, confirme la contrasenia actual: ");
        string confirmacion = Console.ReadLine() ?? "";

        if (confirmacion != contrasenaActual)
        {
            Console.WriteLine("[ERROR] La contrasenia ingresada no coincide con la actual. No se realizo ningun cambio.\n");
            return null;
        }

        string nuevaContrasena;
        string repetirContrasena;
        do
        {
            Console.Write("Ingrese la nueva contrasenia: ");
            nuevaContrasena = Console.ReadLine() ?? "";
            Console.Write("Confirme la nueva contrasenia: ");
            repetirContrasena = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(nuevaContrasena))
            {
                Console.WriteLine("[ERROR] La contrasenia no puede estar vacia. Intente de nuevo.");
            }
            else if (nuevaContrasena != repetirContrasena)
            {
                Console.WriteLine("[ERROR] Las contrasenias no coinciden. Intente de nuevo.");
            }
        }
        while (string.IsNullOrWhiteSpace(nuevaContrasena) || nuevaContrasena != repetirContrasena);

        try
        {
            // Se crea un modulo temporal con la nueva contrasenia unicamente
            // para volver a encriptar y guardar los datos ya existentes.
            ModuloCreacion moduloConNuevaContrasena = new ModuloCreacion(nuevaContrasena, rutaActual);
            moduloConNuevaContrasena.GuardarArchivo(expedientesRegistrados);

            Console.WriteLine("[OK] Contrasenia actualizada correctamente. A partir de ahora use la nueva contrasenia para ingresar al sistema.\n");
            return nuevaContrasena;
        }
        catch (Exception ex)
        {
            Console.WriteLine("[ERROR] No se pudo actualizar la contrasenia: " + ex.Message);
            return null;
        }
    }

    // ============================================================
    // Punto de entrada
    // ============================================================

    [STAThread]
    static void Main()
    {
        Console.WriteLine("========== SISTEMA DE GESTION DE EXPEDIENTES ACADEMICOS ==========\n");
        Console.Write("Ingrese la contrasenia del sistema: ");
        string contrasena = Console.ReadLine() ?? "";

        if (string.IsNullOrWhiteSpace(contrasena))
        {
            Console.WriteLine("[ERROR] La contrasenia no puede ser vacia. El sistema no puede continuar sin una contrasenia.");
            return;
        }

        Console.WriteLine("\nSeleccione el archivo de expedientes (.xml.enc) a utilizar:");
        string ruta = ObtenerRutaArchivo() ?? "";

        if (string.IsNullOrWhiteSpace(ruta))
        {
            Console.WriteLine("[ERROR] No se selecciono ningun archivo. El sistema no puede continuar sin un archivo de datos.");
            return;
        }

        ModuloCreacion moduloCreacion = new ModuloCreacion(contrasena, ruta);
        ModuloBusqueda moduloBusqueda = new ModuloBusqueda(ruta, contrasena);
        ModuloActualizacionEliminacion moduloActualizacion = new ModuloActualizacionEliminacion(ruta, contrasena);

        // Carga de los expedientes, una sola vez, con manejo de errores.
        // Si algo falla aqui (contrasenia incorrecta, archivo corrupto),
        Expedientes? expedientesRegistrados = null;
        try
        {
            expedientesRegistrados = moduloCreacion.LeerArchivo();

            if (expedientesRegistrados == null)
            {
                Console.WriteLine("[ERROR] No se pudo cargar el archivo de expedientes. Verifique que la contrasenia y el archivo seleccionado sean correctos.");
                return;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("[ERROR] Ocurrio un problema al cargar los datos: " + ex.Message);
            return;
        }

        Console.WriteLine("[OK] Datos cargados correctamente.\n");

        EjecutarMenu(expedientesRegistrados, ruta, contrasena, moduloBusqueda, moduloCreacion, moduloActualizacion);
    }

    // ============================================================
    // Menu principal
    // ============================================================

    static void EjecutarMenu(Expedientes expedientesRegistrados, string ruta, string contrasena, ModuloBusqueda moduloBusqueda, ModuloCreacion moduloCreacion, ModuloActualizacionEliminacion moduloActualizacion)
    {
        int opcionIngresada = 0;
        while (opcionIngresada != 9)
        {
            Console.WriteLine(" ========== SISTEMA DE GESTION DE EXPEDIENTES ACADEMICOS ========== ");
            Console.WriteLine(" -------------- MENU PRINCIPAL DE OPCIONES -------------- ");
            Console.WriteLine("[1]| Registrar un nuevo expediente academico |");
            Console.WriteLine("[2]| Consultar un expediente academico por codigo de estudiante |");
            Console.WriteLine("[3]| Consultar todos los expedientes academicos registrados |");
            Console.WriteLine("[4]| Actualizar un expediente academico existente |");
            Console.WriteLine("[5]| Actualizar/Agregar un curso de un expediente academico |");
            Console.WriteLine("[6]| Eliminar un expediente academico por numero |");
            Console.WriteLine("[7]| Eliminar un curso de un expediente academico |");
            Console.WriteLine("[8]| Cambiar la contrasenia del sistema |");
            Console.WriteLine("[9]| Salir del sistema |");

            opcionIngresada = LeerEntero("Seleccione una Opcion:", minimo: 1, maximo: 9);
            Console.WriteLine();

            switch (opcionIngresada)
            {
                case 1:
                    Console.WriteLine(" | Opcion Seleccionada: Registrar un nuevo expediente academico |");

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

                        int opcionAgregarMas = LeerEntero("Desea agregar otro curso?\n[1]| Si\n[2]| No", minimo: 1, maximo: 2);
                        agregarMasCursos = opcionAgregarMas == 1;

                    }
                    bool registrado = moduloCreacion.RegistrarExpediente(expedientesRegistrados, nuevoExpediente);
                    if (registrado)
                    {
                        Console.WriteLine($"[OK] Expediente de {nuevoExpediente.CodigoEstudiante} registrado exitosamente.\n");
                    }
                    break;

                case 2:
                    Console.WriteLine(" | Opcion Seleccionada: Consultar un expediente academico por codigo de estudiante |");
                    moduloBusqueda.CargarDesdeXml();
                    Console.Write("Codigo a buscar: ");
                    string codigoBuscar = Console.ReadLine() ?? "";
                    Expediente? encontrado = moduloBusqueda.BuscarPorCodigo(codigoBuscar);
                    if (encontrado != null)
                    {
                        moduloBusqueda.ImprimirExpediente(encontrado);
                    }
                    break;

                case 3:
                    Console.WriteLine(" | Opcion Seleccionada: Consultar todos los expedientes academicos registrados |");
                    moduloBusqueda.CargarDesdeXml();
                    var todos = moduloBusqueda.ListarTodos();
                    if (todos.Count == 0)
                    {
                        Console.WriteLine("Aun no hay expedientes registrados en el sistema.\n");
                    }
                    else
                    {
                        Console.WriteLine($"\nSe encontraron {todos.Count} expediente(s):");
                        foreach (var e in todos)
                            moduloBusqueda.ImprimirExpediente(e);
                    }
                    break;

                case 4:
                    Console.WriteLine(" | Opcion Seleccionada: Actualizar un expediente academico existente |");
                    moduloActualizacion.CargarDesdeXml();
                    Console.Write("Ingrese el codigo del expediente a actualizar: ");
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
                    Console.WriteLine(" | Opcion Seleccionada: Actualizar/Agregar un curso de un expediente academico |");
                    moduloActualizacion.CargarDesdeXml();
                    Console.Write("Ingrese el codigo del expediente al que pertenece el curso: ");
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
                    Console.WriteLine(" | Opcion Seleccionada: Eliminar un expediente academico |");
                    moduloActualizacion.CargarDesdeXml();
                    Console.Write("Ingrese el codigo del expediente a eliminar: ");
                    string codigoEliminar = Console.ReadLine() ?? "";
                    moduloActualizacion.EliminarExpediente(codigoEliminar);
                    break;

                case 7:
                    Console.WriteLine(" | Opcion Seleccionada: Eliminar un curso de un expediente academico |");
                    moduloActualizacion.CargarDesdeXml();
                    Console.Write("Ingrese el codigo del expediente: ");
                    string codigoExpedienteEliminarCurso = Console.ReadLine() ?? "";
                    Console.Write("Ingrese el codigo del curso a eliminar: ");
                    string codigoCursoEliminar = Console.ReadLine() ?? "";
                    moduloActualizacion.EliminarCurso(codigoExpedienteEliminarCurso, codigoCursoEliminar);
                    break;

                case 8:
                    string? contrasenaNueva = CambiarContrasena(contrasena, expedientesRegistrados, ruta);
                    if (contrasenaNueva != null)
                    {
                        // Se actualiza la contrasenia en uso y se recrean los modulos
                        // que dependen de ella, para que las siguientes operaciones
                        // (buscar, actualizar, eliminar) usen la contrasenia nueva.
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

