module Builder.Tests

open System.Text
open Oxpecker.ViewEngine
open Xunit
open FsUnit.Light

[<Fact>]
let ``FragmentNode with no children renders empty`` () =
    let fragment = Builder.FragmentNode()
    let sb = StringBuilder()
    fragment.Render(sb)
    sb.ToString() |> shouldEqual ""

[<Fact>]
let ``FragmentNode with single child renders correctly`` () =
    let fragment = Builder.FragmentNode()
    fragment.AddChild(Builder.RegularTextNode("Hello"))
    let sb = StringBuilder()
    fragment.Render(sb)
    sb.ToString() |> shouldEqual "Hello"

[<Fact>]
let ``FragmentNode with multiple children renders in order`` () =
    let fragment = Builder.FragmentNode()
    fragment.AddChild(Builder.RegularTextNode("First"))
    fragment.AddChild(Builder.RegularTextNode("Second"))
    fragment.AddChild(Builder.RegularTextNode("Third"))
    let sb = StringBuilder()
    fragment.Render(sb)
    sb.ToString() |> shouldEqual "FirstSecondThird"

[<Fact>]
let ``FragmentNode Children property returns enumerable`` () =
    let fragment = Builder.FragmentNode()
    fragment.AddChild(Builder.RegularTextNode("Test"))
    fragment.Children |> Seq.length |> shouldEqual 1

[<Fact>]
let ``RegularNode with no attributes or children`` () =
    let node = Builder.RegularNode("div")
    let sb = StringBuilder()
    node.Render(sb)
    sb.ToString() |> shouldEqual "<div></div>"

[<Fact>]
let ``RegularNode with single attribute`` () =
    let node = Builder.RegularNode("div")
    node.AddAttribute(
        {
            Builder.HtmlAttribute.Name = "id"
            Value = "test"
        }
    )
    let sb = StringBuilder()
    node.Render(sb)
    sb.ToString() |> shouldEqual """<div id="test"></div>"""

[<Fact>]
let ``RegularNode with multiple attributes`` () =
    let node = Builder.RegularNode("div")
    node.AddAttribute(
        {
            Builder.HtmlAttribute.Name = "id"
            Value = "test"
        }
    )
    node.AddAttribute(
        {
            Builder.HtmlAttribute.Name = "class"
            Value = "my-class"
        }
    )
    let sb = StringBuilder()
    node.Render(sb)
    sb.ToString() |> shouldEqual """<div id="test" class="my-class"></div>"""

[<Fact>]
let ``RegularNode with null-valued attribute renders as boolean attribute`` () =
    let node = Builder.RegularNode("input")
    node.AddAttribute(
        {
            Builder.HtmlAttribute.Name = "disabled"
            Value = null
        }
    )
    let sb = StringBuilder()
    node.Render(sb)
    sb.ToString() |> shouldEqual "<input disabled></input>"

[<Fact>]
let ``RegularNode with children`` () =
    let node = Builder.RegularNode("div")
    node.AddChild(Builder.RegularTextNode("Hello"))
    node.AddChild(Builder.RegularTextNode(" World"))
    let sb = StringBuilder()
    node.Render(sb)
    sb.ToString() |> shouldEqual "<div>Hello World</div>"

[<Fact>]
let ``RegularNode with attributes and children`` () =
    let node = Builder.RegularNode("div")
    node.AddAttribute(
        {
            Builder.HtmlAttribute.Name = "class"
            Value = "container"
        }
    )
    node.AddChild(Builder.RegularTextNode("Content"))
    let sb = StringBuilder()
    node.Render(sb)
    sb.ToString() |> shouldEqual """<div class="container">Content</div>"""

[<Fact>]
let ``RegularNode TagName property returns correct name`` () =
    let node = Builder.RegularNode("span")
    node.TagName |> shouldEqual "span"

[<Fact>]
let ``RegularNode Children property returns enumerable`` () =
    let node = Builder.RegularNode("div")
    node.AddChild(Builder.RegularTextNode("Test"))
    node.Children |> Seq.length |> shouldEqual 1

[<Fact>]
let ``RegularNode Attributes property returns enumerable`` () =
    let node = Builder.RegularNode("div")
    node.AddAttribute(
        {
            Builder.HtmlAttribute.Name = "id"
            Value = "test"
        }
    )
    node.Attributes |> Seq.length |> shouldEqual 1

[<Fact>]
let ``VoidNode with no attributes`` () =
    let node = Builder.VoidNode("br")
    let sb = StringBuilder()
    node.Render(sb)
    sb.ToString() |> shouldEqual "<br>"

