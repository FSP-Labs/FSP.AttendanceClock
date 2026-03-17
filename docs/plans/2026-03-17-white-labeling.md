# White-labeling: D3Fichadora → FSP.AttendanceClock

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Remove all traces of D3Fichadora/Desing3 from the codebase and prepare it for open-source release on GitHub as "FSP Attendance Clock".

**Architecture:** Renombrado completo en tres capas — directorios físicos, archivos de proyecto (.sln/.csproj) y namespaces/usings en código fuente — seguido de cambios de branding visual y preparación GitHub (.gitignore, README, appsettings seguro).

**Tech Stack:** ASP.NET Core 9, C#, PostgreSQL, EF Core 9, Razor Views, ClosedXML.

**Working directory:** `D:\Proyectos\FSP Attendance Clock\` (raíz del repo)

---

## Task 1: Añadir .gitignore estándar de .NET

**Files:**
- Create: `.gitignore`

**Step 1: Crear el fichero .gitignore**

```
## .NET build artifacts
bin/
obj/
*.user
.vs/

## appsettings con credenciales (ver appsettings.Example.json)
**/appsettings.json
**/appsettings.Development.json

## JetBrains Rider
.idea/

## macOS
.DS_Store
```

Crear `D:\Proyectos\FSP Attendance Clock\.gitignore` con ese contenido.

**Step 2: Verificar que no hay nada más que añadir**

```bash
cat .gitignore
```
Expected: el contenido del fichero.

---

## Task 2: Renombrar directorios físicos

Los directorios actuales son `D3Fichadora.Core`, `D3Fichadora.Infrastructure`, `D3Fichadora.Web`. Hay que renombrarlos.

**Step 1: Renombrar los tres directorios**

```bash
mv "D3Fichadora.Core" "FSP.AttendanceClock.Core"
mv "D3Fichadora.Infrastructure" "FSP.AttendanceClock.Infrastructure"
mv "D3Fichadora.Web" "FSP.AttendanceClock.Web"
```

**Step 2: Renombrar el fichero .sln**

```bash
mv "D3Fichadora.sln" "FSP.AttendanceClock.sln"
```

**Step 3: Verificar**

```bash
ls -1
```
Expected: ver `FSP.AttendanceClock.Core/`, `FSP.AttendanceClock.Infrastructure/`, `FSP.AttendanceClock.Web/`, `FSP.AttendanceClock.sln`.

---

## Task 3: Reescribir el fichero .sln

**Files:**
- Modify: `FSP.AttendanceClock.sln`

**Step 1: Reemplazar el contenido completo del .sln**

Sobreescribir `FSP.AttendanceClock.sln` con este contenido (los GUIDs de los proyectos se conservan igual — son identificadores del Visual Studio, no de marca):

```

Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version 17
VisualStudioVersion = 17.0.31903.59
MinimumVisualStudioVersion = 10.0.40219.1
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "FSP.AttendanceClock.Core", "FSP.AttendanceClock.Core\FSP.AttendanceClock.Core.csproj", "{3983D4EC-698B-470C-A235-B485CBAE8B6D}"
EndProject
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "FSP.AttendanceClock.Infrastructure", "FSP.AttendanceClock.Infrastructure\FSP.AttendanceClock.Infrastructure.csproj", "{9CA896B9-A6E8-4187-A7D3-C4A772370B83}"
EndProject
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "FSP.AttendanceClock.Web", "FSP.AttendanceClock.Web\FSP.AttendanceClock.Web.csproj", "{D5882D29-3C10-4A3B-B181-D3F6947BC0A7}"
EndProject
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Debug|x64 = Debug|x64
		Debug|x86 = Debug|x86
		Release|Any CPU = Release|Any CPU
		Release|x64 = Release|x64
		Release|x86 = Release|x86
	EndGlobalSection
	GlobalSection(ProjectConfigurationPlatforms) = postSolution
		{3983D4EC-698B-470C-A235-B485CBAE8B6D}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{3983D4EC-698B-470C-A235-B485CBAE8B6D}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{3983D4EC-698B-470C-A235-B485CBAE8B6D}.Debug|x64.ActiveCfg = Debug|Any CPU
		{3983D4EC-698B-470C-A235-B485CBAE8B6D}.Debug|x64.Build.0 = Debug|Any CPU
		{3983D4EC-698B-470C-A235-B485CBAE8B6D}.Debug|x86.ActiveCfg = Debug|Any CPU
		{3983D4EC-698B-470C-A235-B485CBAE8B6D}.Debug|x86.Build.0 = Debug|Any CPU
		{3983D4EC-698B-470C-A235-B485CBAE8B6D}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{3983D4EC-698B-470C-A235-B485CBAE8B6D}.Release|Any CPU.Build.0 = Release|Any CPU
		{3983D4EC-698B-470C-A235-B485CBAE8B6D}.Release|x64.ActiveCfg = Release|Any CPU
		{3983D4EC-698B-470C-A235-B485CBAE8B6D}.Release|x64.Build.0 = Release|Any CPU
		{3983D4EC-698B-470C-A235-B485CBAE8B6D}.Release|x86.ActiveCfg = Release|Any CPU
		{3983D4EC-698B-470C-A235-B485CBAE8B6D}.Release|x86.Build.0 = Release|Any CPU
		{9CA896B9-A6E8-4187-A7D3-C4A772370B83}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{9CA896B9-A6E8-4187-A7D3-C4A772370B83}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{9CA896B9-A6E8-4187-A7D3-C4A772370B83}.Debug|x64.ActiveCfg = Debug|Any CPU
		{9CA896B9-A6E8-4187-A7D3-C4A772370B83}.Debug|x64.Build.0 = Debug|Any CPU
		{9CA896B9-A6E8-4187-A7D3-C4A772370B83}.Debug|x86.ActiveCfg = Debug|Any CPU
		{9CA896B9-A6E8-4187-A7D3-C4A772370B83}.Debug|x86.Build.0 = Debug|Any CPU
		{9CA896B9-A6E8-4187-A7D3-C4A772370B83}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{9CA896B9-A6E8-4187-A7D3-C4A772370B83}.Release|Any CPU.Build.0 = Release|Any CPU
		{9CA896B9-A6E8-4187-A7D3-C4A772370B83}.Release|x64.ActiveCfg = Release|Any CPU
		{9CA896B9-A6E8-4187-A7D3-C4A772370B83}.Release|x64.Build.0 = Release|Any CPU
		{9CA896B9-A6E8-4187-A7D3-C4A772370B83}.Release|x86.ActiveCfg = Release|Any CPU
		{9CA896B9-A6E8-4187-A7D3-C4A772370B83}.Release|x86.Build.0 = Release|Any CPU
		{D5882D29-3C10-4A3B-B181-D3F6947BC0A7}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{D5882D29-3C10-4A3B-B181-D3F6947BC0A7}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{D5882D29-3C10-4A3B-B181-D3F6947BC0A7}.Debug|x64.ActiveCfg = Debug|Any CPU
		{D5882D29-3C10-4A3B-B181-D3F6947BC0A7}.Debug|x64.Build.0 = Debug|Any CPU
		{D5882D29-3C10-4A3B-B181-D3F6947BC0A7}.Debug|x86.ActiveCfg = Debug|Any CPU
		{D5882D29-3C10-4A3B-B181-D3F6947BC0A7}.Debug|x86.Build.0 = Debug|Any CPU
		{D5882D29-3C10-4A3B-B181-D3F6947BC0A7}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{D5882D29-3C10-4A3B-B181-D3F6947BC0A7}.Release|Any CPU.Build.0 = Release|Any CPU
		{D5882D29-3C10-4A3B-B181-D3F6947BC0A7}.Release|x64.ActiveCfg = Release|Any CPU
		{D5882D29-3C10-4A3B-B181-D3F6947BC0A7}.Release|x64.Build.0 = Release|Any CPU
		{D5882D29-3C10-4A3B-B181-D3F6947BC0A7}.Release|x86.ActiveCfg = Release|Any CPU
		{D5882D29-3C10-4A3B-B181-D3F6947BC0A7}.Release|x86.Build.0 = Release|Any CPU
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
EndGlobal
```

---

## Task 4: Renombrar y actualizar los ficheros .csproj

**Files:**
- Rename+Modify: `FSP.AttendanceClock.Core/D3Fichadora.Core.csproj` → `FSP.AttendanceClock.Core.csproj`
- Rename+Modify: `FSP.AttendanceClock.Infrastructure/D3Fichadora.Infrastructure.csproj` → `FSP.AttendanceClock.Infrastructure.csproj`
- Rename+Modify: `FSP.AttendanceClock.Web/D3Fichadora.Web.csproj` → `FSP.AttendanceClock.Web.csproj`

**Step 1: Renombrar los .csproj**

```bash
mv "FSP.AttendanceClock.Core/D3Fichadora.Core.csproj" "FSP.AttendanceClock.Core/FSP.AttendanceClock.Core.csproj"
mv "FSP.AttendanceClock.Infrastructure/D3Fichadora.Infrastructure.csproj" "FSP.AttendanceClock.Infrastructure/FSP.AttendanceClock.Infrastructure.csproj"
mv "FSP.AttendanceClock.Web/D3Fichadora.Web.csproj" "FSP.AttendanceClock.Web/FSP.AttendanceClock.Web.csproj"
```

**Step 2: Reescribir `FSP.AttendanceClock.Core/FSP.AttendanceClock.Core.csproj`**

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <RootNamespace>FSP.AttendanceClock.Core</RootNamespace>
    <AssemblyName>FSP.AttendanceClock.Core</AssemblyName>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
```

