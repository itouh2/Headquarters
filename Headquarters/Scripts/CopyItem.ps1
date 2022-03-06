param($session,$localPath,$remotePath)
#EXPLAIN コピーする
#EXPLAIN hello!?
#EXPLAIN 以上
Copy-Item -ToSession $session -Recurse -Force -Path $localPath -Destination $remotePath