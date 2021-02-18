Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports System.Windows.Forms
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq


Module Sindicato

    Dim tokenSindicato = ""
    Async Sub getPreinscripciones()
        Try
            Dim fecha = "2021-01-27" 'formato año - mes -dia
            Dim id = 7 ' id de la inscripcion
            'Dim url = "https://grupoprosoft.net/sindicato-api/public/index.php/api/v1/preinscripciones?_fecha=" & fecha
            ' Si quieres todas utiliza la URL de abajo 
            'Dim url = "https://grupoprosoft.net/sindicato-api/public/index.php/api/v1/preinscripciones?_fecha="
            'Obtener inscripcion por id
            'Dim url = "https://grupoprosoft.net/sindicato-api/public/index.php/api/v1/preinscripciones?_id=" & id

            Dim url = "http://localhost:8000/api/v1/preinscripciones?_id=" & id

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
            'Console.WriteLine(json)
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

    Async Sub downloadFile()
        Try
            Dim id = 1
            'Dim url = "https://grupoprosoft.net/sindicato-api/public/index.php/api/user/login_check"
            Dim url = "http://localhost:8000/api/v1/documentos/download?id=" & id
            Dim data = New Dictionary(Of String, String)
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
            Dim req = New HttpRequestMessage(HttpMethod.Get, url)

            If tokenSindicato <> "" Then
                req.Headers.Add("Authorization", "Bearer " & tokenSindicato)
            End If
            Dim response As HttpResponseMessage = Await client.SendAsync(req)
            response.EnsureSuccessStatusCode()

            Dim cd = response.Content.Headers.ContentDisposition
            If (IsNothing(cd)) Then
                Dim responseBody As String = Await response.Content.ReadAsStringAsync()
                Dim json = JObject.Parse(responseBody)
                Console.WriteLine(json)
            Else
                Dim filename = cd.FileName
                Dim path = Environment.GetEnvironmentVariable("USERPROFILE") & "\\Downloads\" & filename
                Dim responseBody As Byte() = Await response.Content.ReadAsByteArrayAsync()

                Dim fs As FileStream = File.Create(path, responseBody.Length)
                fs.Write(responseBody, 0, responseBody.Length)
                fs.Close()

                Console.Write("Archivo Guardado Correctamente en " & path)
            End If

        Catch ex As Exception
            Console.WriteLine(ex)
        End Try
    End Sub

End Module
