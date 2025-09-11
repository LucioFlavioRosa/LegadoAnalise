# configure-iis.ps1

# Importar o certificado PFX
$pwd = ConvertTo-SecureString -String "140286Ch#" -Force -AsPlainText
Import-PfxCertificate -FilePath 'C:\certs\localhost.pfx' -CertStoreLocation 'Cert:\LocalMachine\My' -Password $pwd

# Limpar todos os bindings existentes do site Default Web Site
& C:\Windows\System32\inetsrv\appcmd.exe set site "Default Web Site" /bindings:

# Adicionar o binding HTTP (porta 80)
& C:\Windows\System32\inetsrv\appcmd.exe set site "Default Web Site" /+bindings.[protocol='http',bindingInformation='*:80:']

# Adicionar o binding HTTPS (porta 50367)
& C:\Windows\System32\inetsrv\appcmd.exe set site "Default Web Site" /+bindings.[protocol='https',bindingInformation='*:50367:']

# Obter o Thumbprint do certificado (com Subject contendo 'CN=localhost')
$thumb = (Get-ChildItem -Path Cert:\LocalMachine\My | Where-Object { $_.Subject -like '*CN=localhost*' } | Select-Object -First 1).Thumbprint

# Associar o certificado ao binding HTTPS da porta 50367
netsh http add sslcert ipport=0.0.0.0:50367 certhash=$thumb appid='{00112233-4455-6677-8899-AABBCCDDEEFF}'

# Ativar erros detalhados para o site
& C:\Windows\System32\inetsrv\appcmd.exe set config "Default Web Site" /section:httpErrors /errorMode:Detailed
& C:\Windows\System32\inetsrv\appcmd.exe set config /section:asp /scriptErrorSentToBrowser:true

# Garantir que a pasta de logs exista e tenha permissão de escrita
$logPath = 'C:\inetpub\wwwroot\App_Data\Logs'
if (-not (Test-Path $logPath)) {
    New-Item -Path $logPath -ItemType Directory -Force | Out-Null
}
$acl = Get-Acl $logPath
$rule = New-Object System.Security.AccessControl.FileSystemAccessRule('IIS_IUSRS','Modify','ContainerInherit,ObjectInherit','None','Allow')
$acl.SetAccessRule($rule)
Set-Acl $logPath $acl

Import-Module WebAdministration; Set-WebConfigurationProperty -pspath 'MACHINE/WEBROOT/APPHOST'  -filter 'system.webServer/httpErrors' -name errorMode -value 'Detailed'