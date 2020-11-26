param(
    [Parameter(Mandatory = $True)][string]$Path)

$app = "Eklee.Azure.Functions.Http"
$filePath = "$Path\$app\bin\Release\netstandard2.1\bin\$app.dll"

if (![System.IO.File]::Exists($filePath)){
    Write-Host "Missing $filePath"
    Get-ChildItem -Path "$Path\$app\bin\Release\netstandard2.1\bin"
}

Remove-Item -Path "$Path\$app\bin\Release\netstandard2.1\bin" -Recurse
Copy-Item "$Path\LICENSE" "$Path\LICENSE.txt"