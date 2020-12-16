﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

namespace ReportServiceReportView
{
    public class ReportGenerator
    {
        private float multipageRatio = 1.5F;
        private CultureInfo ci = new CultureInfo("en-US");
        private DataTable dt = null;
        private string dsName = "";
        private string nsRd = "http://schemas.microsoft.com/SQLServer/reporting/reportdesigner";
        private string ns = "http://schemas.microsoft.com/sqlserver/reporting/2005/01/reportdefinition";

        public ReportGenerator(DataTable dt, string dsName)
        {
            this.dt = dt;
            this.dsName = dsName;
        }

        public Stream GeneraReport()
        {
            string xml;
            StringBuilder sb = new StringBuilder();

            #region Settings

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.CheckCharacters = true;
            settings.CloseOutput = true;
            settings.Encoding = Encoding.UTF8;
            settings.Indent = true;
            settings.IndentChars = "\t";
            settings.NewLineChars = "\r\n";
            settings.NewLineHandling = NewLineHandling.Replace;
            settings.NewLineOnAttributes = false;
            settings.OmitXmlDeclaration = false;

            #endregion Settings

            XmlWriter writer = XmlWriter.Create(sb, settings);
            writer.WriteStartDocument();
            {
                writer.WriteStartElement("Report", ns);
                writer.WriteAttributeString("xmlns", "rd", "", nsRd);
                {
                    AddDataSource(writer, dsName);
                    float htb = 0.63492F, maxWidth = 4.0F;
                    RectangleF dimensioni = new RectangleF(1.37301F, 0.68783F, 13.25397F, 2.98941F);
                    Padding pad = new Padding(2, 2, 2, 2);
                    SizeF size = new SizeF(21F, 29.5F);
                    Padding margin = new Padding(0.5F, 0.5F, 0.5F, 0.5F);
                    GenerateSettingsHeader(writer, size, size, margin);

                    AddDataSet(writer, dt, dsName);
                    writer.WriteStartElement("Body");
                    {
                        writer.WriteElementString("ColumnSpacing", "1cm");
                        writer.WriteElementString("Height", "5cm");
                        writer.WriteStartElement("ReportItems");
                        {
                            GeneraTabella(writer, dt, dsName, dimensioni, pad, pad, htb, maxWidth);
                        }
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            writer.WriteEndDocument();
            writer.Flush();
            writer.Close();
            xml = sb.ToString().Replace("utf-16", "utf-8");
            Stream ret = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            return ret;
        }

        #region Metodi Privati

        private SizeF GetDynamicSize(string s)
        {
            Font f = new Font(FontFamily.GenericSansSerif, 10);
            Bitmap bmp = new Bitmap(1, 1);
            Graphics g = Graphics.FromImage(bmp);
            g.PageUnit = GraphicsUnit.Millimeter;
            SizeF ret = SizeF.Empty;
            ret = g.MeasureString(s, f);
            g.Dispose();
            return ret;
        }

        private void GeneraSezioneTabella(SezioneTabella sezione, XmlWriter writer, DataTable dt, Padding padding, float height)
        {
            string nomeSezione = "", templateValore = "", valore = "";
            CellColors colors = null;
            switch (sezione)
            {
                case SezioneTabella.Header:
                    {
                        nomeSezione = "Header";
                        templateValore = "{0}";
                        colors = new CellColors(Color.Black, Color.White);
                        break;
                    }
                case SezioneTabella.Details:
                    {
                        nomeSezione = "Details";
                        templateValore = "=Fields!{0}.Value";
                        break;
                    }
                case SezioneTabella.Footer:
                    {
                        nomeSezione = "Footer";
                        templateValore = "{0}";
                        break;
                    }
            }
            writer.WriteStartElement(nomeSezione);
            {
                if (sezione == SezioneTabella.Header)
                    writer.WriteElementString("RepeatOnNewPage", "true");
                writer.WriteStartElement("TableRows");
                {
                    writer.WriteStartElement("TableRow");
                    {
                        writer.WriteElementString("Height", height.ToString(ci) + "cm");
                        writer.WriteStartElement("TableCells");
                        {
                            for (int i = 0; i < dt.Columns.Count; i++)
                            {
                                writer.WriteStartElement("TableCell");
                                {
                                    writer.WriteStartElement("ReportItems");
                                    {
                                        valore = String.Format(templateValore, dt.Columns[i].ColumnName);
                                        GeneraTextBox(writer, "textbox" + nomeSezione + i, RectangleF.Empty, padding, colors, valore);
                                    }
                                    writer.WriteEndElement();
                                }
                                writer.WriteEndElement();
                            }
                        }
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        private void GeneraTabella(XmlWriter writer, DataTable dt, string dsName, RectangleF dimensioniTabella, Padding paddingTextBox, Padding paddingHeader, float heightTextBox, float MaxWidth)
        {
            writer.WriteStartElement("Table");
            writer.WriteAttributeString("Name", "tabella" + dsName);
            {
                writer.WriteStartElement("Style");
                {
                    writer.WriteStartElement("BorderStyle");
                    {
                        writer.WriteElementString("Default", "Solid");
                    }
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();

                writer.WriteElementString("Top", dimensioniTabella.Top.ToString(ci) + "cm");
                writer.WriteElementString("Left", dimensioniTabella.Left.ToString(ci) + "cm");
                writer.WriteElementString("Width", dimensioniTabella.Width.ToString(ci) + "cm");
                writer.WriteElementString("Height", dimensioniTabella.Height.ToString(ci) + "cm");

                writer.WriteStartElement("TableColumns");
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        writer.WriteStartElement("TableColumn");
                        {
                            DataColumn dc = dt.Columns[i];
                            float sizeWidthComputed = 0.0F;
                            float RowMaxLength = GetDynamicSize(dt.Rows[0][i].ToString()).Width / 10;
                            float HeaderMaxLength = (GetDynamicSize(dc.ColumnName).Width / 10) + 0.2F;
                            foreach (DataRow row in dt.Rows)
                            {
                                float rowSizeWidth = GetDynamicSize(row[i].ToString()).Width / 10;
                                if (rowSizeWidth > RowMaxLength)
                                    RowMaxLength = rowSizeWidth;
                            }

                            if (RowMaxLength > HeaderMaxLength)
                                if (RowMaxLength > MaxWidth)
                                    sizeWidthComputed = MaxWidth;
                                else
                                    sizeWidthComputed = RowMaxLength;
                            else
                                sizeWidthComputed = HeaderMaxLength;

                            writer.WriteElementString("Width", (sizeWidthComputed).ToString(ci) + "cm");
                        }
                        writer.WriteEndElement();
                    }
                }
                writer.WriteEndElement();

                GeneraSezioneTabella(SezioneTabella.Header, writer, dt, paddingHeader, heightTextBox);
                GeneraSezioneTabella(SezioneTabella.Details, writer, dt, paddingTextBox, heightTextBox);
            }
            writer.WriteEndElement();
        }

        private void AddDataSet(XmlWriter writer, DataTable dt, string dsName)
        {
            writer.WriteStartElement("DataSets");
            {
                writer.WriteStartElement("DataSet");
                writer.WriteAttributeString("Name", dsName);
                {
                    writer.WriteStartElement("Fields");
                    {
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            writer.WriteStartElement("Field");
                            writer.WriteAttributeString("Name", dt.Columns[i].ColumnName);
                            {
                                writer.WriteElementString("DataField", dt.Columns[i].ColumnName);
                                writer.WriteElementString("rd", "TypeName", nsRd, dt.Columns[i].DataType.ToString());
                            }
                            writer.WriteEndElement();
                        }
                    }
                    writer.WriteEndElement();

                    writer.WriteStartElement("Query");
                    {
                        writer.WriteElementString("DataSourceName", dsName);
                        writer.WriteElementString("CommandText", "");
                        writer.WriteElementString("rd", "DataSourceName", nsRd, "true");
                    }
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        private void AddDataSource(XmlWriter writer, string dsName)
        {
            writer.WriteStartElement("DataSources");
            {
                writer.WriteStartElement("DataSource");
                {
                    writer.WriteAttributeString("Name", dsName);
                    writer.WriteElementString("DataSourceReference", dsName);
                }
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        private void GeneraTextBox(XmlWriter writer, string textboxName, RectangleF dimensioni, Padding padding, CellColors colors, string value)
        {
            writer.WriteStartElement("Textbox");
            writer.WriteAttributeString("Name", textboxName);
            {
                writer.WriteElementString("rd", "DefaultName", nsRd, textboxName);
                if (dimensioni != RectangleF.Empty)
                {
                    writer.WriteElementString("Top", dimensioni.Top.ToString(ci) + "cm");
                    writer.WriteElementString("Left", dimensioni.Left.ToString(ci) + "cm");
                    writer.WriteElementString("Width", dimensioni.Width.ToString(ci) + "cm");
                    writer.WriteElementString("Height", dimensioni.Height.ToString(ci) + "cm");
                }
                writer.WriteElementString("CanGrow", "true");
                writer.WriteElementString("Value", value);
                if (padding != null)
                {
                    writer.WriteStartElement("Style");
                    {
                        writer.WriteStartElement("BorderStyle");
                        {
                            writer.WriteElementString("Default", "Solid");
                        }
                        writer.WriteEndElement();

                        if (colors != null)
                        {
                            writer.WriteElementString("Color", colors.ForegroundColor.Name);
                            writer.WriteElementString("BackgroundColor", colors.BackgroundColor.Name);
                        }

                        writer.WriteElementString("PaddingLeft", padding.Left.ToString(ci) + "pt");
                        writer.WriteElementString("PaddingRight", padding.Right.ToString(ci) + "pt");
                        writer.WriteElementString("PaddingTop", padding.Top.ToString(ci) + "pt");
                        writer.WriteElementString("PaddingBottom", padding.Bottom.ToString(ci) + "pt");
                    }
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();
        }

        private void GenerateSettingsHeader(XmlWriter writer, SizeF InteractiveSize, SizeF PageSize, Padding margin)
        {
            writer.WriteElementString("Language", "it-IT");
            writer.WriteElementString("rd", "DrawGrid", nsRd, "true");
            writer.WriteElementString("rd", "gridspacing", nsRd, "0.25cm");
            writer.WriteElementString("rd", "snaptogrid", nsRd, "true");
            writer.WriteElementString("InteractiveHeight", InteractiveSize.Height.ToString(ci) + "cm");
            writer.WriteElementString("InteractiveWidth", InteractiveSize.Width.ToString(ci) + "cm");
            writer.WriteElementString("RightMargin", margin.Right.ToString(ci) + "cm");
            writer.WriteElementString("LeftMargin", margin.Left.ToString(ci) + "cm");
            writer.WriteElementString("BottomMargin", margin.Bottom.ToString(ci) + "cm");
            writer.WriteElementString("TopMargin", margin.Top.ToString(ci) + "cm");
            writer.WriteElementString("PageHeight", PageSize.Height.ToString(ci) + "cm");
            writer.WriteElementString("PageWidth", PageSize.Width.ToString(ci) + "cm");
            writer.WriteElementString("Width", PageSize.Width.ToString(ci) + "cm");
        }

        #endregion Metodi Privati
    }

    #region Oggetti Custom

    public enum SezioneTabella
    {
        Header,
        Details,
        Footer
    }

    public class CellColors
    {
        public CellColors(Color bg, Color fore)
        {
            this.bg = bg;
            this.fore = fore;
        }

        private Color bg = Color.Empty;
        private Color fore = Color.Empty;

        public Color BackgroundColor { get { return bg; } }

        public Color ForegroundColor { get { return fore; } }
    }

    public class Padding
    {
        public Padding(float Top, float Left, float Bottom, float Right)
        {
            TopLeft = new PointF(Left, Top);
            BottomRight = new PointF(Right, Bottom);
        }

        private PointF TopLeft { get; set; }

        private PointF BottomRight { get; set; }

        public float Top { get { return TopLeft.Y; } }

        public float Left { get { return TopLeft.X; } }

        public float Bottom { get { return BottomRight.Y; } }

        public float Right { get { return BottomRight.X; } }
    }

    #endregion Oggetti Custom
}