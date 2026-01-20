# Start-up guide for users

Tested on Windows 10 and 11<br>
Requirements: git ([Git for Windows](https://git-scm.com/download/win)), Docker ([Docker for Windows](https://docs.docker.com/desktop/install/windows-install/))


Steps:
1. open terminal in the desired folder on the computer, run command **git clone https://github.com/Clean-CaDET/dataset-explorer.git**
2. position terminal in **dataset-explorer/DataSetExplorer** folder
3. Build the Docker image:
   ```
   docker build -t dataset-explorer .
   ```
4. Run the Docker container with Git credentials (if needed for cloning private repositories):
   ```
   docker run -p 5000:5000 -e GIT_USER="your-github-username" -e GIT_TOKEN="your-github-token" dataset-explorer
   ```

   **Note:** If you don't need to clone private repositories, you can omit the Git credentials:
   ```
   docker run -p 5000:5000 dataset-explorer
   ```

5. Access the application at http://localhost:5000


### Destroy Infrastructure

In order to completely remove the previously created infrastructure, the following commands should be executed:

1. open terminal, run command **docker system prune -a**
2. run command **docker volume prune**
