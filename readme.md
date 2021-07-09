## Installation

This project requires dotnetcore to be installed. The link to install it can be found [here](https://dotnet.microsoft.com/download).

Before the project can be run, nuget packages have to be restored by running the following in {Main Directory}/"Boomtown Notes Api":

```bash
dotnet restore
```

Included in the project is an sqlite database. It should not need any modification to run the project, but migrations are also available to recreate it.

To recreate the database, run:

(cli)
```bash
dotnet ef database update
```

(visual studio)
```bash
Update-Database
```

## Running the application

The application can be run in visual studio or in the terminal:

```bash
dotnet run
```

On running the application, a webpage with the Swagger framework should appear. It has a gui for testing all of our api endpoints.

A separate unit test project also exists using the NUnit framework.