**Step 3: Reescribir `FSP.AttendanceClock.Infrastructure/FSP.AttendanceClock.Infrastructure.csproj`**

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <ItemGroup>
    <ProjectReference Include="..\FSP.AttendanceClock.Core\FSP.AttendanceClock.Core.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="9.0.0">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="9.0.2" />
  </ItemGroup>

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <RootNamespace>FSP.AttendanceClock.Infrastructure</RootNamespace>
    <AssemblyName>FSP.AttendanceClock.Infrastructure</AssemblyName>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
```

**Step 4: Reescribir `FSP.AttendanceClock.Web/FSP.AttendanceClock.Web.csproj`**

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <ItemGroup>
    <ProjectReference Include="..\FSP.AttendanceClock.Infrastructure\FSP.AttendanceClock.Infrastructure.csproj" />
    <ProjectReference Include="..\FSP.AttendanceClock.Core\FSP.AttendanceClock.Core.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="ClosedXML" Version="0.105.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="9.0.0">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
  </ItemGroup>

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <RootNamespace>FSP.AttendanceClock.Web</RootNamespace>
    <AssemblyName>FSP.AttendanceClock.Web</AssemblyName>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

</Project>
```

---

## Task 5: Reemplazar namespaces D3Fichadora en todos los ficheros fuente

