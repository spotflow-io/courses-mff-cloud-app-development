# MFF/NSWI152 - Cloud Application Development - Data Intensive Systems, IoT backends & Observability

This repository contains materials for a subcourse of [NSWI152 (Cloud Application Development / Vývoj cloudových aplikací)](https://www.ksi.mff.cuni.cz/teaching/nswi152-web/) course at the Faculty of Mathematics and Physics, Charles University in Prague. The subcourse is focused Data Intensive Systems, IoT backends & Observability.

The goal of the subcourse is to:

* introduce students to the specifics of software engineering for IoT and cloud,
* give an example of how an IoT solution can be designed on various levels of complexity and
* to provide an understanding of how fundamental Azure services can be used to implement IoT solutions.

There will be 5 lessons and one semestral project in total. First four lessons will be used to deliver most of the planned content and the last lesson will be dedicated for the presentation of the semestral projects.

# Course Outline

* **Lesson 1** (17. 04. 2025)
  * Introduction.
  * Specifics of software engineering for IoT and cloud.
  * Overview of relevant Azure resources - SQL, Functions with HTTP trigger, ARM Templates. 
  * Case study (basic solution).
  * For more details visit [lesson-1](./lesson-1/README.md).

* **Lesson 2** (24. 04. 2025)
  * Horizontal vs. vertical scalability.
  * Partitioning & replication. 
  * Types, scalability and economic aspects of various storages.
  * Overview of relevant Azure resources - Storage (Tables, Blobs), App Service.
  * Case study (continued - scalable storage).
  * For more details visit [lesson-2](./lesson-2/README.md).

* **Lesson 3** (15. 05. 2025)
  * Asynchronous communication, messaging.
  * Overview of relevant Azure resources - Event Hubs, Service Bus, Functions with Event Hub trigger.
  * Case study (continued - asynchronous communication)
  * For more details visit [lesson-3](./lesson-3/README.md).

* **Lesson 4** (22. 05. 2025)
  * Observability, OpenTelemetry, Instrumentation and Troubleshooting of Cloud Services.
  * Case study (continued - observability).
  * For more details visit [lesson-4](./lesson-4/README.md).
  * Course summary.
  * Semestral project assignment. For more details visit [semestral-project-assignment.md](./semestral-project-assignment.md).

* **Lesson 5 / Presentation** (???)
  * Presentations of semestral projects.

# Prerequisites 

Students are expected to have a basic understanding of the following topics:

* computer programming,
* asymptotic analysis and big O notation,
* software engineering,
* databases and indexes,
* cloud computing.

During the lessons, some topics will be accommpanied with examples in C# programming language. This examples are available in this repository and following software is required to run them:

* IDE for .NET (Visual Studio, Visual Studio Code, Rider)
* [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
* [Azure Function Core Tools](https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local?)
* [Azure CLI](https://learn.microsoft.com/en-us/cli/azure/install-azure-cli)

# Semestral project

The semestral project will require students to create a system design for a simple IoT solution. Students are expected to deliver the design in the form of text documents and diagrams. Students can work on the assignment individually or in pairs.

Last lesson of the subcourse will be dedicated to the **voluntary** presentation of the semestral projects. Students who decide to present during this last lesson will have at least 15 minutes to present their solution followed-up with a discussion. During the discussion, additional questions asked and possible problems in the solution might be resolved. Authors of presented solutions for which all problems are resolved during the discussion **will receive the subcourse credit right away**.

> [!IMPORTANT]
> The recommended due date for submitting the semestral project is **31. 05. 2025**. The semestral project including all feedback rounds must be completed till the end of exam period, i.e. **30. 06. 2025**. When submitted by the recommended due date we guarantee to provide 1 or 2 feedback rounds if needed.


The documents and diagrams representing the solution should be stored in **private GitHub repository** and pushed to the branch `feature/solution`. After that following actions are required:

* Create a  **Pull Request** from `feature/solution` branch to `main` branch 
* Add `Read` permissions on the repository to the following GitHub users:
  * Tomáš Pajurek ([tomas-pajurek](https://github.com/tomas-pajurek))
  * David Nepožitek ([DavidNepozitek](https://github.com/DavidNepozitek))
* Send email to `mff-nswi152@spotflow.io`.

This process will enable smooth delivery of feedback via comments.

Formal assignment can be found in the [semestral-project-assignment.md](./semestral-project-assignment.md) file.

## Additional recommended resources

* Kleppmann, M. (2017). **Designing Data-Intensive Applications**. O'Reilly Media, Inc. ISBN: 978-1-4493-7332-0 
* [Azure Cloud Architecture Patterns](https://docs.microsoft.com/en-us/azure/architecture/patterns/)
* Azure Storage Documentation
  * [Blobs](https://azure.microsoft.com/en-us/services/storage/blobs/)
  * [Tables - Design](https://docs.microsoft.com/en-us/azure/storage/tables/table-storage-design-guidelines)
  * [Tables - Patterns](https://docs.microsoft.com/en-us/azure/storage/tables/table-storage-design-patterns)
  * [Tables - Query](https://docs.microsoft.com/en-us/azure/storage/tables/table-storage-design-for-query)
  * [Tables - Writes/Updates](https://docs.microsoft.com/en-us/azure/storage/tables/table-storage-design-for-modification)
  * [Tables - Modelling](https://docs.microsoft.com/en-us/azure/storage/tables/table-storage-design-modeling)
* Azure Event Hubs Documentation
  * [Terminology](https://docs.microsoft.com/en-us/azure/event-hubs/event-hubs-features)
  * [Scaling](https://docs.microsoft.com/en-us/azure/event-hubs/event-hubs-scalability)
  * [Availability/Consistency](https://docs.microsoft.com/en-us/azure/event-hubs/event-hubs-availability-and-consistency?tabs=dotnet)
* [OpenTelemetry Docs](https://opentelemetry.io/docs/)
* Charity Majors, Liz Fong-Jones, George Miranda (2022). **Observability Engineering**. O'Reilly Media, Inc. ISBN: 9781492076445
