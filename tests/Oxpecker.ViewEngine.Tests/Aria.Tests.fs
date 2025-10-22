module Aria.Tests

open System.Text
open Oxpecker.ViewEngine
open Oxpecker.ViewEngine.Aria
open Xunit
open FsUnit.Light

[<Fact>]
let ``role attribute test`` () =
    let result = div(role = "button") { "Click me" } |> Render.toString
    result |> shouldEqual """<div role="button">Click me</div>"""

[<Fact>]
let ``ariaActiveDescendant attribute test`` () =
    let result = div(ariaActiveDescendant = "option-1") { "List" } |> Render.toString
    result |> shouldEqual """<div aria-activedescendant="option-1">List</div>"""

[<Fact>]
let ``ariaAtomic attribute test with true`` () =
    let result = div(ariaAtomic = true) { "Content" } |> Render.toString
    result |> shouldEqual """<div aria-atomic="true">Content</div>"""

[<Fact>]
let ``ariaAtomic attribute test with false`` () =
    let result = div(ariaAtomic = false) { "Content" } |> Render.toString
    result |> shouldEqual """<div aria-atomic="false">Content</div>"""

[<Fact>]
let ``ariaAutoComplete attribute test`` () =
    let result = input(ariaAutoComplete = "list") |> Render.toString
    result |> shouldEqual """<input aria-autocomplete="list">"""

[<Fact>]
let ``ariaBrailleLabel attribute test`` () =
    let result = button(ariaBrailleLabel = "Close dialog") { "X" } |> Render.toString
    result |> shouldEqual """<button aria-braillelabel="Close dialog">X</button>"""

[<Fact>]
let ``ariaBrailleRoleDescription attribute test`` () =
    let result =
        div(ariaBrailleRoleDescription = "custom widget") { "Widget" }
        |> Render.toString
    result
    |> shouldEqual """<div aria-brailleroledescription="custom widget">Widget</div>"""

[<Fact>]
let ``ariaBusy attribute test with true`` () =
    let result = div(ariaBusy = true) { "Loading..." } |> Render.toString
    result |> shouldEqual """<div aria-busy="true">Loading...</div>"""

[<Fact>]
let ``ariaBusy attribute test with false`` () =
    let result = div(ariaBusy = false) { "Ready" } |> Render.toString
    result |> shouldEqual """<div aria-busy="false">Ready</div>"""

[<Fact>]
let ``ariaChecked attribute test`` () =
    let result =
        span(role = "checkbox", ariaChecked = "true") { "Agreed" } |> Render.toString
    result
    |> shouldEqual """<span role="checkbox" aria-checked="true">Agreed</span>"""

[<Fact>]
let ``ariaColCount attribute test`` () =
    let result = div(role = "grid", ariaColCount = 5) { "Table" } |> Render.toString
    result |> shouldEqual """<div role="grid" aria-colcount="5">Table</div>"""

[<Fact>]
let ``ariaColIndex attribute test`` () =
    let result = div(role = "gridcell", ariaColIndex = 3) { "Cell" } |> Render.toString
    result |> shouldEqual """<div role="gridcell" aria-colindex="3">Cell</div>"""

[<Fact>]
let ``ariaColIndexText attribute test`` () =
    let result =
        div(role = "gridcell", ariaColIndexText = "Column C") { "Data" }
        |> Render.toString
    result
    |> shouldEqual """<div role="gridcell" aria-colindextext="Column C">Data</div>"""

[<Fact>]
let ``ariaControls attribute test`` () =
    let result = button(ariaControls = "panel-1") { "Toggle Panel" } |> Render.toString
    result
    |> shouldEqual """<button aria-controls="panel-1">Toggle Panel</button>"""

[<Fact>]
let ``ariaCurrent attribute test`` () =
    let result =
        a(href = "/page", ariaCurrent = "page") { "Current Page" } |> Render.toString
    result |> shouldEqual """<a href="/page" aria-current="page">Current Page</a>"""

