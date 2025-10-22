module Tags.Tests

open Oxpecker.ViewEngine
open Xunit
open FsUnit.Light

// Tests for global HTML attributes
[<Fact>]
let ``style attribute test`` () =
    div(style = "color: red; font-size: 14px")
    |> Render.toString
    |> shouldEqual """<div style="color: red; font-size: 14px"></div>"""

[<Fact>]
let ``lang attribute test`` () =
    div(lang = "en")
    |> Render.toString
    |> shouldEqual """<div lang="en"></div>"""

[<Fact>]
let ``dir attribute test`` () =
    div(dir = "ltr")
    |> Render.toString
    |> shouldEqual """<div dir="ltr"></div>"""

[<Fact>]
let ``title attribute test`` () =
    div(title = "Tooltip text")
    |> Render.toString
    |> shouldEqual """<div title="Tooltip text"></div>"""

[<Fact>]
let ``accesskey attribute test`` () =
    button(accesskey = 's') { "Save" }
    |> Render.toString
    |> shouldEqual """<button accesskey="s">Save</button>"""

[<Fact>]
let ``contenteditable attribute test`` () =
    div(contenteditable = "true") { "Edit me" }
    |> Render.toString
    |> shouldEqual """<div contenteditable="true">Edit me</div>"""

[<Fact>]
let ``draggable attribute test`` () =
    div(draggable = "true") { "Drag me" }
    |> Render.toString
    |> shouldEqual """<div draggable="true">Drag me</div>"""

[<Fact>]
let ``enterkeyhint attribute test`` () =
    input(enterkeyhint = "send")
    |> Render.toString
    |> shouldEqual """<input enterkeyhint="send">"""

[<Fact>]
let ``hidden attribute test`` () =
    div(hidden = "hidden") { "Hidden" }
    |> Render.toString
    |> shouldEqual """<div hidden="hidden">Hidden</div>"""

[<Fact>]
let ``inert attribute test with true`` () =
    div(inert = true) { "Inert" }
    |> Render.toString
    |> shouldEqual """<div inert>Inert</div>"""

[<Fact>]
let ``inert attribute test with false`` () =
    div(inert = false) { "Not inert" }
    |> Render.toString
    |> shouldEqual """<div>Not inert</div>"""

[<Fact>]
let ``inputmode attribute test`` () =
    input(inputmode = "numeric")
    |> Render.toString
    |> shouldEqual """<input inputmode="numeric">"""

[<Fact>]
let ``popover attribute test`` () =
    div(popover = "auto") { "Popover content" }
    |> Render.toString
    |> shouldEqual """<div popover="auto">Popover content</div>"""

[<Fact>]
let ``spellcheck attribute test with true`` () =
    textarea(spellcheck = true)
    |> Render.toString
    |> shouldEqual """<textarea spellcheck="true"></textarea>"""

[<Fact>]
let ``spellcheck attribute test with false`` () =
    textarea(spellcheck = false)
    |> Render.toString
    |> shouldEqual """<textarea spellcheck="false"></textarea>"""

[<Fact>]
let ``translate attribute test`` () =
    p(translate = "no") { "Don't translate" }
    |> Render.toString
    |> shouldEqual """<p translate="no">Don&#39;t translate</p>"""

[<Fact>]
let ``autocapitalize attribute test`` () =
    input(autocapitalize = "words")
    |> Render.toString
    |> shouldEqual """<input autocapitalize="words">"""

[<Fact>]
let ``is attribute test`` () =
    button(is = "custom-button") { "Click" }
    |> Render.toString
    |> shouldEqual """<button is="custom-button">Click</button>"""

[<Fact>]
let ``part attribute test`` () =
    div(part = "container") { "Content" }
    |> Render.toString
    |> shouldEqual """<div part="container">Content</div>"""

[<Fact>]
let ``slot attribute test`` () =
    span(slot = "header") { "Header Content" }
    |> Render.toString
    |> shouldEqual """<span slot="header">Header Content</span>"""

// Tests for extension methods
[<Fact>]
let ``on extension method test`` () =
    button().on("click", "alert('clicked')") { "Click" }
    |> Render.toString
    |> shouldEqual """<button onclick="alert(&#39;clicked&#39;)">Click</button>"""

