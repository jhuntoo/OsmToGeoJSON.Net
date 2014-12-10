$root = (split-path -parent $MyInvocation.MyCommand.Definition) + '\..'
$version = [System.Reflection.Assembly]::LoadFile("$root\OsmToGeoJSON\bin\Release\OsmToGeoJSON.dll").GetName().Version
$versionStr = "{0}.{1}.{2}" -f ($version.Major, $version.Minor, $version.Build)

Write-Host "Setting .nuspec version tag to $versionStr"

$content = (Get-Content $root\.nuGet\OsmToGeoJSON.nuspec) 
$content = $content -replace '\$version\$',$versionStr

$content | Out-File $root\.nuget\OsmToGeoJSON.compiled.nuspec

& $root\.nuGet\NuGet.exe pack $root\.nuget\OsmToGeoJSON.compiled.nuspec