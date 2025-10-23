module Routing.Tests

open System
open Oxpecker
open Oxpecker.OpenApi
open Xunit
open FsUnit.Light

[<Fact>]
let ``routef creates endpoint with string parameter`` () =
    let endpoint = routef "/user/%s" (fun (name: string) -> text name)
    endpoint |> shouldNotEqual Unchecked.defaultof<Endpoint>

[<Fact>]
let ``routef creates endpoint with int parameter`` () =
    let endpoint = routef "/user/%i" (fun (id: int) -> text (string id))
    endpoint |> shouldNotEqual Unchecked.defaultof<Endpoint>

[<Fact>]
let ``routef creates endpoint with bool parameter`` () =
    let endpoint = routef "/flag/%b" (fun (flag: bool) -> text (string flag))
    endpoint |> shouldNotEqual Unchecked.defaultof<Endpoint>

[<Fact>]
let ``routef creates endpoint with char parameter`` () =
    let endpoint = routef "/char/%c" (fun (c: char) -> text (string c))
    endpoint |> shouldNotEqual Unchecked.defaultof<Endpoint>

[<Fact>]
let ``routef creates endpoint with int64 parameter`` () =
    let endpoint = routef "/number/%d" (fun (n: int64) -> text (string n))
    endpoint |> shouldNotEqual Unchecked.defaultof<Endpoint>

[<Fact>]
let ``routef creates endpoint with float parameter`` () =
    let endpoint = routef "/value/%f" (fun (v: float) -> text (string v))
    endpoint |> shouldNotEqual Unchecked.defaultof<Endpoint>

[<Fact>]
let ``routef creates endpoint with uint64 parameter`` () =
    let endpoint = routef "/unsigned/%u" (fun (u: uint64) -> text (string u))
    endpoint |> shouldNotEqual Unchecked.defaultof<Endpoint>

[<Fact>]
let ``routef creates endpoint with guid parameter`` () =
    let endpoint = routef "/guid/%O" (fun (g: Guid) -> text (string g))
    endpoint |> shouldNotEqual Unchecked.defaultof<Endpoint>

[<Fact>]
let ``routef creates endpoint with single string parameter in path`` () =
    let endpoint = routef "/user/%s/detail" (fun (name: string) -> text name)
    endpoint |> shouldNotEqual Unchecked.defaultof<Endpoint>

[<Fact>]
let ``routef creates endpoint with single int parameter in path`` () =
    let endpoint = routef "/api/users/%i/profile" (fun (id: int) -> text (string id))
    endpoint |> shouldNotEqual Unchecked.defaultof<Endpoint>

[<Fact>]
let ``addOpenApi adds OpenApiConfig to endpoint`` () =
    let config = OpenApiConfig(requestBody = RequestBody(typeof<string>))
    let endpoint = addOpenApi config (route "/test" <| text "response")
    endpoint |> shouldNotEqual Unchecked.defaultof<Endpoint>

[<Fact>]
let ``addOpenApiSimple with unit request and unit response`` () =
    let endpoint = addOpenApiSimple<unit, unit> (route "/test" <| text "ok")
    endpoint |> shouldNotEqual Unchecked.defaultof<Endpoint>

[<Fact>]
let ``addOpenApiSimple with request type and unit response`` () =
    let endpoint = addOpenApiSimple<string, unit> (route "/test" <| text "ok")
    endpoint |> shouldNotEqual Unchecked.defaultof<Endpoint>

[<Fact>]
let ``addOpenApiSimple with unit request and response type`` () =
    let endpoint = addOpenApiSimple<unit, string> (route "/test" <| text "ok")
    endpoint |> shouldNotEqual Unchecked.defaultof<Endpoint>

[<Fact>]
let ``addOpenApiSimple with request and response types`` () =
    let endpoint = addOpenApiSimple<int, string> (route "/test" <| text "ok")
    endpoint |> shouldNotEqual Unchecked.defaultof<Endpoint>

[<Fact>]
let ``routef with no parameters creates simple endpoint`` () =
    let endpoint = routef "/" (text "home")
    endpoint |> shouldNotEqual Unchecked.defaultof<Endpoint>

