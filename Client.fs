namespace MidPrjFuji

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Templating
open WebSharper.Sitelets
open WebSharper.UI.Notation
open WebSharper.Charting
open WebSharper.UI.Html

// The templates are loaded from the DOM, so you just can edit index.html
// and refresh your browser, no need to recompile unless you add or remove holes.
type IndexTemplate = Template<"wwwroot/index.html", ClientLoad.FromDocument>

type EndPoint =
    | [<EndPoint "/">] Home
    | [<EndPoint "/page1">] Page1


[<JavaScript>]
module Pages =
    open WebSharper.UI.Notation

    let newMonth = Var.Create ""
    //let newIncome = Var.Create ""
    let newSalary = Var.Create ""
    let newUtility = Var.Create ""
    let newFood = Var.Create ""
    let newBalance = Var.Create ""

    let Balance =
        ListModel.FromSeq [
            ("January", "500000", "150000","100000", "250000")
            ("Febrary", "500000", "142000", "98000","260000")
            ("March", "500000", "120000", "90000","290000")
            ("April", "500000", "100000", "100000","300000")
       ]

    let HomePage() =
        //div [] [text "this is the homepage"]


       IndexTemplate.HomePage()
            .ListContainer(
                Balance.View.DocSeqCached(fun (month: string, salary: string, utility: string, food: string, balance: string) ->
                    IndexTemplate.ListItem().Month(month).Salary(salary).Utility(utility).Food(food).Balance(balance).Doc()
                )
            )
            .Month(newMonth)
            .Salary(newSalary)
            .Utility(newUtility)
            .Food(newFood)
            .Add(fun _ ->
                let balance = int newSalary.Value - int newUtility.Value - int newFood.Value
                newBalance.Value <- string balance
                Balance.Add(newMonth.Value, newSalary.Value, newUtility.Value, newFood.Value, newBalance.Value)
                // –_ƒOƒ‰ƒt
                //let ct = Pages.counter.Value
                let month = "test"  //Pages.newMonth.Value
                let labels =
                    [| "January"; "Febrary"; "March"; "April"; newMonth.Value|]
                let dataset1 = [|float 250000; 260000; 290000; 300000; int newBalance.Value|]
    
                let chart =
                    Chart.Combine [
                        Chart.Bar(Array.zip labels dataset1)
                            .WithFillColor(Color.Rgba(151, 187, 205, 0.2))
                            .WithStrokeColor(Color.Name "blue")
                    ]
                 // Clear the existing chart before rendering a new one
                JS.Window.Document.GetElementById("Chart1").InnerHTML <- ""

                Renderers.ChartJs.Render(chart, Size = Size(600, 300))
                |> Doc.RunAppendById "Chart1"
            )
            //.Balance(newBalance.Value)
            .Doc()
    
    //open System


    let strage = JS.Window.LocalStorage
    let counter = 
        let curr = strage.GetItem "counter"
        if curr = "" then
            0
        else
            int curr
        |> Var.Create

    let Stock =
        ListModel.FromSeq [
            ("a company", "0.5")
            ("b company", "1.0")
            ("c company", "0.5")
       ]

    let newCompany = Var.Create ""
    let newPrice = Var.Create ""

    let Page1() =
        IndexTemplate.Page1()
            .ListContainer2(
                Stock.View.DocSeqCached(fun (company: string, price: string) ->
                    IndexTemplate.ListItem2().Company(company).Price(price).Doc()
                )
            )
            .Company(newCompany)
            //.Salary(newSalary)

            .Doc()

[<JavaScript>]
module App =
    open WebSharper.UI.Notation
    open WebSharper.JavaScript

    let router = Router.Infer<EndPoint>()
    let currentPage = Router.InstallHash Home router

    [<SPAEntryPoint>]
    let Main () =

        let renderInnerPage (currentPage: Var<EndPoint>) =
            currentPage.View.Map(fun endpoint ->
                match endpoint with
                | Home   -> Pages.HomePage()
                | Page1  -> Pages.Page1()
            )
            |> Doc.EmbedView
        IndexTemplate()
            .Url_Home("/")
            .Url_Page1("/#/page1")
            .MainContainer(renderInnerPage currentPage)
            .Bind()
        // –_ƒOƒ‰ƒt
        let ct = Pages.counter.Value
        let month = "test"  //Pages.newMonth.Value
        let labels =
            [| "January"; "Febrary"; "March"; "April"; "You"|]
        let dataset1 = [|float 250000; 260000; 290000; 300000; 0|]
    
        let chart =
            Chart.Combine [
                Chart.Bar(Array.zip labels dataset1)
                    .WithFillColor(Color.Rgba(151, 187, 205, 0.2))
                    .WithStrokeColor(Color.Name "blue")
            ]
    
        Renderers.ChartJs.Render(chart, Size = Size(600, 300))
        |> Doc.RunAppendById "Chart1"

