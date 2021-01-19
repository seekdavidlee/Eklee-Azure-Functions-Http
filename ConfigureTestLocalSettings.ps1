param(
	[Parameter(Mandatory=$True)][string]$Name,
	[Parameter(Mandatory=$True)][string]$SourceRootDir)

$ErrorActionPreference = "Stop"

function ConvertSecretToPlainText($Secret) {

	$bstr = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($Secret)
	return [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($bstr)
}


$values = @( @{ "key" = "endpoint"; "value" = "http://localhost:7071"; "enabled" = "true" } )

$environmentFile = @{ "id"="d9a0b2d1-5c39-4671-83fb-e9a1f7f404a1"; "name" = "Eklee.Azure.Functions.Http.Local"; "values" = $values }

Get-AzKeyVaultSecret -VaultName $Name| ForEach-Object {
    $keyName = $_.Name

    if ($keyName.StartsWith("postman-")) {

        $keyVaultKeyName = $keyName.Replace("postman-","")
        Write-Host "Processing $keyVaultKeyName"

        $secret = (Get-AzKeyVaultSecret -VaultName $Name -Name $keyName)

        if (!$secret){
            Write-Host "Unable to find $keyVaultKeyName in $Name"
        } else {
			$text = ConvertSecretToPlainText -Secret $secret.SecretValue
            $environmentFile.values += @{ "key" = $keyVaultKeyName; "value" = $text; "enabled" = "true" }
        }          
    }
}

$environmentFile | ConvertTo-Json | Out-File $SourceRootDir\Tests\Eklee.Azure.Functions.Http.Local.postman_environment.json -Encoding ASCII

$localSettingsFileContent = '{
	"IsEncrypted": false,
	"Values": {
		"Audience": "%audienceId%",
		"Issuers": "%issuer1% %issuer2%"
	}	
}'

$audienceId = ConvertSecretToPlainText -Secret (Get-AzKeyVaultSecret -VaultName $Name -Name "local-audienceId2").SecretValue
$issuer1 = ConvertSecretToPlainText -Secret (Get-AzKeyVaultSecret -VaultName $Name -Name "local-issuer1").SecretValue
$issuer2 = ConvertSecretToPlainText -Secret (Get-AzKeyVaultSecret -VaultName $Name -Name "local-issuer2").SecretValue

$localSettingsFileContent = $localSettingsFileContent.Replace("%audienceId%", $audienceId)
$localSettingsFileContent = $localSettingsFileContent.Replace("%issuer1%", $issuer1)
$localSettingsFileContent = $localSettingsFileContent.Replace("%issuer2%", $issuer2)

$localSettingsFileContent | Out-File $SourceRootDir\Examples\Eklee.Azure.Functions.Http.Example\local.settings.json -Encoding ASCII