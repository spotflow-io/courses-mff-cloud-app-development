# Assignment B: Fraud detection in transaction batches from e-commerce merchants

## Goal

Design a system/solution architecture of a cloud system for collecting transaction batches from e-commerce merchants, performing fraud detection on them and reporting results to a fraud operations application. Actual **implementation is not required**.

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

There is a number of e-commerce merchants operating across West Europe region. Each merchant processes orders coming from multiple sales channels and payment methods. Merchant's backend periodically collects newly created orders, payments, refunds, chargebacks and account updates, dumps them into a single file, compresses it into ZIP archive and sends it to the cloud.

In the cloud, the transaction batches from each merchant must be stored in blob storage and also must be passed through fraud detection algorithm in near-realtime, streaming manner.

The fraud detection algorithm searches for suspicious behavior within the data. When suspicious activity is found, it must be published into a fraud operations application that provides the alerts to investigators (via web or mobile app with good internet connection). Each investigator must be able to efficiently obtain a list of all unresolved fraud alerts for all merchants that she or he oversees. The fraud operations application also offers functionality to mark individual alerts as resolved by investigators. The mapping of merchants to investigators is provided by a 3rd party REST API that is implementing following two HTTP GET endpoints:

### List merchants for investigator

Signature: `GET /api/v1/investigators/{investigatorId:GUID}/merchants`

Example response:

Content-Type: application/x-ndjson

```json
{"merchantId": "merchant1", "market": "de-de"}
{"merchantId": "merchant2", "market": "de-de"}
{"merchantId": "merchant3", "market": "fr-fr"}
{"merchantId": "merchant4", "market": "de-de"}
{"merchantId": "merchant5", "market": "nl-nl"}
{"merchantId": "merchant6", "market": "fr-fr"}
```

### Get merchant info

Signature: `GET /api/v1/merchants/{merchantId:string}`

Example response:

Content-Type: application/x-ndjson

```json
{
    "merchantId": "merchant1",
    "responsibleInvestigatorId": "0c8af4d3-3d3b-4f43-b188-47910f3f00f0",
    "market": "de-de"
}
```

This 3rd party REST API has problems with performance and availability so some caching mechanisms should be considered. No other endpoints are available and won't be implemented.

There are three teams working on the solution: Cloud Platform Team, Machine Learning Engineering Team and Fraud Operations Application Team. You need to design the architecture of the overall system spanning all three teams.

The Cloud Platform Team is well versed in C#, Golang and most Azure services. They should develop the core functionality for collecting and storing the transaction batches as well as infrastructure for other teams to run their algorithms.

The fraud detection algorithm is developed by the Machine Learning Engineering Team using Python and Scikit-learn library. The fraud detection algorithm is stateful and requires that all transaction batches from a single merchant are processed in-order. It is not required to deal with the machine learning aspects of the algorithm. However, you need to provide guidance how to integrate it into the whole system. How the algorithm will be triggered? Where it will be running? How the state will be retrieved and persisted? Will the state be cached in memory?

The Fraud Operations Application Team has good skills in enterprise integration and application development but they lack knowledge of cloud and big data streaming architectures. Provide them with suitable data sources/APIs and guidance on how to run their app.

### Observability

Choose 3 metric types (counter, upDownCounter, async counter, async upDownCounter, gauge, histogram) and define metrics that would enhance the observability of the system. The definition should include Name, Kind, Description, and Unit (if applicable).

Choose one endpoint of the web application and describe the trace of a request. Draw a simple flame graph. Provide a sample JSON representation of each span, containing all required information to build the trace. Include a few attributes you find valuable to capture (you don't need to include every attribute that is recommended by OpenTelemetry).

## Constraints

* Maximum number of merchants expected: 100 000.
* Maximum number of markets: 1000 (merchants are evenly distributed across markets).
* Batch export period: 6 hours during night, 30 minutes during day
* Size of single transaction batch: 100 KiB - 1 GiB
* CPU time required per 1 MiB of transaction data: 0.5 seconds

## Bonus 1

Merchants and investigators are distributed across 6 regions around the world. How the architecture will change?

## Bonus 2

Optimize the costs of storage of transaction batches in your system under following assumptions:

* The data needs to be available for first 30 days after delivery to cloud for immediate analysis by fraud investigators and retraining of machine learning algorithms.
* After 30 days, the data are expected to not be used ever again. However, they should must be kept for unexpected future use cases.

What storage configuration would you choose?

## Bonus 3

The Fraud Operations Application Team would like to add new functionality to their application: Listing all merchants in single market. Can you provide them with such data so they do not need to integrate inefficiently with the third party REST API?