[<Fact>]
let ``VoidNode with single attribute`` () =
    let node = Builder.VoidNode("input")
    node.AddAttribute(
        {
            Builder.HtmlAttribute.Name = "type"
            Value = "text"
        }
    )
    let sb = StringBuilder()
    node.Render(sb)
    sb.ToString() |> shouldEqual """<input type="text">"""

[<Fact>]
let ``VoidNode with multiple attributes`` () =
    let node = Builder.VoidNode("input")
    node.AddAttribute(
        {
            Builder.HtmlAttribute.Name = "type"
            Value = "checkbox"
        }
    )
    node.AddAttribute(
        {
            Builder.HtmlAttribute.Name = "checked"
            Value = null
        }
    )
    let sb = StringBuilder()
    node.Render(sb)
    sb.ToString() |> shouldEqual """<input type="checkbox" checked>"""

[<Fact>]
let ``VoidNode TagName property returns correct name`` () =
    let node = Builder.VoidNode("hr")
    node.TagName |> shouldEqual "hr"

[<Fact>]
let ``VoidNode Attributes property returns enumerable`` () =
    let node = Builder.VoidNode("br")
    node.AddAttribute(
        {
            Builder.HtmlAttribute.Name = "class"
            Value = "my-br"
        }
    )
    node.Attributes |> Seq.length |> shouldEqual 1

[<Fact>]
let ``RegularTextNode with simple text`` () =
    let node = Builder.RegularTextNode("Hello World")
    let sb = StringBuilder()
    node.Render(sb)
    sb.ToString() |> shouldEqual "Hello World"

[<Fact>]
let ``RegularTextNode with null text`` () =
    let node = Builder.RegularTextNode(null)
    let sb = StringBuilder()
    node.Render(sb)
    sb.ToString() |> shouldEqual ""

[<Fact>]
let ``RegularTextNode HTML-escapes special characters`` () =
    let node = Builder.RegularTextNode("<div>Test & \"quotes\"</div>")
    let sb = StringBuilder()
    node.Render(sb)
    sb.ToString()
    |> shouldEqual "&lt;div&gt;Test &amp; &quot;quotes&quot;&lt;/div&gt;"

[<Fact>]
let ``RawTextNode with simple text`` () =
    let node = Builder.RawTextNode("Hello World")
    let sb = StringBuilder()
    node.Render(sb)
    sb.ToString() |> shouldEqual "Hello World"

[<Fact>]
let ``RawTextNode with null text`` () =
    let node = Builder.RawTextNode(null)
    let sb = StringBuilder()
    node.Render(sb)
    sb.ToString() |> shouldEqual ""

[<Fact>]
let ``RawTextNode does NOT HTML-escape special characters`` () =
    let node = Builder.RawTextNode("<div>Test & \"quotes\"</div>")
    let sb = StringBuilder()
    node.Render(sb)
    sb.ToString() |> shouldEqual "<div>Test & \"quotes\"</div>"

[<Fact>]
let ``raw function creates RawTextNode`` () =
    let node = Builder.raw("<strong>Bold</strong>")
    let sb = StringBuilder()
    (node :> Builder.HtmlElement).Render(sb)
    sb.ToString() |> shouldEqual "<strong>Bold</strong>"

[<Fact>]
let ``HtmlContainer Combine combines two functions`` () =
    let fragment = Builder.FragmentNode()
    let first: Builder.HtmlContainerFun =
        fun c -> c.AddChild(Builder.RegularTextNode("First"))
    let second: Builder.HtmlContainerFun =
        fun c -> c.AddChild(Builder.RegularTextNode("Second"))
    let combined = (fragment :> Builder.HtmlContainer).Combine(first, second)
    combined fragment
    let sb = StringBuilder()
    fragment.Render(sb)
    sb.ToString() |> shouldEqual "FirstSecond"

[<Fact>]
let ``HtmlContainer Zero returns ignore function`` () =
    let fragment = Builder.FragmentNode()
    let zero = (fragment :> Builder.HtmlContainer).Zero()
    zero fragment
    let sb = StringBuilder()
    fragment.Render(sb)
    sb.ToString() |> shouldEqual ""

[<Fact>]
let ``HtmlContainer Delay delays execution`` () =
    let fragment = Builder.FragmentNode()
    let delayed =
        (fragment :> Builder.HtmlContainer).Delay(fun () -> fun c -> c.AddChild(Builder.RegularTextNode("Delayed")))
    delayed fragment
    let sb = StringBuilder()
    fragment.Render(sb)
    sb.ToString() |> shouldEqual "Delayed"