Este task reemplaza todas las ocurrencias en ficheros `.cs` y `.cshtml` **fuera de bin/ y obj/**.

**Step 1: Reemplazar en todos los .cs (excepto bin/obj)**

```bash
find . \( -path "*/bin/*" -o -path "*/obj/*" \) -prune -o -name "*.cs" -print | \
  xargs sed -i \
    -e 's/D3Fichadora\.Core/FSP.AttendanceClock.Core/g' \
    -e 's/D3Fichadora\.Infrastructure/FSP.AttendanceClock.Infrastructure/g' \
    -e 's/D3Fichadora\.Web/FSP.AttendanceClock.Web/g'
```

**Step 2: Reemplazar en todos los .cshtml**

```bash
find . \( -path "*/bin/*" -o -path "*/obj/*" \) -prune -o -name "*.cshtml" -print | \
  xargs sed -i \
    -e 's/D3Fichadora\.Core/FSP.AttendanceClock.Core/g' \
    -e 's/D3Fichadora\.Infrastructure/FSP.AttendanceClock.Infrastructure/g' \
    -e 's/D3Fichadora\.Web/FSP.AttendanceClock.Web/g'
```

**Step 3: Verificar que no quedan referencias antiguas**

```bash
grep -r "D3Fichadora" --include="*.cs" --include="*.cshtml" \
  --exclude-dir=bin --exclude-dir=obj .
```
Expected: sin output (cero resultados).

---

## Task 6: Renombrar clase y fichero del migration snapshot

El fichero `D3FichadoraInfrastructureModelSnapshot.cs` contiene la clase del mismo nombre. Hay que renombrarlo y actualizar la clase.

**Files:**
- Rename: `FSP.AttendanceClock.Infrastructure/Migrations/D3FichadoraInfrastructureModelSnapshot.cs` → `FSPAttendanceClockInfrastructureModelSnapshot.cs`

**Step 1: Renombrar el fichero**

```bash
mv "FSP.AttendanceClock.Infrastructure/Migrations/D3FichadoraInfrastructureModelSnapshot.cs" \
   "FSP.AttendanceClock.Infrastructure/Migrations/FSPAttendanceClockInfrastructureModelSnapshot.cs"
```

**Step 2: Renombrar la clase dentro del fichero**

En `FSP.AttendanceClock.Infrastructure/Migrations/FSPAttendanceClockInfrastructureModelSnapshot.cs`, reemplazar:
- `class D3FichadoraInfrastructureModelSnapshot` → `class FSPAttendanceClockInfrastructureModelSnapshot`

Usar Edit tool para hacer el cambio en esa línea concreta.

**Step 3: Verificar que no quedan referencias al nombre antiguo**

```bash
grep -r "D3FichadoraInfrastructureModelSnapshot" --exclude-dir=bin --exclude-dir=obj .
```
Expected: sin output.

---

## Task 7: Actualizar branding visual en _Layout.cshtml

**Files:**
- Modify: `FSP.AttendanceClock.Web/Views/Shared/_Layout.cshtml`

**Step 1: Actualizar el título de la página (línea 6)**

Cambiar:
```html
<title>@ViewData["Title"] - D3Fichadora</title>
```
Por:
```html
<title>@ViewData["Title"] - FSP Attendance Clock</title>
```

**Step 2: Actualizar la referencia al CSS de scoped styles (línea 11)**

Cambiar:
```html
<link rel="stylesheet" href="~/D3Fichadora.Web.styles.css" asp-append-version="true" />
```
Por:
```html
<link rel="stylesheet" href="~/FSP.AttendanceClock.Web.styles.css" asp-append-version="true" />
```

**Step 3: Actualizar el logo en el navbar (línea 18)**

Cambiar:
```html
<img src="~/img/desing3_logo-principal-05.png" alt="Desing3" class="brand-logo" />
```
Por:
```html
<img src="~/img/logo.png" alt="FSP Attendance Clock" class="brand-logo" />
```

---

## Task 8: Reemplazar el logo

**Files:**
- Create: `FSP.AttendanceClock.Web/wwwroot/img/logo.png` (placeholder)
- Delete: `FSP.AttendanceClock.Web/wwwroot/img/desing3_logo-principal-05.png`

**Step 1: Crear un PNG placeholder mínimo**

Ejecutar en bash (genera un PNG válido de 200x50px usando Python):

```bash
python3 -c "
import struct, zlib

def make_png(width, height):
    def chunk(name, data):
        c = name + data
        return struct.pack('>I', len(data)) + c + struct.pack('>I', zlib.crc32(c) & 0xffffffff)

    sig = b'\\x89PNG\\r\\n\\x1a\\n'
    ihdr = chunk(b'IHDR', struct.pack('>IIBBBBB', width, height, 8, 2, 0, 0, 0))

    # White pixel row
    raw = b''
    for y in range(height):
        raw += b'\\x00' + b'\\xff\\xff\\xff' * width

    idat = chunk(b'IDAT', zlib.compress(raw))
    iend = chunk(b'IEND', b'')
    return sig + ihdr + idat + iend

with open('FSP.AttendanceClock.Web/wwwroot/img/logo.png', 'wb') as f:
    f.write(make_png(200, 50))
print('logo.png created')
"
```

**Step 2: Eliminar el logo antiguo**

```bash
rm "FSP.AttendanceClock.Web/wwwroot/img/desing3_logo-principal-05.png"
```

**Step 3: Verificar**

```bash
ls FSP.AttendanceClock.Web/wwwroot/img/
```
Expected: solo `logo.png`.

---

## Task 9: Crear appsettings.Example.json (seguridad para GitHub)

El `appsettings.json` contiene credenciales reales y NO debe subirse a GitHub. En su lugar se provee un fichero de ejemplo.

**Files:**
- Create: `FSP.AttendanceClock.Web/appsettings.Example.json`

**Step 1: Crear el fichero de ejemplo**

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Host=<your-db-host>;Port=5432;Database=FSPAttendanceClock;Username=<your-db-user>;Password=<your-db-password>"
  }
}
```

**Step 2: Verificar que appsettings.json está en .gitignore**

```bash
grep "appsettings.json" .gitignore
```
Expected: ver la línea `**/appsettings.json`.

---

## Task 10: Actualizar SECURITY_REVIEW_X_FORWARDED_FOR.md

**Files:**
- Modify: `SECURITY_REVIEW_X_FORWARDED_FOR.md`

**Step 1: Reemplazar referencias al namespace antiguo en el fichero**

```bash
sed -i 's/D3Fichadora\.Web\.Extensions/FSP.AttendanceClock.Web.Extensions/g' SECURITY_REVIEW_X_FORWARDED_FOR.md
sed -i 's/namespace D3Fichadora/namespace FSP.AttendanceClock/g' SECURITY_REVIEW_X_FORWARDED_FOR.md
```

**Step 2: Verificar**

```bash
grep "D3Fichadora" SECURITY_REVIEW_X_FORWARDED_FOR.md
```
Expected: sin output.

---

## Task 11: Crear README.md

**Files:**
- Create: `README.md`

**Step 1: Crear el README**

```markdown
# FSP Attendance Clock

