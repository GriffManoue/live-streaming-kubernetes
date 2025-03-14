Get-ChildItem -Path * -Filter "deployment.yaml" -Recurse | Where-Object { $_.FullName -like "*\k8s\*" } | ForEach-Object {
    (Get-Content $_.FullName) -replace '\${REGISTRY_URL}', 'bugapeti' | Set-Content $_.FullName
}