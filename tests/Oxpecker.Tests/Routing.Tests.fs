module Oxpecker.Tests.Routing

open System
open System.Net
open System.Threading.Tasks
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http.Metadata
open Microsoft.AspNetCore.TestHost
open Microsoft.Extensions.DependencyInjection
open Xunit
open FsUnit.Light
open Oxpecker
open Microsoft.AspNetCore.Routing

module WebApp =

    let notFoundHandler = setStatusCode 404 >=> text "Not found"

    let webApp (endpoints: Endpoint seq) =
        let builder =
            WebHostBuilder()
                .UseKestrel()
                .Configure(fun app -> app.UseRouting().UseOxpecker(endpoints).Run(notFoundHandler))
                .ConfigureServices(fun services -> services.AddRouting() |> ignore)
        new TestServer(builder)

    let webAppOneRoute (endpoint: Endpoint) =
        let builder =
            WebHostBuilder()
                .UseKestrel()
                .Configure(fun app -> app.UseRouting().UseOxpecker(endpoint).Run(notFoundHandler))
                .ConfigureServices(fun services -> services.AddRouting() |> ignore)
        new TestServer(builder)

// ---------------------------------
// route Tests
// ---------------------------------

[<Fact>]
let ``route: GET "/" returns "Hello World"`` () =
    task {
        let endpoint = GET [ route "/" <| text "Hello World"; route "/foo" <| text "bar" ]
        let server = WebApp.webAppOneRoute endpoint
        let client = server.CreateClient()

        let! result = client.GetAsync("/")
        let! resultString = result.Content.ReadAsStringAsync()

        result.StatusCode |> shouldEqual HttpStatusCode.OK
        resultString |> shouldEqual "Hello World"
    }

[<Fact>]
let ``route: GET "/foo" returns "bar"`` () =
    task {
        let endpoints = [ GET [ route "/" <| text "Hello World"; route "/foo" <| text "bar" ] ]
        let server = WebApp.webApp endpoints
        let client = server.CreateClient()

        let! result = client.GetAsync("/foo")
        let! resultString = result.Content.ReadAsStringAsync()

        result.StatusCode |> shouldEqual HttpStatusCode.OK
        resultString |> shouldEqual "bar"
    }

// ---------------------------------
// routex Tests
// ---------------------------------



[<Fact>]
let ``routex: GET "/foo///" returns "bar"`` () =
    task {
        let endpoints = [ GET [ route "/" <| text "Hello World"; route "/foo/{**path}" <| text "bar" ] ]
        let server = WebApp.webApp endpoints
        let client = server.CreateClient()

        let! result = client.GetAsync("/foo///")
        let! resultString = result.Content.ReadAsStringAsync()

        result.StatusCode |> shouldEqual HttpStatusCode.OK
        resultString |> shouldEqual "bar"
    }

[<Fact>]
let ``routex: GET "/foo2" returns "bar"`` () =
    task {
        let endpoints = [ GET [ route "/" <| text "Hello World"; route "/foo2/{*path}" <| text "bar" ] ]
        let server = WebApp.webApp endpoints
        let client = server.CreateClient()

        let! result = client.GetAsync("/foo2")
        let! resultString = result.Content.ReadAsStringAsync()

        result.StatusCode |> shouldEqual HttpStatusCode.OK
        resultString |> shouldEqual "bar"
    }


// ---------------------------------
// routef Tests
// ---------------------------------

[<Fact>]
let ``routef generates route correctly`` () =
    task {
        let endpoint = routef "/foo/{%s}/{%i}/{%O:guid}" (fun x y z -> text "Hello")

        match endpoint with
        | SimpleEndpoint(_, route, _, _) -> route |> shouldEqual "/foo/{x}/{y}/{z:guid}"
        | _ -> failwith "Expected SimpleEndpoint"
    }


[<Fact>]
let ``routef: GET "/foo/blah blah/bar" returns "blah blah"`` () =

    task {
        let endpoints = [
            GET [
                route "/" <| text "Hello World"
                route "/foo" <| text "bar"
                routef "/foo/{%s}/bar" text
                routef "/foo/{%s}/{%i}" (fun name age -> text $"Name: %s{name}, Age: %i{age}")
            ]
        ]
        let server = WebApp.webApp endpoints
        let client = server.CreateClient()

        let! result = client.GetAsync("/foo/blah blah/bar")
        let! resultString = result.Content.ReadAsStringAsync()

        result.StatusCode |> shouldEqual HttpStatusCode.OK
        resultString |> shouldEqual "blah blah"
    }