[<Fact>]
let ``data extension method test`` () =
    div().data("user-id", "123").data("role", "admin") { "User" }
    |> Render.toString
    |> shouldEqual """<div data-user-id="123" data-role="admin">User</div>"""

// Tests for various HTML elements
[<Fact>]
let ``head element test`` () =
    head() { title() { "Page Title" } }
    |> Render.toString
    |> shouldEqual """<head><title>Page Title</title></head>"""

[<Fact>]
let ``body element test`` () =
    body() { div() { "Content" } }
    |> Render.toString
    |> shouldEqual """<body><div>Content</div></body>"""

[<Fact>]
let ``article element test`` () =
    article() { h1() { "Article Title" } }
    |> Render.toString
    |> shouldEqual """<article><h1>Article Title</h1></article>"""

[<Fact>]
let ``section element test`` () =
    section() { p() { "Section content" } }
    |> Render.toString
    |> shouldEqual """<section><p>Section content</p></section>"""

[<Fact>]
let ``nav element test`` () =
    nav() { a(href = "/") { "Home" } }
    |> Render.toString
    |> shouldEqual """<nav><a href="/">Home</a></nav>"""

[<Fact>]
let ``aside element test`` () =
    aside() { p() { "Sidebar content" } }
    |> Render.toString
    |> shouldEqual """<aside><p>Sidebar content</p></aside>"""

[<Fact>]
let ``header element test`` () =
    header() { h1() { "Site Header" } }
    |> Render.toString
    |> shouldEqual """<header><h1>Site Header</h1></header>"""

[<Fact>]
let ``footer element test`` () =
    footer() { p() { "Copyright 2025" } }
    |> Render.toString
    |> shouldEqual """<footer><p>Copyright 2025</p></footer>"""

[<Fact>]
let ``main element test`` () =
    main() { article() { "Main content" } }
    |> Render.toString
    |> shouldEqual """<main><article>Main content</article></main>"""

[<Fact>]
let ``figure element test`` () =
    figure() {
        img(src = "image.jpg")
        figcaption() { "Image caption" }
    }
    |> Render.toString
    |> shouldEqual """<figure><img src="image.jpg"><figcaption>Image caption</figcaption></figure>"""

[<Fact>]
let ``table elements test`` () =
    table() {
        thead() {
            tr() {
                th() { "Header 1" }
                th() { "Header 2" }
            }
        }
        tbody() {
            tr() {
                td() { "Data 1" }
                td() { "Data 2" }
            }
        }
    }
    |> Render.toString
    |> shouldEqual """<table><thead><tr><th>Header 1</th><th>Header 2</th></tr></thead><tbody><tr><td>Data 1</td><td>Data 2</td></tr></tbody></table>"""

