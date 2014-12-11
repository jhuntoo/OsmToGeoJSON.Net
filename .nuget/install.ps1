param($installPath, $toolsPath, $package, $project)

$polygonFeaturesFile = $project.ProjectItems.Item("polygonFeatures.json")

# set 'Copy To Output Directory' to 'Copy always'
$copyToOutput = $polygonFeaturesFile.Properties.Item("CopyToOutputDirectory")
$copyToOutput.Value = 1
# 1 = CopyAlways