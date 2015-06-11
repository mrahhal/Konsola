$konsola_nuspec = "Konsola.nuspec"
$nugetDir = "_nupkg"

Function mkdirb($path)
{
    if (!(Test-Path $path))
    {
        mkdir $path > $null
    }
}

mkdirb $nugetDir

# Build for Release
msbuild Konsola.sln /nologo /verbosity:minimal /property:Configuration=Release

# Build nuget packages
nuget pack $konsola_nuspec -Properties Configuration=Release -OutputDirectory $nugetDir