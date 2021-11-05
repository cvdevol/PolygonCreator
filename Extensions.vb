Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Windows
Imports System.Windows.Shapes

Public Module Extensions
	Public Enum PolygonHitTypes
		None
		Vertex
		Edge
	End Enum

	<System.Runtime.CompilerServices.Extension>
	Public Function IsAt(ByVal polygon As Polygon, ByVal point As Point, <System.Runtime.InteropServices.Out()> ByRef hit_type As PolygonHitTypes, <System.Runtime.InteropServices.Out()> ByRef item_index As Integer) As Boolean
		Const hit_radius As Double = 5
		Dim num_points As Integer = polygon.Points.Count

		' See if the point is at a vertex.
		For i As Integer = 0 To num_points - 1
			If point.DistanceToPoint(polygon.Points(i)) < hit_radius Then
				hit_type = PolygonHitTypes.Vertex
				item_index = i
				Return True
			End If
		Next i

		' See if the point is on an edge.
		Dim closest As Point = Nothing
		For i As Integer = 0 To num_points - 1
			Dim j As Integer = (i + 1) Mod num_points
			If point.DistanceToSegment(polygon.Points(i), polygon.Points(j), closest) < hit_radius Then
				hit_type = PolygonHitTypes.Edge
				item_index = i
				Return True
			End If
		Next i
		hit_type = PolygonHitTypes.None
		item_index = -1
		Return False
	End Function

	<System.Runtime.CompilerServices.Extension>
	Public Function DistanceToPoint(ByVal from_point As Point, ByVal to_point As Point) As Double
		Dim dx As Double = to_point.X - from_point.X
		Dim dy As Double = to_point.Y - from_point.Y
		Return Math.Sqrt(dx * dx + dy * dy)
	End Function



	' Calculate the distance between
	' point pt and the segment p1 --> p2.
	<System.Runtime.CompilerServices.Extension>
	Public Function DistanceToSegment(ByVal from_point As Point, ByVal p1 As Point, ByVal p2 As Point, <System.Runtime.InteropServices.Out()> ByRef closest As Point) As Double
		Dim dx As Double = p2.X - p1.X
		Dim dy As Double = p2.Y - p1.Y
		If (dx = 0) AndAlso (dy = 0) Then
			' It's a point not a line segment.
			closest = p1
			dx = from_point.X - p1.X
			dy = from_point.Y - p1.Y
			Return Math.Sqrt(dx * dx + dy * dy)
		End If

		' Calculate the t that minimizes the distance.
		Dim t As Double = ((from_point.X - p1.X) * dx + (from_point.Y - p1.Y) * dy) / (dx * dx + dy * dy)

		' See if this represents one of the segment's
		' end points or a point in the middle.
		If t < 0 Then
			closest = New Point(p1.X, p1.Y)
			dx = from_point.X - p1.X
			dy = from_point.Y - p1.Y
		ElseIf t > 1 Then
			closest = New Point(p2.X, p2.Y)
			dx = from_point.X - p2.X
			dy = from_point.Y - p2.Y
		Else
			closest = New Point(p1.X + t * dx, p1.Y + t * dy)
			dx = from_point.X - closest.X
			dy = from_point.Y - closest.Y
		End If

		Return Math.Sqrt(dx * dx + dy * dy)
	End Function


	<System.Runtime.CompilerServices.Extension>
	Public Function MovePoint(ByVal p1 As Point, distance As Double, theta As Double) As Point
		Dim p2 As New Point
		p2.X = p1.X + distance * Math.Cos(theta)
		p2.Y = p1.Y + distance * Math.Sin(theta)
		Return New Point(p2.X, p2.Y)
	End Function



	<System.Runtime.CompilerServices.Extension>
	Public Function GetCentroid(ByVal points As IEnumerable(Of Point)) As Point
		Dim pt As Point = points.Aggregate(New With {
	Key .xSum = 0.0,
	Key .ySum = 0.0,
	Key .n = 0
}, Function(acc, p) New With {
	Key .xSum = acc.xSum + p.X,
	Key .ySum = acc.ySum + p.Y,
	Key .n = acc.n + 1
}, Function(acc) New Point(acc.xSum / acc.n, acc.ySum / acc.n))
		Return pt
	End Function

	<System.Runtime.CompilerServices.Extension>
	Public Function GetPathGeometry(ByVal Vertices() As Point) As PathGeometry
		If Vertices.Count = 0 Then Return Nothing
		Dim LineSeg As LineSegment
		Dim PathFig As New PathFigure()
		PathFig.StartPoint = Vertices(0)
		Dim PathSegs As New PathSegmentCollection()
		For i = 1 To Vertices.Count - 1
			LineSeg = New LineSegment()
			LineSeg.Point = Vertices(i)
			PathSegs.Add(LineSeg)
		Next
		LineSeg = New LineSegment()
		LineSeg.Point = Vertices(0)
		PathSegs.Add(LineSeg)

		PathFig.Segments = PathSegs
		Dim PathFigs = New PathFigureCollection()
		PathFigs.Add(PathFig)

		Dim PathGeo As New PathGeometry()
		PathGeo.Figures = PathFigs

		Return PathGeo
	End Function


	''' <summary>
	''' Splits an array of points into either an array of all the x-values or an array of all the y-values
	''' </summary>
	''' <param name="points"></param>
	''' <param name="splitx"></param>
	''' <returns></returns>
	<System.Runtime.CompilerServices.Extension>
	Public Function SplitXY(ByVal points() As Point, splitx As Boolean) As Double()
		Dim x(points.Count - 1) As Double
		Dim y(points.Count - 1) As Double
		If splitx = True Then
			For i = 0 To points.Count - 1
				x(i) = points(i).X
			Next
			Return x
		Else
			For i = 0 To points.Count - 1
			Next
			Return y
		End If
	End Function

	<System.Runtime.CompilerServices.Extension>
	Public Function PointInPolygon(ByVal nvert As Integer, ByVal vertx() As Double, ByVal verty() As Double, ByVal testx As Double, ByVal testy As Double) As Integer
		Dim i As Integer
		Dim j As Integer
		Dim c As Integer = 0
		i = 0
		j = nvert - 1
		Do While i < nvert
			If ((verty(i) > testy) <> (verty(j) > testy)) AndAlso (testx < (vertx(j) - vertx(i)) * (testy - verty(i)) / (verty(j) - verty(i)) + vertx(i)) Then
				c = c = 0
			End If
			j = i
			i += 1
		Loop
		Return c
	End Function

End Module


