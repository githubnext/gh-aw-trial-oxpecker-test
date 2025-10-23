module Oxpecker.Tests.ModelValidation

open Oxpecker
open System.ComponentModel.DataAnnotations
open Xunit
open FsUnit.Light
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection

type Model = {
    Name: string | null
    [<Range(0, 100)>]
    Age: int
    [<Required>]
    [<MinLength(4)>]
    [<EmailAddress>]
    Email: string
    Active: bool
}

[<Fact>]
let ``Valid model doesn't raise any issues`` () =
    let model = {
        Name = "John"
        Age = 30
        Email = "my@email.com"
        Active = true
    }
    let result = validateModel model
    result.IsInvalid |> shouldEqual false
    match result with
    | ModelValidationResult.Valid x ->
        x |> shouldEqual model
        let modelState = ModelState.Valid model
        modelState.Value(_.Name) |> shouldEqual "John"
        modelState.BoolValue(_.Active) |> shouldEqual true
        modelState.Value(_.Age >> string) |> shouldEqual "30"
        modelState.Value(_.Email) |> shouldEqual "my@email.com"
    | ModelValidationResult.Invalid _ -> failwith "Expected valid model"


[<Fact>]
let ``Invalid model raises issues`` () =
    let model = {
        Name = null
        Age = 200
        Email = "abc"
        Active = false
    }
    let result = validateModel model
    result.IsValid |> shouldEqual false
    match result with
    | ModelValidationResult.Invalid(x, errors) ->
        x |> shouldEqual model
        errors.All |> shouldHaveLength 3
        errors.ErrorMessagesFor(nameof x.Age)
        |> Seq.head
        |> shouldEqual "The field Age must be between 0 and 100."
        errors.ErrorMessagesFor(nameof x.Email)
        |> Seq.toList
        |> shouldEqual [
            "The field Email must be a string or array type with a minimum length of '4'."
            "The Email field is not a valid e-mail address."
        ]
        errors.ErrorMessagesFor(nameof x.Name) |> shouldBeEmpty
    | _ -> failwith "Expected invalid model"

[<Fact>]
let ``Empty model returns default values`` () =
    let model = ModelState.Empty
    model.Value(_.Name) |> shouldEqual null
    model.BoolValue(_.Active) |> shouldEqual false

[<Fact>]
let ``Invalid ModelState.Value returns model field value`` () =
    let model = {
        Name = "Alice"
        Age = 200
        Email = "bad"
        Active = true
    }
    let result = validateModel model
    match result with
    | ModelValidationResult.Invalid(x, errors) ->
        let modelState = ModelState.Invalid(x, errors)
        modelState.Value(_.Name) |> shouldEqual "Alice"
        modelState.Value(_.Age >> string) |> shouldEqual "200"
        modelState.Value(_.Email) |> shouldEqual "bad"
    | _ -> failwith "Expected invalid model"

[<Fact>]
let ``Invalid ModelState.BoolValue returns model field value`` () =
    let model = {
        Name = "Bob"
        Age = 200
        Email = "abc"
        Active = true
    }
    let result = validateModel model
    match result with
    | ModelValidationResult.Invalid(x, errors) ->
        let modelState = ModelState.Invalid(x, errors)
        modelState.BoolValue(_.Active) |> shouldEqual true
    | _ -> failwith "Expected invalid model"

[<Fact>]
let ``ValidationErrors.ErrorMessagesFor returns empty for non-existent member`` () =
    let model = {
        Name = null
        Age = 200
        Email = "bad"
        Active = false
    }
    let result = validateModel model
    match result with
    | ModelValidationResult.Invalid(_, errors) ->
        errors.ErrorMessagesFor("NonExistentField") |> shouldBeEmpty
    | _ -> failwith "Expected invalid model"