[<Fact>]
let ``HtmlContainer For iterates over sequence`` () =
    let fragment = Builder.FragmentNode()
    let items = [ "A"; "B"; "C" ]
    let forLoop =
        (fragment :> Builder.HtmlContainer).For(items, fun item -> fun c -> c.AddChild(Builder.RegularTextNode(item)))
    forLoop fragment
    let sb = StringBuilder()
    fragment.Render(sb)
    sb.ToString() |> shouldEqual "ABC"

[<Fact>]
let ``HtmlContainer Yield with HtmlElement`` () =
    let fragment = Builder.FragmentNode()
    let element = Builder.RegularTextNode("Test")
    let yieldFun = (fragment :> Builder.HtmlContainer).Yield(element)
    yieldFun fragment
    let sb = StringBuilder()
    fragment.Render(sb)
    sb.ToString() |> shouldEqual "Test"

[<Fact>]
let ``HtmlContainer YieldFrom with sequence of elements`` () =
    let fragment = Builder.FragmentNode()
    let elements = [
        Builder.RegularTextNode("First") :> Builder.HtmlElement
        Builder.RegularTextNode("Second") :> Builder.HtmlElement
    ]
    let yieldFrom = (fragment :> Builder.HtmlContainer).YieldFrom(elements)
    yieldFrom fragment
    let sb = StringBuilder()
    fragment.Render(sb)
    sb.ToString() |> shouldEqual "FirstSecond"

[<Fact>]
let ``HtmlContainer Yield with string`` () =
    let fragment = Builder.FragmentNode()
    let yieldFun = (fragment :> Builder.HtmlContainer).Yield("Hello")
    yieldFun fragment
    let sb = StringBuilder()
    fragment.Render(sb)
    sb.ToString() |> shouldEqual "Hello"

[<Fact>]
let ``HtmlContainer Yield with null string`` () =
    let fragment = Builder.FragmentNode()
    let yieldFun = (fragment :> Builder.HtmlContainer).Yield(null)
    yieldFun fragment
    let sb = StringBuilder()
    fragment.Render(sb)
    sb.ToString() |> shouldEqual ""

[<Fact>]
let ``HtmlContainerExtensions Run executes and returns container`` () =
    let fragment = Builder.FragmentNode()
    let runExpr: Builder.HtmlContainerFun =
        fun c -> c.AddChild(Builder.RegularTextNode("Run"))
    let result = Builder.HtmlContainerExtensions.Run(fragment, runExpr)
    Assert.Same(fragment, result)
    let sb = StringBuilder()
    fragment.Render(sb)
    sb.ToString() |> shouldEqual "Run"

[<Fact>]
let ``Attribute value HTML-escaping with special characters`` () =
    let node = Builder.RegularNode("div")
    node.AddAttribute(
        {
            Builder.HtmlAttribute.Name = "data-value"
            Value = "Test & <script>"
        }
    )
    let sb = StringBuilder()
    node.Render(sb)
    sb.ToString()
    |> shouldEqual """<div data-value="Test &amp; &lt;script&gt;"></div>"""

[<Fact>]
let ``Attribute value HTML-escaping with quotes`` () =
    let node = Builder.RegularNode("div")
    node.AddAttribute(
        {
            Builder.HtmlAttribute.Name = "title"
            Value = """He said "Hello" """
        }
    )
    let sb = StringBuilder()
    node.Render(sb)
    sb.ToString()
    |> shouldEqual """<div title="He said &quot;Hello&quot; "></div>"""

// Integration tests using actual HTML element builder API

open Oxpecker.ViewEngine

[<Fact>]
let ``Builder computation expression with nested elements`` () =
    let result =
        div() {
            h1() { "Title" }
            p() { "Paragraph" }
        }
    result
    |> Render.toString
    |> shouldEqual "<div><h1>Title</h1><p>Paragraph</p></div>"

[<Fact>]
let ``Builder computation expression with for loop`` () =
    let items = [ "Apple"; "Banana"; "Cherry" ]
    let result =
        ul() {
            for item in items do
                li() { item }
        }
    result
    |> Render.toString
    |> shouldEqual "<ul><li>Apple</li><li>Banana</li><li>Cherry</li></ul>"

