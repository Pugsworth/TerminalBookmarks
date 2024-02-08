dotnet build
if (-not $?) {
    Write-Host "Failed to build"
    exit
}

dotnet pack
if (-not $?) {
    Write-Host "Failed to pack"
    exit
}

dotnet tool install -g --add-source ./nupkg terminalbookmarks
if (-not $?) {
    Write-Host "Failed to install, trying to update instead"
    
    dotnet tool update -g --add-source ./nupkg terminalbookmarks
    if (-not $?) {
        Write-Host "Failed to update!!"
        exit
    }
    
    Write-Host "Done!"
    
    exit
}