[<Fact>]
let ``routef: GET "/foo/johndoe/59" returns "Name: johndoe, Age: 59"`` () =
    task {
        let endpoints = [
            GET [
                route "/" <| text "Hello World"
                route "/foo" <| text "bar"
                routef "/foo/{%s}/bar" text
                routef "/foo/{%s}/{%i}" (fun name age -> text $"Name: %s{name}, Age: %i{age}")
            ]
        ]
        let server = WebApp.webApp endpoints
        let client = server.CreateClient()

        let! result = client.GetAsync("/foo/johndoe/59")
        let! resultString = result.Content.ReadAsStringAsync()

        result.StatusCode |> shouldEqual HttpStatusCode.OK
        resultString |> shouldEqual "Name: johndoe, Age: 59"
    }

[<Fact>]
let ``routef: GET "/foo/b%2Fc/bar" returns "b%2Fc"`` () =
    task {
        let endpoints = [
            GET [
                route "/" <| text "Hello World"
                route "/foo" <| text "bar"
                routef "/foo/{%s}/bar" text
                routef "/foo/{%s}/{%i}" (fun name age -> text $"Name: %s{name}, Age: %i{age}")
            ]
        ]
        let server = WebApp.webApp endpoints
        let client = server.CreateClient()

        let! result = client.GetAsync("/foo/b%2Fc/bar")
        let! resultString = result.Content.ReadAsStringAsync()

        result.StatusCode |> shouldEqual HttpStatusCode.OK
        resultString |> shouldEqual "b/c"
    }

[<Fact>]
let ``routef: GET "/foo/a%2Fb%2Bc.d%2Ce/bar" returns "a/b+c.d,e"`` () =
    task {
        let endpoints = [
            GET [
                route "/" <| text "Hello World"
                route "/foo" <| text "bar"
                routef "/foo/{%s}/bar" text
                routef "/foo/{%s}/{%i}" (fun name age -> text $"Name: %s{name}, Age: %i{age}")
            ]
        ]
        let server = WebApp.webApp endpoints
        let client = server.CreateClient()

        let! result = client.GetAsync("/foo/a%2Fb%2Bc.d%2Ce/bar")
        let! resultString = result.Content.ReadAsStringAsync()

        result.StatusCode |> shouldEqual HttpStatusCode.OK
        resultString |> shouldEqual "a/b+c.d,e"
    }


[<Fact>]
let ``routef: GET "/foo/%O/bar/%O" returns "Guid1: ..., Guid2: ..."`` () =
    task {
        let endpoints = [
            GET [
                route "/" <| text "Hello World"
                route "/foo" <| text "bar"
                routef "/foo/{%s}/bar" text
                routef "/foo/{%s}/{%i}" (fun name age -> text $"Name: %s{name}, Age: %i{age}")
                routef "/foo/{%O:guid}/bar/{%O:guid}" (fun (guid1: Guid) (guid2: Guid) ->
                    text $"Guid1: %O{guid1}, Guid2: %O{guid2}")
            ]
        ]
        let server = WebApp.webApp endpoints
        let client = server.CreateClient()

        let! result = client.GetAsync("/foo/4ec87f064d1e41b49342ab1aead1f99d/bar/2a6c9185-95d9-4d8c-80a6-575f99c2a716")
        let! resultString = result.Content.ReadAsStringAsync()

        result.StatusCode |> shouldEqual HttpStatusCode.OK
        resultString
        |> shouldEqual "Guid1: 4ec87f06-4d1e-41b4-9342-ab1aead1f99d, Guid2: 2a6c9185-95d9-4d8c-80a6-575f99c2a716"
    }

