module Attributes.Tests

open System.Text
open Oxpecker.ViewEngine
open Oxpecker.Htmx
open Xunit
open FsUnit.Light

// Helper to render an HtmlTag to string
let renderTag (tag: HtmlTag) =
    let sb = StringBuilder()
    tag.Render(sb)
    sb.ToString()

[<Fact>]
let ``hxGet sets hx-get attribute`` () =
    let node = Builder.RegularNode("button")
    node.hxGet <- "/api/data"
    renderTag node |> shouldEqual """<button hx-get="/api/data"></button>"""

[<Fact>]
let ``hxGet with null value`` () =
    let node = Builder.RegularNode("button")
    node.hxGet <- null
    renderTag node |> shouldEqual "<button></button>"

[<Fact>]
let ``hxPost sets hx-post attribute`` () =
    let node = Builder.RegularNode("form")
    node.hxPost <- "/api/submit"
    renderTag node |> shouldEqual """<form hx-post="/api/submit"></form>"""

[<Fact>]
let ``hxPost with null value`` () =
    let node = Builder.RegularNode("form")
    node.hxPost <- null
    renderTag node |> shouldEqual "<form></form>"

[<Fact>]
let ``hxOn sets hx-on event attribute`` () =
    let node = Builder.RegularNode("button")
    node.hxOn("click", "alert('clicked')") |> ignore
    renderTag node
    |> shouldEqual """<button hx-on:click="alert(&#39;clicked&#39;)"></button>"""

[<Fact>]
let ``hxOn with different event types`` () =
    let node = Builder.RegularNode("div")
    node.hxOn("mouseover", "console.log('hover')") |> ignore
    renderTag node
    |> shouldEqual """<div hx-on:mouseover="console.log(&#39;hover&#39;)"></div>"""

[<Fact>]
let ``hxPushUrl sets hx-push-url attribute`` () =
    let node = Builder.RegularNode("a")
    node.hxPushUrl <- "/page1"
    renderTag node |> shouldEqual """<a hx-push-url="/page1"></a>"""

[<Fact>]
let ``hxPushUrl with null value`` () =
    let node = Builder.RegularNode("a")
    node.hxPushUrl <- null
    renderTag node |> shouldEqual "<a></a>"

[<Fact>]
let ``hxSelect sets hx-select attribute`` () =
    let node = Builder.RegularNode("div")
    node.hxSelect <- "#content"
    renderTag node |> shouldEqual """<div hx-select="#content"></div>"""

[<Fact>]
let ``hxSelect with complex CSS selector`` () =
    let node = Builder.RegularNode("div")
    node.hxSelect <- ".container > .item:first-child"
    renderTag node
    |> shouldEqual """<div hx-select=".container &gt; .item:first-child"></div>"""

[<Fact>]
let ``hxSelectOob sets hx-select-oob attribute`` () =
    let node = Builder.RegularNode("div")
    node.hxSelectOob <- "#sidebar"
    renderTag node |> shouldEqual """<div hx-select-oob="#sidebar"></div>"""

[<Fact>]
let ``hxSwap sets hx-swap attribute`` () =
    let node = Builder.RegularNode("div")
    node.hxSwap <- "outerHTML"
    renderTag node |> shouldEqual """<div hx-swap="outerHTML"></div>"""

[<Fact>]
let ``hxSwap with different swap strategies`` () =
    let strategies = [
        "innerHTML"
        "outerHTML"
        "beforebegin"
        "afterbegin"
        "beforeend"
        "afterend"
    ]
    for strategy in strategies do
        let node = Builder.RegularNode("div")
        node.hxSwap <- strategy
        renderTag node |> shouldEqual $"""<div hx-swap="{strategy}"></div>"""

[<Fact>]
let ``hxSwapOob sets hx-swap-oob attribute`` () =
    let node = Builder.RegularNode("div")
    node.hxSwapOob <- "true"
    renderTag node |> shouldEqual """<div hx-swap-oob="true"></div>"""

[<Fact>]
let ``hxTarget sets hx-target attribute`` () =
    let node = Builder.RegularNode("button")
    node.hxTarget <- "#result"
    renderTag node |> shouldEqual """<button hx-target="#result"></button>"""

[<Fact>]
let ``hxTarget with complex selector`` () =
    let node = Builder.RegularNode("button")
    node.hxTarget <- "closest .parent"
    renderTag node
    |> shouldEqual """<button hx-target="closest .parent"></button>"""

[<Fact>]
let ``hxTrigger sets hx-trigger attribute`` () =
    let node = Builder.VoidNode("input")
    node.hxTrigger <- "keyup changed delay:500ms"
    renderTag node
    |> shouldEqual """<input hx-trigger="keyup changed delay:500ms">"""

