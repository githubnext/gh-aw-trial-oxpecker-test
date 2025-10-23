module Oxpecker.Tests.RouteTemplateBuilder

open System
open System.Net
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.TestHost
open Microsoft.Extensions.DependencyInjection
open Xunit
open FsUnit.Light
open Oxpecker

module WebApp =
    let notFoundHandler = setStatusCode 404 >=> text "Not found"

    let webApp (endpoints: Endpoint seq) =
        let builder =
            WebHostBuilder()
                .UseKestrel()
                .Configure(fun app -> app.UseRouting().UseOxpecker(endpoints).Run(notFoundHandler))
                .ConfigureServices(fun services -> services.AddRouting() |> ignore)
        new TestServer(builder)

// Tests for routef with different parameter types
[<Fact>]
let ``routef: string parameter with %2F is decoded`` () =
    task {
        let endpoint =
            GET [ routef "api/{%s}" (fun (path: string) -> text path) ]
        let server = WebApp.webApp [ endpoint ]
        let client = server.CreateClient()

        let! result = client.GetAsync("/api/foo%2Fbar")
        let! resultString = result.Content.ReadAsStringAsync()

        result.StatusCode |> shouldEqual HttpStatusCode.OK
        resultString |> shouldEqual "foo/bar"
    }

[<Fact>]
let ``routef: integer parameter parses correctly`` () =
    task {
        let endpoint =
            GET [ routef "api/item/{%i}" (fun (id: int) -> text (string id)) ]
        let server = WebApp.webApp [ endpoint ]
        let client = server.CreateClient()

        let! result = client.GetAsync("/api/item/42")
        let! resultString = result.Content.ReadAsStringAsync()

        result.StatusCode |> shouldEqual HttpStatusCode.OK
        resultString |> shouldEqual "42"
    }

[<Fact>]
let ``routef: negative integer parameter`` () =
    task {
        let endpoint =
            GET [ routef "api/item/{%i}" (fun (id: int) -> text (string id)) ]
        let server = WebApp.webApp [ endpoint ]
        let client = server.CreateClient()

        let! result = client.GetAsync("/api/item/-123")
        let! resultString = result.Content.ReadAsStringAsync()

        result.StatusCode |> shouldEqual HttpStatusCode.OK
        resultString |> shouldEqual "-123"
    }

[<Fact>]
let ``routef: boolean parameter with true`` () =
    task {
        let endpoint =
            GET [ routef "api/flag/{%b}" (fun (flag: bool) -> text (string flag)) ]
        let server = WebApp.webApp [ endpoint ]
        let client = server.CreateClient()

        let! result = client.GetAsync("/api/flag/true")
        let! resultString = result.Content.ReadAsStringAsync()

        result.StatusCode |> shouldEqual HttpStatusCode.OK
        resultString |> shouldEqual "True"
    }

[<Fact>]
let ``routef: boolean parameter with false`` () =
    task {
        let endpoint =
            GET [ routef "api/flag/{%b}" (fun (flag: bool) -> text (string flag)) ]
        let server = WebApp.webApp [ endpoint ]
        let client = server.CreateClient()

        let! result = client.GetAsync("/api/flag/false")
        let! resultString = result.Content.ReadAsStringAsync()

        result.StatusCode |> shouldEqual HttpStatusCode.OK
        resultString |> shouldEqual "False"
    }

[<Fact>]
let ``routef: char parameter`` () =
    task {
        let endpoint =
            GET [ routef "api/char/{%c}" (fun (ch: char) -> text (string ch)) ]
        let server = WebApp.webApp [ endpoint ]
        let client = server.CreateClient()

        let! result = client.GetAsync("/api/char/a")
        let! resultString = result.Content.ReadAsStringAsync()

        result.StatusCode |> shouldEqual HttpStatusCode.OK
        resultString |> shouldEqual "a"
    }

[<Fact>]
let ``routef: int64 parameter`` () =
    task {
        let endpoint =
            GET [ routef "api/bignum/{%d}" (fun (num: int64) -> text (string num)) ]
        let server = WebApp.webApp [ endpoint ]
        let client = server.CreateClient()

        let! result = client.GetAsync("/api/bignum/9223372036854775807")
        let! resultString = result.Content.ReadAsStringAsync()

        result.StatusCode |> shouldEqual HttpStatusCode.OK
        resultString |> shouldEqual "9223372036854775807"
    }

[<Fact>]
let ``routef: float parameter`` () =
    task {
        let endpoint =
            GET [ routef "api/price/{%f}" (fun (price: float) -> text (string price)) ]
        let server = WebApp.webApp [ endpoint ]
        let client = server.CreateClient()

        let! result = client.GetAsync("/api/price/3.14159")
        let! resultString = result.Content.ReadAsStringAsync()

        result.StatusCode |> shouldEqual HttpStatusCode.OK
        resultString |> shouldEqual "3.14159"
    }

