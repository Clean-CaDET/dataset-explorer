# Start-up guide for users

Tested on Windows 10 and 11<br>
Requirements: git ([Git for Windows](https://git-scm.com/download/win)), Docker ([Docker for Windows](https://docs.docker.com/desktop/install/windows-install/))


Steps:
1. open terminal in the desired folder on the computer, run command **git clone https://github.com/Clean-CaDET/dataset-explorer.git**
2. position terminal in **dataset-explorer** folder (run command **cd dataset-explorer**)
3. in file explorer, open the docker-compose file (dataset-explorer/docker-compose.yml) in Notepad
4. in docker-compose file, write down the path on your computer where you will clone the Github projects that you will annotate; check the images below: first image represents the docker-compose file before the path is written, and the second image represents the docker-compose file after the path is written<br><br> <img src="https://github.com/Clean-CaDET/dataset-explorer/assets/15254876/89e0df78-3ed1-41d6-a27a-c31a419eacbd" width="500px"><br><br><img src="https://github.com/Clean-CaDET/dataset-explorer/assets/15254876/fc16637a-2ecc-48f7-a6aa-7a92a31dd11c" width="500px">

5. in terminal, run command **docker-compose up**
<br><br>Last lines of the output should be like this:

```
database_1     | 2024-04-02 11:42:52.832 UTC [1] LOG:  listening on IPv4 address "0.0.0.0", port 5432
database_1     | 2024-04-02 11:42:52.832 UTC [1] LOG:  listening on IPv6 address "::", port 5432
database_1     | 2024-04-02 11:42:52.838 UTC [1] LOG:  listening on Unix socket "/var/run/postgresql/.s.PGSQL.5432"
database_1     | 2024-04-02 11:42:52.846 UTC [63] LOG:  database system was shut down at 2024-04-02 11:42:52 UTC
database_1     | 2024-04-02 11:42:52.852 UTC [1] LOG:  database system is ready to accept connections
```
7. open new terminal, position in dataset-explorer (as in the previous terminal), run command **docker-compose -f docker-compose-migration.yml up**


### Destroy Infrastructure

In order to completely remove the previously created infrastructure, the following commands should be executed:

1. open terminal, run command **docker system prune -a**
2. run command **docker volume prune**