[<Fact>]
let ``hxTrigger with various event modifiers`` () =
    let node = Builder.RegularNode("div")
    node.hxTrigger <- "click once"
    renderTag node |> shouldEqual """<div hx-trigger="click once"></div>"""

[<Fact>]
let ``hxVals sets hx-vals attribute`` () =
    let node = Builder.RegularNode("form")
    node.hxVals <- """{"key": "value"}"""
    renderTag node
    |> shouldEqual """<form hx-vals="{&quot;key&quot;: &quot;value&quot;}"></form>"""

[<Fact>]
let ``hxBoost sets hx-boost attribute to true`` () =
    let node = Builder.RegularNode("a")
    node.hxBoost <- true
    renderTag node |> shouldEqual """<a hx-boost="true"></a>"""

[<Fact>]
let ``hxBoost sets hx-boost attribute to false`` () =
    let node = Builder.RegularNode("a")
    node.hxBoost <- false
    renderTag node |> shouldEqual """<a hx-boost="false"></a>"""

[<Fact>]
let ``hxConfirm sets hx-confirm attribute`` () =
    let node = Builder.RegularNode("button")
    node.hxConfirm <- "Are you sure?"
    renderTag node |> shouldEqual """<button hx-confirm="Are you sure?"></button>"""

[<Fact>]
let ``hxDelete sets hx-delete attribute`` () =
    let node = Builder.RegularNode("button")
    node.hxDelete <- "/api/items/1"
    renderTag node |> shouldEqual """<button hx-delete="/api/items/1"></button>"""

[<Fact>]
let ``hxDisable sets hx-disable attribute when true`` () =
    let node = Builder.RegularNode("div")
    node.hxDisable <- true
    renderTag node |> shouldEqual """<div hx-disable="true"></div>"""

[<Fact>]
let ``hxDisable does not set attribute when false`` () =
    let node = Builder.RegularNode("div")
    node.hxDisable <- false
    renderTag node |> shouldEqual "<div></div>"

[<Fact>]
let ``hxDisabledElt sets hx-disabled-elt attribute`` () =
    let node = Builder.RegularNode("button")
    node.hxDisabledElt <- "#submit-btn"
    renderTag node
    |> shouldEqual """<button hx-disabled-elt="#submit-btn"></button>"""

[<Fact>]
let ``hxDisinherit sets hx-disinherit attribute`` () =
    let node = Builder.RegularNode("div")
    node.hxDisinherit <- "hx-select hx-get"
    renderTag node |> shouldEqual """<div hx-disinherit="hx-select hx-get"></div>"""

[<Fact>]
let ``hxEncoding sets hx-encoding attribute`` () =
    let node = Builder.RegularNode("form")
    node.hxEncoding <- "multipart/form-data"
    renderTag node
    |> shouldEqual """<form hx-encoding="multipart/form-data"></form>"""

[<Fact>]
let ``hxExt sets hx-ext attribute`` () =
    let node = Builder.RegularNode("div")
    node.hxExt <- "json-enc"
    renderTag node |> shouldEqual """<div hx-ext="json-enc"></div>"""

[<Fact>]
let ``hxHeaders sets hx-headers attribute`` () =
    let node = Builder.RegularNode("button")
    node.hxHeaders <- """{"X-Custom": "value"}"""
    renderTag node
    |> shouldEqual """<button hx-headers="{&quot;X-Custom&quot;: &quot;value&quot;}"></button>"""

[<Fact>]
let ``hxHistory sets hx-history attribute to true`` () =
    let node = Builder.RegularNode("div")
    node.hxHistory <- true
    renderTag node |> shouldEqual """<div hx-history="true"></div>"""

[<Fact>]
let ``hxHistory sets hx-history attribute to false`` () =
    let node = Builder.RegularNode("div")
    node.hxHistory <- false
    renderTag node |> shouldEqual """<div hx-history="false"></div>"""

[<Fact>]
let ``hxHistoryElt sets hx-history-elt attribute when true`` () =
    let node = Builder.RegularNode("div")
    node.hxHistoryElt <- true
    renderTag node |> shouldEqual """<div hx-history-elt=""></div>"""

[<Fact>]
let ``hxHistoryElt does not set attribute when false`` () =
    let node = Builder.RegularNode("div")
    node.hxHistoryElt <- false
    renderTag node |> shouldEqual "<div></div>"

[<Fact>]
let ``hxInclude sets hx-include attribute`` () =
    let node = Builder.RegularNode("button")
    node.hxInclude <- "[name='email']"
    renderTag node
    |> shouldEqual """<button hx-include="[name=&#39;email&#39;]"></button>"""

