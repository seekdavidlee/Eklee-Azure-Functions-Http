
	$ErrorActionPreference = "Stop"

	Push-Location Eklee.Azure.Functions.Http.Tests
	dotnet test
	Pop-Location

	if ($lastexitcode -ne 0){
		return;
	}

	$buildConfig = "Release"
	$app = "Eklee.Azure.Functions.Http"	

	$currentDir = Get-Location

	pushd Examples\Eklee.Azure.Functions.Http.Example
	dotnet build --configuration=$buildConfig
	popd

	pushd Examples\Eklee.Azure.Functions.Http.Example\bin\Release\netstandard2.0
	npm install --save-dev azure-functions-core-tools@3
	npm install --save-dev newman
	popd
	$hostJob = Start-Job -ScriptBlock {
		param([string]$currentDir)
		Write-Host $currentDir
		cd $currentDir
		Get-Location
		pushd Examples\Eklee.Azure.Functions.Http.Example\bin\Release\netstandard2.0
		node_modules\.bin\func host start
	} -ArgumentList $currentDir
	
	Start-Sleep -s 10

	Receive-Job -Name $hostJob.Name

	$testJob = Start-Job -ScriptBlock {
		param([string]$currentDir)
		Write-Host $currentDir
		cd $currentDir
		pushd Examples\Eklee.Azure.Functions.Http.Example\bin\Release\netstandard2.0
		node_modules\.bin\newman run ..\..\..\..\..\tests\Eklee.Azure.Functions.Http.Tests.postman_collection.json -e ..\..\..\..\..\tests\Eklee.Azure.Functions.Http.Local.postman_environment.json
	} -ArgumentList $currentDir

	Wait-Job -Name $testJob.Name

	$testResult = Receive-Job -Name $testJob.Name
	$testResult

	Write-Host "Stopping Jobs"

	Stop-Job $hostJob
	Stop-Job $testJob

	if ($testResult -like "*AssertionError*") {
		Write-Host "Failed!" -ForegroundColor red
	} else {
		Write-Host "All good!" -ForegroundColor green

		Write-Host "Generating nuget package"

		pushd .\$app
		dotnet clean --configuration $buildConfig
		dotnet build --configuration $buildConfig
		Move-Item -Path bin\Release\netstandard2.0\bin\$app.dll -Destination bin\Release\netstandard2.0\$app.dll
		popd
		Remove-Item $currentDir\*.nupkg
		Copy-Item $currentDir\LICENSE $currentDir\LICENSE.txt
		nuget.exe pack $app\$app.csproj -Properties Configuration=$buildConfig -IncludeReferencedProjects
		Remove-Item $currentDir\LICENSE.txt
	}
	