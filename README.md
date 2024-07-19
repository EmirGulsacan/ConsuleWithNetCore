# ConsuleWithNetCore

This project is a .NET 8 Web API application that interacts with Consul for key-value and lock management. It provides endpoints to manage configuration data and distributed locks using Consul.

## Features

- **Key-Value Store Management**:
  - Retrieve and set key-value pairs in Consul.
- **Distributed Lock Management**:
  - Acquire and release distributed locks.
  - Check if a lock exists.
  - Retrieve the value of a lock.
- **Service Management**:
  - List all services registered with Consul.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Consul](https://www.consul.io/downloads) (optional, if not using Docker)

## Consul Setup

### Docker Installation

To run Consul using Docker, follow these steps:

1. **Pull the Consul Docker Image**:

   ```bash
   docker pull consul:latest

2. **Run Consul in a Docker Container**:

   ```bash
   docker run -d --name consul -p 8500:8500 consul:latest
  This command runs Consul in a detached mode and maps port 8500 of the container to port 8500 on your host machine.

3. **Verify Consul is Running:**

  Open your browser and navigate to http://localhost:8500. You should see the Consul UI.
  You can also verify the container is running with:
  
   ```bash
   docker ps
   ```
## Installation

1. **Clone the Repository:**
   
   ```bash
   git clone https://github.com/EmirGulsacan/ConsuleWithNetCore.git
   cd ConsuleWithNetCore
   ```
    
2. **Restore Dependencies:**

   ```bash
    dotnet restore
   ```
3. **Run the Application:**

    ```bash
     dotnet run
   ```
## Configuration
   The application settings are stored in appsettings.json. Configure the Consul address and other settings as needed:
   ```appsettings.json
   {
  "Consul": {
    "Address": "http://localhost:8500",
    "ServiceName": "YourServiceName",
    "ServicePort": 5000
  }
}
```
## API Endpoints
### Key-Value Store
- Get Key-Value: GET /api/kv/{key}
  Retrieve the value for the specified key.

- Set Key-Value: PUT /api/kv/{key}
  Set the value for the specified key. Send the value in the request body.

### Distributed Locks
- Acquire Lock: POST /api/locks/acquire/{key}
  Acquire a lock with the specified key. Returns a lock ID.

- Release Lock: DELETE /api/locks/release/{key}
  Release the lock with the specified key.

- Get Lock Value: GET /api/locks/value/{key}
  Retrieve the value of the lock with the specified key.

- Check Lock Exists: HEAD /api/locks/exists/{key}
  Check if a lock with the specified key exists.

### Service Management
- Get Services: GET /api/services
 List all services registered with Consul.

## Contributing
Contributions are welcome! Please submit issues and pull requests on the GitHub repository.

## License
This project is licensed under the MIT License. See the LICENSE file for details.

   
