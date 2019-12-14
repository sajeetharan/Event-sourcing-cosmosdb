# Event Sourcing on Azure

This is a sample project showing how to use various Azure PaaS services as event store for CQRS & event sourced applications.
Two different event store implementations are illustrated, each on its own branch.

### Running the sample

1. Perform a `git checkout` of the `cosmos-db` branch.
2. Using the `Deployment/azuredeploy.json` ARM template, create the required resources on your Azure account.
   **Watch out, as some services may cost some money.**
3. Rename `AzureTableEventSourcingTest.EventConsumers.AzureFunctions/local.settings.sample.json` to 
   `AzureTableEventSourcingTest.EventConsumers.AzureFunctions/local.settings.json` and set the following keys:
    1. `AzureWebJobsStorage` & `AzureWebJobsDashboard`: the connection string of the Azure Storage account.
    2. `CosmosDb:ConnectionString` : the connection string of the Azure CosmosDb account.
4. Rename `AzureTableEventSourcingTest.WebApi/appsettings.sample.json` to `AzureTableEventSourcingTest.WebApi/appsettings.json`
   and set the following keys:
    1. `Azure` / `CosmosDb` / `AccountEndpoint`: the URI of the Azure CosmosDb account.
    2. `Azure` / `CosmosDb` / `AccountKey`: the secret key of the Azure CosmosDb account.
5. Launch the application (make sure to run both `AzureTableEventSourcingTest.WebApi` and 
   `AzureTableEventSourcingTest.EventConsumers.AzureFunctions`).
