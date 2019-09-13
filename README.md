# Epinova.PostnordShipping
Epinova's take on a Postnord shipping API

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Epinova.PostnordShipping&metric=alert_status)](https://sonarcloud.io/dashboard?id=Epinova.PostnordShipping)
[![Build status](https://ci.appveyor.com/api/projects/status/f67kodnd0pkbo9dc/branch/master?svg=true)](https://ci.appveyor.com/project/Epinova_AppVeyor_Team/epinova-postnordshipping/branch/master)
![Tests](https://img.shields.io/appveyor/tests/Epinova_AppVeyor_Team/epinova-postnordshipping.svg)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Platform](https://img.shields.io/badge/platform-.NET%20Standard%202.0-blue?style=flat&logo=.net)](https://docs.microsoft.com/en-us/dotnet/standard/net-standard)

## Getting Started

### Configuration

No configuration via config files are needed, but you can override API call timeout via an appSetting. Defaults to 3 seconds.

web.config:
```xml
<configuration>
    <appSettings>
      <!-- Up timeout setting to 5 seconds: -->
      <add key="Postnord.Api.TimeOutInSeconds" value="5" />
    <appSettings>
</configuration>
```

### Add registry to IoC container

If using Structuremap:
```csharp
    container.Configure(
        x =>
        {
            x.Scan(y =>
            {
                y.TheCallingAssembly();
                y.WithDefaultConventions();
            });

            x.AddRegistry<Epinova.PostnordShipping.DeliveryRegistry>();
        });
```

If you cannot use the [structuremap registry](src/DeliveryRegistry.cs) provided with this module,
you can manually set up [DeliveryServiceService](src/DeliveryService.cs) for [IDeliveryService](src/IDeliveryService.cs)
and [CacheHelper](src/CacheHelper.cs) for [ICacheHelper](src/ICacheHelper.cs).

### Inject contract and use service

[Epinova.PostnordShipping.IDeliveryService](src/IDeliveryService.cs) describes the main service.

### Prerequisites

* [Epinova.Infrastructure](https://github.com/Epinova/Epinova.Infrastructure) >= v11.1.0.95.
* [EPiServer.Framework](http://www.episerver.com/web-content-management) >= v11.1 for logging purposes.
* [Automapper](https://github.com/AutoMapper/AutoMapper) >= v9.0 for mapping models.
* [StructureMap](http://structuremap.github.io/) >= v4.7 for registering service contract.

### Installing

The module is published on nuget.org.

```bat
nuget install Epinova.PostnordShipping
```

## Target framework

* .NET Standard 2.0
* Tests target .NET Core 2.1

## Authors

* **Tarjei Olsen** - *Initial work* - [apeneve](https://github.com/apeneve)

See also the list of [contributors](https://github.com/Epinova/Epinova.PostnordShipping/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details

## Further reading

[Postnord API documentation](https://developer.postnord.com/api/docs/general-information)