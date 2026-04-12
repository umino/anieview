dotnet publish AnieView.Wpf/AnieView.Wpf.csproj -c Release -o publish/
$path = "AnieView.zip"
if (Test-Path $path){
  remove-item AnieView.zip
}

Compress-Archive -Path publish/AnieView.Wpf.exe, publish/*.dll -DestinationPath AnieView.zip
