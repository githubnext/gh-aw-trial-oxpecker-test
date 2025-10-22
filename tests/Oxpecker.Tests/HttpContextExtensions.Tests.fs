module Oxpecker.Tests.HttpContextExtensions

open System
open System.IO
open System.Collections.Generic
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.WebUtilities
open Microsoft.Extensions.DependencyInjection
open Oxpecker.ViewEngine
open Xunit
open FsUnit.Light
open Oxpecker


#nowarn "3391"

[<Fact>]
let ``GetRequestUrl returns entire URL of the HTTP request`` () =
    let ctx = DefaultHttpContext()
    ctx.Request.Scheme <- "http"
    ctx.Request.Host <- HostString("example.org:81")
    ctx.Request.PathBase <- PathString("/something")
    ctx.Request.Path <- PathString("/hello")
    ctx.Request.QueryString <- QueryString("?a=1&b=2")
    ctx.Request.Method <- "GET"
    ctx.Response.Body <- new MemoryStream()

    let result = ctx.GetRequestUrl()

    result |> shouldEqual "http://example.org:81/something/hello?a=1&b=2"

[<Fact>]
let ``TryGetRequestHeader during HTTP GET request with returns correct result`` () =
    let ctx = DefaultHttpContext()
    ctx.TryGetHeaderValue "X-Test" |> shouldEqual None
    ctx.Request.Headers.Add("X-Test", "It works!")

    let result = ctx.TryGetHeaderValue "X-Test"

    result |> shouldEqual(Some "It works!")

[<Fact>]
let ``TryGetQueryStringValue during HTTP GET request with query string returns correct result`` () =
    let ctx = DefaultHttpContext()
    ctx.TryGetQueryValue "BirthDate" |> shouldEqual None
    let queryStr =
        "?Name=John%20Doe&IsVip=true&BirthDate=1990-04-20&Balance=150000.5&LoyaltyPoints=137"
    let query = QueryHelpers.ParseQuery queryStr
    ctx.Request.Query <- QueryCollection(query)

    let result = ctx.TryGetQueryValue "BirthDate"

    result |> shouldEqual(Some "1990-04-20")

[<Fact>]
let ``WriteText with HTTP GET should return text in body`` () =
    task {
        let ctx = DefaultHttpContext()
        ctx.Response.Body <- new MemoryStream()

        do! ctx.WriteText "Hello World"

        ctx.Response.Body.Seek(0, SeekOrigin.Begin) |> ignore
        use reader = new StreamReader(ctx.Response.Body)
        let result = reader.ReadToEnd()
        result |> shouldEqual "Hello World"
    }

[<Fact>]
let ``WriteText with HTTP HEAD should not return text in body`` () =
    task {
        let ctx = DefaultHttpContext()
        ctx.Response.Body <- new MemoryStream()
        ctx.Request.Method <- "HEAD"

        do! ctx.WriteText "Hello World"

        ctx.Response.Body.Seek(0, SeekOrigin.Begin) |> ignore
        use reader = new StreamReader(ctx.Response.Body)
        reader.ReadToEnd() |> shouldEqual ""
    }

[<Fact>]
let ``WriteJson should add json to the context`` () =
    task {
        let ctx = DefaultHttpContext()
        ctx.Response.Body <- new MemoryStream()
        let services = ServiceCollection()
        services.AddSingleton<IJsonSerializer>(fun sp -> SystemTextJsonSerializer() :> IJsonSerializer)
        |> ignore
        ctx.RequestServices <- DefaultServiceProviderFactory().CreateServiceProvider(services)

        do! ctx.WriteJson({| Hello = "World" |})

        ctx.Response.Body.Seek(0, SeekOrigin.Begin) |> ignore
        use reader = new StreamReader(ctx.Response.Body)
        let result = reader.ReadToEnd()
        ctx.Response.Headers.ContentType
        |> shouldEqual "application/json; charset=utf-8"
        ctx.Response.Headers.ContentLength |> shouldEqual 17L
        result |> shouldEqual """{"hello":"World"}"""
    }

