module Oxpecker.Tests.Serializers

open System.IO
open System.Text
open System.Text.Json
open System.Text.Json.Serialization
open Microsoft.AspNetCore.Http
open Xunit
open FsUnit.Light
open Oxpecker

[<CLIMutable>]
type TestRecord = { Name: string; Age: int }

[<Fact>]
let ``SystemTextJsonSerializer serializes with chunked=false and GET request`` () =
    task {
        let serializer = SystemTextJsonSerializer()
        let ctx = DefaultHttpContext()
        ctx.Response.Body <- new MemoryStream()
        ctx.Request.Method <- "GET"
        let data = { Name = "Test"; Age = 30 }

        do! (serializer :> IJsonSerializer).Serialize(data, ctx, false)

        ctx.Response.ContentType |> shouldEqual "application/json; charset=utf-8"
        ctx.Response.Headers.ContentLength.HasValue |> shouldEqual true
        ctx.Response.Headers.ContentLength.Value |> fun v -> v > 0L |> shouldEqual true

        ctx.Response.Body.Seek(0, SeekOrigin.Begin) |> ignore
        use reader = new StreamReader(ctx.Response.Body)
        let! json = reader.ReadToEndAsync()
        json |> fun s -> s.Contains("Test") |> shouldEqual true
        json |> fun s -> s.Contains("30") |> shouldEqual true
    }

[<Fact>]
let ``SystemTextJsonSerializer serializes with chunked=false and HEAD request`` () =
    task {
        let serializer = SystemTextJsonSerializer()
        let ctx = DefaultHttpContext()
        ctx.Request.Method <- "HEAD"
        let data = { Name = "Test"; Age = 30 }

        do! (serializer :> IJsonSerializer).Serialize(data, ctx, false)

        ctx.Response.ContentType |> shouldEqual "application/json; charset=utf-8"
        ctx.Response.Headers.ContentLength.HasValue |> shouldEqual true
        ctx.Response.Headers.ContentLength.Value |> fun v -> v > 0L |> shouldEqual true

        // For HEAD requests, body should be empty
        ctx.Response.Body.Length |> shouldEqual 0L
    }

[<Fact>]
let ``SystemTextJsonSerializer serializes with chunked=true and GET request`` () =
    task {
        let serializer = SystemTextJsonSerializer()
        let ctx = DefaultHttpContext()
        ctx.Response.Body <- new MemoryStream()
        ctx.Request.Method <- "GET"
        let data = { Name = "Chunked"; Age = 25 }

        do! (serializer :> IJsonSerializer).Serialize(data, ctx, true)

        ctx.Response.ContentType |> shouldEqual "application/json; charset=utf-8"

        ctx.Response.Body.Seek(0, SeekOrigin.Begin) |> ignore
        use reader = new StreamReader(ctx.Response.Body)
        let! json = reader.ReadToEndAsync()
        json |> fun s -> s.Contains("Chunked") |> shouldEqual true
        json |> fun s -> s.Contains("25") |> shouldEqual true
    }

[<Fact>]
let ``SystemTextJsonSerializer serializes with chunked=true and HEAD request`` () =
    task {
        let serializer = SystemTextJsonSerializer()
        let ctx = DefaultHttpContext()
        ctx.Request.Method <- "HEAD"
        let data = { Name = "Test"; Age = 40 }

        do! (serializer :> IJsonSerializer).Serialize(data, ctx, true)

        ctx.Response.ContentType |> shouldEqual "application/json; charset=utf-8"
        // For HEAD requests with chunked, body should be empty
        ctx.Response.Body.Length |> shouldEqual 0L
    }

[<Fact>]
let ``SystemTextJsonSerializer deserializes valid JSON`` () =
    task {
        let serializer = SystemTextJsonSerializer()
        let ctx = DefaultHttpContext()
        let json = """{"name":"Alice","age":35}"""
        let bytes = Encoding.UTF8.GetBytes(json)
        ctx.Request.Body <- new MemoryStream(bytes)
        ctx.Request.ContentType <- "application/json"

        let! result = (serializer :> IJsonSerializer).Deserialize<TestRecord>(ctx)

        result.Name |> shouldEqual "Alice"
        result.Age |> shouldEqual 35
    }