[<Fact>]
let ``ariaDescribedBy attribute test`` () =
    let result = input(ariaDescribedBy = "help-text") |> Render.toString
    result |> shouldEqual """<input aria-describedby="help-text">"""

[<Fact>]
let ``ariaDescription attribute test`` () =
    let result =
        button(ariaDescription = "Opens a new window") { "External Link" }
        |> Render.toString
    result
    |> shouldEqual """<button aria-description="Opens a new window">External Link</button>"""

[<Fact>]
let ``ariaDetails attribute test`` () =
    let result = div(ariaDetails = "details-1") { "Summary" } |> Render.toString
    result |> shouldEqual """<div aria-details="details-1">Summary</div>"""

[<Fact>]
let ``ariaDisabled attribute test with true`` () =
    let result = button(ariaDisabled = true) { "Disabled" } |> Render.toString
    result |> shouldEqual """<button aria-disabled="true">Disabled</button>"""

[<Fact>]
let ``ariaDisabled attribute test with false`` () =
    let result = button(ariaDisabled = false) { "Enabled" } |> Render.toString
    result |> shouldEqual """<button aria-disabled="false">Enabled</button>"""

[<Fact>]
let ``ariaErrorMessage attribute test`` () =
    let result = input(ariaErrorMessage = "error-1") |> Render.toString
    result |> shouldEqual """<input aria-errormessage="error-1">"""

[<Fact>]
let ``ariaExpanded attribute test with true`` () =
    let result = button(ariaExpanded = true) { "Collapse" } |> Render.toString
    result |> shouldEqual """<button aria-expanded="true">Collapse</button>"""

[<Fact>]
let ``ariaExpanded attribute test with false`` () =
    let result = button(ariaExpanded = false) { "Expand" } |> Render.toString
    result |> shouldEqual """<button aria-expanded="false">Expand</button>"""

[<Fact>]
let ``ariaFlowTo attribute test`` () =
    let result = div(ariaFlowTo = "next-section") { "Section" } |> Render.toString
    result |> shouldEqual """<div aria-flowto="next-section">Section</div>"""

[<Fact>]
let ``ariaHasPopup attribute test`` () =
    let result = button(ariaHasPopup = "menu") { "Menu" } |> Render.toString
    result |> shouldEqual """<button aria-haspopup="menu">Menu</button>"""

[<Fact>]
let ``ariaHidden attribute test with true`` () =
    let result = div(ariaHidden = true) { "Hidden" } |> Render.toString
    result |> shouldEqual """<div aria-hidden="true">Hidden</div>"""

[<Fact>]
let ``ariaHidden attribute test with false`` () =
    let result = div(ariaHidden = false) { "Visible" } |> Render.toString
    result |> shouldEqual """<div aria-hidden="false">Visible</div>"""

[<Fact>]
let ``ariaInvalid attribute test`` () =
    let result = input(ariaInvalid = "true") |> Render.toString
    result |> shouldEqual """<input aria-invalid="true">"""

[<Fact>]
let ``ariaKeyShortcuts attribute test`` () =
    let result = button(ariaKeyShortcuts = "Alt+S") { "Save" } |> Render.toString
    result |> shouldEqual """<button aria-keyshortcuts="Alt+S">Save</button>"""

[<Fact>]
let ``ariaLabel attribute test`` () =
    let result = button(ariaLabel = "Close dialog") { "X" } |> Render.toString
    result |> shouldEqual """<button aria-label="Close dialog">X</button>"""

[<Fact>]
let ``ariaLabelledBy attribute test`` () =
    let result = div(ariaLabelledBy = "label-1") { "Content" } |> Render.toString
    result |> shouldEqual """<div aria-labelledby="label-1">Content</div>"""

[<Fact>]
let ``ariaLevel attribute test`` () =
    let result = div(role = "heading", ariaLevel = 3) { "Heading" } |> Render.toString
    result |> shouldEqual """<div role="heading" aria-level="3">Heading</div>"""

