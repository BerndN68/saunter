## Hacktoberfest
🎃 I'm accepting [Hacktoberfest](https://hacktoberfest.digitalocean.com/) contributions - Please check the Issues, or create a new Issue before raising a Pull Request 🎃

---

# Saunter

![CI](https://github.com/tehmantra/saunter/workflows/CI/badge.svg)
[![NuGet Badge](https://buildstats.info/nuget/saunter?includePreReleases=true)](https://www.nuget.org/packages/Saunter/)


Saunter is an [AsyncAPI](https://github.com/asyncapi/asyncapi) documentation generator for dotnet.


## Getting Started

See [examples/StreetlightsAPI](examples/StreetlightsAPI).


1. Install the Saunter package

    ```
    dotnet add package Saunter
    ```

2. In the `ConfigureServices` method of `Startup.cs`, configure Saunter.

    ```csharp
    // Add Saunter to the application services. 
    services.AddAsyncApiSchemaGeneration(options =>
    {
        // Specify example type(s) from assemblies to scan.
        options.AssemblyMarkerTypes = new[] {typeof(StreetlightMessageBus)};

        // Build as much (or as little) of the AsyncApi document as you like.
        // Saunter will generate Channels, Operations, Messages, etc, but you
        // may want to specify Info here.
        options.AsyncApi = new AsyncApiDocument
        {
            Info = new Info("Streetlights API", "1.0.0")
            {
                Description = "The Smartylighting Streetlights API allows you\nto remotely manage the city lights.",
                License = new License("Apache 2.0")
                {
                    Url = "https://www.apache.org/licenses/LICENSE-2.0"
                }
            },
            Servers =
            {
                { "mosquitto", new Server("test.mosquitto.org", "mqtt") }
            }
        };
    });
    ```

3. Add attributes to your classes which publish or subscribe to messages.

    ```csharp
    [AsyncApi] // Tells Saunter to scan this class.
    public class StreetlightMessageBus : IStreetlightMessageBus
    {
        [Channel("publish/light/measured")] // Creates a Channel
        [PublishOperation(typeof(LightMeasuredEvent), Summary = "Inform about environmental lighting conditions for a particular streetlight.")] // A simple Publish operation.
        public void PublishLightMeasuredEvent(Streetlight streetlight, int lumens) {}
    ```

4. Add saunter middleware to host the AsyncApi json document. In the `Configure` method of `Startup.cs`:

    ```csharp
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapAsyncApiDocuments();
    });
    ```

5. Use the published AsyncApi document:

    ```jsonc
    // HTTP GET /asyncapi/asyncapi.json
    {
        // Properties from Startup.cs
        "asyncapi": "2.0.0",
        "info": {
            "title": "Streetlights API",
            "version": "1.0.0",
            "description": "The Smartylighting Streetlights API allows you\nto remotely manage the city lights.",
           // ...
        },
        // Properties generated from Attributes
        "channels": {
            "light/measured": {
            "publish": {
                "operationId": "PublishLightMeasuredEvent",
                "summary": "Inform about environmental lighting conditions for a particular streetlight.",
            //...
    }
    ```

## Bindings
Bindings are used to describe protocol specific information. Currently the only implementation is via filters

Http bindings:
 
To use Http Operation filter add in the `ConfigureServices` method of the `Startup.cs`

 ``` 
    services.AddAsyncApiSchemaGeneration(options =>
    {
        options.OperationFilters.Add(new HttpOperationFilter());
    });
});
```

and create a new class into your app called `HttpOperationFilter` 

```
public class HttpOperationFilter : OperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            operation.Bindings = new OperationBindings
            {
                Http = new HttpOperationBinding
                {
                    Type = "request",
                    Method = "GET",
                    Query = new HttpOperationBindingQuery
                    {
                        Type = "object",
                        Required = new string[] { "companyId" },
                        Properties = new
                        {
                            CompanyId = new
                            {
                                Type = "number",
                                Minimum = 1,
                                Desciption = "The Id of the company."
                            }
                        },
                        AdditionalProperties = false
                    },
                    BindingVersion = "0.1.0"
                }
            };
        }
    }
```
In the `HttpOperationFilter` you can customise the binding information. 

Once these steps are complete, when you generate your documentation any class with a `[PublishOperation()]` or `[SubscribeOperation()]` attributes will have the http binding information added to the documentation.


Kafka bindings:
 
To use Kafka Operation filter add in the `ConfigureServices` method of the `Startup.cs`

 ``` 
    services.AddAsyncApiSchemaGeneration(options =>
    {
        options.OperationFilters.Add(new KafkaOperationFilter());
    });
});
```

and create a new class into your app called `KafkaOperationFilter` 

```
public class KafkaOperationFilter : OperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            operation.Bindings = new OperationBindings
            {
                Kafka = new KafkaOperationBinding
                {          
                    GroupId = new KafkaOperationBindingGroupId
                    {
                        Type = "string",
                        Enum = new string[] { "myGroupId" }     
                    },
                    ClientId = new KafkaOperationBindingClientId
                    {
                        Type = "string",
                        Enum = new string[] { "myClientId" }
                    },
                    BindingVersion = "0.1.0"
                }
            };
        }
    }
```
In the `KafkaOperationFilter` you can customise the binding information. 

Once these steps are complete, when you generate your documentation any class with a `[PublishOperation()]` or `[SubscribeOperation()]` attributes will have the kafka binding information added to the documentation.


## Changelog

This project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

See [CHANGELOG.md](./CHANGELOG.md) for all changes.


## Contributing

See our [contributing guide](./CONTRIBUTING.md).

Feel free to get involved in the project by opening issues, or submitting pull requests.

## Thanks

* This project is heavily inspired by [Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore).
* We use [Namotion.Reflection](https://github.com/RicoSuter/Namotion.Reflection) for pulling xml documentation.

