Imports System.Net

Public Class Home
    ' Cose per il pannello
    Dim drag As Boolean
    Dim mousex As Integer
    Dim mousey As Integer
    Private Sub panel_MouseDown() Handles TitleText.MouseDown
        drag = True
        mousex = Cursor.Position.X - Me.Left
        mousey = Cursor.Position.Y - Me.Top
        Cursor = Cursors.SizeAll
    End Sub
    Private Sub panel_MouseMove() Handles TitleText.MouseMove
        If drag Then
            Me.Top = Cursor.Position.Y - mousey
            Me.Left = Cursor.Position.X - mousex
        End If
    End Sub
    Private Sub panel_MouseUp() Handles TitleText.MouseUp
        drag = False
        Cursor = Cursors.Default
    End Sub
    ' Fine cose per il pannello

    Public currentversion As String = 24
    Dim client1 As WebClient
    Dim chiudendo As Boolean = False
    Private Sub closebutton_Click(sender As Object, e As EventArgs) Handles closebutton.Click
        chiudendo = True
        Try
            client1.CancelAsync()
        Catch
        End Try
        trydelete(Application.StartupPath & "\downloadertemp")
        iffail()
    End Sub

    Private Async Sub Home_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'Imposta Versione
        versione.Text = "Versione " & currentversion.Insert(1, ".")

        Await trydelete(Application.StartupPath & "\downloadertemp")
        Await trydelete(Application.StartupPath & "\todo.bat")
        Await trydelete(Application.StartupPath & "\newversion")
        Await trydelete(Application.StartupPath & "\fondamentaltemp")

        Dim versioneh As String = 0
        Dim task As New Task(Sub()
                                 Try
                                     Dim boh As New WebClient
                                     versioneh = boh.DownloadString("https://api.onedrive.com/v1.0/shares/s!AgnIj48L3IE3gogrQJHQpTyFhADLlg/root/content")
                                     If versioneh.Trim = "" Then versioneh = 0
                                 Catch
                                 End Try
                             End Sub)
        task.Start()
        Await task

        Dim control As Integer = versioneh
        If control > currentversion Then
            Try
                Label1.Text = "Scarico l'Aggiornamento ..."
                Dim boh As New WebClient
                AddHandler boh.DownloadFileCompleted, AddressOf afterboh
                boh.DownloadFileAsync(New Uri("https://api.onedrive.com/v1.0/shares/s!AgnIj48L3IE3gogsSck-UvTmz34ihw/root/content"), Application.StartupPath & "\newversion")
                Exit Sub
            Catch
            End Try
        End If

        Download()
    End Sub

    Private Sub Download()
        If My.Computer.Network.IsAvailable = True Then
            Label1.Text = "Scarico INSTALLER ..."
            Try
                client1 = New WebClient
                AddHandler client1.DownloadFileCompleted, AddressOf afterdownload1
                client1.DownloadFileAsync(New Uri("https://api.onedrive.com/v1.0/shares/s!AgnIj48L3IE3gpNZLbwBWTHpjPZ85w/root/content"), Application.StartupPath & "\downloadertemp")
            Catch
                MsgBox("Download non riuscito, Riprova", MsgBoxStyle.Critical, "Scaricamento Fallito")
                iffail()
            End Try
        Else
            MsgBox("Connessione ad Internet non disponibile!", MsgBoxStyle.Critical, "Scaricamento Fallito")
            iffail()
        End If
    End Sub

    Private Async Sub afterdownload1()
        If chiudendo = True Then Exit Sub
        Label1.Text = "INSTALLER Scaricato, elaboro ..."
        Try
            Dim anfs As New IO.FileInfo(Application.StartupPath & "\downloadertemp")
            Dim size As Long = anfs.Length
            If size < 1000000 Then
                Await trydelete(Application.StartupPath & "\downloadertemp")
                MsgBox("Download non riuscito, Riprova", MsgBoxStyle.Critical, "Scaricamento Fallito")
                iffail()
            Else
                If IO.File.Exists(Application.StartupPath & "\path") Then
                    Dim r As Boolean = True
                    Dim task As New Task(Sub()
                                             Try
                                                 IO.File.Copy(Application.StartupPath & "\downloadertemp", IO.File.ReadAllText(Application.StartupPath & "\path") & "\INSTALLER.exe", True)
                                             Catch
                                                 r = False
                                             End Try
                                         End Sub)
                    task.Start()
                    Await task
                    Await trydelete(Application.StartupPath & "\downloadertemp")
                    If r = False Then
                        MsgBox("C'è stato un errore durante la copia di INSTALLER", MsgBoxStyle.Critical, "Errore Copia")
                        iffail()
                        Exit Sub
                    End If
                    Dim checkupwrite2 As New IO.StreamWriter(IO.File.ReadAllText(Application.StartupPath & "\path") & "\Fondamental\Settings\tool.txt")
                    checkupwrite2.WriteLine("yes")
                    checkupwrite2.Close()
                    MsgBox("Operazione Riuscita con Successo!", MsgBoxStyle.Exclamation, "Tutto fatto!")
                    iffail()
                Else
                    wheresave.Close()
                    wheresave.ShowDialog()
                    If fondamentalc = True Then
                        checkfondamental()
                    Else
                        Close()
                    End If
                End If
            End If
        Catch
            MsgBox("Errore Generico! Riprova!", MsgBoxStyle.Critical, "Scaricamento Fallito")
            iffail()
        End Try
    End Sub

    Private Async Sub iffail()
        If IO.File.Exists(Application.StartupPath & "\path") Then
            Try
                Dim updater As New Process
                With updater.StartInfo
                    .FileName = ("INSTALLER.exe")
                    .WorkingDirectory = IO.File.ReadAllText(Application.StartupPath & "\path") & "\"
                End With
                updater.Start()
            Catch
                Try
                    Dim updater As New Process
                    With updater.StartInfo
                        .FileName = ("INSTALLER.exe")
                        .WorkingDirectory = IO.File.ReadAllText(Application.StartupPath & "\path")
                    End With
                    updater.Start()
                Catch
                    MsgBox("Errore Fatale! INSTALLER non trovato!", MsgBoxStyle.Critical, "Errore!")
                End Try
            End Try
            Await trydelete(Application.StartupPath & "\path")
        End If
        Close()
    End Sub

    Private Sub afterboh()
        Try
            Dim anfs As New IO.FileInfo(Application.StartupPath & "\newversion")
            Dim size As Long = anfs.Length

            If size > 20000 Then
                Dim scrivi As New IO.StreamWriter(Application.StartupPath & "\todo.bat")
                scrivi.Write("taskkill /F /IM ""INSTALLER Downloader.exe"" & if exist newversion del ""INSTALLER Downloader.exe"" & rename newversion ""INSTALLER Downloader.exe"" & start """" ""INSTALLER Downloader.exe""")
                scrivi.Close()
            End If
            If IO.File.Exists(Application.StartupPath & "\todo.bat") Then
                Dim p As New Process
                p.StartInfo.CreateNoWindow = True
                p.StartInfo.FileName = ("todo.bat")
                p.StartInfo.WorkingDirectory = Application.StartupPath & "\"
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
                Try
                    p.Start()
                    Close()
                Catch
                    Try
                        p.StartInfo.WorkingDirectory = Application.StartupPath
                        p.Start()
                        Close()
                    Catch
                        trydelete(Application.StartupPath & "\todo.bat")
                        trydelete(Application.StartupPath & "\newversion")
                        Download()
                    End Try
                End Try
            Else
                Download()
            End If
        Catch
            Download()
        End Try
    End Sub

    Public Async Function trydelete(filetodelete As String) As Task
        Dim task As New Task(Sub()
                                 Try
                                     If IO.File.Exists(filetodelete) Then IO.File.Delete(filetodelete)
                                 Catch
                                 End Try
                                 Try
                                     If IO.Directory.Exists(filetodelete) Then IO.Directory.Delete(filetodelete, True)
                                 Catch
                                 End Try
                             End Sub)
        task.Start()
        Await task
    End Function

    Public fondamentalc As Boolean = False
    Public topath As String = Application.StartupPath
    Private Async Sub checkfondamental()
        If fondamentalc = False Then Close()
        Label1.Text = "Scarico la Configurazione ..."
        Dim tuttook As Boolean = True
        Dim task As New Task(Sub()
                                 Try
                                     Dim web As New WebClient
                                     web.DownloadFile("https://api.onedrive.com/v1.0/shares/s!AgnIj48L3IE3gqob8yM1SRudWZfKig/root/content", Application.StartupPath & "\fondamentaltemp")
                                 Catch
                                     tuttook = False
                                 End Try
                             End Sub)
        task.Start()
        Await task

        If tuttook = False Then
            Label1.Text = "Aglia! C'è stato qualche errore."
            MsgBox("Errore Scaricamento della Cartella di Configurazione di INSTALLER!", MsgBoxStyle.Critical, "Errore!")
            wheresave.ShowDialog()
            Close()
        End If

        Label1.Text = "Elimino la Configurazione precedente ..."
        If IO.Directory.Exists(topath & "\Fondamental") Then
            task = New Task(Sub()
                                Try
                                    My.Computer.FileSystem.DeleteDirectory(topath & "\Fondamental", FileIO.DeleteDirectoryOption.DeleteAllContents)
                                Catch
                                End Try
                            End Sub)
            task.Start()
            Await task
            Await trydelete(topath & "\Fondamental")
        End If

        Label1.Text = "Estraggo la Configurazione ..."
        task = New Task(Sub()
                            Try
                                IO.Compression.ZipFile.ExtractToDirectory(Application.StartupPath & "\fondamentaltemp", topath)
                                Dim checkupwrite1 As New IO.StreamWriter(topath & "\Fondamental\Settings\impostazioni.ini")
                                checkupwrite1.WriteLine("checkup=false")
                                checkupwrite1.Close()
                                Dim checkupwrite2 As New IO.StreamWriter(topath & "\Fondamental\Settings\tool.txt")
                                checkupwrite2.WriteLine("yes")
                                checkupwrite2.Close()
                            Catch
                                tuttook = False
                            End Try
                        End Sub)
        task.Start()
        Await task
        Await trydelete(Application.StartupPath & "\fondamentaltemp")
        If tuttook = False Then
            Label1.Text = "Aglia! C'è stato qualche errore."
            MsgBox("Errore Scompattamento Cartella!", MsgBoxStyle.Critical, "Errore!")
            wheresave.ShowDialog()
            Close()
        End If

        Label1.Text = "Tutto Pronto!"
        wheresave.ShowDialog()
        Close()
    End Sub
End Class
