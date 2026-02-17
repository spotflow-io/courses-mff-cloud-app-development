# Assignment A: Anomaly detection in log dumps from IoT Devices

## Goal

Design an system/solution architecture of a cloud system for collecting logs from IoT devices, performing anomaly detection on them and reporting results to a web application. Actual **implementation is not required**.

* Use Microsoft Azure Cloud.
* Choose suitable Azure services and their integration approach.
* The resulting solution must:
    * Provide 100% correct results even in the face of transient network or infrastructure faults.
    * Be cost efficient.
    * Be scalable enough to provide stable throughput in face of variable load.
* You can use any available Azure service but the assignment is satisfiable only with services discussed during this course.
* Next to available Azure services, it is allowed to run any custom code (e.g. REST API developed in any programming language).
    * For this custom code, proper hosting environment must be chosen and tradeoffs discussed (e.g. Azure Functions, App Service, AKS)
    * For each high-level operation implemented by this custom code (e.g. REST API endpoints), properly document all dependencies (including transitive) on other custom components and Azure services.
* If there are more possible approaches for a given problem, always opt for the simpler one.
* Discuss your solution's limits and trade-offs.
* The specific configuration options of individual Azure services does not have to be discussed (but details are of course very welcomed).
* Use any drawing tool and text format you are comfortable with to capture the resulting architecture.
    * We recommend https://excalidraw.com/ or https://diagrams.net and Markdown files.
* For all diagrams, make sure to also provide textual descriptions for data flows, dependencies, data schemas, assumptions, etc.

## Scenario

There is a number of devices deployed across West Europe region. The devices are complex mechanical machines consisting of many components where each component is generating log streams at various rates. Device's embedded software periodically collects the logs from all components, dumps them into a single file and compresses it into ZIP archive and sends it to the cloud.

In the cloud, the log dumps from each device must be stored in the blob storage and also must be passed through anomaly detection algorithm in near-realtime, streaming manner.
 
The anomaly detection algorithm searches for anomalies within the data. When anomaly is found, it must be published into an service support application that provides the anomalies to technicians (via mobile app with good internet connection). Each technician must be able to efficiently obtain a list of all unresolved anomalies for all devices that she or he maintains. The service support application also offers functionality to mark individual anomalies as resolved by technicians. The mapping of devices to technicians is provided by a 3rd party REST API that is implementing following two HTTP GET endpoints:


### List devices for technician

Signature: `GET /api/v1/technicians/{technicianId:GUID}/devices`

Example response:

Content-Type: application/x-ndjson

```json
{"deviceId": "device1", "location": "c05a3ec5-7460-4f84-ba80-55ce62bf0e95"}
{"deviceId": "device2", "location": "c05a3ec5-7460-4f84-ba80-55ce62bf0e95"}
{"deviceId": "device3", "location": "eb7a4b73-1451-4575-95a6-381b4cf90942"}
{"deviceId": "device4", "location": "c05a3ec5-7460-4f84-ba80-55ce62bf0e95"}
{"deviceId": "device5", "location": "e88ace7d-d2c2-4a82-b966-8009ccd7afa5"}
{"deviceId": "device6", "location": "eb7a4b73-1451-4575-95a6-381b4cf90942"}
```

### Get device info

Signature: `GET /api/v1/devices/{deviceId:string}`

Example response:

Content-Type: application/x-ndjson

```json
{
    "deviceId": "device1",
    "responsibleTechnicianId": "0c8af4d3-3d3b-4f43-b188-47910f3f00f0",
    "location": "c05a3ec5-7460-4f84-ba80-55ce62bf0e95"
}
```

This 3rd party REST API has problems with performance and availability so some caching mechanisms should be considered. No other endpoints are available and won't be implemented.

There are three teams working on the solution: Cloud Platform Team, Machine Learning Engineering Team and Service Support Application Team. You need to design the architecture of the overall system spanning all three teams.

The Cloud Platform Team is well versed in C#, Golang and most Azure services. They should develop the core functionality for collecting and storing the log dumps as well as infrastructure for other teams to run their algorithms.

The anomaly detection algorithm is developed by the Machine Learning Engineering Team using Python and Scikit-learn library. The anomaly detection algorithm is stateful and requires that all log dumps from a single device are processed in-order. It is not required to deal with the machine learning aspects of the algorithm. However, you need to provide guidance how to integrate it into the whole system. How the algorithm will be triggered? Where it will be running? How the state will be retrieved and persisted? Will the state be cached in memory?  

The Service Support Application Team has good skills in enterprise integration and application development but they lack knowledge of cloud and big data streaming architectures. Provide them with suitable data sources/APIs and guidance on how to run their app.

### Observability

Choose 3 metric types (counter, upDownCounter, async counter, async upDownCounter, gauge, histogram) and define metrics that would enhance the observability of the system. The definition should include Name, Kind, Description, and Unit (if applicable).

Choose one endpoint of the web application and describe the trace of a request. Draw a simple flame graph. Provide a sample JSON representation of each span, containing all required information to build the trace. Include a few attributes you find valuable to capture (you don't need to include every attribute that is recommended by OpenTelemetry).

## Constraints

* Maximum number of devices expected: 100 000.
* Maximum number of locations: 1000 (devices are evenly distributed across locations).
* Log collection period: 6 hours during night, 30 minutes during day
* Size of single log dump: 100 KiB - 1 GiB
* CPU time required per 1 MiB of logs: 0.5 seconds

## Bonus 1

Devices and technicians are distributed across 6 regions around the world. How the architecture will change?

## Bonus 2

Optimize the costs of storage of log dumps in your system under following assumptions:

* The data needs to be available for first 30 days after delivery to cloud for immediate analysis by engineers and retraining of machine learning algorithms.
* After 30 days, the data are expected to not be used ever again. However, they should must be kept for unexpected future use cases.

What storage configuration would you choose?

## Bonus 3

The Service Support App Team would like to add new functionality to their application: Listing all devices in single location. Can you provide them with such data so they do not need to integrate inefficiently with the third party REST API?