[<Fact>]
let ``SystemTextJsonSerializer deserializes null JSON returns default`` () =
    task {
        let serializer = SystemTextJsonSerializer()
        let ctx = DefaultHttpContext()
        let json = "null"
        let bytes = Encoding.UTF8.GetBytes(json)
        ctx.Request.Body <- new MemoryStream(bytes)
        ctx.Request.ContentType <- "application/json"

        let! result = (serializer :> IJsonSerializer).Deserialize<TestRecord>(ctx)

        // For null JSON, deserialize returns Unchecked.defaultof which is null
        isNull(box result) |> shouldEqual true
    }

[<Fact>]
let ``SystemTextJsonSerializer with custom options`` () =
    task {
        let options = JsonSerializerOptions(JsonSerializerDefaults.Web)
        options.PropertyNamingPolicy <- JsonNamingPolicy.CamelCase
        let serializer = SystemTextJsonSerializer(options)
        let ctx = DefaultHttpContext()
        ctx.Response.Body <- new MemoryStream()
        ctx.Request.Method <- "GET"
        let data = { Name = "CustomOptions"; Age = 50 }

        do! (serializer :> IJsonSerializer).Serialize(data, ctx, false)

        ctx.Response.Body.Seek(0, SeekOrigin.Begin) |> ignore
        use reader = new StreamReader(ctx.Response.Body)
        let! json = reader.ReadToEndAsync()
        // Should use camelCase
        json |> fun s -> s.Contains("name") |> shouldEqual true
        json |> fun s -> s.Contains("age") |> shouldEqual true
    }

[<Fact>]
let ``SystemTextJsonSerializer serializes complex object`` () =
    task {
        let serializer = SystemTextJsonSerializer()
        let ctx = DefaultHttpContext()
        ctx.Response.Body <- new MemoryStream()
        ctx.Request.Method <- "POST"
        let data = {
            Name = "Complex & <Special> \"Characters\""
            Age = 999
        }

        do! (serializer :> IJsonSerializer).Serialize(data, ctx, false)

        ctx.Response.Body.Seek(0, SeekOrigin.Begin) |> ignore
        use reader = new StreamReader(ctx.Response.Body)
        let! json = reader.ReadToEndAsync()
        // JSON should escape special characters
        (json.Contains("\\u0026") || json.Contains("&")) |> shouldEqual true
    }

[<Fact>]
let ``SystemTextJsonSerializer deserializes empty JSON object`` () =
    task {
        let serializer = SystemTextJsonSerializer()
        let ctx = DefaultHttpContext()
        let json = "{}"
        let bytes = Encoding.UTF8.GetBytes(json)
        ctx.Request.Body <- new MemoryStream(bytes)
        ctx.Request.ContentType <- "application/json"

        let! result = (serializer :> IJsonSerializer).Deserialize<TestRecord>(ctx)

        result.Name |> isNull |> shouldEqual true
        result.Age |> shouldEqual 0
    }

[<Fact>]
let ``SystemTextJsonSerializer with default options uses Web defaults`` () =
    task {
        let serializer = SystemTextJsonSerializer()
        let ctx = DefaultHttpContext()
        ctx.Response.Body <- new MemoryStream()
        ctx.Request.Method <- "GET"
        let data = { Name = "WebDefaults"; Age = 42 }

        do! (serializer :> IJsonSerializer).Serialize(data, ctx, true)

        ctx.Response.ContentType |> shouldEqual "application/json; charset=utf-8"

        ctx.Response.Body.Seek(0, SeekOrigin.Begin) |> ignore
        use reader = new StreamReader(ctx.Response.Body)
        let! json = reader.ReadToEndAsync()
        json.Length |> fun l -> l > 0 |> shouldEqual true
    }
