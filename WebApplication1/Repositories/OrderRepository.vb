Imports System.Data.SqlClient

Public Class OrderRepository
    Public Sub New()
    End Sub

    Public Function Find(ByRef orderNo As String) As List(Of OrderDto)
        Dim result As New List(Of OrderDto)
        Dim connStr As String = ConfigurationManager.ConnectionStrings("SampleDb").ConnectionString
        Using conn As New SqlConnection(connStr)
            conn.Open()
            Dim sql As String = "SELECT
                                    oh.OrderNo AS OrderNo,
                                    oh.OrderDate AS OrderDate,
                                    c.CustomerCode AS CustomerCode,
                                    c.CustomerName AS CustomerName,
                                    c.Address1 AS Address1,
                                    p.ProductCode AS ProductCode,
                                    p.ProductName AS ProductName,
                                    od.Quantity AS Quantity,
                                    od.UnitPrice AS UnitPrice,
                                    od.Quantity * od.UnitPrice AS Amount
                                FROM OrderHeaders oh
                                INNER JOIN Customers c
                                    ON c.CustomerId = oh.CustomerId
                                INNER JOIN OrderDetails od
                                    ON od.OrderId = oh.OrderId
                                INNER JOIN Products p
                                    ON p.ProductId = od.ProductId
                                WHERE oh.OrderNo = @OrderNo
                                ORDER BY od.OrderDetailId;"

            Using cmd As New SqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@OrderNo", orderNo)
                Using reader = cmd.ExecuteReader()
                    While reader.Read()
                        result.Add(New OrderDto With {
                        .OrderNo = reader("OrderNo").ToString(),
                        .OrderDate = reader("OrderDate").ToString(),
                        .CustomerCode = reader("CustomerCode").ToString(),
                        .CustomerName = reader("CustomerName").ToString(),
                        .Address1 = reader("Address1").ToString(),
                        .ProductCode = reader("ProductCode").ToString(),
                        .ProductName = reader("ProductName").ToString(),
                        .Quantity = reader("Quantity").ToString(),
                        .UnitPrice = reader("UnitPrice").ToString(),
                        .Amount = reader("Amount").ToString()
                               })
                    End While
                End Using
            End Using
        End Using
        Return result
    End Function

End Class