[<Fact>]
let ``ariaLive attribute test`` () =
    let result = div(ariaLive = "polite") { "Status" } |> Render.toString
    result |> shouldEqual """<div aria-live="polite">Status</div>"""

[<Fact>]
let ``ariaModal attribute test with true`` () =
    let result = div(role = "dialog", ariaModal = true) { "Dialog" } |> Render.toString
    result |> shouldEqual """<div role="dialog" aria-modal="true">Dialog</div>"""

[<Fact>]
let ``ariaModal attribute test with false`` () =
    let result =
        div(role = "dialog", ariaModal = false) { "Non-modal" } |> Render.toString
    result
    |> shouldEqual """<div role="dialog" aria-modal="false">Non-modal</div>"""

[<Fact>]
let ``ariaMultiLine attribute test with true`` () =
    let result = textarea(ariaMultiLine = true) { "Text" } |> Render.toString
    result |> shouldEqual """<textarea aria-multiline="true">Text</textarea>"""

[<Fact>]
let ``ariaMultiLine attribute test with false`` () =
    let result = input(ariaMultiLine = false) |> Render.toString
    result |> shouldEqual """<input aria-multiline="false">"""

[<Fact>]
let ``ariaMultiSelectable attribute test with true`` () =
    let result =
        div(role = "listbox", ariaMultiSelectable = true) { "Options" }
        |> Render.toString
    result
    |> shouldEqual """<div role="listbox" aria-multiselectable="true">Options</div>"""

[<Fact>]
let ``ariaMultiSelectable attribute test with false`` () =
    let result =
        div(role = "listbox", ariaMultiSelectable = false) { "Options" }
        |> Render.toString
    result
    |> shouldEqual """<div role="listbox" aria-multiselectable="false">Options</div>"""

[<Fact>]
let ``ariaOrientation attribute test`` () =
    let result =
        div(role = "slider", ariaOrientation = "vertical") { "Slider" }
        |> Render.toString
    result
    |> shouldEqual """<div role="slider" aria-orientation="vertical">Slider</div>"""

[<Fact>]
let ``ariaOwns attribute test`` () =
    let result = div(ariaOwns = "item-1 item-2") { "Container" } |> Render.toString
    result |> shouldEqual """<div aria-owns="item-1 item-2">Container</div>"""

[<Fact>]
let ``ariaPlaceholder attribute test`` () =
    let result = input(ariaPlaceholder = "Enter text") |> Render.toString
    result |> shouldEqual """<input aria-placeholder="Enter text">"""

[<Fact>]
let ``ariaPosInSet attribute test`` () =
    let result = div(role = "listitem", ariaPosInSet = 2) { "Item" } |> Render.toString
    result |> shouldEqual """<div role="listitem" aria-posinset="2">Item</div>"""

[<Fact>]
let ``ariaPressed attribute test`` () =
    let result = button(ariaPressed = "true") { "Toggle" } |> Render.toString
    result |> shouldEqual """<button aria-pressed="true">Toggle</button>"""

[<Fact>]
let ``ariaReadOnly attribute test`` () =
    let result = input(ariaReadOnly = "true") |> Render.toString
    result |> shouldEqual """<input aria-readonly="true">"""

[<Fact>]
let ``ariaRelevant attribute test`` () =
    let result =
        div(ariaRelevant = "additions text") { "Live region" } |> Render.toString
    result
    |> shouldEqual """<div aria-relevant="additions text">Live region</div>"""

[<Fact>]
let ``ariaRequired attribute test with true`` () =
    let result = input(ariaRequired = true) |> Render.toString
    result |> shouldEqual """<input aria-required="true">"""

[<Fact>]
let ``ariaRequired attribute test with false`` () =
    let result = input(ariaRequired = false) |> Render.toString
    result |> shouldEqual """<input aria-required="false">"""

