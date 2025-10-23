module Configuration.Tests

open System
open Microsoft.AspNetCore.Http.Metadata
open Microsoft.OpenApi.Models
open Oxpecker.OpenApi
open Xunit
open FsUnit.Light

[<Fact>]
let ``RequestBody creates AcceptsMetadata with default values`` () =
    let requestBody = RequestBody(typeof<string>)
    let metadata = requestBody.ToAttribute()

    metadata.ContentTypes |> Seq.toList |> shouldEqual ["application/json"]
    metadata.RequestType |> shouldEqual typeof<string>
    metadata.IsOptional |> shouldEqual false

[<Fact>]
let ``RequestBody with custom content types`` () =
    let requestBody = RequestBody(typeof<int>, [| "application/xml"; "text/plain" |])
    let metadata = requestBody.ToAttribute()

    metadata.ContentTypes |> Seq.toList |> shouldEqual ["application/xml"; "text/plain"]

[<Fact>]
let ``RequestBody with optional flag set`` () =
    let requestBody = RequestBody(typeof<bool>, isOptional = true)
    let metadata = requestBody.ToAttribute()

    metadata.IsOptional |> shouldEqual true

[<Fact>]
let ``RequestBody with no type specified`` () =
    let requestBody = RequestBody()
    let metadata = requestBody.ToAttribute()

    metadata.RequestType |> shouldEqual null

[<Fact>]
let ``RequestBody with all parameters specified`` () =
    let requestBody = RequestBody(typeof<float>, [| "application/octet-stream" |], true)
    let metadata = requestBody.ToAttribute()

    metadata.RequestType |> shouldEqual typeof<float>
    metadata.ContentTypes |> Seq.toList |> shouldEqual ["application/octet-stream"]
    metadata.IsOptional |> shouldEqual true

[<Fact>]
let ``ResponseBody creates ProducesResponseTypeMetadata with default values`` () =
    let responseBody = ResponseBody(typeof<string>)
    let metadata = responseBody.ToAttribute()

    metadata.StatusCode |> shouldEqual 200
    metadata.Type |> shouldEqual typeof<string>

[<Fact>]
let ``ResponseBody with custom status code`` () =
    let responseBody = ResponseBody(typeof<int>, statusCode = 201)
    let metadata = responseBody.ToAttribute()

    metadata.StatusCode |> shouldEqual 201

[<Fact>]
let ``ResponseBody with custom content types`` () =
    let responseBody = ResponseBody(typeof<bool>, [| "text/xml" |])
    let metadata = responseBody.ToAttribute()

    metadata.ContentTypes |> Seq.toList |> shouldEqual ["text/xml"]

[<Fact>]
let ``ResponseBody with no type specified`` () =
    let responseBody = ResponseBody()
    let metadata = responseBody.ToAttribute()

    metadata.Type |> shouldEqual null
    metadata.StatusCode |> shouldEqual 200

[<Fact>]
let ``ResponseBody with all parameters`` () =
    let responseBody = ResponseBody(typeof<obj>, [| "application/json"; "application/xml" |], 404)
    let metadata = responseBody.ToAttribute()

    metadata.Type |> shouldEqual typeof<obj>
    metadata.ContentTypes |> Seq.toList |> shouldEqual ["application/json"; "application/xml"]
    metadata.StatusCode |> shouldEqual 404

[<Fact>]
let ``ResponseBody with various status codes`` () =
    let codes = [200; 201; 204; 400; 404; 500]
    for code in codes do
        let responseBody = ResponseBody(statusCode = code)
        let metadata = responseBody.ToAttribute()
        metadata.StatusCode |> shouldEqual code

[<Fact>]
let ``RequestBody with multiple content types`` () =
    let contentTypes = [| "application/json"; "application/xml"; "text/plain"; "application/octet-stream" |]
    let requestBody = RequestBody(typeof<string>, contentTypes)
    let metadata = requestBody.ToAttribute()

    metadata.ContentTypes |> Seq.toList |> shouldEqual (contentTypes |> Array.toList)

[<Fact>]
let ``ResponseBody with multiple content types`` () =
    let contentTypes = [| "text/html"; "text/plain"; "application/json" |]
    let responseBody = ResponseBody(typeof<string>, contentTypes, 200)
    let metadata = responseBody.ToAttribute()

    metadata.ContentTypes |> Seq.toList |> shouldEqual (contentTypes |> Array.toList)

[<Fact>]
let ``OpenApiConfig can be created with no parameters`` () =
    let config = OpenApiConfig()
    config |> ignore  // Just verify it can be created

[<Fact>]
let ``OpenApiConfig can be created with requestBody`` () =
    let requestBody = RequestBody(typeof<string>)
    let config = OpenApiConfig(requestBody = requestBody)
    config |> ignore  // Just verify it can be created

[<Fact>]
let ``OpenApiConfig can be created with responseBodies`` () =
    let responseBodies = [
        ResponseBody(typeof<string>, statusCode = 200)
        ResponseBody(typeof<int>, statusCode = 404)
    ]
    let config = OpenApiConfig(responseBodies = responseBodies)
    config |> ignore  // Just verify it can be created

[<Fact>]
let ``OpenApiConfig can be created with configureOperation`` () =
    let configureOp (op: OpenApiOperation) =
        op.Summary <- "Test summary"
        op
    let config = OpenApiConfig(configureOperation = configureOp)
    config |> ignore  // Just verify it can be created

[<Fact>]
let ``OpenApiConfig can be created with all parameters`` () =
    let requestBody = RequestBody(typeof<int>)
    let responseBodies = [
        ResponseBody(typeof<string>, statusCode = 200)
        ResponseBody(typeof<unit>, statusCode = 204)
    ]
    let configureOp (op: OpenApiOperation) =
        op.Description <- "Complex operation"
        op
    let config = OpenApiConfig(requestBody = requestBody, responseBodies = responseBodies, configureOperation = configureOp)
    config |> ignore  // Just verify it can be created