[<Fact>]
let ``bindAndValidateJson with valid model`` () =
    task {
        let ctx = Microsoft.AspNetCore.Http.DefaultHttpContext()
        let json = """{"Name":"Test","Age":25,"Email":"test@test.com","Active":true}"""
        let stream = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(json))
        ctx.Request.Body <- stream
        ctx.Request.ContentType <- "application/json"
        ctx.Response.Body <- new System.IO.MemoryStream()

        let services = ServiceCollection()
        services.AddSingleton<IJsonSerializer>(fun sp -> SystemTextJsonSerializer() :> IJsonSerializer) |> ignore
        ctx.RequestServices <- services.BuildServiceProvider()

        let mutable result = None
        let testHandler (validationResult: ModelValidationResult<Model>) : EndpointHandler =
            fun (ctx: HttpContext) ->
                result <- Some validationResult
                ctx.Response.StatusCode <- 200
                System.Threading.Tasks.Task.CompletedTask

        let handler = bindAndValidateJson testHandler
        do! handler ctx

        match result with
        | Some (ModelValidationResult.Valid m) ->
            m.Name |> shouldEqual "Test"
            m.Age |> shouldEqual 25
        | _ -> failwith "Expected valid model"
    }

[<Fact>]
let ``bindAndValidateJson with invalid model`` () =
    task {
        let ctx = Microsoft.AspNetCore.Http.DefaultHttpContext()
        let json = """{"Name":"Test","Age":200,"Email":"bad","Active":true}"""
        let stream = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(json))
        ctx.Request.Body <- stream
        ctx.Request.ContentType <- "application/json"
        ctx.Response.Body <- new System.IO.MemoryStream()

        let services = ServiceCollection()
        services.AddSingleton<IJsonSerializer>(fun sp -> SystemTextJsonSerializer() :> IJsonSerializer) |> ignore
        ctx.RequestServices <- services.BuildServiceProvider()

        let mutable result = None
        let testHandler (validationResult: ModelValidationResult<Model>) : EndpointHandler =
            fun (ctx: HttpContext) ->
                result <- Some validationResult
                ctx.Response.StatusCode <- 200
                System.Threading.Tasks.Task.CompletedTask

        let handler = bindAndValidateJson testHandler
        do! handler ctx

        match result with
        | Some (ModelValidationResult.Invalid (m, errors)) ->
            errors.All |> Seq.length |> fun l -> l > 0 |> shouldEqual true
        | _ -> failwith "Expected invalid model"
    }

[<Fact>]
let ``bindAndValidateForm with valid model`` () =
    task {
        let ctx = Microsoft.AspNetCore.Http.DefaultHttpContext()
        ctx.Request.ContentType <- "application/x-www-form-urlencoded"
        ctx.Response.Body <- new System.IO.MemoryStream()

        let formFields = System.Collections.Generic.Dictionary<string, Microsoft.Extensions.Primitives.StringValues>()
        formFields.Add("Name", Microsoft.Extensions.Primitives.StringValues("FormTest"))
        formFields.Add("Age", Microsoft.Extensions.Primitives.StringValues("30"))
        formFields.Add("Email", Microsoft.Extensions.Primitives.StringValues("form@test.com"))
        formFields.Add("Active", Microsoft.Extensions.Primitives.StringValues("true"))
        ctx.Request.Form <- Microsoft.AspNetCore.Http.FormCollection(formFields)

        let services = ServiceCollection()
        services.AddLogging() |> ignore
        services.AddSingleton<IModelBinder>(fun sp -> ModelBinder(ModelBinderOptions.Default) :> IModelBinder) |> ignore
        ctx.RequestServices <- services.BuildServiceProvider()

        let mutable result = None
        let testHandler (validationResult: ModelValidationResult<Model>) : EndpointHandler =
            fun (ctx: HttpContext) ->
                result <- Some validationResult
                ctx.Response.StatusCode <- 200
                System.Threading.Tasks.Task.CompletedTask

        let handler = bindAndValidateForm testHandler
        do! handler ctx

        match result with
        | Some (ModelValidationResult.Valid m) ->
            m.Name |> shouldEqual "FormTest"
            m.Age |> shouldEqual 30
        | _ -> failwith "Expected valid model"
    }

