Imports System.Net
Imports System.Net.Http
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Module Sindicato

    Dim tokenSindicato = ""
    Async Sub getPreinscripciones()
        Try
            Dim fecha = "2021-01-27" 'formato año - mes -dia
            Dim url = "https://grupoprosoft.net/sindicato-api/public/index.php/api/v1/preinscripciones?_fecha=" & fecha
            ' Si quieres todas utiliza la URL de abajo 
            'Dim url = "https://grupoprosoft.net/sindicato-api/public/index.php/api/v1/preinscripciones?_fecha="
            Dim data = New Dictionary(Of String, String)
            Dim responseBody = Await sendSindicatoRequest(HttpMethod.Get, url, data)
            Dim json = JObject.Parse(responseBody)
            Dim inscripciones = json.Item("data")
            Console.WriteLine(inscripciones)
            Dim arrayIns = inscripciones.ToArray()

            Dim listaInscripciones = New List(Of Inscripcion)

            If arrayIns.Length > 0 Then
                For Each jsonIns In arrayIns
                    Dim ins As Inscripcion = JsonConvert.DeserializeObject(Of Inscripcion)(jsonIns.ToString())
                    listaInscripciones.Add(ins)
                Next
            Else
                Console.WriteLine("No se encontró inscripciones")
            End If
            Console.WriteLine(json)
        Catch ex As Exception
            Console.WriteLine(ex)
        End Try
    End Sub
    Async Sub getToken()
        Try
            Dim url = "https://grupoprosoft.net/sindicato-api/public/index.php/api/user/login_check"
            Dim data = New Dictionary(Of String, String)
            data.Add("_username", "admin777")
            data.Add("_password", "xxxx4444")
            Dim responseBody = Await sendSindicatoRequest(HttpMethod.Post, url, data)
            Dim json = JObject.Parse(responseBody)
            Dim token = json.Item("token")
            If token.ToString().Length > 0 Then
                tokenSindicato = token
            End If
            Console.WriteLine(token)
            Console.ReadKey()
        Catch ex As Exception
            Console.WriteLine(ex)
        End Try
    End Sub
    Async Function sendSindicatoRequest(method As HttpMethod, url As String, data As Dictionary(Of String, String)) As Task(Of String)

        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
        Dim req = New HttpRequestMessage(method, url)
        If method = HttpMethod.Post Then
            req.Content = New FormUrlEncodedContent(data)
        End If

        If tokenSindicato <> "" Then
            req.Headers.Add("Authorization", "Bearer " & tokenSindicato)
        End If

        Dim response As HttpResponseMessage = Await client.SendAsync(req)
        response.EnsureSuccessStatusCode()
        Dim responseBody As String = Await response.Content.ReadAsStringAsync()
        Return responseBody

    End Function
    Class Inscripcion
        Public usuario As Usuario
        Public productoServicio As Servicio
    End Class
    Class Usuario
        Public nombres As String
        Public apellidos As String
        ' aqui van los demas campos correo , direccion  , etc aumenta los que necesites
        'email
        'username
        'cedula
        'nacionalidad
        'lugarNac
        'direccion
        'calle
        'calle2
        'referencia
        'telefono
        'fechaNac

    End Class
    Class Servicio
        Public precio As Decimal
        Public curso As Curso
        Public requisitos As List(Of Requisito)
    End Class
    Class Curso
        Public nombre As String
    End Class
    Class Requisito
        Public descripcion As String
    End Class
End Module
