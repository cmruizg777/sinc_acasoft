Module ModeloSindicato
    Class Inscripcion
        Public id As Integer
        Public observaciones As String
        Public usuario As Usuario
        Public productoServicio As Servicio
        Public documentos As List(Of Documento)
    End Class
    Class Usuario
        Public nombres As String
        Public apellidos As String
        ' aqui van los demas campos correo , direccion  , etc aumenta los que necesites
        Public email As String
        Public username As String
        Public cedula As String
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
        Public examen As Examen
        Public tipo As Integer
        Public observaciones As String
    End Class
    Class Curso
        Public nombre As String
    End Class
    Class Requisito
        Public descripcion As String
    End Class
    Class Examen
        Public nombre As String
    End Class
    Class Documento
        Public id As Integer
        Public tieneAdjunto As Boolean
        Public requisito As Requisito
    End Class
End Module
