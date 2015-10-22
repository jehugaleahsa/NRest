# NRest

Make REST API calls using a fluent syntax.

Download using NuGet: [NRest](http://nuget.org/packages/nrest)

## Overview
REST is easy, so interacting with a REST API should be easy too! The aim of NRest is provide a simple library that encapsulates of a lot of the heavy-lifting involved with building REST requests. REST is a pretty flexible and open standard, so a good REST client shouldn't prevent you from tweaking the request to handle those special cases, either.

Here's an example that builds up a request and processes the results, using JSON:

    RestClient client = new RestClient("http://example.com/api");
    var response = client.Get("customer/{CustomerId}", new { CustomerId = 123 })
        .Success(r => r.FromJson<Customer>())
        .Error(r => r.FromJson<ApiError>())
        .Execute();
    if (response.HasError)
    {
        ApiError error = response.GetResult<ApiError>();
        throw new Exception(error.Message);
    }
    return response.GetResult<Customer>();
    
Just some highlights:
- You can pass a base URL to the `RestClient` constructor and just specify the path later.
- You can use placeholders in your URLs so you don't have to constantly build strings.
- You can handle success (200 status codes) and errors (400+ status codes) separately.
- Or, you can handle specific codes using the `When` methods.
- Or, you can provide a catch-all using the `WhenUnhandled` method.
- Each handler can convert the result to a different value; use `GetResult<T>` to extract the value afterwards.
- You can execute synchronously (`Execute`) or asynchronously (`ExecuteAsync`).
- The response object will tell you:
    - the HTTP status code
    - whether it considered the response to be an error (400+) 
    - the extracted response value.

You can send updates just as easily:

    RestClient client = new RestClient("http://example.com/api");
    var response = client.Put("customer")
        .WithJsonBody(customer)
        .Success(r => r.FromJson<UpdateResponse>())
        .Error(r => r.FromJson<ApiError>())
        .Execute();
    if (response.HasError)
    {
        ApiError error = response.GetResult<ApiError>();
        throw new Exception(error.Message);
    }
    return response.GetResult<UpdateResponse>();
    
**NOTE** This project is in research and development mode. At first, the interfaces may change quite a bit, but will hopefully soon stabilize. Feel free to shout out feature requests and bug reports.

## Supported HTTP Methods
The `RestClient` class provides the following methods:
- GET
- HEAD
- POST
- PUT
- PATCH
- DELETE
- OPTIONS
- TRACE

## An Open Model
One of the big issues with most .NET REST clients is that they try to hide away the underlying .NET HTTP web classes. NRest does the exact opposite and lets you directly interact with the underlying `HttpWebRequest`. When configuring the request, call `ConfigureRequest` to directly manipulate the underlying request.

    var response = client.Get("http://example.com/customers/")
        .ConfigureRequest(r => r.UseDefaultCredentials = true)
        .Execute();
        
## Headers
If your REST API requires special headers, you can add them to the request using the `WithHeader` method.

    var response = client.Get("http://example.com/customers/")
        .WithHeader("username", "SecurityExpert")
        .WithHeader("password", "SuperSecurePassword")
        .Execute();

You don't need to worry about overwriting special headers (**Accept**, **Content-Type**, etc.), NRest will automatically set these properties on the underlying `HttpWebRequest`. Don't worry if you don't know what I'm talking about.

## Query Strings
If you have a query string, you can either incorporate placeholders into your URL or you can use the `WithQueryParameter` methods.

    var response = client.Get("http://example.com/customers/")
        .WithQueryParameter("name", "pizza")
        .Execute();
        
You can add as many query string pairs as you need. If you have an array of values, you can simply call the method multiple times with the same name.

    var configuration = client.Get("http://example.com/customers/");
    foreach (string value in values)
    {
        configuration = configuration.WithQueryParameter("name", value);
    }
    var response = configuration.Execute();
        
## JSON
NRest depends on [Json.NET](http://www.newtonsoft.com/json) to handle JSON serialization/deserialization. All of the JSON-related functionality is implemented in terms of extension methods. To use them, put `using NRest.Json;` at the top of your source.

To parse JSON results, you can use the `FromJson<T>` methods. The deserialized value will be stored in the response's `Result` property. You can either do a cast or call `GetResult<T>` (which also just does a cast).

    var response = client.Get("customer/{CustomerId}", new { CustomerId = 123 })
        .Success(r => r.FromJson<Customer>())
        .Execute();
        
If you are grabbing a list of records, just deserialize to a `List<Customer>` or `Customer[]`.

You can pass JSON objects in the body of your request using the `WithJsonBody` method.

    var response = client.Put("customer")
            .WithJsonBody(customer)
            .Execute();
            
## Url Encoded Data (Forms)
NRest can also interpret URL encoded data, like that sent when submitting a form in HTML. All of the URL encoded data functionality is implemented in terms of extensions methods. To use them, put `use NRest.Forms;` at the top of your source.

To parse form data, you can use the `FromForm` methods. The key/value pairs will be stored in a `NameValueCollection`, which can be retrieved from the response's `Result` property. You can either do a cast or call `GetResult<NameValueCollection>` (which also just does a cast).

    var response = client.Get("customer/{CustomerId}", new { CustomerId = 123 })
        .Success(r => r.FromForm())
        .Execute();
        
Eventually, I may provide an additional helper to convert from a `NameValueCollection` to an object.

You can pass your URL encoded data in the body of your request using the `WithUrlEncodedBody` method. This method either takes a `NameValueCollection` or allows you to build one on the fly.

    var response = client.Put("customer")
        .WithUrlEncodedBody(b => b.WithParameter("CustomerId", 123).WithParameter("Name", "Joe's Pizza"))
        .Execute();
        
If you have an array of values, you can build up a `NameValueCollection` ahead of time and pass that to `WithUrlEncodedBody`.

    var data = new NameValueCollection();
    foreach (string value in values)
    {
        data.Add("name", value);
    }
    var response = client.Put("customer")
        .WithUrlEncodedBody(data)
        .Execute();
