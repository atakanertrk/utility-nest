## Spinning Up Services
Created this project at first in local environment then I practiced it on remote Linux server.  
So I will be sharing steps for both.  

[1) Local (Docker Desktop Windows WSL)](#on-local)  
[2) Remote Server (Docker on Linux Machine via SSH terminal)](#on-server)

## <a id="on-local"></a>1) Local (Docker Desktop Windows WSL)
* First we will spin-up Kafka containers [Quick Start for Confluent Platform](https://docs.confluent.io/platform/current/platform-quickstart.html)  
Download compose file https://github.com/confluentinc/cp-all-in-one/blob/7.6.1-post/cp-all-in-one-kraft/docker-compose.yml  
We have updated kafka compose file to use our custom taskNotifyNetwork, so you can get the modified version at [kafka-docker-compose.yml](https://github.com/atakanertrk/utility-nest/blob/main/TaskNotify/kafka-docker-compose.yml)  

      docker-compose -f kafka-docker-compose.yml -p task-notify-kafka-container up

![image](https://github.com/user-attachments/assets/074b153e-3b5d-4d2e-a8c4-b32ffac9558b)

Open control panel and add two topics with default settings.  
`tasks-to-process`
`tasks-processed` 

![image](https://github.com/user-attachments/assets/062fae7d-5530-4d9e-97e8-1d2458aaa1ad)

* Crate MSSQL Database container

      docker run --name task-notify-mssql-container --network taskNotifyNetwork -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=asd123PWD" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest

Currently, we need to run [SpinUpTemplateData](https://github.com/atakanertrk/utility-nest/tree/main/TaskNotify/TaskNotifyBackend/SpinUpTemplateData) to create default tables, procedures and pre-defined users  

That will print the results.  
`created 'users' table with users as PatricStar (pw: patricstar123) and SpongeBob (pw: spongebob123)`  
`created 'tasks' table with procedures insert_user_task, get_tasks_by_user_name` 
![image](https://github.com/user-attachments/assets/a5ed0106-67b5-402f-8bb6-ccd1990537a8)

* Spin-up Backend APIs  
  we will be using [backend-api-docker-compose.yml](https://github.com/atakanertrk/utility-nest/blob/main/TaskNotify/backend-api-docker-compose.yml)  

      docker-compose -f backend-api-docker-compose.yml -p task-notify-api-list up

![image](https://github.com/user-attachments/assets/265ec4d0-af70-4db1-af34-b14e79f4164e)

* Now we can start our Angular UI  
For local environment, we will start with `ng serve --open`  
But we create image for server with following steps. (we use that image when we spin-up in remote Linux machine...)  
  
      docker build . -t task-notify-ui
      docker run -it --rm -p 4200:4200 --network taskNotifyNetwork --name task-notify-ui task-notify-ui

![image](https://github.com/user-attachments/assets/052b2583-c89e-42fa-bdd3-a767bb777e58)

      
## <a id="on-server"></a>2) Remote Server (Docker on Linux Machine via SSH terminal)
Instructions for setting up the project on a server.



1. SSH into your server:
    ```sh
    ssh your-user@your-server-ip
    ```
