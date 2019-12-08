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
