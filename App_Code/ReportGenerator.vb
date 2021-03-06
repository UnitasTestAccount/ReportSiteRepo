﻿Imports System.Data
Imports System.Drawing
Imports System.Globalization
Imports System.IO
Imports System.Text
Imports System.Xml
Imports System.Web.UI.WebControls
Imports Microsoft.VisualBasic

Namespace GenerateReport
    Public Class ReportGenerator
        Private multipageRatio As Single = 1.5F
        Private ci As New CultureInfo("en-gb")
        Private dt As DataTable = Nothing
        Private dsName As String = ""
        Private nsRd As String = "http://schemas.microsoft.com/SQLServer/reporting/reportdesigner"
        Private ns As String = "http://schemas.microsoft.com/sqlserver/reporting/2005/01/reportdefinition"

        Public Sub New(ByVal dt As DataTable, ByVal dsName As String)
            Me.dt = dt
            Me.dsName = dsName
        End Sub

        Public Function GenerateReport() As Stream
            Dim xml As String
            Dim sb As New StringBuilder()

            Dim settings As New XmlWriterSettings()
            settings.CheckCharacters = True
            settings.CloseOutput = True
            settings.Encoding = Encoding.UTF8
            settings.Indent = True
            settings.IndentChars = vbTab
            settings.NewLineChars = vbCr & vbLf
            settings.NewLineHandling = NewLineHandling.Replace
            settings.NewLineOnAttributes = False
            settings.OmitXmlDeclaration = False

            Dim writer As XmlWriter = XmlWriter.Create(sb, settings)
            writer.WriteStartDocument()
            If True Then
                writer.WriteStartElement("Report", ns)
                writer.WriteAttributeString("xmlns", "rd", "", nsRd)
                If True Then
                    AddDataSource(writer, dsName)
                    Dim htb As Single = 0.63492F, maxWidth As Single = 4.0F
                    Dim dimension As New RectangleF(0.0F, 0.0F, 13.25397F, 2.98941F)
                    Dim pad As New Padding(2, 2, 2, 2)
                    Dim size As New SizeF(21.0F, 29.5F)
                    Dim margin As New Padding(0.5F, 0.5F, 0.5F, 0.5F)
                    GenerateSettingsHeader(writer, size, size, margin)

                    AddDataSet(writer, dt, dsName)
                    writer.WriteStartElement("Body")
                    If True Then
                        writer.WriteElementString("ColumnSpacing", "1cm")
                        writer.WriteElementString("Height", "5cm")
                        writer.WriteStartElement("ReportItems")
                        If True Then
                            GenerateTable(writer, dt, dsName, dimension, pad, pad, _
                             htb, maxWidth)
                        End If
                        writer.WriteEndElement()
                    End If
                    writer.WriteEndElement()
                End If
                writer.WriteEndElement()
            End If
            writer.WriteEndDocument()
            writer.Flush()
            writer.Close()
            xml = sb.ToString().Replace("utf-16", "utf-8")
            Dim ret As Stream = New MemoryStream(Encoding.UTF8.GetBytes(xml))
            Return ret
        End Function

