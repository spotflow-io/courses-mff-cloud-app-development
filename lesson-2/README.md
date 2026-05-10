# Lesson 2

# Horizontal vs. vertical scaling

|                                      | Horizontal (scale-out)                                                                       | Vertical (scale-up)                                                                    |
| ------------------------------------ | -------------------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------- |
| What is it?                          | “Adding more machines”                                                                       | “Making the machines larger”                                                           |
| How complex is it in the beginning ? | Might be very complicated. The system architecture becomes distributed.                      | Easy. The system architecture remains the same. It can run as a simple single process. |
| How complex is it later?             | Once the system already works on multiple machines, adding more machines is relatively easy. | Provisioning and managing larger machines is more complicated.                         |
| Limits?                              | Virtually unlimited.                                                                         | Very hard physical limits. What is largest RAM/CPU that exist?                         |

# Partitioning & replication

- Techniques that enable systems to be horizontally-scalable
- Can be applied to both storage and compute.
- They are not exclusive. System can be both partitioned and replicated.

## Replication (storage)

![Replication (storage)](./imgs/replication-storage.png)

## Partitioning (storage)

![Partitioning (storage)](./imgs/partitioning-storage.png)

## Replication (compute)

![Replication (compute)](./imgs/replication-compute.png)

## Partitioning (compute)

![Partitioning (compute)](./imgs/partitioning-compute.png)

## Partitioning & Replication combined (storage)

![Partitioning & Replication combined (storage)](./imgs/partitioning-replication-combined-storage.png)

# CAP / PACELC theorem

- **P** = Partition-tolerance.
- **A** = Availability.
- **C** = Consistency.
- **L** = Latency.

**CAP theorem:** In case of network partition, a distributed system cannot be both consistent and available. Each system can be classified as either AP or CP.

${\huge \textbf{P} \Rightarrow \textbf{A} \oplus \textbf{C}}$

**PACELC theorem:** even without network partition, we need to choose between latency (EL system) and consistency (EC system).

${\huge if \ \textbf{P} \ \lbrace \ \textbf{A} \oplus \textbf{C} \ \rbrace  \ else \ \lbrace \ \textbf{L} \oplus \textbf{C} \ \rbrace}$

Note: In this context, the (network) partition refers to failure in communication amongst two or more nodes/machines.

![CAP/PACELC](./imgs/cap-pacelc.png)

# Overview of relevant Azure resources

## Azure Storage

Highly-available, scalable, and durable storage for data.

Azure Docs: https://learn.microsoft.com/en-us/azure/storage/tables/

### Blobs

Object storage optimized for storing massive amounts of unstructured data.

- **Key** = `Blob Name` (filesystem-like path, e.g. `images/2024-05-02/parcels/parcel-1234.jpg`)
- **Value** = `Blob` (binary data + metadata, which are few short key-value pairs)

Azure Docs: https://learn.microsoft.com/en-us/azure/storage/blobs/

### Tables

Key-value storage for semi-structured data.

- **Key** = `Partition Key` + `Row Key` (strings).
- **Value** = `Entity` (set of strongly-typed `Properties`, each with a name and a value).

The `Partition Key` and `Row Key` structure must be carefully designed to optimize the access patterns:

#### Partitioning

- All entities with the same `Partition Key` are stored on the same partition and single node.
  - Transactions are available only within a single partition.
  - Throughput of single partition is limited.
- One node can contain multiple partitions.
- Partitions can be moved between nodes automatically.

#### Queries

Only efficient query patterns are:

- `Point query` (e.g. single entity with `Partition Key` = `X` and `Row Key` = `Y`)
- `Range query` (e.g. all entities with `Partition Key` = `X` and `Row Key` between `A` and `B`)

Both keys can use usual comparisons such as `eq`, `gt`, `lt`. In every case, the comparison is performed lexicographically and never numerically.

Other patterns are also possible, but they result in a full partition scan (if at least `Partition Key` is specified) or table scan.


### Replication

* **LRS**: 3 copies in a single data center.
* **ZRS**: 3 copies in 3 data centers within a single region.
* **GRS**: 3 copies in a primary data center and 3 copies in a secondary data center in a different region (distance > hundreds of kilometers).
  * **RA-GRS**
* **GZRS**: 3 copies in 3 data centers within a single primary region and  3 copies in one secondary data center in a different region.


## Azure App Service

PaaS serving as a general-purpose managed web server.

Azure Docs: https://learn.microsoft.com/en-us/azure/app-service/

# Case study problem statement

The business of your client is thriving!

Now, they have hundreds of sorting facilities and the projection is to store tens of TBs of data. They already hit the storage limits of the general-purpose Azure SQL and the price gets very high.

They want to present transportation data via a web application accessible to their technicians for troubleshooting within the facilities. Initially, the application will provide data from our existing APIs alongside additional device information from the client's "Device Inventory Service". It is expected that the app will include data from more services, both ours and those of our client.

The data access patterns stay the same.

**Our task:**

- Optimize the storage for higher traffic and higher amounts of data stored
- Design the changes needed to add the web application to the system

## The Original Design

![Design](./imgs/diagram_1.png)

## Ideas - discussion

## Resulting design

![Design](./imgs/diagram_2.png)

## Components

- **Event Consumer**, Azure Function with HTTP trigger
- **Stats Reporter**, Azure Function with HTTP trigger
- **Backend for Frontend**, Azure App Service
- **Storage**, Azure Tables

## Storage Design

Azure Table

### Transport Table Entity

We should ideally use one table per day, so we can efficiently delete the data according to our retention policy.

