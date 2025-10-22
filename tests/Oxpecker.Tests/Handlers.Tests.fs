module Oxpecker.Tests.Handlers

open System.IO
open System.Text
open System.Threading.Tasks
open Microsoft.AspNetCore.Http
open Oxpecker
open Oxpecker.ViewEngine
open Xunit
open FsUnit.Light

// Helper function for string containment checks
let shouldContainString (substring: string) (str: string) =
    str.Contains(substring) |> shouldEqual true

// ============================================================================
// Request Handlers Tests
// ============================================================================

// Note: bindJson, bindForm, bindQuery require DI setup with model binders and are tested in ModelParser.Tests.fs

// ============================================================================
// Response Handlers Tests
// ============================================================================

[<Fact>]
let ``redirectTo with permanent=false redirects with 302`` () =
    task {
        let ctx = DefaultHttpContext()
        ctx.Response.Body <- new MemoryStream()

        do! redirectTo "https://example.com" false ctx

        ctx.Response.StatusCode |> shouldEqual 302
        ctx.Response.Headers.Location.ToString() |> shouldEqual "https://example.com"
    }

[<Fact>]
let ``redirectTo with permanent=true redirects with 301`` () =
    task {
        let ctx = DefaultHttpContext()
        ctx.Response.Body <- new MemoryStream()

        do! redirectTo "https://example.com" true ctx

        ctx.Response.StatusCode |> shouldEqual 301
        ctx.Response.Headers.Location.ToString() |> shouldEqual "https://example.com"
    }

[<Fact>]
let ``bytes writes byte array to response`` () =
    task {
        let ctx = DefaultHttpContext()
        ctx.Response.Body <- new MemoryStream()

        let data = [| 1uy; 2uy; 3uy; 4uy; 5uy |]
        do! bytes data ctx

        ctx.Response.Body.Position <- 0L
        let buffer = Array.zeroCreate(int ctx.Response.Body.Length)
        let! _ = ctx.Response.Body.ReadAsync(buffer, 0, buffer.Length)

        buffer |> shouldEqual data
    }

[<Fact>]
let ``text writes UTF-8 string to response`` () =
    task {
        let ctx = DefaultHttpContext()
        ctx.Response.Body <- new MemoryStream()

        let content = "Hello, World!"
        do! text content ctx

        ctx.Response.Body.Position <- 0L
        use reader = new StreamReader(ctx.Response.Body)
        let! result = reader.ReadToEndAsync()

        result |> shouldEqual content
        ctx.Response.ContentType |> shouldEqual "text/plain; charset=utf-8"
    }

[<Fact>]
let ``text with empty string writes empty response`` () =
    task {
        let ctx = DefaultHttpContext()
        ctx.Response.Body <- new MemoryStream()

        do! text "" ctx

        ctx.Response.Body.Position <- 0L
        use reader = new StreamReader(ctx.Response.Body)
        let! result = reader.ReadToEndAsync()

        result |> shouldEqual ""
    }

// Note: json and jsonChunked require DI setup with IJsonSerializer, tested in Json.Tests.fs

[<Fact>]
let ``htmlString writes HTML string to response`` () =
    task {
        let ctx = DefaultHttpContext()
        ctx.Response.Body <- new MemoryStream()

        let html = "<html><body><h1>Hello</h1></body></html>"
        do! htmlString html ctx

        ctx.Response.Body.Position <- 0L
        use reader = new StreamReader(ctx.Response.Body)
        let! result = reader.ReadToEndAsync()

        result |> shouldEqual html
        ctx.Response.ContentType |> shouldEqual "text/html; charset=utf-8"
    }

[<Fact>]
let ``htmlView renders HtmlElement to response`` () =
    task {
        let ctx = DefaultHttpContext()
        ctx.Response.Body <- new MemoryStream()

        let view = html() { body() { h1() { "Hello World" } } }
        do! htmlView view ctx

        ctx.Response.Body.Position <- 0L
        use reader = new StreamReader(ctx.Response.Body)
        let! result = reader.ReadToEndAsync()

        result |> shouldContainString "<html>"
        result |> shouldContainString "<body>"
        result |> shouldContainString "<h1>"
        result |> shouldContainString "Hello World"
        ctx.Response.ContentType |> shouldEqual "text/html; charset=utf-8"
    }