[<Fact>]
let ``routef: uint64 parameter`` () =
    task {
        let endpoint =
            GET [ routef "api/unsigned/{%u}" (fun (num: uint64) -> text (string num)) ]
        let server = WebApp.webApp [ endpoint ]
        let client = server.CreateClient()

        let! result = client.GetAsync("/api/unsigned/18446744073709551615")
        let! resultString = result.Content.ReadAsStringAsync()

        result.StatusCode |> shouldEqual HttpStatusCode.OK
        resultString |> shouldEqual "18446744073709551615"
    }

[<Fact>]
let ``routef: GUID parameter with guid constraint`` () =
    task {
        let guidValue = "123e4567-e89b-12d3-a456-426614174000"
        let endpoint =
            GET [ routef "api/user/{%O:guid}" (fun (id: Guid) -> text (string id)) ]
        let server = WebApp.webApp [ endpoint ]
        let client = server.CreateClient()

        let! result = client.GetAsync($"/api/user/{guidValue}")
        let! resultString = result.Content.ReadAsStringAsync()

        result.StatusCode |> shouldEqual HttpStatusCode.OK
        resultString |> shouldEqual guidValue
    }

[<Fact>]
let ``routef: multiple parameters of different types`` () =
    task {
        let endpoint =
            GET [ routef "api/{%s}/{%i}/{%b}" (fun (name: string) (id: int) (active: bool) ->
                text $"{name}-{id}-{active}") ]
        let server = WebApp.webApp [ endpoint ]
        let client = server.CreateClient()

        let! result = client.GetAsync("/api/test/42/true")
        let! resultString = result.Content.ReadAsStringAsync()

        result.StatusCode |> shouldEqual HttpStatusCode.OK
        resultString |> shouldEqual "test-42-True"
    }

[<Fact>]
let ``routef: catchall parameter with single asterisk`` () =
    task {
        let endpoint =
            GET [ routef "files/{*%s}" (fun (path: string) -> text path) ]
        let server = WebApp.webApp [ endpoint ]
        let client = server.CreateClient()

        let! result = client.GetAsync("/files/docs/readme.txt")
        let! resultString = result.Content.ReadAsStringAsync()

        result.StatusCode |> shouldEqual HttpStatusCode.OK
        resultString |> shouldEqual "docs/readme.txt"
    }

[<Fact>]
let ``routef: catchall parameter with double asterisk`` () =
    task {
        let endpoint =
            GET [ routef "files/{**%s}" (fun (path: string) -> text path) ]
        let server = WebApp.webApp [ endpoint ]
        let client = server.CreateClient()

        let! result = client.GetAsync("/files/docs/readme.txt")
        let! resultString = result.Content.ReadAsStringAsync()

        result.StatusCode |> shouldEqual HttpStatusCode.OK
        resultString |> shouldEqual "docs/readme.txt"
    }

[<Fact>]
let ``routef: zero value for integer`` () =
    task {
        let endpoint =
            GET [ routef "api/item/{%i}" (fun (id: int) -> text (string id)) ]
        let server = WebApp.webApp [ endpoint ]
        let client = server.CreateClient()

        let! result = client.GetAsync("/api/item/0")
        let! resultString = result.Content.ReadAsStringAsync()

        result.StatusCode |> shouldEqual HttpStatusCode.OK
        resultString |> shouldEqual "0"
    }

[<Fact>]
let ``routef: zero value for uint64`` () =
    task {
        let endpoint =
            GET [ routef "api/unsigned/{%u}" (fun (num: uint64) -> text (string num)) ]
        let server = WebApp.webApp [ endpoint ]
        let client = server.CreateClient()

        let! result = client.GetAsync("/api/unsigned/0")
        let! resultString = result.Content.ReadAsStringAsync()

        result.StatusCode |> shouldEqual HttpStatusCode.OK
        resultString |> shouldEqual "0"
    }

[<Fact>]
let ``routef: string with special characters`` () =
    task {
        let endpoint =
            GET [ routef "api/{%s}" (fun (value: string) -> text value) ]
        let server = WebApp.webApp [ endpoint ]
        let client = server.CreateClient()

        let! result = client.GetAsync("/api/test@example.com")
        let! resultString = result.Content.ReadAsStringAsync()

        result.StatusCode |> shouldEqual HttpStatusCode.OK
        resultString |> shouldEqual "test@example.com"
    }

[<Fact>]
let ``routef: negative float`` () =
    task {
        let endpoint =
            GET [ routef "api/temp/{%f}" (fun (temp: float) -> text (string temp)) ]
        let server = WebApp.webApp [ endpoint ]
        let client = server.CreateClient()

        let! result = client.GetAsync("/api/temp/-2.5")
        let! resultString = result.Content.ReadAsStringAsync()

        result.StatusCode |> shouldEqual HttpStatusCode.OK
        resultString |> shouldEqual "-2.5"
    }

[<Fact>]
let ``routef: negative int64`` () =
    task {
        let endpoint =
            GET [ routef "api/bignum/{%d}" (fun (num: int64) -> text (string num)) ]
        let server = WebApp.webApp [ endpoint ]
        let client = server.CreateClient()

        let! result = client.GetAsync("/api/bignum/-9223372036854775808")
        let! resultString = result.Content.ReadAsStringAsync()

        result.StatusCode |> shouldEqual HttpStatusCode.OK
        resultString |> shouldEqual "-9223372036854775808"
    }
