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
    div(lang = "en") |> Render.toString |> shouldEqual """<div lang="en"></div>"""

[<Fact>]
let ``dir attribute test`` () =
    div(dir = "ltr") |> Render.toString |> shouldEqual """<div dir="ltr"></div>"""

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
    |> shouldEqual
        """<table><thead><tr><th>Header 1</th><th>Header 2</th></tr></thead><tbody><tr><td>Data 1</td><td>Data 2</td></tr></tbody></table>"""

[<Fact>]
let ``form elements test`` () =
    form() {
        label(for' = "name") { "Name:" }
        input(type' = "text", id = "name", name = "name")
        button(type' = "submit") { "Submit" }
    }
    |> Render.toString
    |> shouldEqual
        """<form><label for="name">Name:</label><input type="text" id="name" name="name"><button type="submit">Submit</button></form>"""

[<Fact>]
let ``select element test`` () =
    select(name = "country") {
        option(value = "us") { "United States" }
        option(value = "uk") { "United Kingdom" }
    }
    |> Render.toString
    |> shouldEqual
        """<select name="country"><option value="us">United States</option><option value="uk">United Kingdom</option></select>"""

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
    blockquote() { p() { "Quote text" } }
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
    |> shouldEqual
        """<picture><source srcset="image.webp" type="image/webp"><img src="image.jpg" alt="Description"></picture>"""

[<Fact>]
let ``audio element test`` () =
    audio(controls = true) {
        source(src = "audio.mp3", type' = "audio/mpeg")
        "Your browser does not support audio"
    }
    |> Render.toString
    |> shouldEqual
        """<audio controls><source src="audio.mp3" type="audio/mpeg">Your browser does not support audio</audio>"""

[<Fact>]
let ``video element test`` () =
    video(controls = true, width = 640, height = 480) {
        source(src = "video.mp4", type' = "video/mp4")
        "Your browser does not support video"
    }
    |> Render.toString
    |> shouldEqual
        """<video controls width="640" height="480"><source src="video.mp4" type="video/mp4">Your browser does not support video</video>"""

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
    object'(data = "file.pdf", type' = "application/pdf") { p() { "Fallback content" } }
    |> Render.toString
    |> shouldEqual """<object data="file.pdf" type="application/pdf"><p>Fallback content</p></object>"""

[<Fact>]
let ``canvas element test`` () =
    canvas(id = "myCanvas", width = 400, height = 300) { "Canvas fallback text" }
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
    ) {
        "Content"
    }
    |> Render.toString
    |> shouldEqual
        """<div id="main" class="container" lang="en" dir="ltr" title="Main container" tabindex="0" style="padding: 10px">Content</div>"""

// Tests for heading elements
[<Fact>]
let ``h2 through h6 elements test`` () =
    div() {
        h2() { "Heading 2" }
        h3() { "Heading 3" }
        h4() { "Heading 4" }
        h5() { "Heading 5" }
        h6() { "Heading 6" }
    }
    |> Render.toString
    |> shouldEqual
        """<div><h2>Heading 2</h2><h3>Heading 3</h3><h4>Heading 4</h4><h5>Heading 5</h5><h6>Heading 6</h6></div>"""

// Tests for list elements
[<Fact>]
let ``ul and li elements test`` () =
    ul() {
        li() { "Item 1" }
        li() { "Item 2" }
        li() { "Item 3" }
    }
    |> Render.toString
    |> shouldEqual """<ul><li>Item 1</li><li>Item 2</li><li>Item 3</li></ul>"""

[<Fact>]
let ``ol and li elements test`` () =
    ol() {
        li() { "First" }
        li() { "Second" }
    }
    |> Render.toString
    |> shouldEqual """<ol><li>First</li><li>Second</li></ol>"""

// Tests for void elements
[<Fact>]
let ``br element test`` () =
    p() {
        "Line 1"
        br()
        "Line 2"
    }
    |> Render.toString
    |> shouldEqual """<p>Line 1<br>Line 2</p>"""

[<Fact>]
let ``hr element test`` () =
    div() {
        p() { "Above" }
        hr()
        p() { "Below" }
    }
    |> Render.toString
    |> shouldEqual """<div><p>Above</p><hr><p>Below</p></div>"""

// Tests for text formatting elements
[<Fact>]
let ``i b u s elements test`` () =
    p() {
        i() { "Italic" }
        " "
        b() { "Bold" }
        " "
        u() { "Underline" }
        " "
        s() { "Strikethrough" }
    }
    |> Render.toString
    |> shouldEqual """<p><i>Italic</i> <b>Bold</b> <u>Underline</u> <s>Strikethrough</s></p>"""

[<Fact>]
let ``sub and sup elements test`` () =
    p() {
        "H"
        sub() { "2" }
        "O and x"
        sup() { "2" }
    }
    |> Render.toString
    |> shouldEqual """<p>H<sub>2</sub>O and x<sup>2</sup></p>"""

[<Fact>]
let ``del and ins elements test`` () =
    p() {
        del() { "Old text" }
        " "
        ins() { "New text" }
    }
    |> Render.toString
    |> shouldEqual """<p><del>Old text</del> <ins>New text</ins></p>"""

[<Fact>]
let ``dfn element test`` () =
    p() {
        dfn() { "API" }
        " stands for Application Programming Interface"
    }
    |> Render.toString
    |> shouldEqual """<p><dfn>API</dfn> stands for Application Programming Interface</p>"""

[<Fact>]
let ``cite and q elements test`` () =
    p() {
        q() { "Quote text" }
        " from "
        cite() { "Book Title" }
    }
    |> Render.toString
    |> shouldEqual """<p><q>Quote text</q> from <cite>Book Title</cite></p>"""

[<Fact>]
let ``address element test`` () =
    address() {
        "123 Main St"
        br()
        "City, State 12345"
    }
    |> Render.toString
    |> shouldEqual """<address>123 Main St<br>City, State 12345</address>"""

[<Fact>]
let ``noscript element test`` () =
    noscript() { "JavaScript is required" }
    |> Render.toString
    |> shouldEqual """<noscript>JavaScript is required</noscript>"""

[<Fact>]
let ``template element test`` () =
    template() { div() { "Template content" } }
    |> Render.toString
    |> shouldEqual """<template><div>Template content</div></template>"""

[<Fact>]
let ``search element test`` () =
    search() {
        form() {
            input(type' = "search", name = "q")
            button(type' = "submit") { "Search" }
        }
    }
    |> Render.toString
    |> shouldEqual
        """<search><form><input type="search" name="q"><button type="submit">Search</button></form></search>"""

// Tests for anchor element attributes
[<Fact>]
let ``anchor with href and target test`` () =
    a(href = "https://example.com", target = "_blank") { "External Link" }
    |> Render.toString
    |> shouldEqual """<a href="https://example.com" target="_blank">External Link</a>"""

[<Fact>]
let ``anchor with rel attribute test`` () =
    a(href = "/page", rel = "nofollow") { "Link" }
    |> Render.toString
    |> shouldEqual """<a href="/page" rel="nofollow">Link</a>"""

[<Fact>]
let ``anchor with download attribute test`` () =
    a(href = "/file.pdf", download = "document.pdf") { "Download PDF" }
    |> Render.toString
    |> shouldEqual """<a href="/file.pdf" download="document.pdf">Download PDF</a>"""

[<Fact>]
let ``anchor with hreflang attribute test`` () =
    a(href = "/es/page", hreflang = "es") { "Spanish Version" }
    |> Render.toString
    |> shouldEqual """<a href="/es/page" hreflang="es">Spanish Version</a>"""

[<Fact>]
let ``anchor with type attribute test`` () =
    a(href = "/file.pdf", type' = "application/pdf") { "PDF Link" }
    |> Render.toString
    |> shouldEqual """<a href="/file.pdf" type="application/pdf">PDF Link</a>"""

// Tests for image element attributes
[<Fact>]
let ``img with srcset attribute test`` () =
    img(src = "image.jpg", srcset = "image-2x.jpg 2x, image-3x.jpg 3x", alt = "Image")
    |> Render.toString
    |> shouldEqual """<img src="image.jpg" srcset="image-2x.jpg 2x, image-3x.jpg 3x" alt="Image">"""

[<Fact>]
let ``img with loading lazy test`` () =
    img(src = "image.jpg", loading = "lazy", alt = "Lazy image")
    |> Render.toString
    |> shouldEqual """<img src="image.jpg" loading="lazy" alt="Lazy image">"""

[<Fact>]
let ``img with crossorigin test`` () =
    img(src = "https://cdn.example.com/image.jpg", crossorigin = "anonymous", alt = "CDN Image")
    |> Render.toString
    |> shouldEqual """<img src="https://cdn.example.com/image.jpg" crossorigin="anonymous" alt="CDN Image">"""

[<Fact>]
let ``img with sizes attribute test`` () =
    img(src = "image.jpg", sizes = "(max-width: 600px) 100vw, 50vw", alt = "Responsive")
    |> Render.toString
    |> shouldEqual """<img src="image.jpg" sizes="(max-width: 600px) 100vw, 50vw" alt="Responsive">"""

[<Fact>]
let ``img with decoding attribute test`` () =
    img(src = "image.jpg", decoding = "async", alt = "Async decoded")
    |> Render.toString
    |> shouldEqual """<img src="image.jpg" decoding="async" alt="Async decoded">"""

[<Fact>]
let ``img with fetchpriority test`` () =
    img(src = "hero.jpg", fetchpriority = "high", alt = "Hero image")
    |> Render.toString
    |> shouldEqual """<img src="hero.jpg" fetchpriority="high" alt="Hero image">"""

[<Fact>]
let ``img with ismap attribute test`` () =
    a(href = "/map") { img(src = "map.jpg", ismap = true, alt = "Map") }
    |> Render.toString
    |> shouldEqual """<a href="/map"><img src="map.jpg" ismap alt="Map"></a>"""

// Tests for input element attributes
[<Fact>]
let ``input with placeholder test`` () =
    input(type' = "text", placeholder = "Enter your name")
    |> Render.toString
    |> shouldEqual """<input type="text" placeholder="Enter your name">"""

[<Fact>]
let ``input with required attribute test`` () =
    input(type' = "email", name = "email", required = true)
    |> Render.toString
    |> shouldEqual """<input type="email" name="email" required>"""

[<Fact>]
let ``input with autofocus attribute test`` () =
    input(type' = "text", name = "search", autofocus = true)
    |> Render.toString
    |> shouldEqual """<input type="text" name="search" autofocus>"""

[<Fact>]
let ``input with autocomplete test`` () =
    input(type' = "email", name = "email", autocomplete = "email")
    |> Render.toString
    |> shouldEqual """<input type="email" name="email" autocomplete="email">"""

[<Fact>]
let ``input with min and max test`` () =
    input(type' = "number", name = "age", min = "18", max = "100")
    |> Render.toString
    |> shouldEqual """<input type="number" name="age" min="18" max="100">"""

[<Fact>]
let ``input with step attribute test`` () =
    input(type' = "number", name = "price", step = "0.01")
    |> Render.toString
    |> shouldEqual """<input type="number" name="price" step="0.01">"""

[<Fact>]
let ``input with pattern attribute test`` () =
    input(type' = "text", name = "code", pattern = "[A-Z]{3}[0-9]{3}")
    |> Render.toString
    |> shouldEqual """<input type="text" name="code" pattern="[A-Z]{3}[0-9]{3}">"""

[<Fact>]
let ``input with readonly attribute test`` () =
    input(type' = "text", name = "username", value = "admin", readonly = true)
    |> Render.toString
    |> shouldEqual """<input type="text" name="username" value="admin" readonly>"""

[<Fact>]
let ``input with disabled attribute test`` () =
    input(type' = "text", name = "field", disabled = true)
    |> Render.toString
    |> shouldEqual """<input type="text" name="field" disabled>"""

[<Fact>]
let ``input with multiple attribute for file test`` () =
    input(type' = "file", name = "files", multiple = true)
    |> Render.toString
    |> shouldEqual """<input type="file" name="files" multiple>"""

[<Fact>]
let ``input checkbox checked test`` () =
    input(type' = "checkbox", name = "agree", checked' = true)
    |> Render.toString
    |> shouldEqual """<input type="checkbox" name="agree" checked>"""

[<Fact>]
let ``input with size and maxlength test`` () =
    input(type' = "text", name = "code", size = 10, maxlength = 10)
    |> Render.toString
    |> shouldEqual """<input type="text" name="code" size="10" maxlength="10">"""

// Tests for script element
[<Fact>]
let ``script with src test`` () =
    script(src = "/js/app.js")
    |> Render.toString
    |> shouldEqual """<script src="/js/app.js"></script>"""

[<Fact>]
let ``script with async attribute test`` () =
    script(src = "/js/analytics.js", async = true)
    |> Render.toString
    |> shouldEqual """<script src="/js/analytics.js" async></script>"""

[<Fact>]
let ``script with defer attribute test`` () =
    script(src = "/js/app.js", defer = true)
    |> Render.toString
    |> shouldEqual """<script src="/js/app.js" defer></script>"""

[<Fact>]
let ``script with type module test`` () =
    script(src = "/js/module.js", type' = "module")
    |> Render.toString
    |> shouldEqual """<script src="/js/module.js" type="module"></script>"""

[<Fact>]
let ``script with nomodule attribute test`` () =
    script(src = "/js/legacy.js", nomodule = true)
    |> Render.toString
    |> shouldEqual """<script src="/js/legacy.js" nomodule></script>"""

[<Fact>]
let ``script with integrity test`` () =
    script(src = "https://cdn.example.com/lib.js", integrity = "sha384-abc123")
    |> Render.toString
    |> shouldEqual """<script src="https://cdn.example.com/lib.js" integrity="sha384-abc123"></script>"""

[<Fact>]
let ``script with crossorigin test`` () =
    script(src = "https://cdn.example.com/lib.js", crossorigin = "anonymous")
    |> Render.toString
    |> shouldEqual """<script src="https://cdn.example.com/lib.js" crossorigin="anonymous"></script>"""

// Tests for link element
[<Fact>]
let ``link stylesheet test`` () =
    link(rel = "stylesheet", href = "/css/style.css")
    |> Render.toString
    |> shouldEqual """<link rel="stylesheet" href="/css/style.css">"""

[<Fact>]
let ``link with type and media test`` () =
    link(rel = "stylesheet", href = "/css/print.css", type' = "text/css", media = "print")
    |> Render.toString
    |> shouldEqual """<link rel="stylesheet" href="/css/print.css" type="text/css" media="print">"""

[<Fact>]
let ``link preload test`` () =
    link(rel = "preload", href = "/fonts/font.woff2", as' = "font")
    |> Render.toString
    |> shouldEqual """<link rel="preload" href="/fonts/font.woff2" as="font">"""

[<Fact>]
let ``link with integrity test`` () =
    link(rel = "stylesheet", href = "https://cdn.example.com/style.css", integrity = "sha384-xyz789")
    |> Render.toString
    |> shouldEqual """<link rel="stylesheet" href="https://cdn.example.com/style.css" integrity="sha384-xyz789">"""

[<Fact>]
let ``link with crossorigin test`` () =
    link(rel = "stylesheet", href = "https://cdn.example.com/style.css", crossorigin = "anonymous")
    |> Render.toString
    |> shouldEqual """<link rel="stylesheet" href="https://cdn.example.com/style.css" crossorigin="anonymous">"""

[<Fact>]
let ``link icon with sizes test`` () =
    link(rel = "icon", href = "/favicon.png", sizes = "32x32")
    |> Render.toString
    |> shouldEqual """<link rel="icon" href="/favicon.png" sizes="32x32">"""

[<Fact>]
let ``link with disabled attribute test`` () =
    link(rel = "stylesheet", href = "/css/optional.css", disabled = true)
    |> Render.toString
    |> shouldEqual """<link rel="stylesheet" href="/css/optional.css" disabled>"""

// Tests for meta element
[<Fact>]
let ``meta charset test`` () =
    meta(charset = "UTF-8")
    |> Render.toString
    |> shouldEqual """<meta charset="UTF-8">"""

[<Fact>]
let ``meta name and content test`` () =
    meta(name = "description", content = "Page description")
    |> Render.toString
    |> shouldEqual """<meta name="description" content="Page description">"""

[<Fact>]
let ``meta http-equiv test`` () =
    meta(httpEquiv = "X-UA-Compatible", content = "IE=edge")
    |> Render.toString
    |> shouldEqual """<meta http-equiv="X-UA-Compatible" content="IE=edge">"""

[<Fact>]
let ``meta viewport test`` () =
    meta(name = "viewport", content = "width=device-width, initial-scale=1.0")
    |> Render.toString
    |> shouldEqual """<meta name="viewport" content="width=device-width, initial-scale=1.0">"""

// Tests for form element attributes
[<Fact>]
let ``form with action and method test`` () =
    form(action = "/submit", method = "post") { input(type' = "text", name = "data") }
    |> Render.toString
    |> shouldEqual """<form action="/submit" method="post"><input type="text" name="data"></form>"""

[<Fact>]
let ``form with enctype test`` () =
    form(action = "/upload", method = "post", enctype = "multipart/form-data") { input(type' = "file", name = "file") }
    |> Render.toString
    |> shouldEqual
        """<form action="/upload" method="post" enctype="multipart/form-data"><input type="file" name="file"></form>"""

// Tests for base element
[<Fact>]
let ``base element with href test`` () =
    base'(href = "https://example.com/")
    |> Render.toString
    |> shouldEqual """<base href="https://example.com/">"""

[<Fact>]
let ``base element with target test`` () =
    base'(href = "https://example.com/", target = "_blank")
    |> Render.toString
    |> shouldEqual """<base href="https://example.com/" target="_blank">"""

// Tests for html element
[<Fact>]
let ``html element with xmlns test`` () =
    html(xmlns = "http://www.w3.org/1999/xhtml") { head() { title() { "Page" } } }
    |> Render.toString
    |> shouldEqual """<html xmlns="http://www.w3.org/1999/xhtml"><head><title>Page</title></head></html>"""

// Tests for caption and span elements
[<Fact>]
let ``caption in table test`` () =
    table() {
        caption() { "Table Caption" }
        tr() { td() { "Data" } }
    }
    |> Render.toString
    |> shouldEqual """<table><caption>Table Caption</caption><tr><td>Data</td></tr></table>"""

[<Fact>]
let ``span element test`` () =
    p() {
        "Text with "
        span(class' = "highlight") { "highlighted" }
        " word"
    }
    |> Render.toString
    |> shouldEqual """<p>Text with <span class="highlight">highlighted</span> word</p>"""
