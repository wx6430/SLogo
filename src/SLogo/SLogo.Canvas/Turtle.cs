using System;
using System.Drawing;
using SVGLib;
using System.IO;
using System.Xml;
using System.Text;

namespace SLogo.Canvas
{
    public enum PrintType
    {
        Png,
        Svg
    }

	public class Point
	{
		public double x = 0;
		public double y = 0;

		public Point(double x, double y)
		{
			this.x = x;
			this.y = y;
		}
		public double GetDistance(Point destination)
		{
			return Math.Sqrt((this.x - destination.x) * (this.x - destination.x)
				+ (this.y - destination.y) * (this.y - destination.y));
		}
	}

	public class Turtle
	{
		//��������
		private Point position;
		private double angle;
		int isPenUp;

		//��������
		private Color strokeColor;
		private Color fillColor;
		private String fillOpacity;
		private String strokeWidth;
		private String strokeOpacity;
		private String fontSize;
		private Color background;
		private int marginL;
		private int marginR;
		private int marginU;
		private int marginD;
		private bool marginFlag;

		//����
		private SvgDoc doc;
		private SvgRoot root;

		public Turtle()
		{
			position = new Point(0, 0);
			angle = 0;
			isPenUp = 0;
			strokeColor = Color.Black;
			fillColor = Color.Black;
			background = Color.White;
			fillOpacity = "0";
			strokeWidth = "1";
			strokeOpacity = "1";
			fontSize = "20";
			marginFlag = false;
			
			doc = new SvgDoc();
			root = doc.CreateNewDocument();
			root.Width = "1000";
			root.Height = "1000";
		}

		#region ͼ������ת��

		private string TransformX(double x)
		{
			return (x + 500).ToString();
		}

		private string TransformY(double y)
		{
			return (-y + 500).ToString();
		}
		#endregion

		#region ������ͼָ��
		// FD ָ��
		public void Forward(double step)
		{
			double x, y;
			x = position.x + step * Math.Sin(angle / 180 * Math.PI);
			y = position.y + step * Math.Cos(angle / 180 * Math.PI);
			SetXY(x, y);
		}

		// BK ָ��
		public void Back(double step)
		{
			double x, y;
			x = position.x - step * Math.Sin(angle / 180 * Math.PI);
			y = position.y - step * Math.Cos(angle / 180 * Math.PI);
			SetXY(x, y);
		}

		// RT ָ��
		public void TurnRight(double angle)
		{
			this.angle += angle;
		}

		// LT ָ��
		public void TurnLeft(double angle)
		{
			this.angle -= angle;
		}

		// PU ָ��
		public void PenUp()
		{
			this.isPenUp = 1;
		}

		// PD ָ��
		public void PenDown()
		{
			this.isPenUp = 0;
		}

		// CS ָ��
		public void ClearScreen()
		{
			SetXY(0, 0);
			SetH(0);

			// ��ջ���
			root = doc.CreateNewDocument();
			root.Width = "1000";
			root.Height = "1000";
		}

		// HOME ָ��
		public void Home()
		{
			SetXY(0, 0);
			SetH(0);
		}

		// SETX ָ��
		public void SetX(double x)
		{
			SetXY(x, this.position.y);
		}

		// SETY ָ��
		public void SetY(double y)
		{
			SetXY(this.position.x, y);
		}

		// SETXY ָ��
		public void SetH(double angle)
		{
			this.angle = angle;
		}

		// SETXY ָ��
		public void SetXY(double x, double y)
		{
			if (isPenUp == 0)
			{
				AddLine(this.position.x, this.position.y, x, y);
			}
			this.position.x = x;
			this.position.y = y;
		}
		#endregion

		#region ���ʡ�������������

		// ���ñ߿���ɫ
		public void SetStrokeColor(int r, int g, int b)
		{
			this.strokeColor = Color.FromArgb(r, g, b);
		}

		// ���ñ߿���ɫ
		public void SetStrokeColor(Color color)
		{
			this.strokeColor = color;
		}

		// ���������ɫ
		public void SetFillColor(int r, int g, int b)
		{
			this.fillColor = Color.FromArgb(r, g, b);
		}

		// ���������ɫ
		public void SetFillColor(Color color)
		{
			this.fillColor = color;
		}

