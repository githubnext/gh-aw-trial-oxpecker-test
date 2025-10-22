module Oxpecker.Tests.Middleware

open System.IO
open System.Threading.Tasks
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open Oxpecker
open Xunit
open FsUnit.Light

[<Fact>]
let ``UseOxpecker with endpoints seq registers endpoints`` () =
    task {
        let services = ServiceCollection()
        services.AddRouting() |> ignore
        let provider = services.BuildServiceProvider()

        let app = ApplicationBuilder(provider)
        app.UseRouting() |> ignore
        let endpoint1 = route "/" (text "Hello")
        let endpoint2 = route "/test" (text "Test")
        let endpoints = [ endpoint1; endpoint2 ]

        let result = app.UseOxpecker(endpoints)

        result |> shouldNotEqual null
    }

[<Fact>]
let ``UseOxpecker with single endpoint registers endpoint`` () =
    task {
        let services = ServiceCollection()
        services.AddRouting() |> ignore
        let provider = services.BuildServiceProvider()

        let app = ApplicationBuilder(provider)
        app.UseRouting() |> ignore
        let endpoint = route "/" (text "Hello")

        let result = app.UseOxpecker(endpoint)

        result |> shouldNotEqual null
    }

[<Fact>]
let ``AddOxpecker registers IJsonSerializer`` () =
    let services = ServiceCollection()
    services.AddLogging() |> ignore
    services.AddSingleton<IWebHostEnvironment>(fun _ ->
        { new IWebHostEnvironment with
            member _.ApplicationName
                with get () = "TestApp"
                and set _ = ()
            member _.ContentRootFileProvider
                with get () = null
                and set _ = ()
            member _.ContentRootPath
                with get () = ""
                and set _ = ()
            member _.EnvironmentName
                with get () = "Test"
                and set _ = ()
            member _.WebRootFileProvider
                with get () = null
                and set _ = ()
            member _.WebRootPath
                with get () = ""
                and set _ = ()
        })
    |> ignore

    services.AddOxpecker() |> ignore
    let provider = services.BuildServiceProvider()

    let serializer = provider.GetService<IJsonSerializer>()

    serializer |> shouldNotEqual null

[<Fact>]
let ``AddOxpecker registers IModelBinder`` () =
    let services = ServiceCollection()
    services.AddLogging() |> ignore
    services.AddSingleton<IWebHostEnvironment>(fun _ ->
        { new IWebHostEnvironment with
            member _.ApplicationName
                with get () = "TestApp"
                and set _ = ()
            member _.ContentRootFileProvider
                with get () = null
                and set _ = ()
            member _.ContentRootPath
                with get () = ""
                and set _ = ()
            member _.EnvironmentName
                with get () = "Test"
                and set _ = ()
            member _.WebRootFileProvider
                with get () = null
                and set _ = ()
            member _.WebRootPath
                with get () = ""
                and set _ = ()
        })
    |> ignore

    services.AddOxpecker() |> ignore
    let provider = services.BuildServiceProvider()

    let binder = provider.GetService<IModelBinder>()

    binder |> shouldNotEqual null

[<Fact>]
let ``AddOxpecker registers ILogger`` () =
    let services = ServiceCollection()
    services.AddLogging() |> ignore
    services.AddSingleton<IWebHostEnvironment>(fun _ ->
        { new IWebHostEnvironment with
            member _.ApplicationName
                with get () = "TestApp"
                and set _ = ()
            member _.ContentRootFileProvider
                with get () = null
                and set _ = ()
            member _.ContentRootPath
                with get () = ""
                and set _ = ()
            member _.EnvironmentName
                with get () = "Test"
                and set _ = ()
            member _.WebRootFileProvider
                with get () = null
                and set _ = ()
            member _.WebRootPath
                with get () = ""
                and set _ = ()
        })
    |> ignore

    services.AddOxpecker() |> ignore
    let provider = services.BuildServiceProvider()

    let logger = provider.GetService<ILogger>()

    logger |> shouldNotEqual null

[<Fact>]
let ``AddOxpecker returns IServiceCollection`` () =
    let services = ServiceCollection()
    services.AddLogging() |> ignore
    services.AddSingleton<IWebHostEnvironment>(fun _ ->
        { new IWebHostEnvironment with
            member _.ApplicationName
                with get () = "TestApp"
                and set _ = ()
            member _.ContentRootFileProvider
                with get () = null
                and set _ = ()
            member _.ContentRootPath
                with get () = ""
                and set _ = ()
            member _.EnvironmentName
                with get () = "Test"
                and set _ = ()
            member _.WebRootFileProvider
                with get () = null
                and set _ = ()
            member _.WebRootPath
                with get () = ""
                and set _ = ()
        })
    |> ignore

    let result = services.AddOxpecker()

    result |> shouldEqual services

[<Fact>]
let ``AddOxpecker does not replace existing IJsonSerializer`` () =
    let customSerializer =
        { new IJsonSerializer with
            member _.Serialize(_, _, _) = Task.CompletedTask
            member _.Deserialize(_) = Task.FromResult(Unchecked.defaultof<_>)
        }

    let services = ServiceCollection()
    services.AddLogging() |> ignore
    services.AddSingleton<IWebHostEnvironment>(fun _ ->
        { new IWebHostEnvironment with
            member _.ApplicationName
                with get () = "TestApp"
                and set _ = ()
            member _.ContentRootFileProvider
                with get () = null
                and set _ = ()
            member _.ContentRootPath
                with get () = ""
                and set _ = ()
            member _.EnvironmentName
                with get () = "Test"
                and set _ = ()
            member _.WebRootFileProvider
                with get () = null
                and set _ = ()
            member _.WebRootPath
                with get () = ""
                and set _ = ()
        })
    |> ignore
    services.AddSingleton<IJsonSerializer>(customSerializer) |> ignore

    services.AddOxpecker() |> ignore
    let provider = services.BuildServiceProvider()

    let serializer = provider.GetService<IJsonSerializer>()

    serializer |> shouldEqual customSerializer

[<Fact>]
let ``AddOxpecker does not replace existing IModelBinder`` () =
    let customBinder =
        { new IModelBinder with
            member _.Bind(_) = Unchecked.defaultof<_>
        }

    let services = ServiceCollection()
    services.AddLogging() |> ignore
    services.AddSingleton<IWebHostEnvironment>(fun _ ->
        { new IWebHostEnvironment with
            member _.ApplicationName
                with get () = "TestApp"
                and set _ = ()
            member _.ContentRootFileProvider
                with get () = null
                and set _ = ()
            member _.ContentRootPath
                with get () = ""
                and set _ = ()
            member _.EnvironmentName
                with get () = "Test"
                and set _ = ()
            member _.WebRootFileProvider
                with get () = null
                and set _ = ()
            member _.WebRootPath
                with get () = ""
                and set _ = ()
        })
    |> ignore
    services.AddSingleton<IModelBinder>(customBinder) |> ignore

    services.AddOxpecker() |> ignore
    let provider = services.BuildServiceProvider()

    let binder = provider.GetService<IModelBinder>()

    binder |> shouldEqual customBinder