[<Fact>]
let ``hxIndicator sets hx-indicator attribute`` () =
    let node = Builder.RegularNode("button")
    node.hxIndicator <- "#spinner"
    renderTag node |> shouldEqual """<button hx-indicator="#spinner"></button>"""

[<Fact>]
let ``hxParams sets hx-params attribute`` () =
    let node = Builder.RegularNode("form")
    node.hxParams <- "not password"
    renderTag node |> shouldEqual """<form hx-params="not password"></form>"""

[<Fact>]
let ``hxPatch sets hx-patch attribute`` () =
    let node = Builder.RegularNode("button")
    node.hxPatch <- "/api/items/1"
    renderTag node |> shouldEqual """<button hx-patch="/api/items/1"></button>"""

[<Fact>]
let ``hxPreserve sets hx-preserve attribute when true`` () =
    let node = Builder.RegularNode("div")
    node.hxPreserve <- true
    renderTag node |> shouldEqual """<div hx-preserve=""></div>"""

[<Fact>]
let ``hxPreserve does not set attribute when false`` () =
    let node = Builder.RegularNode("div")
    node.hxPreserve <- false
    renderTag node |> shouldEqual "<div></div>"

[<Fact>]
let ``hxPrompt sets hx-prompt attribute`` () =
    let node = Builder.RegularNode("button")
    node.hxPrompt <- "Enter your name"
    renderTag node
    |> shouldEqual """<button hx-prompt="Enter your name"></button>"""

[<Fact>]
let ``hxPut sets hx-put attribute`` () =
    let node = Builder.RegularNode("button")
    node.hxPut <- "/api/items/1"
    renderTag node |> shouldEqual """<button hx-put="/api/items/1"></button>"""

[<Fact>]
let ``hxReplaceUrl sets hx-replace-url attribute`` () =
    let node = Builder.RegularNode("div")
    node.hxReplaceUrl <- "/new-url"
    renderTag node |> shouldEqual """<div hx-replace-url="/new-url"></div>"""

[<Fact>]
let ``hxRequest sets hx-request attribute`` () =
    let node = Builder.RegularNode("button")
    node.hxRequest <- """{"timeout": 5000}"""
    renderTag node
    |> shouldEqual """<button hx-request="{&quot;timeout&quot;: 5000}"></button>"""

[<Fact>]
let ``hxSync sets hx-sync attribute`` () =
    let node = Builder.RegularNode("button")
    node.hxSync <- "this:replace"
    renderTag node |> shouldEqual """<button hx-sync="this:replace"></button>"""

[<Fact>]
let ``hxValidate sets hx-validate attribute to true`` () =
    let node = Builder.VoidNode("input")
    node.hxValidate <- true
    renderTag node |> shouldEqual """<input hx-validate="true">"""

[<Fact>]
let ``hxValidate sets hx-validate attribute to false`` () =
    let node = Builder.VoidNode("input")
    node.hxValidate <- false
    renderTag node |> shouldEqual """<input hx-validate="false">"""

[<Fact>]
let ``Multiple HTMX attributes can be combined`` () =
    let node = Builder.RegularNode("button")
    node.hxGet <- "/api/data"
    node.hxTarget <- "#result"
    node.hxSwap <- "innerHTML"
    node.hxTrigger <- "click"
    renderTag node
    |> shouldEqual """<button hx-get="/api/data" hx-target="#result" hx-swap="innerHTML" hx-trigger="click"></button>"""

[<Fact>]
let ``HTMX attributes work on VoidNode elements`` () =
    let node = Builder.VoidNode("input")
    node.hxGet <- "/api/search"
    node.hxTrigger <- "keyup changed delay:500ms"
    node.hxTarget <- "#results"
    renderTag node
    |> shouldEqual """<input hx-get="/api/search" hx-trigger="keyup changed delay:500ms" hx-target="#results">"""

[<Fact>]
let ``HTMX attributes are properly HTML escaped`` () =
    let node = Builder.RegularNode("button")
    node.hxConfirm <- """Are you "really" sure? <script>alert('xss')</script>"""
    renderTag node
    |> shouldEqual
        """<button hx-confirm="Are you &quot;really&quot; sure? &lt;script&gt;alert(&#39;xss&#39;)&lt;/script&gt;"></button>"""

[<Fact>]
let ``hxOn with multiple events on same element`` () =
    let node = Builder.RegularNode("div")
    node.hxOn("click", "console.log('click')") |> ignore
    node.hxOn("mouseover", "console.log('hover')") |> ignore
    renderTag node
    |> shouldEqual
        """<div hx-on:click="console.log(&#39;click&#39;)" hx-on:mouseover="console.log(&#39;hover&#39;)"></div>"""
