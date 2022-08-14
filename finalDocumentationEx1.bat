docker-compose build
pwsh %cd%\replaceBI.ps1 2 1
docker cp settings-testing.yml rbbs_cs_rbss_cs_initiator_1:/app/settings-testing.yml
docker cp settings-testing2.yml rbbs_cs_rbss_cs_participant_1:/app/settings-testing2.yml
docker-compose up --abort-on-container-exit | grep initiator
docker cp rbbs_cs_rbss_cs_initiator_1:/app/testResults.log testResultsB2I1.log

pwsh %cd%\replaceBI.ps1 3 2
docker cp settings-testing.yml rbbs_cs_rbss_cs_initiator_1:/app/settings-testing.yml
docker cp settings-testing2.yml rbbs_cs_rbss_cs_participant_1:/app/settings-testing2.yml
docker-compose up --abort-on-container-exit | grep initiator
docker cp rbbs_cs_rbss_cs_initiator_1:/app/testResults.log testResultsB3I2.log

pwsh %cd%\replaceBI.ps1 6 4
docker cp settings-testing.yml rbbs_cs_rbss_cs_initiator_1:/app/settings-testing.yml
docker cp settings-testing2.yml rbbs_cs_rbss_cs_participant_1:/app/settings-testing2.yml
docker-compose up --abort-on-container-exit | grep initiator
docker cp rbbs_cs_rbss_cs_initiator_1:/app/testResults.log testResultsB6I4.log

pwsh %cd%\replaceBI.ps1 100 1
docker cp settings-testing.yml rbbs_cs_rbss_cs_initiator_1:/app/settings-testing.yml
docker cp settings-testing2.yml rbbs_cs_rbss_cs_participant_1:/app/settings-testing2.yml
docker-compose up --abort-on-container-exit | grep initiator
docker cp rbbs_cs_rbss_cs_initiator_1:/app/testResults.log testResultsB100I1.log

pwsh %cd%\replaceBI.ps1 2 100
docker cp settings-testing.yml rbbs_cs_rbss_cs_initiator_1:/app/settings-testing.yml
docker cp settings-testing2.yml rbbs_cs_rbss_cs_participant_1:/app/settings-testing2.yml
docker-compose up --abort-on-container-exit | grep initiator
docker cp rbbs_cs_rbss_cs_initiator_1:/app/testResults.log testResultsB2I100.log

pwsh %cd%\replaceBI.ps1 2 2147483647
docker cp settings-testing.yml rbbs_cs_rbss_cs_initiator_1:/app/settings-testing.yml
docker cp settings-testing2.yml rbbs_cs_rbss_cs_participant_1:/app/settings-testing2.yml
docker-compose up --abort-on-container-exit | grep initiator
docker cp rbbs_cs_rbss_cs_initiator_1:/app/testResults.log testResultsTrivial.log