'This class's imports and settings.
Option Compare Binary
Option Explicit On
Option Infer Off
Option Strict On

Imports System
Imports System.Drawing
Imports System.Windows.Forms

'This class contains this program's main interface window.
Public Class InterfaceWindow
   Private WithEvents MotionGenerator As New Timer With {.Enabled = True, .Interval = 55}

   'This procedure initializes this window.
   Public Sub New()
      Try
         InitializeComponent()

         Me.BackColor = BACK_COLOR_BRUSH.Color
         Me.Font = FONTO
         Me.Size = New Size(CInt(My.Computer.Screen.WorkingArea.Width / 1.1), CInt(My.Computer.Screen.WorkingArea.Height / 1.1))
         Me.Text = $"{My.Application.Info.Title} v{My.Application.Info.Version} - by: {My.Application.Info.CompanyName}, {My.Application.Info.Copyright} - See Help.txt for usage."

         Initialize(Me.ClientSize)
      Catch ExceptionO As Exception
         DisplayException(ExceptionO)
      End Try
   End Sub

   'This procedure refreshes this window when it is activated.
   Private Sub InterfaceWindow_Activated(sender As Object, e As EventArgs) Handles MyBase.Activated
      Try
         SolarSystem.Position = New Point(CInt(Me.ClientSize.Width / 2), CInt(Me.ClientSize.Height / 2))

         DrawSolarSystem(Me.CreateGraphics, Move:=False)
      Catch ExceptionO As Exception
         DisplayException(ExceptionO)
      End Try
   End Sub

   'This procedure handles the user's key strokes.
   Private Sub InterfaceWindow_KeyUp(sender As Object, e As KeyEventArgs) Handles MyBase.KeyUp
      Try
         With SolarSystem
            Select Case e.KeyCode
               Case Keys.Add
                  If MotionGenerator.Interval > 55 Then MotionGenerator.Interval -= 100
               Case Keys.Down
                  .Position.Y -= (1 * .Scale)
               Case Keys.End
                  If .Scale > 1 Then .Scale -= 1
               Case Keys.F1
                  MessageBox.Show(My.Application.Info.Description, My.Application.Info.Title, MessageBoxButtons.OK, MessageBoxIcon.Information)
               Case Keys.Home
                  If .Scale < 10 Then .Scale += 1
               Case Keys.Left
                  .Position.X -= (1 * .Scale)
               Case Keys.PageDown
                  If .Tilt < 10 Then .Tilt += 1
               Case Keys.PageUp
                  If .Tilt > 0 Then .Tilt -= 1
               Case Keys.Pause
                  MotionGenerator.Enabled = Not MotionGenerator.Enabled
               Case Keys.Right
                  .Position.X += (1 * .Scale)
               Case Keys.Subtract
                  If MotionGenerator.Interval < 1055 Then MotionGenerator.Interval += 100
               Case Keys.Up
                  .Position.Y += (1 * .Scale)
            End Select
         End With
      Catch ExceptionO As Exception
         DisplayException(ExceptionO)
      End Try
   End Sub

   'This procedure handles the user's mouse clicks.
   Private Sub InterfaceWindow_MouseUp(sender As Object, e As MouseEventArgs) Handles MyBase.MouseUp
      Try
         If e.Button = MouseButtons.Left Then
            SolarSystem.Position = New Point(e.X, e.Y)
            Me.CreateGraphics.Clear(Me.BackColor)
            DrawSolarSystem(Me.CreateGraphics, Move:=False)
         End If
      Catch ExceptionO As Exception
         DisplayException(ExceptionO)
      End Try
   End Sub

   'This procedure handles the user's mouse clicks.
   Private Sub InterfaceWindow_Paint(sender As Object, e As PaintEventArgs) Handles MyBase.Paint
      Try
         DrawSolarSystem(e.Graphics, Move:=True)
      Catch ExceptionO As Exception
         DisplayException(ExceptionO)
      End Try
   End Sub

   'This procedure gives the command to draw the solar system at specific intervals.
   Private Sub MotionGenerator_Tick(send As Object, e As EventArgs) Handles MotionGenerator.Tick
      Try
         Me.Invalidate()
      Catch ExceptionO As Exception
         DisplayException(ExceptionO)
      End Try
   End Sub
End Class