[<Fact>]
let ``form elements test`` () =
    form() {
        label(for' = "name") { "Name:" }
        input(type' = "text", id = "name", name = "name")
        button(type' = "submit") { "Submit" }
    }
    |> Render.toString
    |> shouldEqual """<form><label for="name">Name:</label><input type="text" id="name" name="name"><button type="submit">Submit</button></form>"""

[<Fact>]
let ``select element test`` () =
    select(name = "country") {
        option(value = "us") { "United States" }
        option(value = "uk") { "United Kingdom" }
    }
    |> Render.toString
    |> shouldEqual """<select name="country"><option value="us">United States</option><option value="uk">United Kingdom</option></select>"""

[<Fact>]
let ``textarea element test`` () =
    textarea(name = "message", rows = 4, cols = 50) { "Default text" }
    |> Render.toString
    |> shouldEqual """<textarea name="message" rows="4" cols="50">Default text</textarea>"""

[<Fact>]
let ``fieldset and legend test`` () =
    fieldset() {
        legend() { "User Info" }
        input(type' = "text", name = "username")
    }
    |> Render.toString
    |> shouldEqual """<fieldset><legend>User Info</legend><input type="text" name="username"></fieldset>"""

[<Fact>]
let ``details and summary test`` () =
    details() {
        summary() { "Click to expand" }
        p() { "Hidden content" }
    }
    |> Render.toString
    |> shouldEqual """<details><summary>Click to expand</summary><p>Hidden content</p></details>"""

[<Fact>]
let ``dialog element test`` () =
    dialog(open' = true) { p() { "Dialog content" } }
    |> Render.toString
    |> shouldEqual """<dialog open><p>Dialog content</p></dialog>"""

[<Fact>]
let ``semantic text elements test`` () =
    p() {
        strong() { "Bold" }
        " "
        em() { "Italic" }
        " "
        mark() { "Highlighted" }
        " "
        small() { "Small" }
    }
    |> Render.toString
    |> shouldEqual """<p><strong>Bold</strong> <em>Italic</em> <mark>Highlighted</mark> <small>Small</small></p>"""

[<Fact>]
let ``code and pre elements test`` () =
    pre() { code() { "let x = 42" } }
    |> Render.toString
    |> shouldEqual """<pre><code>let x = 42</code></pre>"""

[<Fact>]
let ``blockquote element test`` () =
    blockquote() {
        p() { "Quote text" }
    }
    |> Render.toString
    |> shouldEqual """<blockquote><p>Quote text</p></blockquote>"""

[<Fact>]
let ``abbr element test`` () =
    abbr(title = "HyperText Markup Language") { "HTML" }
    |> Render.toString
    |> shouldEqual """<abbr title="HyperText Markup Language">HTML</abbr>"""

[<Fact>]
let ``time element test`` () =
    time(datetime = "2025-10-22") { "October 22, 2025" }
    |> Render.toString
    |> shouldEqual """<time datetime="2025-10-22">October 22, 2025</time>"""

[<Fact>]
let ``progress element test`` () =
    progress(value = "70", max = "100")
    |> Render.toString
    |> shouldEqual """<progress value="70" max="100"></progress>"""

[<Fact>]
let ``meter element test`` () =
    meter(value = "0.6", min = "0", max = "1")
    |> Render.toString
    |> shouldEqual """<meter value="0.6" min="0" max="1"></meter>"""

[<Fact>]
let ``picture element test`` () =
    picture() {
        source(srcset = "image.webp", type' = "image/webp")
        img(src = "image.jpg", alt = "Description")
    }
    |> Render.toString
    |> shouldEqual """<picture><source srcset="image.webp" type="image/webp"><img src="image.jpg" alt="Description"></picture>"""

[<Fact>]
let ``audio element test`` () =
    audio(controls = true) {
        source(src = "audio.mp3", type' = "audio/mpeg")
        "Your browser does not support audio"
    }
    |> Render.toString
    |> shouldEqual """<audio controls><source src="audio.mp3" type="audio/mpeg">Your browser does not support audio</audio>"""

[<Fact>]
let ``video element test`` () =
    video(controls = true, width = 640, height = 480) {
        source(src = "video.mp4", type' = "video/mp4")
        "Your browser does not support video"
    }
    |> Render.toString
    |> shouldEqual """<video controls width="640" height="480"><source src="video.mp4" type="video/mp4">Your browser does not support video</video>"""

[<Fact>]
let ``iframe element test`` () =
    iframe(src = "https://example.com", width = 800, height = 600)
    |> Render.toString
    |> shouldEqual """<iframe src="https://example.com" width="800" height="600"></iframe>"""

[<Fact>]
let ``embed element test`` () =
    embed(src = "file.pdf", type' = "application/pdf", width = 800, height = 600)
    |> Render.toString
    |> shouldEqual """<embed src="file.pdf" type="application/pdf" width="800" height="600">"""

[<Fact>]
let ``object element test`` () =
    object'(data = "file.pdf", type' = "application/pdf") {
        p() { "Fallback content" }
    }
    |> Render.toString
    |> shouldEqual """<object data="file.pdf" type="application/pdf"><p>Fallback content</p></object>"""

[<Fact>]
let ``canvas element test`` () =
    canvas(id = "myCanvas", width = 400, height = 300) {
        "Canvas fallback text"
    }
    |> Render.toString
    |> shouldEqual """<canvas id="myCanvas" width="400" height="300">Canvas fallback text</canvas>"""

[<Fact>]
let ``multiple global attributes combined`` () =
    div(
        id = "main",
        class' = "container",
        lang = "en",
        dir = "ltr",
        title = "Main container",
        tabindex = 0,
        style = "padding: 10px"
    ) { "Content" }
    |> Render.toString
    |> shouldEqual """<div id="main" class="container" lang="en" dir="ltr" title="Main container" tabindex="0" style="padding: 10px">Content</div>"""
