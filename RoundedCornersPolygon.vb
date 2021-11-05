Public Class RoundedCornersPolygon
	Inherits Shape

	Private ReadOnly _path As Path

#Region "Properties "

	Private _points As PointCollection
	''' <summary>
	''' Gets or sets a collection that contains the points of the polygon.
	''' </summary>
	Public Property Points() As PointCollection
		Get
			Return _points
		End Get
		Set(ByVal value As PointCollection)
			_points = value
			RedrawShape()
		End Set
	End Property

	Private _isClosed As Boolean
	''' <summary>
	''' Gets or sets a value that specifies if the polygon will be closed or not.
	''' </summary>
	Public Property IsClosed() As Boolean
		Get
			Return _isClosed
		End Get
		Set(ByVal value As Boolean)
			_isClosed = value
			RedrawShape()
		End Set
	End Property

	Private _useRoundnessPercentage As Boolean
	''' <summary>
	''' Gets or sets a value that specifies if the ArcRoundness property value will be used as a percentage of the connecting segment or not.
	''' </summary>
	Public Property UseRoundnessPercentage() As Boolean
		Get
			Return _useRoundnessPercentage
		End Get
		Set(ByVal value As Boolean)
			_useRoundnessPercentage = value
			RedrawShape()
		End Set
	End Property

	Private _arcRoundness As Double
	''' <summary>
	''' Gets or sets a value that specifies the arc roundness.
	''' </summary>
	Public Property ArcRoundness() As Double
		Get
			Return _arcRoundness
		End Get
		Set(ByVal value As Double)
			_arcRoundness = value
			RedrawShape()
		End Set
	End Property
	Public ReadOnly Property Data() As Geometry
		Get
			Return _path.Data
		End Get
	End Property

#End Region

	Public Sub New()
		Dim geometry = New PathGeometry()
		geometry.Figures.Add(New PathFigure())
		_path = New Path With {.Data = geometry}
		Points = New PointCollection()
		AddHandler Points.Changed, AddressOf Points_Changed
	End Sub

	Private Sub Points_Changed(ByVal sender As Object, ByVal e As EventArgs)
		RedrawShape()
	End Sub

#Region "Implementation of Shape"

	Protected Overrides ReadOnly Property DefiningGeometry() As Geometry
		Get
			Return _path.Data
		End Get
	End Property

#End Region