[<Fact>]
let ``bindAndValidateForm with invalid model`` () =
    task {
        let ctx = Microsoft.AspNetCore.Http.DefaultHttpContext()
        ctx.Request.ContentType <- "application/x-www-form-urlencoded"
        ctx.Response.Body <- new System.IO.MemoryStream()

        let formFields = System.Collections.Generic.Dictionary<string, Microsoft.Extensions.Primitives.StringValues>()
        formFields.Add("Name", Microsoft.Extensions.Primitives.StringValues("Bad"))
        formFields.Add("Age", Microsoft.Extensions.Primitives.StringValues("200"))
        formFields.Add("Email", Microsoft.Extensions.Primitives.StringValues("bad"))
        formFields.Add("Active", Microsoft.Extensions.Primitives.StringValues("false"))
        ctx.Request.Form <- Microsoft.AspNetCore.Http.FormCollection(formFields)

        let services = ServiceCollection()
        services.AddLogging() |> ignore
        services.AddSingleton<IModelBinder>(fun sp -> ModelBinder(ModelBinderOptions.Default) :> IModelBinder) |> ignore
        ctx.RequestServices <- services.BuildServiceProvider()

        let mutable result = None
        let testHandler (validationResult: ModelValidationResult<Model>) : EndpointHandler =
            fun (ctx: HttpContext) ->
                result <- Some validationResult
                ctx.Response.StatusCode <- 200
                System.Threading.Tasks.Task.CompletedTask

        let handler = bindAndValidateForm testHandler
        do! handler ctx

        match result with
        | Some (ModelValidationResult.Invalid (m, errors)) ->
            errors.All |> Seq.length |> fun l -> l > 0 |> shouldEqual true
        | _ -> failwith "Expected invalid model"
    }

[<Fact>]
let ``bindAndValidateQuery with valid model`` () =
    task {
        let ctx = Microsoft.AspNetCore.Http.DefaultHttpContext()
        ctx.Request.QueryString <- Microsoft.AspNetCore.Http.QueryString("?Name=QueryTest&Age=25&Email=query@test.com&Active=true")
        ctx.Response.Body <- new System.IO.MemoryStream()

        let services = ServiceCollection()
        services.AddLogging() |> ignore
        services.AddSingleton<IModelBinder>(fun sp -> ModelBinder(ModelBinderOptions.Default) :> IModelBinder) |> ignore
        ctx.RequestServices <- services.BuildServiceProvider()

        let mutable result = None
        let testHandler (validationResult: ModelValidationResult<Model>) : EndpointHandler =
            fun (ctx: HttpContext) ->
                result <- Some validationResult
                ctx.Response.StatusCode <- 200
                System.Threading.Tasks.Task.CompletedTask

        let handler = bindAndValidateQuery testHandler
        do! handler ctx

        match result with
        | Some (ModelValidationResult.Valid m) ->
            m.Name |> shouldEqual "QueryTest"
            m.Age |> shouldEqual 25
        | _ -> failwith "Expected valid model"
    }

