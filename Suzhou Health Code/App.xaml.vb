Imports System.Diagnostics
Imports System.Resources
Imports System.Windows.Markup

Partial Public Class App
    Inherits Application

    ''' <summary>
    ''' 提供簡單的方法，來存取電話應用程式的根畫面。
    ''' </summary>
    ''' <returns>電話應用程式的根畫面。</returns>
    Public Shared Property RootFrame As PhoneApplicationFrame

    ''' <summary>
    ''' Application 物件的建構函式。
    ''' </summary>
    Public Sub New()
        ' 標準 XAML 初始化
        InitializeComponent()

        ' 電話特有初始化
        InitializePhoneApplication()

        ' 語言顯示初始化
        InitializeLanguage()

        ' 偵錯時顯示圖形分析資訊。
        If Debugger.IsAttached Then
            ' 顯示目前的畫面播放速率計數器。
            Application.Current.Host.Settings.EnableFrameRateCounter = True

            ' 顯示每個畫面中重新繪製的應用程式區域。
            'Application.Current.Host.Settings.EnableRedrawRegions = True

            ' 啟用非實際執行分析視覺化模式，
            ' 用彩色重疊在頁面上顯示交給 GPU 的區域。
            'Application.Current.Host.Settings.EnableCacheVisualization = True


            ' 防止螢幕在執行偵錯工具時關閉，方法是停用
            ' 應用程式的閒置偵測。
            ' 注意: 這項功能僅限偵錯模式使用。當使用者不使用電話時，停用使用者閒置偵測
            ' 的應用程式仍會繼續執行，並消耗電池的電力。
            PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled
        End If
    End Sub

    ' 啟動應用程式 (例如，從 [開始]) 時要執行的程式碼
    ' 重新啟動應用程式時不會執行這段程式碼
    Private Sub Application_Launching(ByVal sender As Object, ByVal e As LaunchingEventArgs)
    End Sub

    ' 啟動應用程式 (帶到前景) 時要執行的程式碼
    ' 第一次啟動應用程式時不會執行這段程式碼
    Private Sub Application_Activated(ByVal sender As Object, ByVal e As ActivatedEventArgs)
    End Sub

    ' 停用應用程式 (移到背景) 時要執行的程式碼
    ' 關閉應用程式時不會執行這段程式碼
    Private Sub Application_Deactivated(ByVal sender As Object, ByVal e As DeactivatedEventArgs)
    End Sub

    ' 關閉應用程式 (例如，使用者按 [上一頁]) 時要執行的程式碼
    ' 停用應用程式時不會執行這段程式碼
    Private Sub Application_Closing(ByVal sender As Object, ByVal e As ClosingEventArgs)
    End Sub

    ' 巡覽失敗時要執行的程式碼
    Private Sub RootFrame_NavigationFailed(ByVal sender As Object, ByVal e As NavigationFailedEventArgs)
        If Diagnostics.Debugger.IsAttached Then
            ' 巡覽失敗; 切換到偵錯工具
            Diagnostics.Debugger.Break()
        End If
    End Sub

    Public Sub Application_UnhandledException(ByVal sender As Object, ByVal e As ApplicationUnhandledExceptionEventArgs) Handles Me.UnhandledException

        ' 偵錯時顯示圖形分析資訊。
        If Diagnostics.Debugger.IsAttached Then
            Diagnostics.Debugger.Break()
        Else
            e.Handled = True
            MessageBox.Show(e.ExceptionObject.Message & Environment.NewLine & e.ExceptionObject.StackTrace,
                            "錯誤", MessageBoxButton.OK)
        End If
    End Sub

#Region "電話應用程式初始化"
    ' 避免重複初始化
    Private phoneApplicationInitialized As Boolean = False

    ' 請勿在這個方法中加入任何其他程式碼
    Private Sub InitializePhoneApplication()
        If phoneApplicationInitialized Then
            Return
        End If

        ' 建立畫面，但還不將它設為 RootVisual; 這樣可以讓啟動顯示
        ' 畫面保持作用中狀態，直到應用程式準備好呈現為止。
        RootFrame = New PhoneApplicationFrame()
        AddHandler RootFrame.Navigated, AddressOf CompleteInitializePhoneApplication

        ' 處理巡覽失敗
        AddHandler RootFrame.NavigationFailed, AddressOf RootFrame_NavigationFailed

        '處理清除 backstack 的重設要求
        AddHandler RootFrame.Navigated, AddressOf CheckForResetNavigation

        ' 確定不會重新初始化
        phoneApplicationInitialized = True
    End Sub

    ' 請勿在這個方法中加入任何其他程式碼
    Private Sub CompleteInitializePhoneApplication(ByVal sender As Object, ByVal e As NavigationEventArgs)
        ' 設定根 Visual，使應用程式能夠呈現
        If RootVisual IsNot RootFrame Then
            RootVisual = RootFrame
        End If

        ' 移除這個處理常式，因為已不再需要
        RemoveHandler RootFrame.Navigated, AddressOf CompleteInitializePhoneApplication
    End Sub

    Private Sub CheckForResetNavigation(ByVal sender As Object, ByVal e As NavigationEventArgs)
        ' 如果應用程式收到 '重設' 巡覽，則我們需要檢查
        ' 在下一次巡覽中查看頁面堆疊是否應該重設
        If e.NavigationMode = NavigationMode.Reset Then
            AddHandler RootFrame.Navigated, AddressOf ClearBackStackAfterReset
        End If
    End Sub

    Private Sub ClearBackStackAfterReset(ByVal sender As Object, ByVal e As NavigationEventArgs)
        ' 移除註冊事件，使它不會再次被呼叫
        RemoveHandler RootFrame.Navigated, AddressOf ClearBackStackAfterReset

        ' 只清除 '新' (下一頁) 和 '重新整理' 巡覽的堆疊
        If e.NavigationMode <> NavigationMode.New And e.NavigationMode <> NavigationMode.Refresh Then
            Return
        End If

        ' 為了 UI 的一致性，清除整個頁面堆疊
        While Not RootFrame.RemoveBackEntry() Is Nothing
            ' 不執行任何動作
        End While
    End Sub
#End Region

    ' 依照應用程式的當地語系化資源字串的定義，將應用程式的字型和文字方向初始化。
    '
    ' 為了確保應用程式的字型與其支援的語言一致，並確保
    ' 這些語言的 FlowDirection 都是依照其傳統方向，ResourceLanguage
    ' 且 ResourceFlowDirection 應該在每個 resx 檔案中初始化，以符合這些值
    ' 檔案的文化特性。例如: 
    '
    ' AppResources.es-ES.resx
    '    ResourceLanguage 的值必須是 "es-ES"
    '    ResourceFlowDirection 的值必須是 "LeftToRight"
    '
    ' AppResources.ar-SA.resx
    '     ResourceLanguage 的值必須是 "ar-SA"
    '     ResourceFlowDirection 的值必須是 "RightToLeft"
    '
    ' 如需將 Windows Phone 應用程式當地語系化的詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=262072。
    '
    Private Sub InitializeLanguage()
        Try
            ' 設定字型以符合顯示語言，該顯示語言設定於
            ' 每個支援的語言的 ResourceLanguage 資源字串。
            '
            ' 回復為中性語言的字型，如果顯示
            ' 不支援電話的語言。
            '
            ' 如果發生編譯器錯誤，則表示以下項目中將遺漏 ResourceLanguage
            ' 資源檔。
            RootFrame.Language = XmlLanguage.GetLanguage(AppResources.ResourceLanguage)

            ' 設定根畫面下的所有項目的 FlowDirection
            ' 在 ResourceFlowDirection 資源字串上，相對於每個
            ' 支援的語言。
            '
            ' 如果發生編譯器錯誤，則表示以下項目中將遺漏 ResourceFlowDirection
            ' 資源檔。
            Dim flow As FlowDirection = DirectCast([Enum].Parse(GetType(FlowDirection), AppResources.ResourceFlowDirection), FlowDirection)
            RootFrame.FlowDirection = flow
        Catch
            ' 如果在這裡攔截到例外狀況，最有可能是因為
            ' ResourceLangauge 未被正確地設定為支援的語言
            ' 代碼，或 ResourceFlowDirection 設定的值不是 LeftToRight
            ' 或 RightToLeft。

            If Debugger.IsAttached Then
                Debugger.Break()
            End If

            Throw
        End Try
    End Sub

End Class