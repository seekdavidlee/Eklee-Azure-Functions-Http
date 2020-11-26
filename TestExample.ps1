param(
	[Parameter(Mandatory = $True)][string]$Path,
	[Parameter(Mandatory = $True)][string]$BuildConfig,
	[Parameter(Mandatory = $True)][string]$ReportDir,
	[Parameter(Mandatory = $True)][string]$EnvironmentPath,
	[Parameter(Mandatory = $True)][string]$Name,
	[Parameter(Mandatory = $True)][string]$Location)

$WorkingDirectory = "$Path\Examples\Eklee.Azure.Functions.Http.Example\bin\$BuildConfig\netstandard2.1"

$StackName = ($Name + $env:Build_BuildNumber).Replace(".", "")

Compress-Archive -Path "$WorkingDirectory\*" -DestinationPath "$WorkingDirectory\Deploy.zip"

az deployment group create `
	--name $StackName `
	--resource-group $Name `
	--template-file Templates/app.json `
	--parameters plan_name=$StackName location=$Location | Out-Null

$content = Get-Content -Path "$Path\Examples\Eklee.Azure.Functions.Http.Example\local.settings.json" | ConvertFrom-Json

$audience = $content.Values.Audience
$issuers = $content.Values.Issuers

az functionapp config appsettings set -n $StackName -g $Name --settings "Audience=$audience" "Issuers=$issuers" | Out-Null

az functionapp deployment source config-zip -g $Name -n $StackName --src "$WorkingDirectory\Deploy.zip" | Out-Null

Push-Location $WorkingDirectory
npm install --save-dev newman
Pop-Location

$content = (Get-Content -Path "$Path\Tests\Eklee.Azure.Functions.Http.Local.postman_environment.json").Replace("http://localhost:7071", "https://$StackName.azurewebsites.net")
$content | Out-File "$Path\Tests\Eklee.Azure.Functions.Http.Local.postman_environment.json" -Encoding ASCII

$reportFilePath = "$ReportDir/report.xml"
Push-Location $Path\Examples\Eklee.Azure.Functions.Http.Example\bin\$BuildConfig\netstandard2.1
node_modules\.bin\newman run "$EnvironmentPath\Tests\Eklee.Azure.Functions.Http.Tests.postman_collection.json" -e "$EnvironmentPath\Tests\Eklee.Azure.Functions.Http.Local.postman_environment.json" --reporters 'cli,junit' --reporter-junit-export $reportFilePath
Pop-Location