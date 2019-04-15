# Epinova.PostnordShipping
Epinova's take on a Postnord shipping API

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