[<Fact>]
let ``WriteJsonChunked should add json to the context`` () =
    task {
        let ctx = DefaultHttpContext()
        ctx.Response.Body <- new MemoryStream()
        let services = ServiceCollection()
        services.AddSingleton<IJsonSerializer>(fun sp -> SystemTextJsonSerializer() :> IJsonSerializer)
        |> ignore
        ctx.RequestServices <- DefaultServiceProviderFactory().CreateServiceProvider(services)

        do! ctx.WriteJsonChunked {| Hello = "World" |}

        ctx.Response.Body.Seek(0, SeekOrigin.Begin) |> ignore
        use reader = new StreamReader(ctx.Response.Body)
        let result = reader.ReadToEnd()
        ctx.Response.Headers.ContentType
        |> shouldEqual "application/json; charset=utf-8"
        ctx.Response.Headers.ContentLength |> shouldEqual(Nullable())
        result |> shouldEqual """{"hello":"World"}"""
    }

[<Fact>]
let ``WriteHtmlViewAsync should add html to the context`` () =
    task {
        let ctx = DefaultHttpContext()
        ctx.Response.Body <- new MemoryStream()
        let htmlDoc =
            html() {
                head()
                body() { h1(id = "header") { "Hello world" } }
            }
        do! ctx.WriteHtmlView(htmlDoc)

        ctx.Response.Body.Seek(0, SeekOrigin.Begin) |> ignore
        use reader = new StreamReader(ctx.Response.Body)
        let result = reader.ReadToEnd()

        result
        |> shouldEqual
            $"""<!DOCTYPE html>{Environment.NewLine}<html><head></head><body><h1 id="header">Hello world</h1></body></html>"""
    }

[<Fact>]
let ``TryGetRouteValue returns Some when route value exists`` () =
    let ctx = DefaultHttpContext()
    ctx.Request.RouteValues.Add("id", box 42)

    let result = ctx.TryGetRouteValue<int>("id")

    result |> shouldEqual(Some 42)

[<Fact>]
let ``TryGetRouteValue returns None when route value does not exist`` () =
    let ctx = DefaultHttpContext()

    let result = ctx.TryGetRouteValue<int>("id")

    result |> shouldEqual None

[<Fact>]
let ``TryGetHeaderValues returns Some seq when header exists`` () =
    let ctx = DefaultHttpContext()
    ctx.Request.Headers.Add(
        "Accept",
        Microsoft.Extensions.Primitives.StringValues([| "text/html"; "application/json" |])
    )

    let result = ctx.TryGetHeaderValues("Accept")

    result.IsSome |> shouldEqual true
    result.Value |> Seq.toList |> shouldEqual [ "text/html"; "application/json" ]

[<Fact>]
let ``TryGetHeaderValues returns None when header does not exist`` () =
    let ctx = DefaultHttpContext()

    let result = ctx.TryGetHeaderValues("Accept")

    result |> shouldEqual None

[<Fact>]
let ``TryGetQueryValues returns Some seq when query parameter exists`` () =
    let ctx = DefaultHttpContext()
    let queryStr = "?tags=fsharp&tags=dotnet&tags=webdev"
    let query = QueryHelpers.ParseQuery queryStr
    ctx.Request.Query <- QueryCollection(query)

    let result = ctx.TryGetQueryValues("tags")

    result.IsSome |> shouldEqual true
    result.Value |> Seq.toList |> shouldEqual [ "fsharp"; "dotnet"; "webdev" ]

[<Fact>]
let ``TryGetQueryValues returns None when query parameter does not exist`` () =
    let ctx = DefaultHttpContext()
    ctx.Request.Query <- QueryCollection()

    let result = ctx.TryGetQueryValues("tags")

    result |> shouldEqual None

[<Fact>]
let ``TryGetFormValue returns Some when form value exists`` () =
    let ctx = DefaultHttpContext()
    let formData = Dictionary<string, Microsoft.Extensions.Primitives.StringValues>()
    formData.Add("username", Microsoft.Extensions.Primitives.StringValues("johndoe"))
    ctx.Request.Form <- FormCollection(formData)

    let result = ctx.TryGetFormValue("username")

    result |> shouldEqual(Some "johndoe")

[<Fact>]
let ``TryGetFormValue returns None when form value does not exist`` () =
    let ctx = DefaultHttpContext()
    let emptyFormData =
        Dictionary<string, Microsoft.Extensions.Primitives.StringValues>()
    ctx.Request.Form <- FormCollection(emptyFormData)

    let result = ctx.TryGetFormValue("username")

    result |> shouldEqual None

