# Fit2Gpx

[Dansk](#dansk) | [English](#english)

## Dansk

Fit2Gpx er en lille Windows-app, der konverterer Garmin FIT-aktiviteter til GPX-filer.

Appen er enkel at bruge:

1. Trak en eller flere `.fit`-filer ind i vinduet, eller vælg dem med **Vælg filer**.
2. Fit2Gpx opretter en `.gpx`-fil i samme mappe som hver inputfil.
3. Resultatlisten viser antallet af GPS-punkter eller eventuelle fejl.

### Krav

- Windows 10 eller nyere
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) for at bygge fra kildekode
- Garmin FIT-filer med GPS-punkter og tidsstempler

Den self-contained Windows-build, der kan publiceres med kommandoen nedenfor, indeholder sin egen .NET-runtime.

### Kør fra kildekode

Installer .NET 10 SDK, og kør derefter:

```powershell
dotnet run --project .\Fit2Gpx.csproj
```

Byg en Release-version:

```powershell
dotnet build .\Fit2Gpx.csproj --configuration Release
```

### Publicer en selvstændig Windows-build

Kommandoen opretter en mappe, der kan køres uden en separat .NET-installation:

```powershell
dotnet publish .\Fit2Gpx.csproj `
  --configuration Release `
  --runtime win-x64 `
  --self-contained true `
  --output .\publish\win-x64
```

Start derefter `publish\win-x64\Fit2Gpx.exe`. GPX-filerne skrives ved siden af de originale FIT-filer; inputfilerne ændres ikke.

### Udgivelser

Når der oprettes et tag som `v1.0.0`, bygger GitHub automatisk en ZIP-fil med en færdig Windows-app og lægger den på en GitHub Release. Brugere skal derfor normalt bare hente ZIP-filen fra [Releases](https://github.com/Tntdruid/Fit2Gpx/releases), pakke den ud og starte `Fit2Gpx.exe`.

### Udvikling

Projektet er en WPF-app, der målretter `net10.0-windows`. FIT-dekodningen leveres af [Garmin.FIT.Sdk](https://www.nuget.org/packages/Garmin.FIT.Sdk/).

Bidrag er velkomne. Hold ændringer fokuserede, byg projektet i Release-mode, og beskriv den synlige ændring i pull requesten.

## English

Fit2Gpx is a small Windows desktop app that converts Garmin FIT activities to GPX files.

The workflow is intentionally simple:

1. Drop one or more `.fit` files onto the window, or choose them with **Vælg filer**.
2. Fit2Gpx creates a `.gpx` file next to each input file.
3. The result list reports the number of GPS points written or any error encountered.

### Requirements

- Windows 10 or later
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) to build from source
- Garmin FIT files containing GPS points and timestamps

The self-contained Windows build published with the command below includes its own .NET runtime.

### Run from source

Install the .NET 10 SDK, then run:

```powershell
dotnet run --project .\Fit2Gpx.csproj
```

Build a Release version:

```powershell
dotnet build .\Fit2Gpx.csproj --configuration Release
```

### Publish a standalone Windows build

The following command creates a folder that can run without a separate .NET installation:

```powershell
dotnet publish .\Fit2Gpx.csproj `
  --configuration Release `
  --runtime win-x64 `
  --self-contained true `
  --output .\publish\win-x64
```

Start `publish\win-x64\Fit2Gpx.exe` after publishing. Output GPX files are written beside the original FIT files; input files are never modified.

### Releases

When a tag such as `v1.0.0` is created, GitHub automatically builds a ZIP file containing a ready-to-run Windows app and attaches it to a GitHub Release. Users can normally just download the ZIP from [Releases](https://github.com/Tntdruid/Fit2Gpx/releases), extract it and start `Fit2Gpx.exe`.

### Development

The project is a WPF application targeting `net10.0-windows`. FIT decoding is provided by [Garmin.FIT.Sdk](https://www.nuget.org/packages/Garmin.FIT.Sdk/).

Contributions are welcome. Please keep changes focused, build the project in Release mode, and describe the user-visible behavior in the pull request.

## License

Fit2Gpx is released under the MIT License. See [LICENSE](LICENSE).