[<Fact>]
let ``routef: GET "/foo/%u/bar/%u" returns "Id1: ..., Id2: ..."`` () =
    task {
        let endpoints = [
            GET [
                route "/" <| text "Hello World"
                route "/foo" <| text "bar"
                routef "/foo/{%s}/bar" text
                routef "/foo/{%s}/{%i}" (fun name age -> text $"Name: %s{name}, Age: %i{age}")
                routef "/foo/{%u}/bar/{%u}" (fun (id1: uint64) (id2: uint64) -> text $"Id1: %u{id1}, Id2: %u{id2}")
            ]
        ]
        let server = WebApp.webApp endpoints
        let client = server.CreateClient()

        let! result = client.GetAsync("/foo/12635000945053400782/bar/16547050693006839099")
        let! resultString = result.Content.ReadAsStringAsync()

        result.StatusCode |> shouldEqual HttpStatusCode.OK
        resultString
        |> shouldEqual "Id1: 12635000945053400782, Id2: 16547050693006839099"
    }

[<Fact>]
let ``routef: GET "/foo/bar/baz/qux" returns 404 "Not found"`` () =
    task {
        let endpoints = [ GET [ routef "/foo/{%s}/{%s}" (fun s1 s2 -> text $"%s{s1},%s{s2}") ] ]
        let server = WebApp.webApp endpoints
        let client = server.CreateClient()

        let! result = client.GetAsync("/foo/bar/baz/qux")
        let! resultString = result.Content.ReadAsStringAsync()

        result.StatusCode |> shouldEqual HttpStatusCode.NotFound
        resultString |> shouldEqual "Not found"
    }


[<Fact>]
let ``routef: GET "/foo/bar/baz/qux" returns "bar/baz/qux"`` () =
    task {
        let endpoints = [ GET [ routef "/foo/{**%s}" text; routef "/moo/{*%s}" text ] ]
        let server = WebApp.webApp endpoints
        let client = server.CreateClient()

        let! result1 = client.GetAsync("/foo/bar/baz/qux")
        let! result1String = result1.Content.ReadAsStringAsync()

        let! result2 = client.GetAsync("/moo/bar/baz/qux")
        let! result2String = result2.Content.ReadAsStringAsync()

        result1.StatusCode |> shouldEqual HttpStatusCode.OK
        result2.StatusCode |> shouldEqual HttpStatusCode.OK
        result1String |> shouldEqual "bar/baz/qux"
        result2String |> shouldEqual "bar/baz/qux"
    }

// ---------------------------------
// subRoute Tests
// ---------------------------------

[<Fact>]
let ``subRoute: Route with empty route`` () =
    task {
        let endpoints = [
            GET [
                route "/" <| text "Hello World"
                route "/foo" <| text "bar"
                subRoute "/api" [
                    route "" <| text "api root"
                    route "/admin" <| text "admin"
                    route "/users" <| text "users"
                ]
                route "/api/test" <| text "test"
            ]
        ]
        let server = WebApp.webApp endpoints
        let client = server.CreateClient()

        let! result = client.GetAsync("/api")
        let! resultString = result.Content.ReadAsStringAsync()

        result.StatusCode |> shouldEqual HttpStatusCode.OK
        resultString |> shouldEqual "api root"
    }

[<Fact>]
let ``subRoute: Normal nested route after subRoute`` () =
    task {
        let endpoints = [
            GET [
                route "/" <| text "Hello World"
                route "/foo" <| text "bar"
                subRoute "/api" [
                    route "" <| text "api root"
                    route "/admin" <| text "admin"
                    route "/users" <| text "users"
                ]
                route "/api/test" <| text "test"
            ]
        ]
        let server = WebApp.webApp endpoints
        let client = server.CreateClient()

        let! result = client.GetAsync("/api/users")
        let! resultString = result.Content.ReadAsStringAsync()

        result.StatusCode |> shouldEqual HttpStatusCode.OK
        resultString |> shouldEqual "users"
    }

[<Fact>]
let ``subRoute: Route after subRoute has same beginning of path`` () =
    task {
        let endpoints = [
            GET [
                route "/" <| text "Hello World"
                route "/foo" <| text "bar"
                subRoute "/api" [
                    route "" <| text "api root"
                    route "/admin" <| text "admin"
                    route "/users" <| text "users"
                ]
                route "/api/test" <| text "test"
            ]
        ]
        let server = WebApp.webApp endpoints
        let client = server.CreateClient()

        let! result = client.GetAsync("/api/test")
        let! resultString = result.Content.ReadAsStringAsync()

        result.StatusCode |> shouldEqual HttpStatusCode.OK
        resultString |> shouldEqual "test"
    }