#Region "Private Methods"
        Private Function GetDynamicSize(ByVal s As String) As SizeF
            Dim f As New Font(FontFamily.GenericSansSerif, 10)
            Dim bmp As New Bitmap(1, 1)
            Dim g As Graphics = Graphics.FromImage(bmp)
            g.PageUnit = GraphicsUnit.Millimeter
            Dim ret As SizeF = SizeF.Empty
            ret = g.MeasureString(s, f)
            g.Dispose()
            Return ret
        End Function

        Private Sub GenerateSessionTable(ByVal session As SessionTable, ByVal writer As XmlWriter, ByVal dt As DataTable, ByVal padding As Padding, ByVal height As Single)
            Dim namedsession As String = "", templatevalue As String = "", value As String = ""
            Dim colors As CellColors = Nothing
            Dim backcol As System.Drawing.Color

            backcol = System.Drawing.ColorTranslator.FromHtml("#8BFEA8")

            Select Case session
                Case SessionTable.Header
                    If True Then
                        namedsession = "Header"
                        templatevalue = "{0}"
                        colors = New CellColors(Color.Black, Color.White)
                        Exit Select
                    End If
                Case SessionTable.Details
                    If True Then
                        namedsession = "Details"
                        templatevalue = "=Fields!{0}.Value"
                        Exit Select
                    End If
                Case SessionTable.Footer
                    If True Then
                        namedsession = "Footer"
                        templatevalue = "{0}"
                        Exit Select
                    End If
            End Select
            writer.WriteStartElement(namedsession)
            If True Then
                If session = SessionTable.Header Then
                    writer.WriteElementString("RepeatOnNewPage", "true")
                End If
                writer.WriteStartElement("TableRows")
                If True Then
                    writer.WriteStartElement("TableRow")
                    If True Then
                        writer.WriteElementString("Height", height.ToString(ci) & "cm")
                        writer.WriteStartElement("TableCells")
                        If True Then
                            For i As Integer = 0 To dt.Columns.Count - 1
                                writer.WriteStartElement("TableCell")
                                If True Then
                                    writer.WriteStartElement("ReportItems")
                                    If True Then
                                        value = [String].Format(templatevalue, dt.Columns(i).ColumnName)
                                        GenerateTextBox(writer, "textbox" & namedsession & i, RectangleF.Empty, padding, colors, value)
                                    End If
                                    writer.WriteEndElement()
                                End If
                                writer.WriteEndElement()
                            Next
                        End If
                        writer.WriteEndElement()
                    End If
                    writer.WriteEndElement()
                End If
                writer.WriteEndElement()
            End If
            writer.WriteEndElement()
        End Sub

        Private Sub GenerateTable(ByVal writer As XmlWriter, ByVal dt As DataTable, ByVal dsName As String, ByVal dimensionTable As RectangleF, ByVal paddingTextBox As Padding, ByVal paddingHeader As Padding, _
         ByVal heightTextBox As Single, ByVal MaxWidth As Single)
            writer.WriteStartElement("Table")
            writer.WriteAttributeString("Name", "tabella" & dsName)
            If True Then
                writer.WriteStartElement("Style")
                If True Then
                    writer.WriteStartElement("BorderStyle")
                    If True Then
                        writer.WriteElementString("Default", "Solid")
                    End If
                    writer.WriteEndElement()
                End If
                writer.WriteEndElement()

                writer.WriteElementString("Top", dimensionTable.Top.ToString(ci) & "cm")
                writer.WriteElementString("Left", dimensionTable.Left.ToString(ci) & "cm")
                writer.WriteElementString("Width", dimensionTable.Width.ToString(ci) & "cm")
                writer.WriteElementString("Height", dimensionTable.Height.ToString(ci) & "cm")

                writer.WriteStartElement("TableColumns")
                If True Then
                    For i As Integer = 0 To dt.Columns.Count - 1
                        writer.WriteStartElement("TableColumn")
                        If True Then
                            Dim dc As DataColumn = dt.Columns(i)
                            Dim sizeWidthComputed As Single = 0.0F
                            Dim RowMaxLength As Single = GetDynamicSize(dt.Rows(0)(i).ToString()).Width / 10
                            Dim HeaderMaxLength As Single = (GetDynamicSize(dc.ColumnName).Width / 10) + 0.2F
                            For Each row As DataRow In dt.Rows
                                Dim rowSizeWidth As Single = GetDynamicSize(row(i).ToString()).Width / 10
                                If rowSizeWidth > RowMaxLength Then
                                    RowMaxLength = rowSizeWidth
                                End If
                            Next

                            If RowMaxLength > HeaderMaxLength Then
                                If RowMaxLength > MaxWidth Then
                                    sizeWidthComputed = MaxWidth
                                Else
                                    sizeWidthComputed = RowMaxLength
                                End If
                            Else
                                sizeWidthComputed = HeaderMaxLength
                            End If

                            writer.WriteElementString("Width", (sizeWidthComputed).ToString(ci) & "cm")
                        End If
                        writer.WriteEndElement()
                    Next
                End If
                writer.WriteEndElement()

                GenerateSessionTable(SessionTable.Header, writer, dt, paddingHeader, heightTextBox)
                GenerateSessionTable(SessionTable.Details, writer, dt, paddingTextBox, heightTextBox)
            End If
            writer.WriteEndElement()
        End Sub

        Private Sub AddDataSet(ByVal writer As XmlWriter, ByVal dt As DataTable, ByVal dsName As String)
            writer.WriteStartElement("DataSets")
            If True Then
                writer.WriteStartElement("DataSet")
                writer.WriteAttributeString("Name", dsName)
                If True Then
                    writer.WriteStartElement("Fields")
                    If True Then
                        For i As Integer = 0 To dt.Columns.Count - 1
                            writer.WriteStartElement("Field")
                            writer.WriteAttributeString("Name", dt.Columns(i).ColumnName)
                            If True Then
                                writer.WriteElementString("DataField", dt.Columns(i).ColumnName)
                                writer.WriteElementString("rd", "TypeName", nsRd, dt.Columns(i).DataType.ToString())
                            End If
                            writer.WriteEndElement()
                        Next
                    End If
                    writer.WriteEndElement()

                    writer.WriteStartElement("Query")
                    If True Then
                        writer.WriteElementString("DataSourceName", dsName)
                        writer.WriteElementString("CommandText", "")
                        writer.WriteElementString("rd", "DataSourceName", nsRd, "true")
                    End If
                    writer.WriteEndElement()
                End If
                writer.WriteEndElement()
            End If
            writer.WriteEndElement()
        End Sub

        Private Sub AddDataSource(ByVal writer As XmlWriter, ByVal dsName As String)
            writer.WriteStartElement("DataSources")
            If True Then
                writer.WriteStartElement("DataSource")
                If True Then
                    writer.WriteAttributeString("Name", dsName)
                    writer.WriteElementString("DataSourceReference", dsName)
                End If
                writer.WriteEndElement()
            End If
            writer.WriteEndElement()
        End Sub

        Private Sub GenerateTextBox(ByVal writer As XmlWriter, ByVal textboxName As String, ByVal dimension As RectangleF, ByVal padding As Padding, ByVal colors As CellColors, ByVal value As String)
            writer.WriteStartElement("Textbox")
            writer.WriteAttributeString("Name", textboxName)
            If True Then
                writer.WriteElementString("rd", "DefaultName", nsRd, textboxName)
                If dimension <> RectangleF.Empty Then
                    writer.WriteElementString("Top", dimension.Top.ToString(ci) & "cm")
                    writer.WriteElementString("Left", dimension.Left.ToString(ci) & "cm")
                    writer.WriteElementString("Width", dimension.Width.ToString(ci) & "cm")
                    writer.WriteElementString("Height", dimension.Height.ToString(ci) & "cm")
                End If
                writer.WriteElementString("CanGrow", "true")
                writer.WriteElementString("Value", value)
                If padding IsNot Nothing Then
                    writer.WriteStartElement("Style")
                    If True Then
                        writer.WriteStartElement("BorderStyle")
                        If True Then
                            writer.WriteElementString("Default", "Solid")
                        End If
                        writer.WriteEndElement()

                        If colors IsNot Nothing Then
                            writer.WriteElementString("Color", colors.ForegroundColor.Name)
                            writer.WriteElementString("BackgroundColor", colors.BackgroundColor.Name)
                        End If

                        writer.WriteElementString("PaddingLeft", padding.Left.ToString(ci) & "pt")
                        writer.WriteElementString("PaddingRight", padding.Right.ToString(ci) & "pt")
                        writer.WriteElementString("PaddingTop", padding.Top.ToString(ci) & "pt")
                        writer.WriteElementString("PaddingBottom", padding.Bottom.ToString(ci) & "pt")
                    End If
                    writer.WriteEndElement()
                End If
            End If
            writer.WriteEndElement()
        End Sub

        Private Sub GenerateSettingsHeader(ByVal writer As XmlWriter, ByVal InteractiveSize As SizeF, ByVal PageSize As SizeF, ByVal margin As Padding)
            writer.WriteElementString("Language", "it-IT")
            writer.WriteElementString("rd", "DrawGrid", nsRd, "true")
            writer.WriteElementString("rd", "gridspacing", nsRd, "0.25cm")
            writer.WriteElementString("rd", "snaptogrid", nsRd, "true")
            writer.WriteElementString("InteractiveHeight", InteractiveSize.Height.ToString(ci) & "cm")
            writer.WriteElementString("InteractiveWidth", InteractiveSize.Width.ToString(ci) & "cm")
            writer.WriteElementString("RightMargin", margin.Right.ToString(ci) & "cm")
            writer.WriteElementString("LeftMargin", margin.Left.ToString(ci) & "cm")
            writer.WriteElementString("BottomMargin", margin.Bottom.ToString(ci) & "cm")
            writer.WriteElementString("TopMargin", margin.Top.ToString(ci) & "cm")
            writer.WriteElementString("PageHeight", PageSize.Height.ToString(ci) & "cm")
            writer.WriteElementString("PageWidth", PageSize.Width.ToString(ci) & "cm")
            writer.WriteElementString("Width", PageSize.Width.ToString(ci) & "cm")
        End Sub