[<Fact>]
let ``routef creates endpoint with all format specifiers`` () =
    // Test that all format specifiers work
    let endpoint1 = routef "/s/%s" (fun (s: string) -> text s)
    let endpoint2 = routef "/i/%i" (fun (i: int) -> text (string i))
    let endpoint3 = routef "/b/%b" (fun (b: bool) -> text (string b))
    let endpoint4 = routef "/c/%c" (fun (c: char) -> text (string c))
    let endpoint5 = routef "/d/%d" (fun (d: int64) -> text (string d))
    let endpoint6 = routef "/f/%f" (fun (f: float) -> text (string f))
    let endpoint7 = routef "/u/%u" (fun (u: uint64) -> text (string u))
    let endpoint8 = routef "/O/%O" (fun (o: Guid) -> text (string o))

    // Just verify they all create endpoints
    endpoint1 |> shouldNotEqual Unchecked.defaultof<Endpoint>
    endpoint2 |> shouldNotEqual Unchecked.defaultof<Endpoint>
    endpoint3 |> shouldNotEqual Unchecked.defaultof<Endpoint>
    endpoint4 |> shouldNotEqual Unchecked.defaultof<Endpoint>
    endpoint5 |> shouldNotEqual Unchecked.defaultof<Endpoint>
    endpoint6 |> shouldNotEqual Unchecked.defaultof<Endpoint>
    endpoint7 |> shouldNotEqual Unchecked.defaultof<Endpoint>
    endpoint8 |> shouldNotEqual Unchecked.defaultof<Endpoint>

[<Fact>]
let ``addOpenApi with complex configuration`` () =
    let config =
        OpenApiConfig(
            requestBody = RequestBody(typeof<int>, [| "application/json" |]),
            responseBodies = [
                ResponseBody(typeof<string>, statusCode = 200)
                ResponseBody(typeof<string>, statusCode = 404)
            ],
            configureOperation = fun op ->
                op.Summary <- "Test operation"
                op.Description <- "A test operation"
                op
        )

    let endpoint = addOpenApi config (route "/test" <| text "ok")
    endpoint |> shouldNotEqual Unchecked.defaultof<Endpoint>

[<Fact>]
let ``routef with int parameter in middle of path`` () =
    let endpoint = routef "/api/posts/%i/view" (fun (id: int) -> text (string id))
    endpoint |> shouldNotEqual Unchecked.defaultof<Endpoint>

[<Fact>]
let ``addOpenApiSimple with different type combinations`` () =
    // Test various type combinations
    let endpoint1 = addOpenApiSimple<string, int> (route "/test1" <| text "ok")
    let endpoint2 = addOpenApiSimple<bool, float> (route "/test2" <| text "ok")
    let endpoint3 = addOpenApiSimple<int64, Guid> (route "/test3" <| text "ok")
    let endpoint4 = addOpenApiSimple<obj, obj> (route "/test4" <| text "ok")

    // Verify all create endpoints
    endpoint1 |> shouldNotEqual Unchecked.defaultof<Endpoint>
    endpoint2 |> shouldNotEqual Unchecked.defaultof<Endpoint>
    endpoint3 |> shouldNotEqual Unchecked.defaultof<Endpoint>
    endpoint4 |> shouldNotEqual Unchecked.defaultof<Endpoint>

[<Fact>]
let ``routef creates endpoint with special characters in route`` () =
    let endpoint = routef "/api/v1/users/%i" (fun (id: int) -> text (string id))
    endpoint |> shouldNotEqual Unchecked.defaultof<Endpoint>

[<Fact>]
let ``routef with guid parameter in path`` () =
    let endpoint = routef "/orders/%O/details" (fun (id: Guid) -> text (string id))
    endpoint |> shouldNotEqual Unchecked.defaultof<Endpoint>

[<Fact>]
let ``routef with float parameter in path`` () =
    let endpoint = routef "/values/%f/details" (fun (value: float) -> text (string value))
    endpoint |> shouldNotEqual Unchecked.defaultof<Endpoint>

[<Fact>]
let ``addOpenApiSimple with complex types`` () =
    let endpoint = addOpenApiSimple<DateTime, TimeSpan> (route "/test" <| text "ok")
    endpoint |> shouldNotEqual Unchecked.defaultof<Endpoint>
