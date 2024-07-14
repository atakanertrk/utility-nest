# Project Title

## Contents
* [1) On Local](#on-local)
* [2) On Server](#on-server)

## <a id="on-local"></a>1) On Local
Instructions for setting up the project on a local environment.

1. Clone the repository:
    ```sh
    git clone https://github.com/your-username/your-repo.git
    ```

2. Navigate to the project directory:
    ```sh
    cd your-repo
    ```

3. Install dependencies:
    ```sh
    npm install
    ```

4. Start the development server:
    ```sh
    npm start
    ```

## <a id="on-server"></a>2) On Server
Instructions for setting up the project on a server.

1. SSH into your server:
    ```sh
    ssh your-user@your-server-ip
    ```

2. Clone the repository:
    ```sh
    git clone https://github.com/your-username/your-repo.git
    ```

3. Navigate to the project directory:
    ```sh
    cd your-repo
    ```

4. Install dependencies:
    ```sh
    npm install
    ```

5. Start the server:
    ```sh
    npm start
    ```

6. (Optional) Set up a process manager like PM2 to keep your server running:
    ```sh
    npm install pm2 -g
    pm2 start npm -- start
    pm2 save
    pm2 startup
    ```
