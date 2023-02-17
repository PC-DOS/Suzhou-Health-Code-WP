''' <summary>
''' 提供字串資源的存取權。
''' </summary>
Public Class LocalizedStrings
    Public Sub New()
    End Sub

    Private Shared _localizedResources As New AppResources()

    Public ReadOnly Property LocalizedResources() As AppResources
        Get
            Return _localizedResources
        End Get
    End Property
End Class