#End Region
    End Class

#Region "Definitions"
    Public Enum SessionTable
        Header
        Details
        Footer
    End Enum

    Public Class CellColors
        Public Sub New(ByVal bg As Color, ByVal fore As Color)
            Me.bg = bg
            Me.fore = fore
        End Sub
        Private bg As Color = Color.Empty
        Private fore As Color = Color.Empty

        Public ReadOnly Property BackgroundColor() As Color
            Get
                Return bg
            End Get
        End Property
        Public ReadOnly Property ForegroundColor() As Color
            Get
                Return fore
            End Get
        End Property
    End Class

    Public Class Padding
        Public Sub New(ByVal Top As Single, ByVal Left As Single, ByVal Bottom As Single, ByVal Right As Single)
            TopLeft = New PointF(Left, Top)
            BottomRight = New PointF(Right, Bottom)
        End Sub

        Private Property TopLeft() As PointF
            Get
                Return m_TopLeft
            End Get
            Set(ByVal value As PointF)
                m_TopLeft = value
            End Set
        End Property
        Private m_TopLeft As PointF
        Private Property BottomRight() As PointF
            Get
                Return m_BottomRight
            End Get
            Set(ByVal value As PointF)
                m_BottomRight = value
            End Set
        End Property
        Private m_BottomRight As PointF

        Public ReadOnly Property Top() As Single
            Get
                Return TopLeft.Y
            End Get
        End Property
        Public ReadOnly Property Left() As Single
            Get
                Return TopLeft.X
            End Get
        End Property
        Public ReadOnly Property Bottom() As Single
            Get
                Return BottomRight.Y
            End Get
        End Property
        Public ReadOnly Property Right() As Single
            Get
                Return BottomRight.X
            End Get
        End Property
    End Class
#End Region

End Namespace


Public Class ReportGenerator
    
End Class