```
PartitionKey: <TransportedDate>_<FacilityId>
RowKey: <ParcelId>

FacilityId: string
ParcelId: string
TransportedAt: DateTime
LocationFrom: string
LocationTo: string
TimeSpentSeconds: int
DeviceId: string
TransportId: string
```

## Implementation

### Deployment

Prerequisites: [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli)

Clone the repository

```
git clone https://github.com/spotflow-io/courses-mff-cloud-app-development.git
```

Navigate to the `lesson-2/arm` directory and see the [ARM template](/lesson-1/arm/resources.azrm.json). It contains

- Function App
- Storage Account with a precreated table
- App Insights
- App Service

First thing is to create a new resource group (or reuse the one from the previous lesson). In the following command, replace the `<resource-group>` with your own name (e.g. "mff-iot-{name}-{surname}").

```pswh
az group create --location 'WestEurope' -n <resource-group>
```

Then, deploy the infrastructure defined in `lesson-2/arm/resources.azrm.json` with the following command. Define the suffix parameter, e.g. your surname. The suffix is used in the names of the resources, so they are unique.

```pwsh
cd lesson-2/arm
az deployment group create `
  --name "deploy-mff-task-components" `
  --resource-group "<resource-group>" `
  --template-file "resources.azrm.json" `
  --parameters suffix="<suffix>"
```

For local development, you need to copy the output values. It should appear in the output as follows:

```json
"outputs": {
      "functionsHostKey": {
        "type": "String",
        "value": "<functionHostKey>"
      },
      "functionsHostUrl": {
        "type": "String",
        "value": "<functionHostUrl>"
      },
      "storageAccountConnectionString": {
        "type": "String",
        "value": "<storageAccountConnectionString>"
      }
    }
```

### Deploy the Functions

Deploy the new versions of the functions. You'll find the name of the function app in the output of the deployment command (it is defined as `mff-iot-fa-<suffix>`).

```
cd lesson-2/sln/AzureFunctions
func azure functionApp publish "<name-of-the-functionapp>" --show-keys
```

### Deploy the Web App

Go to `lesson-2/sln/WebAppBackend`

It is possible to deploy the app directly from your IDE:

- Visual Studio, Rider (with Azure plugin) - Right-click on the project -> Publish

#### Deployment with az cli

Publish the project:

```shell
dotnet publish
```

Create a zip archive from the publish artifacts:

```pwsh
# Powershell
Compress-Archive -Path bin\Release\net8.0\publish\* -DestinationPath deploy.zip
```

Upload the zip file to Azure via az cli:

```shell
az webapp deploy --src-path deploy.zip -n <web-app-name> --resource-group <resource-group-name>
```

## Test

### Send Events to the Consumer Function

Request Method: `POST`

Request Query Parameters: None

Request Body:

```json
{
  "parcelId": "12345",
  "facilityId": "prague",
  "transportedAt": "2022-04-05T15:01:02Z",
  "locationFrom": "in-25",
  "locationTo": "out-35",
  "transportDurationSec": 50,
  "deviceId": "sorter-1654345",
  "transportId": "a1ds3e8r"
}
````

Response Code:

- `201 Created` - Event was successfully stored.
- `400 Bad Request` - Body is not in the correct form.


Powershell

```pwsh
$body = @{
  locationFrom="a";
  locationTo="b";
  transportDurationSec=30;
  parcelId="1";
  transportedAt="2022-04-05T15:01:02Z";
  deviceId="sorter-123";
  facilityId="facility-123";
  transportId="t-4156";
} | ConvertTo-Json

Invoke-WebRequest -Uri <event-consumer-uri> -Method Post -Body $body -ContentType "application/json"
```

cURL

```sh
curl -X POST -H "Content-Type: application/json" \
    -d '{"parcelId": "12345","facilityId": "prague","transportedAt": "2022-04-05T15:01:02Z", "locationFrom": "in-25",  "locationTo": "out-35",  "transportDurationSec": 50,  "deviceId": "sorter-1654345", "transportId": "t-4156"
}' \
    <URI>
```

Response Body: None

### Backend for Frontend

#### Individual Transport

Powershell

```pwsh
Invoke-WebRequest -Uri "<webapp-uri>/transports?date=2022-04-05&facilityId=prague&parcelId=123"
```

cUrl

```sh
curl "<webapp-uri>/transports?date=2022-04-05&facilityId=prague&parcelId=123"
```

Log output of each function can be read via Portal -> Function App `<name-of-the-functionapp>` -> Functions -> Select the Function -> Monitor -> Logs tab

### Local Test

#### Functions

**NOTE:** Only http triggered function can be tested locally.

Add the connection string from arm deployment to `/sln/AzureFunctions/local.settings.json`. They will be accessible to the function as enviromental variables and also automatically loaded as configuration.

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    // Add this:
    "TransportsStorageConnectionsString": "<the-connection-string-from-arm>"
  }
}
```

Then navigate to `<project-root>/sln/AzureFunctions` and [run](https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local?tabs=windows%2Ccsharp%2Cbash#start)

```pwsh
func start
```

#### Backend for Frontend

Create file `/sln/WebAppBackend/appsettings.Development.json` with the function app host key and the url. You can use the url from the output of the deployment. Alternatively, you can use a url of the locally executed functions. It will appear in the output of `func start` (somthing like this: `http://localhost:7071/`).

```json
{
  "StatsReporter": {
    "FunctionHostKey": "<function-host-key>",
    "FunctionHostUrl": "<function-host-url>"
  }
}
```

Run the project in your IDE, or using `dotnet run` in the `/sln/WebAppBackend/` directory.

### Storage Check

Open [Azure Portal](https://portal.azure.com/) in you browser.

Find the Storage Account resource.

Go to the "Storage browser" in the left panel.

Click on Tables and view the "transports" table.