[<Fact>]
let ``TryGetFormValues returns Some seq when form values exist`` () =
    let ctx = DefaultHttpContext()
    let formData = Dictionary<string, Microsoft.Extensions.Primitives.StringValues>()
    formData.Add("hobbies", Microsoft.Extensions.Primitives.StringValues([| "reading"; "coding"; "gaming" |]))
    ctx.Request.Form <- FormCollection(formData)

    let result = ctx.TryGetFormValues("hobbies")

    result.IsSome |> shouldEqual true
    result.Value |> Seq.toList |> shouldEqual [ "reading"; "coding"; "gaming" ]

[<Fact>]
let ``TryGetFormValues returns None when form values do not exist`` () =
    let ctx = DefaultHttpContext()
    let emptyFormData =
        Dictionary<string, Microsoft.Extensions.Primitives.StringValues>()
    ctx.Request.Form <- FormCollection(emptyFormData)

    let result = ctx.TryGetFormValues("hobbies")

    result |> shouldEqual None

[<Fact>]
let ``SetStatusCode sets response status code`` () =
    let ctx = DefaultHttpContext()

    ctx.SetStatusCode(404)

    ctx.Response.StatusCode |> shouldEqual 404

[<Fact>]
let ``SetHttpHeader adds header to response`` () =
    let ctx = DefaultHttpContext()

    ctx.SetHttpHeader("X-Custom-Header", "CustomValue")

    ctx.Response.Headers["X-Custom-Header"] |> string |> shouldEqual "CustomValue"

[<Fact>]
let ``SetHttpHeader overwrites existing header`` () =
    let ctx = DefaultHttpContext()
    ctx.Response.Headers.Add("X-Custom-Header", "OldValue")

    ctx.SetHttpHeader("X-Custom-Header", "NewValue")

    ctx.Response.Headers["X-Custom-Header"] |> string |> shouldEqual "NewValue"

[<Fact>]
let ``SetContentType sets Content-Type header`` () =
    let ctx = DefaultHttpContext()

    ctx.SetContentType("application/xml")

    ctx.Response.Headers.ContentType |> string |> shouldEqual "application/xml"

[<Fact>]
let ``WriteBytes writes byte array to response`` () =
    task {
        let ctx = DefaultHttpContext()
        ctx.Response.Body <- new MemoryStream()
        let bytes = System.Text.Encoding.UTF8.GetBytes("Hello Bytes")

        do! ctx.WriteBytes(bytes)

        ctx.Response.Body.Seek(0, SeekOrigin.Begin) |> ignore
        use reader = new StreamReader(ctx.Response.Body)
        let result = reader.ReadToEnd()
        result |> shouldEqual "Hello Bytes"
        ctx.Response.ContentLength |> shouldEqual(int64 bytes.Length)
    }

[<Fact>]
let ``WriteBytes with HEAD method does not write body`` () =
    task {
        let ctx = DefaultHttpContext()
        ctx.Response.Body <- new MemoryStream()
        ctx.Request.Method <- "HEAD"
        let bytes = System.Text.Encoding.UTF8.GetBytes("Hello Bytes")

        do! ctx.WriteBytes(bytes)

        ctx.Response.Body.Seek(0, SeekOrigin.Begin) |> ignore
        use reader = new StreamReader(ctx.Response.Body)
        let result = reader.ReadToEnd()
        result |> shouldEqual ""
        ctx.Response.ContentLength |> shouldEqual(int64 bytes.Length)
    }

[<Fact>]
let ``WriteHtmlString writes HTML string to response`` () =
    task {
        let ctx = DefaultHttpContext()
        ctx.Response.Body <- new MemoryStream()
        let html = "<h1>Hello HTML</h1>"

        do! ctx.WriteHtmlString(html)

        ctx.Response.Body.Seek(0, SeekOrigin.Begin) |> ignore
        use reader = new StreamReader(ctx.Response.Body)
        let result = reader.ReadToEnd()
        result |> shouldEqual html
        ctx.Response.Headers.ContentType
        |> string
        |> shouldEqual "text/html; charset=utf-8"
    }

[<Fact>]
let ``WriteHtmlView with HEAD method does not write body`` () =
    task {
        let ctx = DefaultHttpContext()
        ctx.Response.Body <- new MemoryStream()
        ctx.Request.Method <- "HEAD"
        let htmlDoc = html() { body() { h1() { "Hello" } } }

        do! ctx.WriteHtmlView(htmlDoc)

        ctx.Response.Body.Seek(0, SeekOrigin.Begin) |> ignore
        use reader = new StreamReader(ctx.Response.Body)
        let result = reader.ReadToEnd()
        result |> shouldEqual ""
        ctx.Response.ContentLength.HasValue |> shouldEqual true
    }

