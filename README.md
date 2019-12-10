Gratify.Grats.Api
=================
_An API for sending, approving and receiving Grats._

Getting started
---------------

### Installation
You'll need the following tools to get started:
* [Git](https://git-scm.com/downloads)
* [.NET Core 3.1 or later](https://dotnet.microsoft.com/download)

In addition you'll need a tool for editing C# code. [VS Code](https://code.visualstudio.com/download) with the [C# extension](https://marketplace.visualstudio.com/items?itemName=ms-vscode.csharp) is a nice option.

If you want to run the deploy scripts locally, you'll also need to install [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli?view=azure-cli-latest).

### Cloning, building and testing
Start by cloning this repo:
```shell
$> git clone https://github.com/gratify-me/grats-api.git
```

Then navigate into the `grats-api/Grats.Api/` folder and start the application with `dotnet run`:
```shell
$ grats-api/Grats.Api> dotnet run
```

Open your favorite browser, and navigate to [localhost:5001/swagger](https://localhost:5001/swagger). This should open [Swagger UI](https://swagger.io/tools/swagger-ui/), where you can try out the API.

![Animation showing how to use Swagger UI](Images/grats-api-swagger.gif)

If you want to run the tests, navigate into the `grats-api/Grats.Api.Test/` folder and run `dotnet test`:
```shell
$ grats-api/Grats.Api.Test> dotnet test
```

Continous Integration
---------------------

Continous Integration (CI) is handles by [GitHub Actions](https://help.github.com/en/actions), and the [built in Azure actions](https://github.com/Azure/actions) handles creation of [resource groups](https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-overview), as well as deployment of [ARM templates](https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-authoring-templates) and applications.

### Generating secrets
In order for the CI workflow to be able to login to Azure, the secret `AZURE_CREDENTIALS` has to be set.

![Screenshot of the GitHub secrets settings page](Images/github-secrets-example.png)

To generate the credentials, you'll have to generate a new service principal as shown below:
```shell
$> az ad sp create-for-rbac \
    --name "GitHubDeployment" \
    --role contributor \
    --scopes /subscriptions/{subscription-id} \
    --sdk-auth
```

The subscription-id can be found by running `az account show`.

You can further scope down the Azure Credentials to the service principal by using the scopes attribute. For example, using `/subscriptions/{subscription-id}/resourceGroups/{resource-group}` would restrict access to a given resource group.

The output of `az ad sp create-for-rbac` should be a json object, containing the login information for the GitHubDeployment service principal. Use this json object as the value for the `AZURE_CREDENTIALS` secret.
```json
{
  "clientId": "<GUID>",
  "clientSecret": "<GUID>",
  "subscriptionId": "<GUID>",
  "tenantId": "<GUID>",
  (...)
}
```

You can view the registered service principal in the Azure portal, by lookin at the [App registrations blade](https://portal.azure.com/#blade/Microsoft_AAD_IAM/ActiveDirectoryMenuBlade/RegisteredApps) under Azure Active Directory.