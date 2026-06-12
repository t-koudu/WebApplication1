Imports System.Net
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Web.Http
Imports WebGrease
<RoutePrefix("api/reports")>
Public Class ReportsController
    Inherits ApiController

    ' GET api/<controller>/a
    <HttpGet>
    <Route("{id}")>
    Public Function GetValue(ByVal id As String) As List(Of OrderDto)
        Dim repository As New OrderRepository()
        Return repository.Find(id)
    End Function

    ' GET /api/reports/csv/ORD0001
    <HttpGet>
    <Route("csv/{id}")>
    Public Function GetCsv(ByVal id As String) As HttpResponseMessage
        ' データ取得
        Dim repository As New OrderRepository()
        Dim items = repository.Find(id)

        Dim sb As New StringBuilder()
        ' ヘッダ
        sb.AppendLine("OrderNo,OrderDate,CustomerCode,CustomerName,Address1,ProductCode,ProductName,Quantity,UnitPrice,Amount")
        For Each item In items
            sb.AppendLine(
                String.Format(
                    "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}",
                    item.OrderNo,
                    item.OrderDate,
                    item.CustomerCode,
                    item.CustomerName,
                    item.Address1,
                    item.ProductCode,
                    item.ProductName,
                    item.Quantity,
                    item.UnitPrice,
                    item.Amount
                )
            )
        Next

        Dim response As New HttpResponseMessage(HttpStatusCode.OK)
        response.Content = New StringContent(sb.ToString(), Text.Encoding.UTF8, "text/csv")
        response.Content.Headers.ContentDisposition =
        New ContentDispositionHeaderValue("attachment") With {
            .FileName = "report.csv"
        }

        Return response
    End Function

End Class