[<Fact>]
let ``subRoute: Nested sub routes`` () =
    task {
        let endpoints = [
            GET [
                route "/" <| text "Hello World"
                route "/foo" <| text "bar"
                subRoute "/api" [
                    route "" <| text "api root"
                    route "/admin" <| text "admin"
                    route "/users" <| text "users"
                    subRoute "/v2" [
                        route "" <| text "api root v2"
                        route "/admin" <| text "admin v2"
                        route "/users" <| text "users v2"
                    ]
                ]
                route "/api/test" <| text "test"
            ]
        ]
        let server = WebApp.webApp endpoints
        let client = server.CreateClient()

        let! result = client.GetAsync("/api/v2/users")
        let! resultString = result.Content.ReadAsStringAsync()

        result.StatusCode |> shouldEqual HttpStatusCode.OK
        resultString |> shouldEqual "users v2"
    }

[<Fact>]
let ``subRoute: Multiple nested sub routes`` () =
    task {
        let endpoints = [
            GET [
                route "/" <| text "Hello World"
                route "/foo" <| text "bar"
                subRoute "/api" [
                    route "/users" <| text "users"
                    subRoute "/v2" [ route "/admin" <| text "admin v2"; route "/users" <| text "users v2" ]
                    subRoute "/v2" [ route "/admin2" <| text "correct admin2" ]
                ]
                route "/api/test" <| text "test"
            ]
        ]
        let server = WebApp.webApp endpoints
        let client = server.CreateClient()

        let! result = client.GetAsync("/api/v2/admin2")
        let! resultString = result.Content.ReadAsStringAsync()

        result.StatusCode |> shouldEqual HttpStatusCode.OK
        resultString |> shouldEqual "correct admin2"
    }

[<Fact>]
let ``subRoute: Route after nested sub routes has same beginning of path`` () =
    task {
        let endpoints = [
            GET [
                route "/" <| text "Hello World"
                route "/foo" <| text "bar"
                subRoute "/api" [
                    route "" <| text "api root"
                    route "/admin" <| text "admin"
                    route "/users" <| text "users"
                    subRoute "/v2" [
                        route "" <| text "api root v2"
                        route "/admin" <| text "admin v2"
                        route "/users" <| text "users v2"
                    ]
                    route "/yada" <| text "yada"
                ]
                route "/api/test" <| text "test"
                route "/api/v2/else" <| text "else"
            ]
        ]
        let server = WebApp.webApp endpoints
        let client = server.CreateClient()

        let! result = client.GetAsync("/api/v2/else")
        let! resultString = result.Content.ReadAsStringAsync()

        result.StatusCode |> shouldEqual HttpStatusCode.OK
        resultString |> shouldEqual "else"
    }

[<Fact>]
let ``subRoute: routef inside subRoute`` () =
    task {
        let endpoints = [
            GET [
                route "/" <| text "Hello World"
                route "/foo" <| text "bar"
                subRoute "/api" [ route "" <| text "api root"; routef "/foo/bar/{%s}" text ]
                route "/api/test" <| text "test"
            ]
        ]
        let server = WebApp.webApp endpoints
        let client = server.CreateClient()

        let! result = client.GetAsync("/api/foo/bar/yadayada")
        let! resultString = result.Content.ReadAsStringAsync()

        result.StatusCode |> shouldEqual HttpStatusCode.OK
        resultString |> shouldEqual "yadayada"
    }

[<Fact>]
let ``subRoute: configureEndpoint inside subRoute`` () =
    task {
        let mutable rootMetadata = Unchecked.defaultof<EndpointMetadataCollection>
        let mutable getMetadata = Unchecked.defaultof<EndpointMetadataCollection>
        let mutable innerMetadata = Unchecked.defaultof<EndpointMetadataCollection>
        let endpoints = [
            route "/" (fun ctx ->
                rootMetadata <- ctx.GetEndpoint() |> Unchecked.nonNull |> _.Metadata
                ctx.WriteText "")
            GET [
                route "/get" (fun ctx ->
                    getMetadata <- ctx.GetEndpoint() |> Unchecked.nonNull |> _.Metadata
                    ctx.WriteText "Hello World")
                subRoute "/api" [
                    routef "/inner" (fun ctx ->
                        innerMetadata <- ctx.GetEndpoint() |> Unchecked.nonNull |> _.Metadata
                        ctx.WriteText "Hi")
                ]
                |> configureEndpoint _.ShortCircuit()
            ]
            |> configureEndpoint _.DisableAntiforgery()
        ]
        let server = WebApp.webApp endpoints
        let client = server.CreateClient()

        let! _ = client.GetAsync("/api/inner")
        let! _ = client.GetAsync("/")
        let! _ = client.GetAsync("/get")

        innerMetadata.Count |> shouldBeGreaterThan getMetadata.Count
        getMetadata.Count |> shouldBeGreaterThan rootMetadata.Count
    }

