REM docker-compose build
docker-compose up --abort-on-container-exit | grep initiator
docker cp rbbs_cs_rbss_cs_initiator_1:/app/memResults.log memResults.log
docker cp rbbs_cs_rbss_cs_initiator_1:/app/testResults.log testResults.log