#Region "Private Methods"

	''' <summary>
	''' Redraws the entire shape.
	''' </summary>
	Private Sub RedrawShape()
		Dim pathGeometry As PathGeometry = TryCast(_path.Data, PathGeometry)
		If pathGeometry Is Nothing Then
			Return
		End If

		Dim pathFigure = pathGeometry.Figures(0)

		pathFigure.Segments.Clear()

		For counter As Integer = 0 To Points.Count - 1
			Select Case counter
				Case 0
					AddPointToPath(Points(counter), Nothing, Nothing)
				Case 1
					AddPointToPath(Points(counter), Points(counter - 1), Nothing)
				Case Else
					AddPointToPath(Points(counter), Points(counter - 1), Points(counter - 2))
			End Select
		Next counter

		If IsClosed Then
			CloseFigure(pathFigure)
		End If
	End Sub

	''' <summary>
	''' Adds a point to the shape
	''' </summary>
	''' <param name="currentPoint">The current point added</param>
	''' <param name="prevPoint">Previous point</param>
	''' <param name="prevPrevPoint">The point before the previous point</param>
	Private Sub AddPointToPath(ByVal currentPoint As Point, ByVal prevPoint? As Point, ByVal prevPrevPoint? As Point)
		If Points.Count = 0 Then
			Return
		End If

		Dim pathGeometry As PathGeometry = TryCast(_path.Data, PathGeometry)
		If pathGeometry Is Nothing Then
			Return
		End If

		Dim pathFigure = pathGeometry.Figures(0)

		'the first point of a polygon
		If prevPoint Is Nothing Then
			pathFigure.StartPoint = currentPoint
			'second point of the polygon, only a line will be drawn
		ElseIf prevPrevPoint Is Nothing Then
			Dim lines = New LineSegment With {.Point = currentPoint}
			pathFigure.Segments.Add(lines)
			'third point and above
		Else
			ConnectLinePoints(pathFigure, prevPrevPoint.Value, prevPoint.Value, currentPoint, ArcRoundness, UseRoundnessPercentage)
		End If
	End Sub

	''' <summary>
	''' Adds the segments necessary to close the shape
	''' </summary>
	''' <param name="pathFigure"></param>
	Private Sub CloseFigure(ByVal pathFigure As PathFigure)
		'No need to visually close the figure if we don't have at least 3 points.
		If Points.Count < 3 Then
			Return
		End If
		Dim backPoint, nextPoint As Point
		If UseRoundnessPercentage Then
			backPoint = GetPointAtDistancePercent(Points(Points.Count - 1), Points(0), ArcRoundness, False)
			nextPoint = GetPointAtDistancePercent(Points(0), Points(1), ArcRoundness, True)
		Else
			backPoint = GetPointAtDistance(Points(Points.Count - 1), Points(0), ArcRoundness, False)
			nextPoint = GetPointAtDistance(Points(0), Points(1), ArcRoundness, True)
		End If
		ConnectLinePoints(pathFigure, Points(Points.Count - 2), Points(Points.Count - 1), backPoint, ArcRoundness, UseRoundnessPercentage)
		Dim line2 = New QuadraticBezierSegment With {
			.Point1 = Points(0),
			.Point2 = nextPoint
		}
		pathFigure.Segments.Add(line2)
		pathFigure.StartPoint = nextPoint
	End Sub

	''' <summary>
	''' Method used to connect 2 segments with a common point, defined by 3 points and aplying an arc segment between them
	''' </summary>
	''' <param name="pathFigure"></param>
	''' <param name="p1">First point, of the first segment</param>
	''' <param name="p2">Second point, the common point</param>
	''' <param name="p3">Third point, the second point of the second segment</param>
	''' <param name="roundness">The roundness of the arc</param>
	''' <param name="usePercentage">A value that indicates if the roundness of the arc will be used as a percentage or not</param>
	Private Shared Sub ConnectLinePoints(ByVal pathFigure As PathFigure, ByVal p1 As Point, ByVal p2 As Point, ByVal p3 As Point, ByVal roundness As Double, ByVal usePercentage As Boolean)
		'The point on the first segment where the curve will start.
		Dim backPoint As Point
		'The point on the second segment where the curve will end.
		Dim nextPoint As Point
		If usePercentage Then
			backPoint = GetPointAtDistancePercent(p1, p2, roundness, False)
			nextPoint = GetPointAtDistancePercent(p2, p3, roundness, True)
		Else
			backPoint = GetPointAtDistance(p1, p2, roundness, False)
			nextPoint = GetPointAtDistance(p2, p3, roundness, True)
		End If

		Dim lastSegmentIndex As Integer = pathFigure.Segments.Count - 1
		'Set the ending point of the first segment.
		CType(pathFigure.Segments(lastSegmentIndex), LineSegment).Point = backPoint
		'Create and add the curve.
		Dim curve = New QuadraticBezierSegment With {
			.Point1 = p2,
			.Point2 = nextPoint
		}
		pathFigure.Segments.Add(curve)
		'Create and add the new segment.
		Dim line = New LineSegment With {.Point = p3}
		pathFigure.Segments.Add(line)
	End Sub

	''' <summary>
	''' Gets a point on a segment, defined by two points, at a given distance.
	''' </summary>
	''' <param name="p1">First point of the segment</param>
	''' <param name="p2">Second point of the segment</param>
	''' <param name="distancePercent">Distance percent to the point</param>
	''' <param name="firstPoint">A value that indicates if the distance is calculated by the first or the second point</param>
	''' <returns></returns>
	Private Shared Function GetPointAtDistancePercent(ByVal p1 As Point, ByVal p2 As Point, ByVal distancePercent As Double, ByVal firstPoint As Boolean) As Point
		Dim rap As Double = If(firstPoint, distancePercent / 100, (100 - distancePercent) / 100)
		Return New Point(p1.X + (rap * (p2.X - p1.X)), p1.Y + (rap * (p2.Y - p1.Y)))
	End Function

	''' <summary>
	''' Gets a point on a segment, defined by two points, at a given distance.
	''' </summary>
	''' <param name="p1">First point of the segment</param>
	''' <param name="p2">Second point of the segment</param>
	''' <param name="distance">Distance  to the point</param>
	''' <param name="firstPoint">A value that indicates if the distance is calculated by the first or the second point</param>
	''' <returns>The point calculated.</returns>
	Private Shared Function GetPointAtDistance(ByVal p1 As Point, ByVal p2 As Point, ByVal distance As Double, ByVal firstPoint As Boolean) As Point
		Dim segmentLength As Double = Math.Sqrt(Math.Pow((p2.X - p1.X), 2) + Math.Pow((p2.Y - p1.Y), 2))
		'The distance cannot be greater than half of the length of the segment
		If distance > (segmentLength / 2) Then
			distance = segmentLength / 2
		End If
		Dim rap As Double = If(firstPoint, distance / segmentLength, (segmentLength - distance) / segmentLength)
		Return New Point(p1.X + (rap * (p2.X - p1.X)), p1.Y + (rap * (p2.Y - p1.Y)))
	End Function

#End Region
End Class