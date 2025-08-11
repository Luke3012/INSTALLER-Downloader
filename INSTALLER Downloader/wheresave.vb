Public Class wheresave
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

    Private Sub closebutton_Click(sender As Object, e As EventArgs) Handles closebutton.Click
        If MessageBox.Show("Sei sicuro/a di voler annullare? Se sei indeciso/a clicca su 'Salva INSTALLER qui'!", "Annullo?", MessageBoxButtons.YesNo) = DialogResult.Yes Then
            Try
                My.Computer.FileSystem.DeleteFile(Application.StartupPath & "\downloadertemp")
            Catch
            End Try
            Close()
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        saveinstaller(Application.StartupPath)
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If Scegli.ShowDialog() = DialogResult.OK Then
            saveinstaller(Scegli.SelectedPath)
        End If
    End Sub

    Private Async Sub saveinstaller(path As String)
        closebutton.Hide()
        Cursor = Cursors.WaitCursor
        applicopanel.BringToFront()
        applicopanel.Visible = True

        Dim fatto As Boolean = True
        Dim task As New Task(Sub()
                                 Try
                                     If IO.File.Exists(path & "\INSTALLER.exe") Then IO.File.Delete(path & "\INSTALLER.exe")
                                     If path = Application.StartupPath Then
                                         My.Computer.FileSystem.RenameFile(Application.StartupPath & "\downloadertemp", "INSTALLER.exe")
                                     Else
                                         IO.File.Move(Application.StartupPath & "\downloadertemp", path & "\INSTALLER.exe")
                                     End If
                                 Catch
                                     fatto = False
                                 End Try
                             End Sub)
        task.Start()
        Await task
        Cursor = Cursors.Default
        closebutton.Show()

        If fatto = False Then
            MsgBox("Salvataggio non Riuscito, Riprova!", MsgBoxStyle.Critical, "Errore!")
            Close()
        Else
            completed(path)
        End If
        applicopanel.Visible = False
        applicopanel.SendToBack()
    End Sub

    Public Sub completed(Path As String)
        If enabledfondamental = True Then
            Home.topath = Path
            Home.fondamentalc = enabledfondamental
            Close()
            Exit Sub
        End If

        If MessageBox.Show("Vuoi avviare INSTALLER?", "Eseguo?", MessageBoxButtons.YesNo) = DialogResult.Yes Then
            Try
                Try
                    Dim updater As New Process
                    With updater.StartInfo
                        .FileName = ("INSTALLER.exe")
                        .WorkingDirectory = Path & "\"
                    End With
                    updater.Start()
                Catch
                    Try
                        Dim updater As New Process
                        With updater.StartInfo
                            .FileName = ("INSTALLER.exe")
                            .WorkingDirectory = Path
                        End With
                        updater.Start()
                    Catch
                        MsgBox("Avvio di INSTALLER fallito!", MsgBoxStyle.Critical, "Errore!")
                    End Try
                End Try
            Catch
                MsgBox("Avvio di INSTALLER fallito!", MsgBoxStyle.Critical, "Errore!")
            End Try
        End If
        Close()
    End Sub

    Dim enabledfondamental As Boolean = False
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If enabledfondamental = True Then
            enabledfondamental = False
            Button3.BackgroundImage = My.Resources.switch_on_icon
            Label4.Text = "Stato: Disabilitato"
        Else
            enabledfondamental = True
            Button3.BackgroundImage = My.Resources.switch_off_icon
            Label4.Text = "Stato: Abilitato"
        End If
    End Sub

    Private Sub wheresave_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If Home.fondamentalc = True Then
            Button3.PerformClick()
            enabledfondamental = False
            completed(Home.topath)
            Exit Sub
        End If

        If IO.File.Exists(Application.StartupPath & "\image") And IO.File.Exists(Application.StartupPath & "\pic1") And IO.File.Exists(Application.StartupPath & "\downloader32.ini") Then
            Button1.Enabled = False
            Label1.Enabled = False
            Dim fnt As Font = Label1.Font
            Label1.Font = New Font(fnt.Name, 11, FontStyle.Bold)
            Label1.Text = "Mi spiace, non puoi salvare INSTALLER qui"
        End If
    End Sub
End Class