[<Fact>]
let ``Builder computation expression with empty for loop`` () =
    let items: string list = []
    let result =
        ul() {
            for item in items do
                li() { item }
        }
    result |> Render.toString |> shouldEqual "<ul></ul>"

[<Fact>]
let ``Builder computation expression with YieldFrom sequence`` () =
    let paragraphs = [
        p() { "First paragraph" }
        p() { "Second paragraph" }
        p() { "Third paragraph" }
    ]
    let result = div() { yield! paragraphs }
    result
    |> Render.toString
    |> shouldEqual "<div><p>First paragraph</p><p>Second paragraph</p><p>Third paragraph</p></div>"

[<Fact>]
let ``Builder computation expression with raw text`` () =
    let result = div() { Builder.raw "<strong>Bold HTML</strong>" }
    result |> Render.toString |> shouldEqual "<div><strong>Bold HTML</strong></div>"

[<Fact>]
let ``Builder computation expression with mixed content`` () =
    let result =
        div() {
            "Plain text "
            strong() { "bold" }
            " and more text"
        }
    result
    |> Render.toString
    |> shouldEqual "<div>Plain text <strong>bold</strong> and more text</div>"

[<Fact>]
let ``Builder computation expression with null text`` () =
    let result =
        div() {
            null
            "content"
        }
    result |> Render.toString |> shouldEqual "<div>content</div>"

[<Fact>]
let ``Builder computation expression with nested loops`` () =
    let rows = [ 1..3 ]
    let cols = [ 1..2 ]
    let result =
        table() {
            for row in rows do
                tr() {
                    for col in cols do
                        td() { $"R{row}C{col}" }
                }
        }
    result
    |> Render.toString
    |> shouldEqual
        "<table><tr><td>R1C1</td><td>R1C2</td></tr><tr><td>R2C1</td><td>R2C2</td></tr><tr><td>R3C1</td><td>R3C2</td></tr></table>"

[<Fact>]
let ``Builder computation expression with multiple yields`` () =
    let result =
        div() {
            span() { "First" }
            span() { "Second" }
            span() { "Third" }
        }
    result
    |> Render.toString
    |> shouldEqual "<div><span>First</span><span>Second</span><span>Third</span></div>"

[<Fact>]
let ``Builder computation expression with conditional content using if-then`` () =
    let showTitle = true
    let result =
        div() {
            if showTitle then
                h1() { "Title" }
            p() { "Content" }
        }
    result
    |> Render.toString
    |> shouldEqual "<div><h1>Title</h1><p>Content</p></div>"

[<Fact>]
let ``Builder computation expression with false conditional`` () =
    let showTitle = false
    let result =
        div() {
            if showTitle then
                h1() { "Title" }
            p() { "Content" }
        }
    result |> Render.toString |> shouldEqual "<div><p>Content</p></div>"

[<Fact>]
let ``Builder computation expression combining all features`` () =
    let items = [ "A"; "B" ]
    let showHeader = true
    let result =
        div(class' = "container") {
            if showHeader then
                h1() { "List" }
            ul() {
                for item in items do
                    li() {
                        "Item: "
                        strong() { item }
                    }
            }
            Builder.raw "<!-- comment -->"
        }
    result
    |> Render.toString
    |> shouldEqual
        """<div class="container"><h1>List</h1><ul><li>Item: <strong>A</strong></li><li>Item: <strong>B</strong></li></ul><!-- comment --></div>"""

[<Fact>]
let ``Builder FragmentNode with computation expression`` () =
    let fragment = Builder.FragmentNode()
    let result =
        Builder.HtmlContainerExtensions.Run(
            fragment,
            fun c ->
                c.AddChild(Builder.RegularTextNode("Fragment"))
                c.AddChild(Builder.RegularTextNode(" content"))
        )
    let sb = StringBuilder()
    result.Render(sb)
    sb.ToString() |> shouldEqual "Fragment content"

[<Fact>]
let ``Builder with deeply nested structure`` () =
    let result =
        html() {
            head() { title() { "Test Page" } }
            body() {
                div(class' = "wrapper") {
                    header() { h1() { "Header" } }
                    main() {
                        article() {
                            h2() { "Article" }
                            p() { "Content" }
                        }
                    }
                    footer() { "Footer" }
                }
            }
        }
    result
    |> Render.toString
    |> shouldEqual
        """<html><head><title>Test Page</title></head><body><div class="wrapper"><header><h1>Header</h1></header><main><article><h2>Article</h2><p>Content</p></article></main><footer>Footer</footer></div></body></html>"""
