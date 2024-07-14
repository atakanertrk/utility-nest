## Spinning Up Services
I created this project first in local environment then practiced it on remote Linux server.  
So I will be sharing both steps.  

[Local (Docker Desktop Windows WSL)](#on-local)

[Remote Server (Docker on Linux Machine via SSH terminal)](#on-server)

## <a id="on-local"></a>1) On Local (Docker Desktop Windows WSL)
First we will spinup Kafka containers [Quick Start for Confluent Platform](https://docs.confluent.io/platform/current/platform-quickstart.html)  
Download compose file https://github.com/confluentinc/cp-all-in-one/blob/7.6.1-post/cp-all-in-one-kraft/docker-compose.yml  
We have updated that compose file to use our custom network, so you can get the modified version at [kafka-docker-compose.yml](https://github.com/atakanertrk/utility-nest/blob/main/TaskNotify/kafka-docker-compose.yml)

    docker-compose -f kafka-docker-compose.yml -p task-notify-kafka-container up
    
Open control panel and add two default topics.  
`tasks-to-process`
`tasks-processed` 
  
     


## <a id="on-server"></a>2) On Server (Docker on Linux Machine via SSH terminal)
Instructions for setting up the project on a server.

1. SSH into your server:
    ```sh
    ssh your-user@your-server-ip
    ```
