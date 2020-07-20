param ([string]$outdir, [string]$outDll, [string]$xml, [string]$solutionDir)
$dllpath = $outdir + $outDll
$version = [System.Reflection.Assembly]::LoadFrom($dllpath).GetName().Version
cd $solutionDir
$commit = git log --pretty="%h" -n 1 -- $solutionDir

[xml]$xdoc  = get-content $xml
$xdoc.LanguageData.BetterColonistBar_Version = $version.ToString()
$xdoc.LanguageData.BetterColonistBar_Commit = $commit.ToString()
$xdoc.Save($xml)