		// �������ɫ͸����
		public void SetFillOpacity(double fillOpacity)
		{
			this.fillOpacity = fillOpacity.ToString();
		}

		// ���ñ߿���
		public void SetStrokeWidth(double strokeWidth)
		{
			this.strokeWidth = strokeWidth.ToString();
		}

		// ���������С
		public void SetFontSize(int fontSize)
		{
			this.fontSize = fontSize.ToString();
		}

		// ���ñ߾�
		public void SetMargin(int margin)
		{
			if (margin < 0)
			{
				throw new ArgumentException("margin cannot be less than zero");
			}
			this.marginU = this.marginD = margin;
			this.marginL = this.marginR = margin;
			marginFlag = true;
		}

		public void SetMargin(int marginU, int marginD, int marginL, int marginR)
		{
			if (marginU < 0 || marginD < 0
				|| marginL < 0 || marginR < 0)
			{
				throw new ArgumentException("margin cannot be less than zero");
			}
			marginFlag = true;
			this.marginU = marginU;
			this.marginD = marginD;
			this.marginL = marginL;
			this.marginR = marginR;
		}

		//���ñ���
		public void SetBackground(int r, int g, int b)
		{
			this.background = Color.FromArgb(r, g, b);
		}

		//���ñ߿�͸����
		public void SetStrokeOpacity(double strokeOpacity)
		{
			this.strokeOpacity = strokeOpacity.ToString();
		}

		//��ӱ���
		private void AddBackground()
		{
			SvgRect rect = new SvgRect(doc);
			rect.X = "0";
			rect.Y = "0";
			rect.Width = root.Width;
			rect.Height = root.Height;
			rect.FillOpacity = "1";
			rect.Fill = background;
			doc.AddElement(root, rect);
			while (doc.ElementPositionUp(rect)) ;
		}
		#endregion

		#region ���ͼ��Ԫ��

		// ����߶�
		private void AddLine(double x1, double y1, double x2, double y2)
		{
			SvgLine line = new SvgLine(doc,
					TransformX(x1),
					TransformY(y1),
					TransformX(x2),
					TransformY(y2),
					fillColor);
			line.Stroke = strokeColor;
			line.StrokeWidth = strokeWidth;
			line.StrokeOpacity = strokeOpacity;
			line.StrokeLineCap = SvgAttribute._SvgLineCap.round;
			doc.AddElement(root, line);
			if (marginFlag)
			{
				doc.UpdateRange(x1, y1);
				doc.UpdateRange(x2, y2);
			}
		}

		// ����ı�
		public void AddText(String textValue)
		{
			SvgText text = new SvgText(doc);
			text.Value = textValue;
			text.X = TransformX(this.position.x);
			text.Y = TransformY(this.position.y);
			text.FontFamily = "Courier New";
			text.FontSize = fontSize;
			text.angle = this.angle;
			text.Transform = "rotate(" + this.angle.ToString() + ","
				+ text.X + "," + text.Y + ")";
			doc.AddElement(root, text);
			if (marginFlag)
			{
				double sin = Math.Sin(angle / 180 * Math.PI);
				double cos = Math.Cos(angle / 180 * Math.PI);
				int k = int.Parse(fontSize);
				double lenth = textValue.Length * 0.6 * k;
				double height = 0.75 * k;
				doc.UpdateRange(this.position.x, this.position.y);
				doc.UpdateRange(this.position.x + cos * lenth, this.position.y - sin * lenth);
				doc.UpdateRange(this.position.x + sin * height, this.position.y + cos * height);
				doc.UpdateRange(this.position.x + cos * lenth + sin * height, this.position.y - sin * lenth + cos * height);
			}
		}

		// ���Բ
		public void AddCircle(double r)
		{
			SvgCircle circle = new SvgCircle(doc);
			circle.CX = TransformX(this.position.x);
			circle.CY = TransformY(this.position.y);
			circle.R = r.ToString();
			circle.StrokeWidth = strokeWidth;
			circle.FillOpacity = fillOpacity;
			circle.Fill = fillColor;
			circle.Stroke = strokeColor;
			circle.StrokeOpacity = strokeOpacity;
			doc.AddElement(root, circle);
			if (marginFlag)
			{
				doc.UpdateRange(this.position.x + r, this.position.y + r);
				doc.UpdateRange(this.position.x - r, this.position.y - r);
			}
		}

