Imports System
Imports System.Threading
Imports System.Windows.Controls
Imports Microsoft.Phone.Controls
Imports Microsoft.Phone.Shell
Imports Coding4Fun.Toolkit
Imports Coding4Fun.Toolkit.Controls
Imports Coding4Fun.Toolkit.Controls.InputPrompt
Imports System.Windows.Input
Imports Coding4Fun.Toolkit.Controls.UserPrompt
Imports System.IO
Imports Microsoft.Phone.Tasks
Imports Microsoft.Phone.Media
Imports System.Windows.Media.Imaging

Partial Public Class MainPage
    Inherits PhoneApplicationPage
    Const NameKey As String = "Name"
    Const IDNumberKey As String = "IDNumber"
    Const LivingPlaceKey As String = "LivingPlace"
    Const WorkplaceKey As String = "Workplace"
    Dim AppSettings As IO.IsolatedStorage.IsolatedStorageSettings
    Dim isoFileSystem As IO.IsolatedStorage.IsolatedStorageFile
    ' 建構函式
    Public Sub New()
        InitializeComponent()

        SupportedOrientations = SupportedPageOrientation.Portrait Or SupportedPageOrientation.Landscape

        AppSettings = IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings

        isoFileSystem = IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication()
        ' 將 ApplicationBar 當地語系化的程式碼範例
        'BuildLocalizedApplicationBar()

    End Sub

    Private Sub MainPage_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Dim LastUpdateTime As New Date
        LastUpdateTime = Date.Now
        txtUpdateTime.Text = "更新于" & LastUpdateTime.Year & "-" & String.Format("{0:D2}", LastUpdateTime.Month) & "-" & String.Format("{0:D2}", LastUpdateTime.Day) & " " & String.Format("{0:D2}", LastUpdateTime.Hour) & ":" & String.Format("{0:D2}", LastUpdateTime.Minute) & ":" & String.Format("{0:D2}", LastUpdateTime.Second)
        If AppSettings.Contains(NameKey) Then
            txtName.Text = AppSettings(NameKey)
        Else
            txtName.Text = "请输入姓名"
        End If
        If AppSettings.Contains(IDNumberKey) Then
            txtIDNumber.Text = AppSettings(IDNumberKey)
        Else
            txtIDNumber.Text = "0000 0000 0000 0000 00"
        End If
        If AppSettings.Contains(LivingPlaceKey) Then
            txtLivingPlace.Text = AppSettings(LivingPlaceKey)
        Else
            txtLivingPlace.Text = "请输入居住地"
        End If
        If AppSettings.Contains(WorkplaceKey) Then
            txtWorkPlace.Text = AppSettings(WorkplaceKey)
        Else
            txtWorkPlace.Text = "请输入工作单位"
        End If
        If isoFileSystem.FileExists("/Photo.jpg") Then
            Dim isoFile As IsolatedStorage.IsolatedStorageFile
            isoFile = IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication
            Dim PhotoUriSource As Imaging.BitmapImage
            PhotoUriSource = New BitmapImage
            Dim fs As IsolatedStorage.IsolatedStorageFileStream
            fs = New IsolatedStorage.IsolatedStorageFileStream("/Photo.jpg", FileMode.Open, isoFile)
            PhotoUriSource.SetSource(fs)
            imgPhoto.Source = PhotoUriSource
            fs.Close()
        End If
    End Sub
    ' 建置當地語系化 ApplicationBar 的程式碼範例
    'Private Sub BuildLocalizedApplicationBar()
    '    ' 將頁面的 ApplicationBar 設定為 ApplicationBar 的新執行個體。
    '    ApplicationBar = New ApplicationBar()

    '    ' 建立新的按鈕並將文字值設定為 AppResources 的當地語系化字串。
    '    Dim appBarButton As New ApplicationBarIconButton(New Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative))
    '    appBarButton.Text = AppResources.AppBarButtonText
    '    ApplicationBar.Buttons.Add(appBarButton)

    '    ' 用 AppResources 的當地語系化字串建立新的功能表項目。
    '    Dim appBarMenuItem As New ApplicationBarMenuItem(AppResources.AppBarMenuItemText)
    '    ApplicationBar.MenuItems.Add(appBarMenuItem)
    'End Sub

    Private Sub txtName_Tap(sender As Object, e As GestureEventArgs) Handles txtName.Tap
        Dim InputBoxEx As InputPrompt
        InputBoxEx = New InputPrompt
        With InputBoxEx
            AddHandler .Completed, AddressOf NameInputPrompt_Completed
            .Title = "请输入姓名"
            .Message = "请输入您的姓名。"
            .Show()
        End With
    End Sub
    Sub NameInputPrompt_Completed(lpSender As Object, e As PopUpEventArgs(Of String, PopUpResult))
        If e.PopUpResult = PopUpResult.Ok Then
            txtName.Text = e.Result
            AppSettings(NameKey) = e.Result
            AppSettings.Save()
        End If
    End Sub

    Private Sub txtIDNumber_Tap(sender As Object, e As GestureEventArgs) Handles txtIDNumber.Tap
        Dim InputBoxEx As InputPrompt
        InputBoxEx = New InputPrompt
        With InputBoxEx
            AddHandler .Completed, AddressOf IDNumberInputPrompt_Completed
            .Title = "请输入身份证号"
            .Message = "请输入您的身份证号。"
            .Show()
        End With
    End Sub
    Sub IDNumberInputPrompt_Completed(lpSender As Object, e As PopUpEventArgs(Of String, PopUpResult))
        If e.PopUpResult = PopUpResult.Ok Then
            txtIDNumber.Text = e.Result
            AppSettings(IDNumberKey) = e.Result
            AppSettings.Save()
        End If
    End Sub

    Private Sub txtLivingPlace_Tap(sender As Object, e As GestureEventArgs) Handles txtLivingPlace.Tap
        Dim InputBoxEx As InputPrompt
        InputBoxEx = New InputPrompt
        With InputBoxEx
            AddHandler .Completed, AddressOf LivingPlaceInputPrompt_Completed
            .Title = "请输入居住地"
            .Message = "请输入您的居住地。"
            .Show()
        End With
    End Sub
    Sub LivingPlaceInputPrompt_Completed(lpSender As Object, e As PopUpEventArgs(Of String, PopUpResult))
        If e.PopUpResult = PopUpResult.Ok Then
            txtLivingPlace.Text = e.Result
            AppSettings(LivingPlaceKey) = e.Result
            AppSettings.Save()
        End If
    End Sub

    Private Sub txtWorkPlace_Tap(sender As Object, e As GestureEventArgs) Handles txtWorkPlace.Tap
        Dim InputBoxEx As InputPrompt
        InputBoxEx = New InputPrompt
        With InputBoxEx
            AddHandler .Completed, AddressOf WorkplaceInputPrompt_Completed
            .Title = "请输入工作单位"
            .Message = "请输入您的工作单位。"
            .Show()
        End With
    End Sub
    Sub WorkplaceInputPrompt_Completed(lpSender As Object, e As PopUpEventArgs(Of String, PopUpResult))
        If e.PopUpResult = PopUpResult.Ok Then
            txtWorkPlace.Text = e.Result
            AppSettings(WorkplaceKey) = e.Result
            AppSettings.Save()
        End If
    End Sub

    Private Sub imgPhoto_Tap(sender As Object, e As GestureEventArgs) Handles imgPhoto.Tap
        Dim ImgChoose As Microsoft.Phone.Tasks.PhotoChooserTask
        ImgChoose = New PhotoChooserTask
        ImgChoose.ShowCamera = True
        AddHandler ImgChoose.Completed, AddressOf ImgChoose_Copmleted
        ImgChoose.Show()
    End Sub
    Sub ImgChoose_Copmleted(lpSender As Object, lpResult As PhotoResult)
        If lpResult.TaskResult = TaskResult.OK Then
            Dim imgSrcMI As BitmapImage
            imgSrcMI = New BitmapImage
            imgSrcMI.SetSource(lpResult.ChosenPhoto)
            Dim bmp As WriteableBitmap
            bmp = New WriteableBitmap(imgSrcMI)
            bmp.SetSource(lpResult.ChosenPhoto)
            Dim IsoFile As IO.IsolatedStorage.IsolatedStorageFile
            IsoFile = IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication
            If IsoFile.FileExists("Photo.jpg") Then
                IsoFile.DeleteFile("Photo.jpg")
            End If
            Dim PhotoStream As IO.IsolatedStorage.IsolatedStorageFileStream
            PhotoStream = IsoFile.CreateFile("Photo.jpg")
            Extensions.SaveJpeg(bmp, PhotoStream, bmp.PixelWidth, bmp.PixelHeight, 0, 100)
            PhotoStream.Close()
            Dim PhotoUriSource As Imaging.BitmapImage
            PhotoUriSource = New BitmapImage
            PhotoUriSource.UriSource = New Uri("isostore:/Photo.jpg", UriKind.RelativeOrAbsolute)
            imgPhoto.Source = PhotoUriSource
            imgPhoto.UpdateLayout()
            Dim fs As IsolatedStorage.IsolatedStorageFileStream
            fs = New IsolatedStorage.IsolatedStorageFileStream("/Photo.jpg", FileMode.Open, IsoFile)
            PhotoUriSource.SetSource(fs)
            imgPhoto.Source = PhotoUriSource
            fs.Close()
        End If
    End Sub

    Private Sub txtUpdateTime_Tap(sender As Object, e As GestureEventArgs) Handles txtUpdateTime.Tap
        Dim LastUpdateTime As New Date
        LastUpdateTime = Date.Now
        txtUpdateTime.Text = "更新于" & LastUpdateTime.Year & "-" & String.Format("{0:D2}", LastUpdateTime.Month) & "-" & String.Format("{0:D2}", LastUpdateTime.Day) & " " & String.Format("{0:D2}", LastUpdateTime.Hour) & ":" & String.Format("{0:D2}", LastUpdateTime.Minute) & ":" & String.Format("{0:D2}", LastUpdateTime.Second)
    End Sub

    Private Sub imgRegresh_Tap(sender As Object, e As GestureEventArgs) Handles imgRegresh.Tap
        Dim LastUpdateTime As New Date
        LastUpdateTime = Date.Now
        txtUpdateTime.Text = "更新于" & LastUpdateTime.Year & "-" & String.Format("{0:D2}", LastUpdateTime.Month) & "-" & String.Format("{0:D2}", LastUpdateTime.Day) & " " & String.Format("{0:D2}", LastUpdateTime.Hour) & ":" & String.Format("{0:D2}", LastUpdateTime.Minute) & ":" & String.Format("{0:D2}", LastUpdateTime.Second)
    End Sub
End Class