Sistema de control de fichajes (check-in/check-out) para empleados. Desarrollado en ASP.NET Core 9 con PostgreSQL.

## Funcionalidades

- **Empleados**: fichar entrada/salida, ver historial, cambiar contraseña
- **Administradores**: gestión de usuarios, auditoría completa, exportación de informes de horas en Excel

## Requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9)
- PostgreSQL 14+

## Configuración

1. Copia el fichero de ejemplo y rellena tus datos de conexión:
   ```bash
   cp FSP.AttendanceClock.Web/appsettings.Example.json FSP.AttendanceClock.Web/appsettings.json
   ```

2. Edita `FSP.AttendanceClock.Web/appsettings.json` con tu cadena de conexión a PostgreSQL.

3. Aplica las migraciones de base de datos:
   ```bash
   dotnet ef database update --project FSP.AttendanceClock.Infrastructure --startup-project FSP.AttendanceClock.Web
   ```

4. Arranca la aplicación:
   ```bash
   dotnet run --project FSP.AttendanceClock.Web
   ```

## Logo personalizado

Reemplaza el fichero `FSP.AttendanceClock.Web/wwwroot/img/logo.png` con tu propio logo. El fichero actual es un placeholder blanco (200×50 px). No es necesario tocar ningún fichero de código.

