
Imports System.Collections.Generic
Imports System.Net
Imports System.Net.Http
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Module Module1

    ReadOnly client As HttpClient = New HttpClient()
    Sub Main()
        ' id estudiante de prueba 53, username test 100
        ' id teacher de prueba 54, username test 200
        'createUserMoodle()
        'id de categoria de prueba -> 122  ,Nombre -> Categoria de Prueba
        'createCategories()
        'id de curso de prueba -> 3  ,shortname -> CP
        'createCourses()
        'enrollUser()
        getUsers()
        'deleteUsers()

        Console.ReadKey()
    End Sub
    Async Sub getUsers()
        Try
            Dim username As String = "acasoft"

            Dim data = New Dictionary(Of String, String)
            data.Add("wstoken", "d2d9278b73b3ea3f415ab245feb5030a")
            data.Add("moodlewsrestformat", "json")
            data.Add("wsfunction", "core_user_get_users")
            data.Add("criteria[0][key]", "username")    ' busca por el username del usuario
            data.Add("criteria[0][value]", username)   ' el username del usuario que buscamos
            Dim responseBody = Await sendMoodleRequest(HttpMethod.Post, data)
            Console.WriteLine(responseBody)

            Dim json = JObject.Parse(responseBody)
            Dim users = json.Item("users")
            Dim arrayUsers = users.ToArray()

            If arrayUsers.Length = 0 Then
                For Each jsonUser In arrayUsers
                    Dim user As userResponse = JsonConvert.DeserializeObject(Of userResponse)(jsonUser.ToString())
                    Console.WriteLine("Id: " & user.id)
                    Console.WriteLine("Username: " & user.username)
                Next
            Else
                Console.WriteLine("No se encontró ningún usuario con username: " & username)
            End If




        Catch ex As Exception
            Console.WriteLine(ex)
        End Try
    End Sub
    Async Sub deleteUsers()
        Try
            Dim data = New Dictionary(Of String, String)
            data.Add("wstoken", "acebcc62c149e567969b459f3085692c")
            data.Add("moodlewsrestformat", "json")
            data.Add("wsfunction", "core_user_delete_users")
            data.Add("userids[0]", "1899")    ' id del usuario a eliminar
            Dim responseBody = Await sendMoodleRequest(HttpMethod.Post, data)
            Console.WriteLine(responseBody)

        Catch ex As Exception
            Console.WriteLine(ex)
        End Try
    End Sub
    Async Sub enrollUser()
        Try
            Dim data = New Dictionary(Of String, String)
            data.Add("wstoken", "acebcc62c149e567969b459f3085692c")
            data.Add("moodlewsrestformat", "json")
            data.Add("wsfunction", "enrol_manual_enrol_users")
            '1 manager, 2 coursecreator, 3 editingteacher, 4 teacher
            '5 student, 6 guest, 7 user, 8 frontpage
            data.Add("enrolments[0][roleid]", "4")
            data.Add("enrolments[0][userid]", "1973")       ' id del usuario
            data.Add("enrolments[0][courseid]", "152")     ' id del curso al que se matricula el usuario

            Dim baseDate = New DateTime(1970, 1, 1) ' NO BORRAR , LA FECHA DE BASE SE MANTIENE
            Dim startDate = New DateTime(2020, 1, 1) ' INICIO DEL CURSO
            Dim endDate = New DateTime(2020, 6, 30) ' FIN DEL CURSO
            Dim startDateSeconds = startDate.Subtract(baseDate).TotalSeconds ' FECHA INICIO EN SEGUNDOS
            Dim endDateSeconds = startDate.Subtract(baseDate).TotalSeconds  ' FECHA FIN EN SEGUNDOS

            data.Add("enrolments[0][timestart]", startDateSeconds)    ' inicio de la matricula
            data.Add("enrolments[0][timeend]", endDateSeconds)      ' fin de la matricula
            data.Add("enrolments[0][suspend]", "0")     ' activo 0 , suspendido 1

            Dim responseBody = Await sendMoodleRequest(HttpMethod.Post, data)
            Console.WriteLine(responseBody)
            ' aqui hay respuesta solo en caso de haber error, si todo sale bien la respuesta es un null
        Catch ex As Exception
            Console.WriteLine(ex)
        End Try
    End Sub

    Async Sub createCategories()
        Try
            Dim data = New Dictionary(Of String, String)
            data.Add("wstoken", "acebcc62c149e567969b459f3085692c")
            data.Add("moodlewsrestformat", "json")
            data.Add("wsfunction", "core_course_create_categories ")
            data.Add("categories[0][name]", "Categoria de Prueba")
            data.Add("categories[0][parent]", "0") ' aqui va el id de la categoria padre, cuando es raiz va el cero
            data.Add("categories[0][description]", "Esta es una categoria de prueba")
            data.Add("categories[0][descriptionformat]", "2") 'description format (1 = HTML, 0 = MOODLE, 2 = PLAIN or 4 = MARKDOWN)
            Dim responseBody = Await sendMoodleRequest(HttpMethod.Post, data)
            Console.WriteLine(responseBody)

            If (responseBody(0) = "[") Then
                Dim json = JObject.Parse("{ categories: " & responseBody & "}")
                Dim categories = json.Item("categories")
                Dim arrayCategories = categories.ToArray()
                For Each jsonCategory In arrayCategories
                    Dim category As categoriesResponse = JsonConvert.DeserializeObject(Of categoriesResponse)(jsonCategory.ToString())
                    Console.WriteLine("Id: " & category.id)
                    Console.WriteLine("Name: " & category.name)
                Next

            Else
                Dim err As errorResponse = JsonConvert.DeserializeObject(Of errorResponse)(responseBody)
                Console.WriteLine(err.exception)
            End If

        Catch ex As Exception
            Console.WriteLine(ex)
        End Try

    End Sub
    Async Sub createCourses()
        Try
            Dim data = New Dictionary(Of String, String)
            data.Add("wstoken", "acebcc62c149e567969b459f3085692c")
            data.Add("moodlewsrestformat", "json")
            data.Add("wsfunction", "core_course_create_courses")
            data.Add("courses[0][fullname]", "Curso de prueba") ' NOMBRE COMPLETO DEL CURSO 
            data.Add("courses[0][shortname]", "CP") ' NOMBRE CORTO DEL CURSO EJ: CCNA
            data.Add("courses[0][categoryid]", "122") ' ID DE LA CATEGORIA/SUBCATEGORIA A LA QUE PERTENECE EL CURSO

            Dim baseDate = New DateTime(1970, 1, 1) ' NO BORRAR , LA FECHA DE BASE SE MANTIENE
            Dim startDate = New DateTime(2020, 1, 1) ' INICIO DEL CURSO
            Dim endDate = New DateTime(2020, 6, 30) ' FIN DEL CURSO
            Dim startDateSeconds = startDate.Subtract(baseDate).TotalSeconds ' FECHA INICIO EN SEGUNDOS
            Dim endDateSeconds = startDate.Subtract(baseDate).TotalSeconds  ' FECHA FIN EN SEGUNDOS

            data.Add("courses[0][startdate]", startDateSeconds)
            data.Add("courses[0][enddate]", endDateSeconds)

            Dim responseBody = Await sendMoodleRequest(HttpMethod.Post, data)
            Console.WriteLine(responseBody)

            If (responseBody(0) = "[") Then
                Dim json = JObject.Parse("{ courses: " & responseBody & "}")
                Dim courses = json.Item("courses")
                Dim arrayCourses = courses.ToArray()
                For Each jsonCourse In arrayCourses
                    Dim course As courseResponse = JsonConvert.DeserializeObject(Of courseResponse)(jsonCourse.ToString())
                    Console.WriteLine("Id: " & course.id)
                    Console.WriteLine("Shortname: " & course.shortname)
                Next

            Else
                Dim err As errorResponse = JsonConvert.DeserializeObject(Of errorResponse)(responseBody)
                Console.WriteLine(err.exception)
            End If

        Catch ex As Exception
            Console.WriteLine(ex)
        End Try
    End Sub
    Async Sub createUserMoodle()
        Try
            Dim data = New Dictionary(Of String, String)
            data.Add("wstoken", "acebcc62c149e567969b459f3085692c")
            data.Add("moodlewsrestformat", "json")
            data.Add("wsfunction", "core_user_create_users")
            data.Add("users[0][username]", "test200")
            data.Add("users[0][password]", "%Est1234")
            data.Add("users[0][firstname]", "test200")
            data.Add("users[0][lastname]", "test200")
            data.Add("users[0][email]", "test200@test.com")
            Dim responseBody As String = Await sendMoodleRequest(HttpMethod.Post, data)
            Console.WriteLine(responseBody)

            If (responseBody(0) = "[") Then
                Dim json = JObject.Parse("{ users: " & responseBody & "}")
                Dim users = json.Item("users")
                Dim arrayUsers = users.ToArray()
                For Each jsonUser In arrayUsers
                    Dim user As userResponse = JsonConvert.DeserializeObject(Of userResponse)(jsonUser.ToString())
                    Console.WriteLine("Id: " & user.id)
                    Console.WriteLine("Username: " & user.username)
                Next
            Else
                Dim err As errorResponse = JsonConvert.DeserializeObject(Of errorResponse)(responseBody)
                Console.WriteLine(err.exception)
            End If

        Catch ex As Exception
            Console.WriteLine(ex)
        End Try
    End Sub
    Async Function sendMoodleRequest(method As HttpMethod, data As Dictionary(Of String, String)) As Task(Of String)

        Dim url As String = "https://abelardomoncayo.edu.ec/eva/webservice/rest/server.php"
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
        Dim req = New HttpRequestMessage(method, url)
        req.Content = New FormUrlEncodedContent(data)
        Dim response As HttpResponseMessage = Await client.SendAsync(req)
        response.EnsureSuccessStatusCode()
        Dim responseBody As String = Await response.Content.ReadAsStringAsync()
        Return responseBody

    End Function

    Class errorResponse
        Public exception As String
        Public errorcode As String
        Public message As String
    End Class
    Class userResponse
        Public id As Integer
        Public username As String
    End Class
    Class categoriesResponse
        Public id As Integer
        Public name As String
    End Class
    Class courseResponse
        Public id As Integer
        Public shortname As String
    End Class
End Module
