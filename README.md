# NRest

Make REST API calls using a fluent syntax.

Download using NuGet: [NDex](http://nuget.org/packages/nrest)

## Overview
REST is easy, so interacting with a REST API should be easy too! The aim of NRest is provide a simple library that encapsulates of a lot of the heavy-lifting involved with building REST requests. REST is a pretty flexible and open standard, so a good REST client shouldn't prevent you from tweaking the request to handle those special cases, either.

Here's an example that builds up a request and processes the results:

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
    
