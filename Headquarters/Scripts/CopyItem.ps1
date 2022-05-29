param([bool]$PC111,[bool]$PC112,[bool]$PC113,[bool]$PC114, $localPath,$remotePath, $noSession)
#EXPLAIN パラメータ　ON/OFF　テスト
# Copy-Item -ToSession $session -Recurse -Force -Path $localPath -Destination $remotePath

Write-Output $PC111, $PC112, $PC113, $PC114