[<Fact>]
let ``bindAndValidateQuery with invalid model`` () =
    task {
        let ctx = Microsoft.AspNetCore.Http.DefaultHttpContext()
        ctx.Request.QueryString <- Microsoft.AspNetCore.Http.QueryString("?Name=Bad&Age=200&Email=bad&Active=false")
        ctx.Response.Body <- new System.IO.MemoryStream()

        let services = ServiceCollection()
        services.AddLogging() |> ignore
        services.AddSingleton<IModelBinder>(fun sp -> ModelBinder(ModelBinderOptions.Default) :> IModelBinder) |> ignore
        ctx.RequestServices <- services.BuildServiceProvider()

        let mutable result = None
        let testHandler (validationResult: ModelValidationResult<Model>) : EndpointHandler =
            fun (ctx: HttpContext) ->
                result <- Some validationResult
                ctx.Response.StatusCode <- 200
                System.Threading.Tasks.Task.CompletedTask

        let handler = bindAndValidateQuery testHandler
        do! handler ctx

        match result with
        | Some (ModelValidationResult.Invalid (m, errors)) ->
            errors.All |> Seq.length |> fun l -> l > 0 |> shouldEqual true
        | _ -> failwith "Expected invalid model"
    }

[<Fact>]
let ``HttpContextExtensions.BindAndValidateJson with valid model`` () =
    task {
        let ctx = Microsoft.AspNetCore.Http.DefaultHttpContext()
        ctx.Request.ContentType <- "application/json"
        let json = """{"Name":"ExtTest","Age":35,"Email":"ext@test.com","Active":true}"""
        ctx.Request.Body <- new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(json))
        let services = ServiceCollection()
        services.AddSingleton<IJsonSerializer>(fun sp -> SystemTextJsonSerializer() :> IJsonSerializer) |> ignore
        ctx.RequestServices <- services.BuildServiceProvider()

        let! result = ctx.BindAndValidateJson<Model>()
        match result with
        | ModelValidationResult.Valid m ->
            m.Name |> shouldEqual "ExtTest"
            m.Age |> shouldEqual 35
        | _ -> failwith "Expected valid model"
    }

[<Fact>]
let ``HttpContextExtensions.BindAndValidateForm with valid model`` () =
    task {
        let ctx = Microsoft.AspNetCore.Http.DefaultHttpContext()
        ctx.Request.ContentType <- "application/x-www-form-urlencoded"
        let formFields = System.Collections.Generic.Dictionary<string, Microsoft.Extensions.Primitives.StringValues>()
        formFields.Add("Name", Microsoft.Extensions.Primitives.StringValues("ExtForm"))
        formFields.Add("Age", Microsoft.Extensions.Primitives.StringValues("40"))
        formFields.Add("Email", Microsoft.Extensions.Primitives.StringValues("form@ext.com"))
        formFields.Add("Active", Microsoft.Extensions.Primitives.StringValues("true"))
        ctx.Request.Form <- Microsoft.AspNetCore.Http.FormCollection(formFields)
        let services = ServiceCollection()
        services.AddLogging() |> ignore
        services.AddSingleton<IModelBinder>(fun sp -> ModelBinder(ModelBinderOptions.Default) :> IModelBinder) |> ignore
        ctx.RequestServices <- services.BuildServiceProvider()

        let! result = ctx.BindAndValidateForm<Model>()
        match result with
        | ModelValidationResult.Valid m ->
            m.Name |> shouldEqual "ExtForm"
            m.Age |> shouldEqual 40
        | _ -> failwith "Expected valid model"
    }

[<Fact>]
let ``HttpContextExtensions.BindAndValidateQuery with valid model`` () =
    let ctx = Microsoft.AspNetCore.Http.DefaultHttpContext()
    ctx.Request.QueryString <- Microsoft.AspNetCore.Http.QueryString("?Name=ExtQuery&Age=45&Email=query@ext.com&Active=false")
    let services = ServiceCollection()
    services.AddLogging() |> ignore
    services.AddSingleton<IModelBinder>(fun sp -> ModelBinder(ModelBinderOptions.Default) :> IModelBinder) |> ignore
    ctx.RequestServices <- services.BuildServiceProvider()

    let result = ctx.BindAndValidateQuery<Model>()
    match result with
    | ModelValidationResult.Valid m ->
        m.Name |> shouldEqual "ExtQuery"
        m.Age |> shouldEqual 45
        m.Active |> shouldEqual false
    | _ -> failwith "Expected valid model"
