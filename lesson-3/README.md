# Lesson 3

# Asynchronous communication & messaging

| Contact                   | Synchronous communication             | Asynchronous communication             |
| ------------------------- | ------------------------------------- | -------------------------------------- |
| Human-to-human (physical) | Speaking to other person directly.    | Sending an letter.                     |
| Human-to-human (digital)  | Making a phone call.                  | Sending an email, sms or document.     |
| Cooperation               | Multiple people sitting in a meeting. | Create and review Merge/Pull requests. |
| Machine-to-machine        | Method call, HTTP request.            | Messaging via queues.                  |

## Asynchronous communication - pros and cons

| Pros                                                         | Cons                                              |
| ------------------------------------------------------------ | ------------------------------------------------- |
| Improves scalability.                                        | More difficult to implement correctly.            |
| Improves reliability.                                        | Requires external infrastructure (in most cases). |
| Simple load balancing.                                       | Latency might suffer.                             |
| Might help reducing architectural complexity via decoupling. |                                                   |

## Messaging patterns

### Point-to-Point

![Publish/Subscribe](./imgs/point-to-point.png)

### Publish/Subscribe

![Publish/Subscribe](./imgs/publish-subscribe.png)

### Competing Consumers

![Competing Consumers](./imgs/competing-consumers.png)

### Sequential Convoy

![Sequential Convoy](./imgs/sequential-convoy.png)

https://learn.microsoft.com/en-us/azure/architecture/patterns/sequential-convoy

## Message queues

### Traditional message brokers

![Traditional message brokers](./imgs/traditional-message-brokers.png)

### Log-based queues

![RLog-based queues](./imgs/log-based-queues.png)

#### Choosing number of partitions

| Incentives for _more_ partitions | Incentives for _less_ partitions                |
| -------------------------------- | ----------------------------------------------- |
| More parallelism.                | Larger batch size.                              |
| More granular isolation.         | Less overhead (# of processors, network calls). |

# Overview of relevant Azure resources

## Azure Service Bus

Traditional message broker. It supports:

- **Queues** (point-to-point, competing consumers)
- **Topics** (publish/subscribe)
  - **Subscriptions** (competing consumers)
- Time-to-live (TTL) configurable per message.
- Filtering.
- Auto-Forwarding.
- Dead-lettering.
- Sessions.
- Transactions.
- Auto-delete.

Azure Docs: https://learn.microsoft.com/en-us/azure/service-bus-messaging/

## Azure Event Hub

Log-based queue:

- Does not provide built-in support for checkpointing (checkpoints are typically stored in Blobs or Tables).
- Time-to-live is configurable on the level of the Event Hub, not individual events.
- No routing features similar to Service Bus.
- Only batch inserts are transactional.

Azure Docs: https://learn.microsoft.com/en-us/azure/event-hubs/

## Azure Functions with Event Hub trigger

Functions are invoked with a batch of messages from a single Event Hub partition. At most one batch from a single partition is processed at a time.

Target-based scaling: https://learn.microsoft.com/en-us/azure/azure-functions/functions-target-based-scaling?tabs=v5%2Ccsharp

![Target-based scaling](./imgs/target-based-scaling-formula.png)

Azure Docs: https://learn.microsoft.com/en-us/azure/azure-functions/functions-bindings-event-hubs

# Case study problem statement

The client wants to detect problems with sorting robots.
They want us to deploy a real-time anomaly detection algorithm on the data sent from the devices.

The client is not happy that the endpoint for daily statistics takes so long.
We agreed to implement a caching mechanism to ensure that only the initial query is slow.

**Our task:**

- Modify the architecture so that the anomaly detection algorithm can process the data alongside our event consumer (the one which stores the data).
- Implement a caching mechanism for the daily statistics query.

## The Original Designs

![Design](./imgs/diagram_1.png)

![Design](./imgs/diagram_2.png)

## Ideas - discussion

## The Final Architecture

![Design](./imgs/diagram_3.png)

## Components

- **Event Consumer**, Azure Function with HTTP trigger
- **Stats Reporter**, Azure Function with HTTP trigger
- **Backend for Frontend**, Azure App Service
- **Storage**, Azure Tables
- **Stats Cache**, Azure Tables

## Event Hub

- **Partition Key**: DeviceId
  - Ensures in-order processing of events per one device
  - Ensures that the data from one device will be processed by the same instance of the Anomaly Detection algorithm
- **Consumer Groups**
  - One for the Event Consumer which stores the data
  - One for the Anomaly Detection algorithm

## Implementation

### Deployment

Prerequisites: [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli)

Clone the repository

```
git clone https://github.com/datamole-ai/mff-cloud-app-development.git
```

Navigate to the `lesson-3/arm` directory and see the [ARM template](/lesson-3/arm/resources.azrm.json). It contains

- Function App
- Storage Account with a precreated table
- App Insights
- App Service

First thing is to create a new resource group (or reuse the one from the previous lesson). In the following command, replace the `<resource-group>` with your own name (e.g. "mff-iot-{name}-{surname}").

```pswh
az group create --location 'WestEurope' -n <resource-group>
```

Then, deploy the infrastructure defined in `lesson-3/arm/resources.azrm.json` with the following command. Define the suffix parameter, e.g. your surname. The suffix is used in the names of the resources, so they are unique.

```pwsh
cd lesson-3/arm
az deployment group create `
  --name "deploy-mff-task-components" `
  --resource-group "<resource-group>" `
  --template-file "resources.azrm.json" `
  --parameters suffix="<suffix>"
```

You'll need to store the connection string to the Event Hub, so you can send some events. It should appear in the output as follows:

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
      "senderEventHubConnectionString": {
        "type": "String",
        "value": "<eventhubSenderConnectionString>"
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
cd lesson-3/sln/AzureFunctions
func azure functionApp publish "<name-of-the-functionapp>" --show-keys
```

### Deploy the Web App

Go to `lesson-3/sln/WebAppBackend`

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

The `EventsGenerator` projects generates and sends events for the past few days.

To generate the events run the project:

```shell
cd lesson-3/sln/EventsGenerator

dotnet run -- "<event-hub-sender-connection-string>"
```

### Backend for Frontend

#### Individual Transport

Powershell

```pwsh
Invoke-WebRequest -Uri "<webapp-uri>/transports?date=2024-04-05&facilityId=prague&parcelId=123"
```

cUrl

```sh
curl "<webapp-uri>/transports?date=2024-04-05&facilityId=prague&parcelId=123"
```

#### Daily Statistics

Powershell

```pwsh
Invoke-WebRequest -Uri "<webapp-uri>/daily-statistics?date=2024-04-05"
```

cUrl

```sh
curl "<webapp-uri>/daily-statistics?date=2024-04-05"
```

### Storage Check

Open [Azure Portal](https://portal.azure.com/) in you browser.

Find the Storage Account resource.

Go to the "Storage browser" in the left panel.

Click on Tables and view the "transports" table.
