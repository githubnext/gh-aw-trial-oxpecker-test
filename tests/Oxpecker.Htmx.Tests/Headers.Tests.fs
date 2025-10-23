module Headers.Tests

open Oxpecker.Htmx
open Xunit
open FsUnit.Light

// Tests for HxRequestHeader module constants
[<Fact>]
let ``HxRequestHeader.Boosted has correct value`` () =
    HxRequestHeader.Boosted |> shouldEqual "HX-Boosted"

[<Fact>]
let ``HxRequestHeader.CurrentUrl has correct value`` () =
    HxRequestHeader.CurrentUrl |> shouldEqual "HX-Current-URL"

[<Fact>]
let ``HxRequestHeader.HistoryRestoreRequest has correct value`` () =
    HxRequestHeader.HistoryRestoreRequest
    |> shouldEqual "HX-History-Restore-Request"

[<Fact>]
let ``HxRequestHeader.Prompt has correct value`` () =
    HxRequestHeader.Prompt |> shouldEqual "HX-Prompt"

[<Fact>]
let ``HxRequestHeader.Request has correct value`` () =
    HxRequestHeader.Request |> shouldEqual "HX-Request"

[<Fact>]
let ``HxRequestHeader.Target has correct value`` () =
    HxRequestHeader.Target |> shouldEqual "HX-Target"

[<Fact>]
let ``HxRequestHeader.TriggerName has correct value`` () =
    HxRequestHeader.TriggerName |> shouldEqual "HX-Trigger-Name"

[<Fact>]
let ``HxRequestHeader.Trigger has correct value`` () =
    HxRequestHeader.Trigger |> shouldEqual "HX-Trigger"

// Tests for HxResponseHeader module constants
[<Fact>]
let ``HxResponseHeader.Location has correct value`` () =
    HxResponseHeader.Location |> shouldEqual "HX-Location"

[<Fact>]
let ``HxResponseHeader.PushUrl has correct value`` () =
    HxResponseHeader.PushUrl |> shouldEqual "HX-Push-Url"

[<Fact>]
let ``HxResponseHeader.Redirect has correct value`` () =
    HxResponseHeader.Redirect |> shouldEqual "HX-Redirect"

[<Fact>]
let ``HxResponseHeader.Refresh has correct value`` () =
    HxResponseHeader.Refresh |> shouldEqual "HX-Refresh"

[<Fact>]
let ``HxResponseHeader.ReplaceUrl has correct value`` () =
    HxResponseHeader.ReplaceUrl |> shouldEqual "HX-Replace-Url"

[<Fact>]
let ``HxResponseHeader.Reswap has correct value`` () =
    HxResponseHeader.Reswap |> shouldEqual "HX-Reswap"

[<Fact>]
let ``HxResponseHeader.Retarget has correct value`` () =
    HxResponseHeader.Retarget |> shouldEqual "HX-Retarget"

[<Fact>]
let ``HxResponseHeader.Trigger has correct value`` () =
    HxResponseHeader.Trigger |> shouldEqual "HX-Trigger"

[<Fact>]
let ``HxResponseHeader.TriggerAfterSettle has correct value`` () =
    HxResponseHeader.TriggerAfterSettle |> shouldEqual "HX-Trigger-After-Settle"

[<Fact>]
let ``HxResponseHeader.TriggerAfterSwap has correct value`` () =
    HxResponseHeader.TriggerAfterSwap |> shouldEqual "HX-Trigger-After-Swap"

// Integration-style tests to verify constants can be used properly
[<Fact>]
let ``All HxRequestHeader constants are unique`` () =
    let headers = [
        HxRequestHeader.Boosted
        HxRequestHeader.CurrentUrl
        HxRequestHeader.HistoryRestoreRequest
        HxRequestHeader.Prompt
        HxRequestHeader.Request
        HxRequestHeader.Target
        HxRequestHeader.TriggerName
        HxRequestHeader.Trigger
    ]
    headers |> List.distinct |> List.length |> shouldEqual(headers |> List.length)

[<Fact>]
let ``All HxResponseHeader constants are unique`` () =
    let headers = [
        HxResponseHeader.Location
        HxResponseHeader.PushUrl
        HxResponseHeader.Redirect
        HxResponseHeader.Refresh
        HxResponseHeader.ReplaceUrl
        HxResponseHeader.Reswap
        HxResponseHeader.Retarget
        HxResponseHeader.Trigger
        HxResponseHeader.TriggerAfterSettle
        HxResponseHeader.TriggerAfterSwap
    ]
    headers |> List.distinct |> List.length |> shouldEqual(headers |> List.length)

[<Fact>]
let ``All request header constants start with HX- prefix`` () =
    let headers = [
        HxRequestHeader.Boosted
        HxRequestHeader.CurrentUrl
        HxRequestHeader.HistoryRestoreRequest
        HxRequestHeader.Prompt
        HxRequestHeader.Request
        HxRequestHeader.Target
        HxRequestHeader.TriggerName
        HxRequestHeader.Trigger
    ]
    headers |> List.iter(fun h -> h.StartsWith("HX-") |> shouldEqual true)

[<Fact>]
let ``All response header constants start with HX- prefix`` () =
    let headers = [
        HxResponseHeader.Location
        HxResponseHeader.PushUrl
        HxResponseHeader.Redirect
        HxResponseHeader.Refresh
        HxResponseHeader.ReplaceUrl
        HxResponseHeader.Reswap
        HxResponseHeader.Retarget
        HxResponseHeader.Trigger
        HxResponseHeader.TriggerAfterSettle
        HxResponseHeader.TriggerAfterSwap
    ]
    headers |> List.iter(fun h -> h.StartsWith("HX-") |> shouldEqual true)

[<Fact>]
let ``Request and Response headers have expected Trigger overlap`` () =
    // Both modules define "HX-Trigger" but for different purposes
    HxRequestHeader.Trigger |> shouldEqual "HX-Trigger"
    HxResponseHeader.Trigger |> shouldEqual "HX-Trigger"
    HxRequestHeader.Trigger |> shouldEqual HxResponseHeader.Trigger