		// ���Բ
		public void AddCircle(double x, double y)
		{
			Point centre = new Point(x, y);
			SvgCircle circle = new SvgCircle(doc);
			circle.CX = TransformX(x);
			circle.CY = TransformY(y);
			double r = this.position.GetDistance(centre);
			circle.R = r.ToString();
			circle.StrokeWidth = strokeWidth;
			circle.FillOpacity = fillOpacity;
			circle.Fill = fillColor;
			circle.Stroke = strokeColor;
			circle.StrokeOpacity = strokeOpacity;
			doc.AddElement(root, circle);
			if (marginFlag)
			{
				doc.UpdateRange(centre.x + r, centre.y + r);
				doc.UpdateRange(centre.y - r, centre.y - r);
			}
		}

		// �����Բ
		public void AddEllipse(double x, double y)
		{
			SvgEllipse ellipse = new SvgEllipse(doc);
			ellipse.CX = TransformX(this.position.x);
			ellipse.CY = TransformY(this.position.y);
			ellipse.RX = x.ToString();
			ellipse.RY = y.ToString();
			ellipse.StrokeWidth = strokeWidth;
			ellipse.FillOpacity = fillOpacity;
			ellipse.Fill = fillColor;
			ellipse.Stroke = strokeColor;
			ellipse.StrokeOpacity = strokeOpacity;
			doc.AddElement(root, ellipse);
			if (marginFlag)
			{
				doc.UpdateRange(this.position.x + x, this.position.y + y);
				doc.UpdateRange(this.position.x - x, this.position.y - y);

			}
		}

		public void AddRect(double width, double height, double r)
		{
			SvgRect rect = new SvgRect(doc);
			rect.X = TransformX(this.position.x);
			rect.Y = TransformY(this.position.y);
			rect.RX = rect.RY = r.ToString();
			rect.Fill = fillColor;
			rect.FillOpacity = fillOpacity;
			rect.Stroke = strokeColor;
			rect.StrokeWidth = strokeWidth;
			rect.StrokeOpacity = strokeOpacity;
			rect.Width = width.ToString();
			rect.Height = height.ToString();
			doc.AddElement(root, rect);
			if (marginFlag)
			{
				doc.UpdateRange(this.position.x + width, this.position.y - height);
				doc.UpdateRange(this.position.x, this.position.y);
			}
		}

		public void AddBezier(double x1, double y1, double x2, double y2)
		{
			SvgPath path = new SvgPath(doc);
			path.Stroke = strokeColor;
			path.Fill = fillColor;
			path.FillOpacity = fillOpacity;
			path.StrokeOpacity = strokeOpacity;
			path.StrokeWidth = strokeWidth;
			path.Data = "q" + x1.ToString() + " " + (-y1).ToString()
				+ " " + x2.ToString() + " " + (-y2).ToString();
			path.SetStart(TransformX(this.position.x), TransformY(this.position.y));
			path.StrokeOpacity = strokeOpacity;
			doc.AddElement(root, path);
			if (marginFlag)
			{
				doc.UpdateRange(this.position.x, this.position.y);
				doc.UpdateRange(this.position.x + x1, this.position.y + y1);
				doc.UpdateRange(this.position.x + x2, this.position.y + y2);
			}
			this.position.x = x2;
			this.position.y = y2;
		}

		#endregion

		public void Print(PrintType type)
		{
			if (marginFlag)
			{
				doc.ChangeSize(marginU, marginD, marginL, marginR);
			}
			AddBackground();
			string xml = doc.GetXML();

			if (type == PrintType.Svg)
			{
				Console.Write(xml);
			}
			else if (type == PrintType.Png)
			{
				byte[] byteArray = System.Text.Encoding.Default.GetBytes(xml);

				using (var stream = new System.IO.MemoryStream(byteArray))
				{
					var sampleDoc = Svg.SvgDocument.Open(stream);
                    sampleDoc.Draw().Save(Console.OpenStandardOutput(), System.Drawing.Imaging.ImageFormat.Png);

					//������
                    //sampleDoc.Draw().Save(@"D:\test.png", System.Drawing.Imaging.ImageFormat.Png);
				}
			}			
		}
	}
}