// ---------------------------------
// HttpVerb.ToString Tests
// ---------------------------------

[<Fact>]
let ``HttpVerb.ToString returns correct string for POST`` () =
    HttpVerb.POST.ToString() |> shouldEqual "POST"

[<Fact>]
let ``HttpVerb.ToString returns correct string for PUT`` () =
    HttpVerb.PUT.ToString() |> shouldEqual "PUT"

[<Fact>]
let ``HttpVerb.ToString returns correct string for PATCH`` () =
    HttpVerb.PATCH.ToString() |> shouldEqual "PATCH"

[<Fact>]
let ``HttpVerb.ToString returns correct string for DELETE`` () =
    HttpVerb.DELETE.ToString() |> shouldEqual "DELETE"

[<Fact>]
let ``HttpVerb.ToString returns correct string for HEAD`` () =
    HttpVerb.HEAD.ToString() |> shouldEqual "HEAD"

[<Fact>]
let ``HttpVerb.ToString returns correct string for OPTIONS`` () =
    HttpVerb.OPTIONS.ToString() |> shouldEqual "OPTIONS"

[<Fact>]
let ``HttpVerb.ToString returns correct string for TRACE`` () =
    HttpVerb.TRACE.ToString() |> shouldEqual "TRACE"

[<Fact>]
let ``HttpVerb.ToString returns correct string for CONNECT`` () =
    HttpVerb.CONNECT.ToString() |> shouldEqual "CONNECT"

// ---------------------------------
// RouteTemplateBuilder.parse Tests
// ---------------------------------

[<Fact>]
let ``RouteTemplateBuilder.parse with 's' format parses string`` () =
    let result = Oxpecker.RouteTemplateBuilder.parse 's' None "test-string"
    result |> shouldEqual "test-string"

[<Fact>]
let ``RouteTemplateBuilder.parse with 's' format decodes %2F`` () =
    let result = Oxpecker.RouteTemplateBuilder.parse 's' None "path%2Fwith%2Fslashes"
    result |> shouldEqual "path/with/slashes"

[<Fact>]
let ``RouteTemplateBuilder.parse with 'i' format parses integer`` () =
    let result = Oxpecker.RouteTemplateBuilder.parse 'i' None "42"
    result |> shouldEqual(box 42)

[<Fact>]
let ``RouteTemplateBuilder.parse with 'i' format parses negative integer`` () =
    let result = Oxpecker.RouteTemplateBuilder.parse 'i' None "-123"
    result |> shouldEqual(box -123)

[<Fact>]
let ``RouteTemplateBuilder.parse with 'b' format parses boolean true`` () =
    let result = Oxpecker.RouteTemplateBuilder.parse 'b' None "true"
    result |> shouldEqual(box true)

[<Fact>]
let ``RouteTemplateBuilder.parse with 'b' format parses boolean false`` () =
    let result = Oxpecker.RouteTemplateBuilder.parse 'b' None "false"
    result |> shouldEqual(box false)

[<Fact>]
let ``RouteTemplateBuilder.parse with 'c' format parses char`` () =
    let result = Oxpecker.RouteTemplateBuilder.parse 'c' None "a"
    result |> shouldEqual(box 'a')

[<Fact>]
let ``RouteTemplateBuilder.parse with 'd' format parses int64`` () =
    let result = Oxpecker.RouteTemplateBuilder.parse 'd' None "9223372036854775807"
    result |> shouldEqual(box 9223372036854775807L)

[<Fact>]
let ``RouteTemplateBuilder.parse with 'f' format parses float`` () =
    let result = Oxpecker.RouteTemplateBuilder.parse 'f' None "3.14159"
    result |> shouldEqual(box 3.14159)

[<Fact>]
let ``RouteTemplateBuilder.parse with 'u' format parses uint64`` () =
    let result = Oxpecker.RouteTemplateBuilder.parse 'u' None "18446744073709551615"
    result |> shouldEqual(box 18446744073709551615UL)

