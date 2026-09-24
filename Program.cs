
using System;
using Proyecto1.Modelos;
using Proyecto1.Servicios;

class Programa
{
    static void Main()
    {
        ExpedienteManejo expedienteManejo = new ExpedienteManejo();
        expedienteManejo.CrearArchivo();
        int opcionIngresada = 0; 
        while(opcionIngresada != 8)
        {
            Console.WriteLine(" ========== SISTEMA DE GESTIÓN DE EXPEDIENTES ACÁDEMICOS ========== ");
            Console.WriteLine(" -------------- MENU PRINCIPAL DE OPCIONES -------------- ");
            Console.WriteLine("[1]| Registrar un nuevo expediente académico |");
            Console.WriteLine("[2]| Consultar un expediente académico por código de estudiante |");
            Console.WriteLine("[3]| Consultar todos los expedientes acádemicos registrados |");
            Console.WriteLine("[4]| Actualizar un expediente acádemico existente |");
            Console.WriteLine("[5]| Eliminar un expediente acádemico por número |");
            Console.WriteLine("[6]| Guardar cambios en el archivo XML |");
            Console.WriteLine("[7]| Encriptar archivo XML |");
            Console.WriteLine("Seleccione una opción:");
            opcionIngresada = int.Parse(Console.ReadLine());
            switch(opcionIngresada)
            {
                case 1:
                    Console.WriteLine(" | Opción Seleccionada:Registrar un nuevo expediente académico |");
                    Console.WriteLine(" A continuación se le solicitará ingresar los datos del expediente académico.");

                    Console.WriteLine("Ingrese el código del estudiante:");
                    string codigoEstudiante = Console.ReadLine();
                    Console.WriteLine("Ingrese el nombre del estudiante:");
                    string nombreEstudiante = Console.ReadLine();
                    Console.WriteLine("Ingrese el apellido del estudiante:");
                    string apellidoEstudiante = Console.ReadLine();
                    Console.WriteLine("Ingrese la carrera del estudiante:");
                    string carreraEstudiante = Console.ReadLine();
                    Console.WriteLine("Ingrese el semestre del estudiante:");
                    int semestreEstudiante = int.Parse(Console.ReadLine());
                    Console.WriteLine("Ingrese el correo del estudiante:");
                    string correoEstudiante = Console.ReadLine();
                    Expediente nuevoExpediente = new Expediente(codigoEstudiante, nombreEstudiante, apellidoEstudiante, carreraEstudiante, semestreEstudiante, correoEstudiante);
                    

                    int agregarCursos = 0;
                    while(agregarCursos != 2)
                    {
                        Console.WriteLine("¿Desea agregar cursos a este expediente académico? ");
                        Console.WriteLine("[1]| Sí deseao agregar cursos 1|");
                        Console.WriteLine("[2]| No deseao agregar cursos 2|");
                        Console.WriteLine("Seleccione una opción:");
                        agregarCursos = int.Parse(Console.ReadLine());
                        if(agregarCursos == 1)
                        {
                            Console.WriteLine("Se le solicitará ingresar los datos del curso.");
                            Console.WriteLine("Ingrese el código del curso:");
                            string codigoCurso = Console.ReadLine();
                            Console.WriteLine("Ingrese el nombre del curso:");
                            string nombreCurso = Console.ReadLine();
                            bool creditosAceptados = false; 
                            int creditosCurso = 0;
                            while(creditosAceptados == false)
                            {
                                Console.WriteLine("Ingrese los créditos del curso:");
                                creditosCurso = int.Parse(Console.ReadLine());
                                if(creditosCurso >= 1)
                                {
                                    creditosAceptados = true;
                                }
                                else
                                {
                                    Console.WriteLine("Los créditos del curso debe ser mayor a 0. Por favor, ingrese un valor válido.");
                                }
                            }
                            bool calificacionAceptada = false; 
                            double calificacionCurso = 0;
                            while(calificacionAceptada == false)
                            {
                                Console.WriteLine("Ingrese la calificación del curso:");
                                calificacionCurso = double.Parse(Console.ReadLine());
                                if(calificacionCurso >= 0 && calificacionCurso <= 100)
                                {
                                    calificacionAceptada = true;
                                }
                                else
                                {
                                    Console.WriteLine("La calificación del curso debe estar entre 0 y 100. Por favor, ingrese un valor válido.");
                                }
                            }

                            Console.WriteLine("Ingrese el ciclo académico:");   
                            string cicloAcademico = Console.ReadLine();        
                            Curso cursoNuevo = new Curso(codigoCurso, nombreCurso, creditosCurso, calificacionCurso, cicloAcademico);
                            nuevoExpediente.AgregarCurso(cursoNuevo);
                        }
                        else if(agregarCursos == 2)
                        {
                            Console.WriteLine("No se agregarán más cursos.");
                        }
                        else
                        {
                            Console.WriteLine("Opción inválida. Por favor, seleccione una opción válida.");
                        }
                    }
                    Expedientes? expedientesRegistrados = expedienteManejo.LeerArchivo();
                    if(expedientesRegistrados == null)
                    {
                        expedientesRegistrados = new Expedientes();
                    }
                    expedienteManejo.RegistrarExpediente(expedientesRegistrados, nuevoExpediente);
                    break;
                case 2:
                    Console.WriteLine(" | Opción Seleccionada:Consultar un expediente académico por código de estudiante |");
                    Console.WriteLine(" A continuación se le solicitará ingresar el código del estudiante del expediente académico que desea consultar.");
                    Console.WriteLine("Ingrese el código del estudiante:");
                    string codigoEstudianteConsulta = Console.ReadLine() ?? string.Empty;
                    Expedientes? expedientesAlmacenados = expedienteManejo.LeerArchivo();

                    if (expedientesAlmacenados == null)
                    {   Console.WriteLine("No se pudieron leer los expedientes.");}
                    else
                    {
                        Expediente? expedienteConsultado =expedienteManejo.BuscarExpedienteCodigo(codigoEstudianteConsulta,expedientesAlmacenados);

                        if (expedienteConsultado != null)
                        {
                            Console.WriteLine(expedienteConsultado);

                            foreach (Curso curso in expedienteConsultado.Cursos)
                            {
                                Console.WriteLine(curso);
                            }
                        }
                    }
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    break;
                case 6:
                    break;
                case 7:
                    break;
                default:
                    Console.WriteLine("Opción inválida. Por favor, seleccione una opción válida.");
                    break;

            }

        }
        

    }
}