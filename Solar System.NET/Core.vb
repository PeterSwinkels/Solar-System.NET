'This module's imports and settings.
Option Compare Binary
Option Explicit On
Option Infer Off
Option Strict On

Imports System
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Globalization
Imports System.IO
Imports System.Linq
Imports System.Math
Imports System.Runtime.CompilerServices
Imports System.Windows.Forms

'This module contains this program's core procedures.
Public Module CoreModule
   Private Const DEGREES_PER_RADIAN As Double = 180 / PI   'Defines the number of degrees per radian.
   Private Const DELIMITER As Char = ","c                  'Defines the solar system data.

   'This structure defines a planet.
   Public Structure PlanetStr
      Public Name As String        'Defines a planet's name.
      Public Radius As Single      'Defines a planet's radius.
      Public ColorO As Color       'Defines a planet's color.
      Public Distance As Integer   'Defines a planet's distance from the sun.
      Public Position As Double    'Defines a planet's position in its orbit.
      Public Velocity As Integer   'Defines a planet's velocity.
   End Structure

   'This structure defines a star.
   Private Structure StarStr
      Public Position As Point      'Defines a star's position.
      Public Twinkling As Boolean   'Defines whether a star is twinkling.
   End Structure

   'This structure defines the sun.
   Public Structure SunStr
      Public Name As String       'Defines the sun's name.
      Public Radius As Single     'Defines the sun's radius.
      Public ColorO As Color      'Defines the sun's color.
   End Structure

   'This structure defines the solar system.
   Public Structure SolarSystemStr
      Public Planets() As PlanetStr   'Defines the solar system's planets.
      Public Sun As SunStr            'Defines the solar system's sun.
      Public Position As Point        'Defines the solar system's position.
      Public Scale As Integer         'Defines the scale used to draw the solar system.
      Public Tilt As Integer          'Defines the solar system's tilt.
   End Structure

   Public ReadOnly BACK_COLOR_BRUSH As New SolidBrush(Color.Black)               'Contains the brush used to paint the background.
   Public ReadOnly FONTO As New Font("MS Sans Serif", 8.25, FontStyle.Regular)   'Contains the font used to display the solar system body's names.
   Private ReadOnly RANDOMO As New Random                                         'Contains the random number generator.

   Public SolarSystem As New SolarSystemStr   'Contains the solar system.
   Private Stars() As StarStr = {}             'Contains the stars.

   'This procedure displays any exceptions that occur.
   Public Sub DisplayException(ExceptionO As Exception)
      If MessageBox.Show(ExceptionO.Message, My.Application.Info.Title, MessageBoxButtons.OKCancel, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1) = DialogResult.Cancel Then
         Application.Exit()
      End If
   End Sub

   'This procedure displays the specified text at the specified position using the specified color on the specified window.
   Private Sub DisplayText(Canvas As Graphics, Text As String, Position As Point, TextColor As Color, EraseV As Boolean)
      Try
         Dim SizeO As New SizeF

         With Canvas
            If EraseV Then
               SizeO = .MeasureString(Text, FONTO)
               .FillRectangle(BACK_COLOR_BRUSH, Position.X, Position.Y, SizeO.Width, SizeO.Height)
            Else
               Using BrushO As New SolidBrush(TextColor)
                  .DrawString(Text, FONTO, BrushO, Position)
               End Using
            End If
         End With
      Catch ExceptionO As Exception
         DisplayException(ExceptionO)
      End Try
   End Sub

   'This procedure draws the specified planets' orbits on the specified window.
   Private Sub DrawOrbits(Canvas As Graphics, SolarSystem As SolarSystemStr, OrbitStart As Integer, OrbitEnd As Integer, OrbitColor As Color)
      Try
         Dim Orbit As List(Of Point) = Nothing
         Dim Position As New Point

         Using PenO As New Pen(OrbitColor)
            With SolarSystem
               For Each Planet As PlanetStr In .Planets
                  Orbit = New List(Of Point)
                  With Planet
                     For Degree As Double = OrbitStart To OrbitEnd
                        Position.X = CInt((Cos(Degree.ToRadians()) * (.Distance * SolarSystem.Scale)) + SolarSystem.Position.X)
                        Position.Y = CInt((Sin(Degree.ToRadians()) * ((.Distance * SolarSystem.Scale) * (SolarSystem.Tilt / 10))) + SolarSystem.Position.Y)
                        Orbit.Add(Position)
                     Next Degree
                  End With
                  Canvas.DrawLines(PenO, Orbit.ToArray())
               Next Planet
            End With
         End Using
      Catch ExceptionO As Exception
         DisplayException(ExceptionO)
      End Try
   End Sub

   'This procedure draws the specified planet on the specified window.
   Private Sub DrawPlanet(Canvas As Graphics, Planet As PlanetStr, EraseV As Boolean)
      Try
         Dim Position As New Point
         Dim Radius As New Integer

         With Planet
            Using BrushO As New SolidBrush(If(EraseV, Color.Black, .ColorO))
               Position.X = CInt((Sin((.Position - 90).ToRadians()) * (.Distance * SolarSystem.Scale)) + SolarSystem.Position.X)
               Position.Y = CInt((Cos((.Position - 90).ToRadians()) * ((.Distance * SolarSystem.Scale) * (SolarSystem.Tilt / 10))) + SolarSystem.Position.Y)
               Radius = CInt(.Radius * (SolarSystem.Scale / 10))
               Canvas.FillEllipse(BrushO, Position.X - Radius, Position.Y - Radius, Radius * 2, Radius * 2)
               DisplayText(Canvas, .Name, Position, TextColor:=Color.White, EraseV:=EraseV)
            End Using
         End With
      Catch ExceptionO As Exception
         DisplayException(ExceptionO)
      End Try
   End Sub

   'This procedure draws the specified planets on the specified window.
   Private Sub DrawPlanets(Canvas As Graphics, SolarSystem As SolarSystemStr, OrbitStart As Integer, OrbitEnd As Integer, Direction As Integer, EraseV As Boolean)
      Try
         Dim First As New Integer
         Dim Last As New Integer

         With SolarSystem
            Select Case Direction
               Case -1
                  First = .Planets.GetUpperBound(0)
                  Last = .Planets.GetLowerBound(0)
               Case 1
                  First = .Planets.GetLowerBound(0)
                  Last = .Planets.GetUpperBound(0)
            End Select

            For Planet As Integer = First To Last Step Direction
               If .Planets(Planet).Position >= OrbitStart AndAlso .Planets(Planet).Position <= OrbitEnd Then
                  DrawPlanet(Canvas, .Planets(Planet), EraseV:=EraseV)
               End If
            Next Planet
         End With
      Catch ExceptionO As Exception
         DisplayException(ExceptionO)
      End Try
   End Sub

   'This procedure draws the solar system on the specified window.
   Public Sub DrawSolarSystem(Canvas As Graphics, Move As Boolean)
      Try
         DrawStars(Canvas, Stars, EraseV:=True)
         DrawPlanets(Canvas, SolarSystem, OrbitStart:=0, OrbitEnd:=180, Direction:=1, EraseV:=True)
         DrawPlanets(Canvas, SolarSystem, OrbitStart:=180, OrbitEnd:=360, Direction:=-1, EraseV:=True)
         DrawStars(Canvas, Stars, EraseV:=False)

         If Move Then MovePlanets(SolarSystem)

         DrawOrbits(Canvas, SolarSystem, OrbitStart:=0, OrbitEnd:=180, OrbitColor:=Color.Gray)
         DrawPlanets(Canvas, SolarSystem, OrbitStart:=0, OrbitEnd:=180, Direction:=1, EraseV:=False)

         DrawSun(Canvas, SolarSystem, RadiusScale:=0.001)

         DrawOrbits(Canvas, SolarSystem, OrbitStart:=180, OrbitEnd:=360, OrbitColor:=Color.Gray)
         DrawPlanets(Canvas, SolarSystem, OrbitStart:=180, OrbitEnd:=360, Direction:=-1, EraseV:=False)
      Catch ExceptionO As Exception
         DisplayException(ExceptionO)
      End Try
   End Sub

   'This procedure draws the specified stars on the specified window.
   Private Sub DrawStars(Canvas As Graphics, Stars() As StarStr, EraseV As Boolean)
      Try
         Dim ColorO As Color = If(EraseV, Color.Black, Color.White)

         TwinkleStars(Stars)

         For Each Star As StarStr In Stars
            If Star.Twinkling Then
               Using BrushO As New SolidBrush(ColorO)
                  Canvas.FillEllipse(BrushO, Star.Position.X - 1, Star.Position.Y - 1, 3, 3)
               End Using
            Else
               Using PenO As New Pen(New SolidBrush(ColorO))
                  Canvas.DrawLine(PenO, Star.Position.X - 1, Star.Position.Y - 1, Star.Position.X, Star.Position.Y)
               End Using
            End If
         Next Star
      Catch ExceptionO As Exception
         DisplayException(ExceptionO)
      End Try
   End Sub

   'This procedure draws the specified sun on the specified window.
   Private Sub DrawSun(Canvas As Graphics, SolarSystem As SolarSystemStr, RadiusScale As Single)
      Try
         Dim Radius As Integer = CInt(SolarSystem.Sun.Radius * RadiusScale)

         With SolarSystem
            Using BrushO As New SolidBrush(.Sun.ColorO)
               Canvas.FillEllipse(BrushO, .Position.X - Radius, .Position.Y - Radius, Radius * 2, Radius * 2)
            End Using
         End With
      Catch ExceptionO As Exception
         DisplayException(ExceptionO)
      End Try
   End Sub

   'This procedure generates the stars using the specified window's dimensions.
   Private Function GenerateStars(Boundaries As Size) As StarStr()
      Try
         Dim NewStars As New List(Of StarStr)

         For Star As Integer = 0 To RANDOMO.Next(99, 199)
            NewStars.Add(New StarStr With {.Position = New Point(RANDOMO.Next(0, Boundaries.Width), RANDOMO.Next(0, Boundaries.Height)), .Twinkling = {False, True}(RANDOMO.Next(0, 2))})
         Next Star

         Return NewStars.ToArray()
      Catch ExceptionO As Exception
         DisplayException(ExceptionO)
      End Try

      Return {}
   End Function

   'This procedure initializes this program.
   Public Sub Initialize(Boundaries As Size)
      Try
         SolarSystem = LoadSolarSystem()
         SolarSystem.Scale = 1
         SolarSystem.Tilt = 2

         Stars = GenerateStars(Boundaries)
      Catch ExceptionO As Exception
         DisplayException(ExceptionO)
      End Try
   End Sub

   'This procedure moves the specified planets.
   Private Sub MovePlanets(ActiveSolarSystem As SolarSystemStr)
      Try
         With ActiveSolarSystem
            For Planet As Integer = .Planets.GetLowerBound(0) To .Planets.GetUpperBound(0)
               .Planets(Planet).Position = NewPosition(.Planets(Planet).Position, .Planets(Planet).Velocity)
            Next Planet
         End With
      Catch ExceptionO As Exception
         DisplayException(ExceptionO)
      End Try
   End Sub

   'This procedure increments or resets the specified position and returns the result.
   Private Function NewPosition(Position As Double, Velocity As Integer) As Double
      Try
         Return If(Position + Velocity >= 360, 0, Position + Velocity)
      Catch ExceptionO As Exception
         DisplayException(ExceptionO)
      End Try

      Return New Integer
   End Function

   'This procedure loads and returns the solar system's data.
   Private Function LoadSolarSystem() As SolarSystemStr
      Try
         Dim Items() As String = {}
         Dim NewSolarSystem As New SolarSystemStr
         Dim PlanetCount As New Integer
         Dim Planets As New List(Of PlanetStr)

         With NewSolarSystem
            Using DataReader As New StringReader(My.Resources.SolarSystem)
               Items = DataReader.ReadLine().Split(DELIMITER)

               .Sun.Name = Items(0)
               .Sun.ColorO = ColorTranslator.FromOle(Integer.Parse(Items(1), NumberStyles.HexNumber))
               .Sun.Radius = Integer.Parse(Items(2))

               PlanetCount = Integer.Parse(DataReader.ReadLine)

               For Planet As Integer = 0 To PlanetCount - 1
                  Items = DataReader.ReadLine().Split(DELIMITER)
                  Planets.Add(New PlanetStr With {
                                                  .Name = Items(0),
                                                  .ColorO = ColorTranslator.FromOle(Integer.Parse(Items(1), NumberStyles.HexNumber)),
                                                  .Radius = Integer.Parse(Items(2)),
                                                  .Distance = Integer.Parse(Items(3)),
                                                  .Velocity = Integer.Parse(Items(4)),
                                                  .Position = Integer.Parse(Items(5))
                                                 })
               Next Planet
               .Planets = Planets.ToArray()
            End Using
         End With

         Return NewSolarSystem
      Catch ExceptionO As Exception
         DisplayException(ExceptionO)
      End Try

      Return New SolarSystemStr
   End Function

   'This procedure controls the specified stars' twinkling.
   Private Sub TwinkleStars(Stars() As StarStr)
      Try
         Dim SelectedStar As New Integer

         For Star As Integer = 0 To Stars.Count \ 2
            SelectedStar = RANDOMO.Next(0, Stars.Count)
            Stars(SelectedStar).Twinkling = {False, True}(RANDOMO.Next(0, 2))
         Next Star
      Catch ExceptionO As Exception
         DisplayException(ExceptionO)
      End Try
   End Sub

   'This procedure returns the value specified in degrees converted to radians.
   <Extension()>
   Public Function ToRadians(Degrees As Double) As Double
      Try
         Return Degrees / DEGREES_PER_RADIAN
      Catch ExceptionO As Exception
         DisplayException(ExceptionO)
      End Try

      Return New Double
   End Function
End Module
