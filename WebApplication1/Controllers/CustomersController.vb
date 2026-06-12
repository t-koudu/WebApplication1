Imports System.Web.Http

Public Class CustomersController
    Inherits ApiController

    ' GET api/<controller>
    Public Function GetValues() As List(Of CustomerDto)
        Dim repository As New CustomersRepository()
        Return repository.FindAll()
    End Function
End Class
