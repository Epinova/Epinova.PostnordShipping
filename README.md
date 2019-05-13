# Epinova.PostnordShipping
Epinova's take on a Postnord shipping API

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Epinova.PostnordShipping&metric=alert_status)](https://sonarcloud.io/dashboard?id=Epinova.PostnordShipping)
[![Build status](https://ci.appveyor.com/api/projects/status/f67kodnd0pkbo9dc/branch/master?svg=true)](https://ci.appveyor.com/project/Epinova_AppVeyor_Team/epinova-postnordshipping/branch/master)
![Tests](https://img.shields.io/appveyor/tests/Epinova_AppVeyor_Team/epinova-postnordshipping.svg)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

## Usage
### Add registry to Structuremap

```
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

### Inject contract and use service

Epinova.PostnordShipping.IDeliveryService describes the service. 