[<Fact>]
let ``htmlView with nested elements renders correctly`` () =
    task {
        let ctx = DefaultHttpContext()
        ctx.Response.Body <- new MemoryStream()

        let view =
            html() {
                body() {
                    div() {
                        h1() { "Title" }
                        p() { "Content" }
                    }
                }
            }
        do! htmlView view ctx

        ctx.Response.Body.Position <- 0L
        use reader = new StreamReader(ctx.Response.Body)
        let! result = reader.ReadToEndAsync()

        result |> shouldContainString "<div>"
        result |> shouldContainString "Title"
        result |> shouldContainString "Content"
    }

// Note: htmlViewChunked tested separately in integration tests

[<Fact>]
let ``clearResponse clears the response`` () =
    task {
        let ctx = DefaultHttpContext()
        ctx.Response.Body <- new MemoryStream()
        ctx.Response.StatusCode <- 404
        ctx.Response.Headers.Add("X-Custom", "Value")

        do! clearResponse ctx

        ctx.Response.StatusCode |> shouldEqual 200
        ctx.Response.Headers.ContainsKey("X-Custom") |> shouldEqual false
    }

[<Fact>]
let ``setContentType sets Content-Type header`` () =
    task {
        let ctx = DefaultHttpContext()
        ctx.Response.Body <- new MemoryStream()

        do! setContentType "application/xml" ctx

        ctx.Response.ContentType |> shouldEqual "application/xml"
    }

[<Fact>]
let ``setContentType with custom mime type sets correctly`` () =
    task {
        let ctx = DefaultHttpContext()
        ctx.Response.Body <- new MemoryStream()

        do! setContentType "application/vnd.api+json" ctx

        ctx.Response.ContentType |> shouldEqual "application/vnd.api+json"
    }

[<Fact>]
let ``setStatusCode sets response status code`` () =
    task {
        let ctx = DefaultHttpContext()
        ctx.Response.Body <- new MemoryStream()

        do! setStatusCode 201 ctx

        ctx.Response.StatusCode |> shouldEqual 201
    }

[<Fact>]
let ``setStatusCode with 404 sets correctly`` () =
    task {
        let ctx = DefaultHttpContext()
        ctx.Response.Body <- new MemoryStream()

        do! setStatusCode 404 ctx

        ctx.Response.StatusCode |> shouldEqual 404
    }

[<Fact>]
let ``setStatusCode with 500 sets correctly`` () =
    task {
        let ctx = DefaultHttpContext()
        ctx.Response.Body <- new MemoryStream()

        do! setStatusCode 500 ctx

        ctx.Response.StatusCode |> shouldEqual 500
    }

[<Fact>]
let ``setHttpHeader adds custom header`` () =
    task {
        let ctx = DefaultHttpContext()
        ctx.Response.Body <- new MemoryStream()

        do! setHttpHeader "X-Custom-Header" "CustomValue" ctx

        ctx.Response.Headers.["X-Custom-Header"].ToString() |> shouldEqual "CustomValue"
    }

[<Fact>]
let ``setHttpHeader with multiple calls sets multiple headers`` () =
    task {
        let ctx = DefaultHttpContext()
        ctx.Response.Body <- new MemoryStream()

        do! setHttpHeader "X-Header-1" "Value1" ctx
        do! setHttpHeader "X-Header-2" "Value2" ctx

        ctx.Response.Headers.["X-Header-1"].ToString() |> shouldEqual "Value1"
        ctx.Response.Headers.["X-Header-2"].ToString() |> shouldEqual "Value2"
    }

[<Fact>]
let ``setHttpHeader overwrites existing header`` () =
    task {
        let ctx = DefaultHttpContext()
        ctx.Response.Body <- new MemoryStream()
        ctx.Response.Headers.Add("X-Test", "OldValue")

        do! setHttpHeader "X-Test" "NewValue" ctx

        ctx.Response.Headers.["X-Test"].ToString() |> shouldEqual "NewValue"
    }

// ============================================================================
// Handler Composition Tests
// ============================================================================

// Note: json handler requires DI setup, so composition tests are omitted

[<Fact>]
let ``compose setHttpHeader with htmlView handler`` () =
    task {
        let ctx = DefaultHttpContext()
        ctx.Response.Body <- new MemoryStream()

        let view = html() { body() { "Test" } }
        let handler = setHttpHeader "X-Custom" "Value" >=> htmlView view
        do! handler ctx

        ctx.Response.Headers.["X-Custom"].ToString() |> shouldEqual "Value"
        ctx.Response.ContentType |> shouldEqual "text/html; charset=utf-8"
    }

// Note: bindJson composition requires DI setup, tested in ModelParser.Tests.fs
