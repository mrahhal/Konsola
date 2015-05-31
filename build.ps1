$konsola_dir = "Konsola"
$konsola_csproj = Join-Path $konsola_dir "/Konsola.csproj"
$konsola_nuspec = Join-Path $konsola_dir "/Konsola.nuspec"
$nugetDir = "_nupkgs"

Function mkdirb($path)
{
    if (!(Test-Path $path))
    {
        mkdir $path > $null
    }
}

# Create .nuget directory
mkdirb $nugetDir

# Build for Release
msbuild /nologo /verbosity:minimal /property:Configuration=Release

# Build nuget packages
nuget pack $konsola_nuspec -Properties Configuration=Release -OutputDirectory $nugetDir