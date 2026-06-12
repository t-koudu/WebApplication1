Imports System.Data.SqlClient

Public Class CustomersRepository

    Public Function FindAll() As List(Of CustomerDto)
        Dim result As New List(Of CustomerDto)
        Dim connStr As String = ConfigurationManager.ConnectionStrings("SampleDb").ConnectionString
        Using conn As New SqlConnection(connStr)
            conn.Open()
            Dim sql As String = "SELECT CustomerId,CustomerCode,CustomerName,Address1,Tel FROM Customers"
            Using cmd As New SqlCommand(sql, conn)
                Using reader = cmd.ExecuteReader()
                    While reader.Read()
                        result.Add(New CustomerDto With {
                        .CustomerId = reader("CustomerId").ToString(),
                        .CustomerCode = reader("CustomerCode").ToString(),
                        .CustomerName = reader("CustomerName").ToString(),
                        .Address1 = reader("Address1").ToString(),
                        .Tel = reader("Tel").ToString()
                               })
                    End While
                End Using
            End Using
        End Using
        Return result
    End Function
End Class