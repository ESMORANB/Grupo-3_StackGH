# Sistema de Gestión de Expedientes Académicos

Proyecto de Aplicación No. 1 — 2026S2
Curso: Manejo e Implementación de Archivos
Universidad Rafael Landívar — Facultad de Ingeniería
**Grupo #3**

Sistema de consola en C# para registrar, consultar, buscar, actualizar,
eliminar y listar expedientes académicos de estudiantes, almacenados en un
archivo XML que permanece **encriptado en disco** en todo momento.

---

## Integrantes

| Integrante | Carné | Módulo |
|---|---|---|
| Stephanie Roxana Ruano Berganza | 1100925 | Modelo de datos y estructura XML |
| Clara Mía Melgar Pineda | 1125825 | Registro y consulta |
| Eduardo Sebastián Morán Bautista | 1119425 | Búsqueda y listado |
| Angela Jimena Santizo Escobar | 1207925 | Actualización, eliminación y validaciones |
| Fátima Joanna López Quiñonez | 1088825 | Encriptación, menú e integración |

---

## Requisitos previos

Antes de correr el sistema o los scripts, la máquina necesita tener instalado:

- **Git**
- **.NET SDK 8**

Ninguno de los scripts instala estas herramientas por sí solo — si faltan,
el script se detiene con un mensaje de error claro indicando en qué paso falló.

---

## Cómo correr el sistema

Hay tres formas, de la más automática a la más manual.

### Opción 1 — Doble click, sin usar terminal (`Desplegar.bat`)

Pensado para cualquier persona, incluso sin conocimientos técnicos
(por ejemplo, para que el catedrático lo use directamente).

1. Descarga únicamente el archivo `Desplegar.bat` desde el repositorio
   (botón de descarga en GitHub — no hace falta clonar nada antes).
2. Dale doble click. Windows mostrará una advertencia de seguridad
   ("publicador no verificado") — es normal en archivos descargados,
   dale **"Ejecutar"**.
3. El script, sin pedir nada más, hace todo el trabajo:
   - Crea la carpeta `C:\mia_proyecto_I`
   - Clona el repositorio dentro de esa carpeta
   - Compila el proyecto con `dotnet build`
   - Copia el ejecutable final a `C:\mia_proyecto_I\ejecutable`
   - Abre el sistema automáticamente al terminar

### Opción 2 — Scripts de despliegue por terminal

**Windows (PowerShell):**
```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
.\deploy.ps1
```
Parámetros opcionales: `-RepoUrl "<url>"` y `-Rama "<rama>"`.

**Linux / Mac (Bash):**
```bash
chmod +x deploy.sh
./deploy.sh
```
Parámetros opcionales (posicionales): `./deploy.sh "<url>" "<rama>"`.

Ambos hacen exactamente las mismas 5 tareas que `Desplegar.bat`, adaptadas
a su sistema operativo (en Linux/Mac se usa `$HOME/mia_proyecto_I` en vez
de `C:\`, ya que ese disco no existe fuera de Windows).

### Opción 3 — Ya tienes el repo clonado (modo desarrollo)

Dentro de la carpeta `ExpedientesAcademicos/`:

```powershell
# Windows
.\Ejecutar.bat
```
```bash
# Linux / Mac
chmod +x Ejecutar.sh
./Ejecutar.sh
```

Compila y corre el sistema directo desde el código fuente local, sin
clonar ni volver a descargar nada.

---

## Ruta del archivo de datos

Al arrancar, el sistema pregunta:

```
Ruta del archivo XML (Enter para usar la ruta por defecto):
```

- **Presiona Enter** para usar la ruta por defecto, que coincide con donde
  quedan los datos tras usar cualquiera de los scripts de despliegue:
  ```
  C:\mia_proyecto_I\repo\ExpedientesAcademicos\datos\expedientes.xml.enc
  ```
- **O escribe una ruta distinta** si quieres guardar o abrir el archivo
  encriptado en otro lugar.

---

## Menú del sistema

1. Registrar un nuevo expediente académico
2. Consultar un expediente por código de estudiante
3. Consultar todos los expedientes registrados
4. Actualizar un expediente existente
5. Actualizar o agregar un curso a un expediente
6. Eliminar un expediente
7. Eliminar un curso de un expediente
8. Cambiar la contraseña del sistema
9. Salir

---

## Estructura del archivo XML

```xml
<expedientes>
  <expediente>
    <codigoEstudiante>20231234</codigoEstudiante>
    <nombre>Nombre</nombre>
    <apellido>Apellido</apellido>
    <carrera>Ingeniería en Informática y Sistemas</carrera>
    <semestre>8</semestre>
    <correo>correo@url.edu.gt</correo>
    <fechaIngreso>2022-01-15</fechaIngreso>
    <estado>Activo</estado>
    <cursos>
      <curso>
        <codigo>MIA101</codigo>
        <nombre>Manejo e Implementación de Archivos</nombre>
        <creditos>3</creditos>
        <nota>90</nota>
        <ciclo>2026-S2</ciclo>
      </curso>
    </cursos>
  </expediente>
</expedientes>
```

El archivo permanece **siempre encriptado en disco** (extensión `.xml.enc`).
Solo se desencripta en memoria, temporalmente, para leer o modificar los
datos, y se vuelve a encriptar de inmediato al guardar.

---

## Estructura del repositorio

```
Grupo-3_StackGH/
├── deploy.ps1              # Script de despliegue (Windows, PowerShell)
├── deploy.sh                # Script de despliegue (Linux/Mac, Bash)
├── Desplegar.bat             # Script de doble click (Windows, autocontenido)
├── README.md
└── ExpedientesAcademicos/
    ├── ExpedientesAcademicos.csproj
    ├── SistemaExpedientes.cs   # Menú principal
    ├── Config.cs               # Ruta y contraseña por defecto
    ├── Encriptacion.cs
    ├── ModuloCreacion.cs
    ├── ModuloBusqueda.cs
    ├── ModuloActualizacionEliminacion.cs
    ├── Ejecutar.bat / Ejecutar.sh   # Compilar y correr en modo desarrollo
    ├── modelos/
    │   ├── Curso.cs
    │   ├── Expediente.cs
    │   └── Expedientes.cs
    └── datos/
        └── expedientes.xml.enc
```