## Seguridad

Lee [`SECURITY_REVIEW_X_FORWARDED_FOR.md`](SECURITY_REVIEW_X_FORWARDED_FOR.md) antes de desplegar en producción detrás de un proxy inverso.

## Licencia

MIT
```

---

## Task 12: Actualizar CLAUDE.md

**Files:**
- Modify: `CLAUDE.md`

**Step 1: Reemplazar todas las referencias a D3Fichadora en CLAUDE.md**

```bash
sed -i \
  -e 's/D3Fichadora\.Core/FSP.AttendanceClock.Core/g' \
  -e 's/D3Fichadora\.Infrastructure/FSP.AttendanceClock.Infrastructure/g' \
  -e 's/D3Fichadora\.Web/FSP.AttendanceClock.Web/g' \
  -e 's/D3Fichadora/FSP.AttendanceClock/g' \
  CLAUDE.md
```

**Step 2: Verificar**

```bash
grep "D3Fichadora\|Desing3" CLAUDE.md
```
Expected: sin output.

---

## Task 13: Verificación final — build limpio

**Step 1: Verificar que no quedan referencias D3Fichadora en ningún fichero de código**

```bash
grep -r "D3Fichadora\|Desing3\|desing3" \
  --include="*.cs" --include="*.cshtml" --include="*.csproj" --include="*.sln" --include="*.json" --include="*.md" \
  --exclude-dir=bin --exclude-dir=obj \
  .
```
Expected: **cero resultados**.

**Step 2: Compilar el proyecto completo**

```bash
dotnet build FSP.AttendanceClock.sln
```
Expected: `Build succeeded. 0 Warning(s). 0 Error(s).`

**Step 3: Si hay errores**

- Error `namespace not found`: revisar que el sed del Task 5 cubrió todos los ficheros.
- Error `file not found` en .sln: verificar rutas en Task 3.
- Error `ProjectReference`: verificar .csproj del Task 4.