[<Fact>]
let ``ariaRoleDescription attribute test`` () =
    let result =
        div(ariaRoleDescription = "custom widget") { "Widget" } |> Render.toString
    result
    |> shouldEqual """<div aria-roledescription="custom widget">Widget</div>"""

[<Fact>]
let ``ariaRowCount attribute test`` () =
    let result = div(role = "grid", ariaRowCount = 10) { "Table" } |> Render.toString
    result |> shouldEqual """<div role="grid" aria-rowcount="10">Table</div>"""

[<Fact>]
let ``ariaRowIndex attribute test`` () =
    let result = div(role = "row", ariaRowIndex = 5) { "Row" } |> Render.toString
    result |> shouldEqual """<div role="row" aria-rowindex="5">Row</div>"""

[<Fact>]
let ``ariaRowIndexText attribute test`` () =
    let result =
        div(role = "row", ariaRowIndexText = "Row 5 of 10") { "Data" }
        |> Render.toString
    result
    |> shouldEqual """<div role="row" aria-rowindextext="Row 5 of 10">Data</div>"""

[<Fact>]
let ``ariaRowSpan attribute test`` () =
    let result = div(role = "gridcell", ariaRowSpan = 2) { "Cell" } |> Render.toString
    result |> shouldEqual """<div role="gridcell" aria-rowspan="2">Cell</div>"""

[<Fact>]
let ``ariaSelected attribute test with true`` () =
    let result =
        div(role = "option", ariaSelected = true) { "Selected" } |> Render.toString
    result
    |> shouldEqual """<div role="option" aria-selected="true">Selected</div>"""

[<Fact>]
let ``ariaSelected attribute test with false`` () =
    let result =
        div(role = "option", ariaSelected = false) { "Not selected" } |> Render.toString
    result
    |> shouldEqual """<div role="option" aria-selected="false">Not selected</div>"""

[<Fact>]
let ``ariaSetSize attribute test`` () =
    let result = div(role = "listitem", ariaSetSize = 5) { "Item" } |> Render.toString
    result |> shouldEqual """<div role="listitem" aria-setsize="5">Item</div>"""

[<Fact>]
let ``ariaSort attribute test`` () =
    let result = th(ariaSort = "ascending") { "Name" } |> Render.toString
    result |> shouldEqual """<th aria-sort="ascending">Name</th>"""

[<Fact>]
let ``ariaValueMax attribute test`` () =
    let result =
        div(role = "slider", ariaValueMax = "100") { "Slider" } |> Render.toString
    result |> shouldEqual """<div role="slider" aria-valuemax="100">Slider</div>"""

[<Fact>]
let ``ariaValueMin attribute test`` () =
    let result =
        div(role = "slider", ariaValueMin = "0") { "Slider" } |> Render.toString
    result |> shouldEqual """<div role="slider" aria-valuemin="0">Slider</div>"""

[<Fact>]
let ``ariaValueNow attribute test`` () =
    let result =
        div(role = "slider", ariaValueNow = "50") { "Slider" } |> Render.toString
    result |> shouldEqual """<div role="slider" aria-valuenow="50">Slider</div>"""

[<Fact>]
let ``ariaValueText attribute test`` () =
    let result =
        div(role = "slider", ariaValueText = "50 percent") { "Slider" }
        |> Render.toString
    result
    |> shouldEqual """<div role="slider" aria-valuetext="50 percent">Slider</div>"""

[<Fact>]
let ``multiple aria attributes combined test`` () =
    let result =
        button(role = "switch", ariaLabel = "Notifications", ariaChecked = "true", ariaDisabled = false) { "Toggle" }
        |> Render.toString
    result
    |> shouldEqual
        """<button role="switch" aria-label="Notifications" aria-checked="true" aria-disabled="false">Toggle</button>"""

[<Fact>]
let ``aria attributes with null values test`` () =
    let result =
        div(ariaLabel = null, ariaDescribedBy = null) { "Content" } |> Render.toString
    result |> shouldEqual """<div>Content</div>"""