[<Fact>]
let ``WriteHtmlViewChunked writes HTML element with chunked encoding`` () =
    task {
        let ctx = DefaultHttpContext()
        ctx.Response.Body <- new MemoryStream()
        let htmlDoc = html() { body() { h1() { "Chunked HTML" } } }

        do! ctx.WriteHtmlViewChunked(htmlDoc)

        ctx.Response.Body.Seek(0, SeekOrigin.Begin) |> ignore
        use reader = new StreamReader(ctx.Response.Body)
        let result = reader.ReadToEnd()
        result |> shouldContainText "Chunked HTML"
        ctx.Response.Headers.ContentType
        |> string
        |> shouldEqual "text/html; charset=utf-8"
    }

[<Fact>]
let ``GetService retrieves service from container`` () =
    let ctx = DefaultHttpContext()
    let services = ServiceCollection()
    services.AddSingleton<IJsonSerializer>(fun sp -> SystemTextJsonSerializer() :> IJsonSerializer)
    |> ignore
    ctx.RequestServices <- DefaultServiceProviderFactory().CreateServiceProvider(services)

    let serializer = ctx.GetService<IJsonSerializer>()

    isNull(box serializer) |> shouldEqual false

[<Fact>]
let ``GetJsonSerializer retrieves JSON serializer from container`` () =
    let ctx = DefaultHttpContext()
    let services = ServiceCollection()
    services.AddSingleton<IJsonSerializer>(fun sp -> SystemTextJsonSerializer() :> IJsonSerializer)
    |> ignore
    ctx.RequestServices <- DefaultServiceProviderFactory().CreateServiceProvider(services)

    let serializer = ctx.GetJsonSerializer()

    isNull(box serializer) |> shouldEqual false

[<Fact>]
let ``GetModelBinder retrieves model binder from container`` () =
    let ctx = DefaultHttpContext()
    let services = ServiceCollection()
    services.AddSingleton<IModelBinder>(fun sp -> ModelBinder() :> IModelBinder)
    |> ignore
    ctx.RequestServices <- DefaultServiceProviderFactory().CreateServiceProvider(services)

    let binder = ctx.GetModelBinder()

    isNull(box binder) |> shouldEqual false

[<Fact>]
let ``BindJson deserializes JSON from request body`` () =
    task {
        let ctx = DefaultHttpContext()
        let services = ServiceCollection()
        services.AddSingleton<IJsonSerializer>(fun sp -> SystemTextJsonSerializer() :> IJsonSerializer)
        |> ignore
        ctx.RequestServices <- DefaultServiceProviderFactory().CreateServiceProvider(services)
        let json = """{"name":"John","age":30}"""
        ctx.Request.Body <- new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json))
        ctx.Request.ContentType <- "application/json"

        let! result = ctx.BindJson<{| name: string; age: int |}>()

        result.name |> shouldEqual "John"
        result.age |> shouldEqual 30
    }

[<Fact>]
let ``BindForm parses form data into object`` () =
    task {
        let ctx = DefaultHttpContext()
        let services = ServiceCollection()
        services.AddSingleton<IModelBinder>(fun sp -> ModelBinder() :> IModelBinder)
        |> ignore
        ctx.RequestServices <- DefaultServiceProviderFactory().CreateServiceProvider(services)
        let formData = Dictionary<string, Microsoft.Extensions.Primitives.StringValues>()
        formData.Add("Name", Microsoft.Extensions.Primitives.StringValues("Jane"))
        formData.Add("Age", Microsoft.Extensions.Primitives.StringValues("25"))
        ctx.Request.Form <- FormCollection(formData)

        let! result = ctx.BindForm<{| Name: string; Age: int |}>()

        result.Name |> shouldEqual "Jane"
        result.Age |> shouldEqual 25
    }

[<Fact>]
let ``BindQuery parses query string into object`` () =
    let ctx = DefaultHttpContext()
    let services = ServiceCollection()
    services.AddSingleton<IModelBinder>(fun sp -> ModelBinder() :> IModelBinder)
    |> ignore
    ctx.RequestServices <- DefaultServiceProviderFactory().CreateServiceProvider(services)
    let queryStr = "?Name=Bob&Age=35"
    let query = QueryHelpers.ParseQuery queryStr
    ctx.Request.Query <- QueryCollection(query)

    let result = ctx.BindQuery<{| Name: string; Age: int |}>()

    result.Name |> shouldEqual "Bob"
    result.Age |> shouldEqual 35
