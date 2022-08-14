REM docker build . -t rbss_cs --force-rm -f ./RBSS_CS/Dockerfile
docker rm -f rbss
docker create --name rbss -e RBSS_CONNECTION=http://25.80.115.11:7042 -e RBSS_SELF=http://25.76.135.166:7042 -p 7042:7042 rbss_cs
REM scp -r ./RBSS_CS/ algov3@25.80.115.11:~/Documents/RBBS_CS/RBSS_CS/
ssh algov3@25.80.115.11 'sudo docker rm -f rbss'
ssh algov3@25.80.115.11 "cd ~/Documents/RBBS_CS ; sudo docker build . -t rbss_cs --force-rm -f ./RBSS_CS/Dockerfile"
ssh algov3@25.80.115.11 'sudo docker create --name rbss -e RBSS_SELF=http://25.80.115.11:7042 -p 7042:7042 rbss_cs'

pwsh %cd%\replaceBI.ps1 2 1
docker cp settings-testing.yml rbss:/app/settings.yml
scp settings-testing2.yml algov3@25.80.115.11:~/settings-testing2.yml
ssh algov3@25.80.115.11 'sudo docker cp settings-testing2.yml rbss:/app/settings.yml'
docker stop rbss
ssh algov3@25.80.115.11 'sudo docker stop rbss'
ssh algov3@25.80.115.11 'sudo docker start rbss'
sleep 10
docker start -i rbss
ssh algov3@25.80.115.11 'sudo docker stop rbss'
docker cp rbss:/app/testResults.log testResultsB2I1.log


pwsh %cd%\replaceBI.ps1 2 100
docker cp settings-testing.yml rbss:/app/settings.yml
scp settings-testing2.yml algov3@25.80.115.11:~/settings-testing2.yml
ssh algov3@25.80.115.11 'sudo docker cp settings-testing2.yml rbss:/app/settings.yml'
docker stop rbss
ssh algov3@25.80.115.11 'sudo docker stop rbss'
ssh algov3@25.80.115.11 'sudo docker start rbss'
sleep 10
docker start -i rbss
ssh algov3@25.80.115.11 'sudo docker stop rbss'
docker cp rbss:/app/testResults.log testResultsB2I100.log

pwsh %cd%\replaceBI.ps1 100 1
docker cp settings-testing.yml rbss:/app/settings.yml
scp settings-testing2.yml algov3@25.80.115.11:~/settings-testing2.yml
ssh algov3@25.80.115.11 'sudo docker cp settings-testing2.yml rbss:/app/settings.yml'
docker stop rbss
ssh algov3@25.80.115.11 'sudo docker stop rbss'
ssh algov3@25.80.115.11 'sudo docker start rbss'
sleep 10
docker start -i rbss
ssh algov3@25.80.115.11 'sudo docker stop rbss'
docker cp rbss:/app/testResults.log testResultsB100I1.log



pwsh %cd%\replaceBI.ps1 2 2147483647
docker cp settings-testing.yml rbss:/app/settings.yml
scp settings-testing2.yml algov3@25.80.115.11:~/settings-testing2.yml
ssh algov3@25.80.115.11 'sudo docker cp settings-testing2.yml rbss:/app/settings.yml'
docker stop rbss
ssh algov3@25.80.115.11 'sudo docker stop rbss'
ssh algov3@25.80.115.11 'sudo docker start rbss'
sleep 10
docker start -i rbss
ssh algov3@25.80.115.11 'sudo docker stop rbss'
docker cp rbss:/app/testResults.log testResultsTrivial.log

pwsh %cd%\replaceBI.ps1 3 2
docker cp settings-testing.yml rbss:/app/settings.yml
scp settings-testing2.yml algov3@25.80.115.11:~/settings-testing2.yml
ssh algov3@25.80.115.11 'sudo docker cp settings-testing2.yml rbss:/app/settings.yml'
docker stop rbss
ssh algov3@25.80.115.11 'sudo docker stop rbss'
ssh algov3@25.80.115.11 'sudo docker start rbss'
sleep 10
docker start -i rbss
ssh algov3@25.80.115.11 'sudo docker stop rbss'
docker cp rbss:/app/testResults.log testResultsB3I2.log

pwsh %cd%\replaceBI.ps1 6 4
docker cp settings-testing.yml rbss:/app/settings.yml
scp settings-testing2.yml algov3@25.80.115.11:~/settings-testing2.yml
ssh algov3@25.80.115.11 'sudo docker cp settings-testing2.yml rbss:/app/settings.yml'
docker stop rbss
ssh algov3@25.80.115.11 'sudo docker stop rbss'
ssh algov3@25.80.115.11 'sudo docker start rbss'
sleep 10
docker start -i rbss
ssh algov3@25.80.115.11 'sudo docker stop rbss'
docker cp rbss:/app/testResults.log testResultsB6I4.log