[<Fact>]
let ``RouteTemplateBuilder.parse with 'O' format and guid modifier parses GUID`` () =
    let guidString = "550e8400-e29b-41d4-a716-446655440000"
    let result = Oxpecker.RouteTemplateBuilder.parse 'O' (Some "guid") guidString
    result |> shouldEqual(box(Guid.Parse guidString))

[<Fact>]
let ``RouteTemplateBuilder.parse with 'O' format without modifier returns string`` () =
    let result = Oxpecker.RouteTemplateBuilder.parse 'O' None "some-value"
    result |> shouldEqual "some-value"

[<Fact>]
let ``RouteTemplateBuilder.parse with unknown format returns string`` () =
    let result = Oxpecker.RouteTemplateBuilder.parse 'x' None "test"
    result |> shouldEqual "test"

[<Fact>]
let ``RouteTemplateBuilder.parse with invalid int throws RouteParseException`` () =
    let action =
        fun () -> Oxpecker.RouteTemplateBuilder.parse 'i' None "not-a-number" |> ignore
    Assert.Throws<RouteParseException>(action) |> ignore

[<Fact>]
let ``RouteTemplateBuilder.parse with invalid bool throws RouteParseException`` () =
    let action =
        fun () -> Oxpecker.RouteTemplateBuilder.parse 'b' None "not-a-bool" |> ignore
    Assert.Throws<RouteParseException>(action) |> ignore

[<Fact>]
let ``RouteTemplateBuilder.parse with invalid GUID throws RouteParseException`` () =
    let action =
        fun () -> Oxpecker.RouteTemplateBuilder.parse 'O' (Some "guid") "not-a-guid" |> ignore
    Assert.Throws<RouteParseException>(action) |> ignore

[<Fact>]
let ``RouteTemplateBuilder.parse with invalid float throws RouteParseException`` () =
    let action =
        fun () -> Oxpecker.RouteTemplateBuilder.parse 'f' None "not-a-float" |> ignore
    Assert.Throws<RouteParseException>(action) |> ignore

// ---------------------------------
// routef Additional Format Specifier Tests
// ---------------------------------

[<Fact>]
let ``routef: GET with valid boolean parameter works`` () =
    task {
        let endpoints = [ GET [ routef "/flag/{%b}" (fun (flag: bool) -> text $"Flag: {flag}") ] ]
        let server = WebApp.webApp endpoints
        let client = server.CreateClient()

        let! response = client.GetAsync("/flag/true")
        let! content = response.Content.ReadAsStringAsync()

        response.StatusCode |> shouldEqual HttpStatusCode.OK
        content |> shouldEqual "Flag: True"
    }

[<Fact>]
let ``routef: GET with char parameter works`` () =
    task {
        let endpoints = [ GET [ routef "/char/{%c}" (fun (c: char) -> text $"Char: {c}") ] ]
        let server = WebApp.webApp endpoints
        let client = server.CreateClient()

        let! response = client.GetAsync("/char/x")
        let! content = response.Content.ReadAsStringAsync()

        response.StatusCode |> shouldEqual HttpStatusCode.OK
        content |> shouldEqual "Char: x"
    }

[<Fact>]
let ``routef: GET with int64 parameter works`` () =
    task {
        let endpoints = [ GET [ routef "/bignum/{%d}" (fun (num: int64) -> text $"BigNum: {num}") ] ]
        let server = WebApp.webApp endpoints
        let client = server.CreateClient()

        let! response = client.GetAsync("/bignum/9223372036854775807")
        let! content = response.Content.ReadAsStringAsync()

        response.StatusCode |> shouldEqual HttpStatusCode.OK
        content |> shouldEqual "BigNum: 9223372036854775807"
    }

[<Fact>]
let ``routef: GET with float parameter works`` () =
    task {
        let endpoints = [ GET [ routef "/decimal/{%f}" (fun (num: float) -> text $"Decimal: {num}") ] ]
        let server = WebApp.webApp endpoints
        let client = server.CreateClient()

        let! response = client.GetAsync("/decimal/3.14159")
        let! content = response.Content.ReadAsStringAsync()

        response.StatusCode |> shouldEqual HttpStatusCode.OK
        content |> shouldEqual "Decimal: 3.14159"
    }
