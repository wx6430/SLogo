using System;
using SLogo.Interpret;
using SLogo.Canvas;
using System.IO;

namespace SLogo.Painter
{
    internal class Canvas
    {
        Turtle turtle;

        public Canvas(Turtle turtle)
        {
            this.turtle = new Turtle();
        }

        public void FD(Variable[] args)
        {
            if (args[0].Type != VarType.Double
                || Double.IsInfinity(args[0].ToDouble())
                || Double.IsNaN(args[0].ToDouble()))
            {
                throw new PrimeExecException("invalid number");
            }
            turtle.Forward(args[0].ToDouble());
        }

        public void BK(Variable[] args)
        {
            if (args[0].Type != VarType.Double
                || Double.IsInfinity(args[0].ToDouble())
                || Double.IsNaN(args[0].ToDouble()))
            {
                throw new PrimeExecException("invalid number");
            }
            turtle.Back(args[0].ToDouble());
        }

        public void RT(Variable[] args)
        {
            if (args[0].Type != VarType.Double
                || Double.IsInfinity(args[0].ToDouble())
                || Double.IsNaN(args[0].ToDouble()))
            {
                throw new PrimeExecException("invalid number for degree");
            }
            turtle.TurnRight(args[0].ToDouble());
        }

        public void LT(Variable[] args)
        {
            if (args[0].Type != VarType.Double
                || Double.IsInfinity(args[0].ToDouble())
                || Double.IsNaN(args[0].ToDouble()))
            {
                throw new PrimeExecException("invalid number for degree");
            }
            turtle.TurnLeft(args[0].ToDouble());
        }

        public void PU(Variable[] args)
        {
            turtle.PenUp();
        }

        public void PD(Variable[] args)
        {
            turtle.PenDown();
        }

        public void CS(Variable[] args)
        {
            turtle.ClearScreen();
        }

        public void HOME(Variable[] args)
        {
            turtle.Home();
        }

        public void SETX(Variable[] args)
        {
            if (args[0].Type != VarType.Double
                || Double.IsInfinity(args[0].ToDouble())
                || Double.IsNaN(args[0].ToDouble()))
            {
                throw new PrimeExecException("invalid number for X");
            }
            turtle.SetX(args[0].ToDouble());
        }

        public void SETY(Variable[] args)
        {
            if (args[0].Type != VarType.Double
                || Double.IsInfinity(args[0].ToDouble())
                || Double.IsNaN(args[0].ToDouble()))
            {
                throw new PrimeExecException("invalid number for Y");
            }
            turtle.SetY(args[0].ToDouble());
        }

        public void SETXY(Variable[] args)
        {
            if (args[0].Type != VarType.Double
                || Double.IsInfinity(args[0].ToDouble())
                || Double.IsNaN(args[0].ToDouble()))
            {
                throw new PrimeExecException("invalid number for X");
            }
            if (args[1].Type != VarType.Double
                || Double.IsInfinity(args[1].ToDouble())
                || Double.IsNaN(args[1].ToDouble()))
            {
                throw new PrimeExecException("invalid number for Y");
            }
            turtle.SetXY(args[0].ToDouble(), args[1].ToDouble());
        }

        public void SETH(Variable[] args)
        {
            if (args[0].Type != VarType.Double
                || Double.IsInfinity(args[0].ToDouble())
                || Double.IsNaN(args[0].ToDouble()))
            {
                throw new PrimeExecException("invalid degree for H");
            }
            turtle.SetH(args[0].ToDouble());
        }

        public void PRINT(Variable[] args)
        {
            string text;
            if (args[0].Type == VarType.Double)
            {
                text = String.Format("{0:0.####}", Math.Truncate(args[0].ToDouble() * 10000) / 10000);   //What the fuck
            }
            else
            {
                text = args[0].ToString();
            }
            turtle.AddText(text);
        }

        public void STOP(Variable[] args)
        {
            throw new ProcStopSignal();
        }

        //设置画笔颜色
        public void COLOR(Variable[] args)
        {
            string rgb = "RGB";
            int[] rgbValues = new int[3];
            for (int i = 0; i < 3; i++)
            {
                if (args[i].Type != VarType.Double)
                {
                    throw new PrimeExecException(String.Format("invalid {0} value for RGB", rgb[i]));
                }
                else if (args[i].ToDouble() < 0 || args[i].ToDouble() > 255)
                {
                    throw new PrimeExecException(String.Format("{0} should range from 0 to 255 in RGB", rgb[i]));
                }
                rgbValues[i] = (int)Math.Round(args[i].ToDouble());
            }
            turtle.SetStrokeColor(rgbValues[0], rgbValues[1], rgbValues[2]);
        }

        //设置画笔粗细
        public void STROKE(Variable[] args)
        {
            if (args[0].Type != VarType.Double
                || Double.IsInfinity(args[0].ToDouble())
                || Double.IsNaN(args[0].ToDouble()))
            {
                throw new PrimeExecException("invalid width for stroke");
            }
            if (args[0].ToDouble() < 1 || args[0].ToDouble() > 20)
            {
                throw new PrimeExecException("stroke should range from 1 to 20");
            }
            turtle.SetStrokeWidth(args[0].ToDouble());
        }

        //设置字体大小
        public void FONTSIZE(Variable[] args)
        {
            if (args[0].Type != VarType.Double
                || Double.IsInfinity(args[0].ToDouble())
                || Double.IsNaN(args[0].ToDouble()))
            {
                throw new PrimeExecException("invalid font size");
            }
            if (args[0].ToDouble() < 1 || args[0].ToDouble() > 72)
            {
                throw new PrimeExecException("font size should range from 1 to 72");
            }
            turtle.SetFontSize((int)Math.Round(args[0].ToDouble()));
        }

        //画圆
        public void CIRCLE(Variable[] args)
        {
            if (args[0].Type != VarType.Double
                || Double.IsInfinity(args[0].ToDouble())
                || Double.IsNaN(args[0].ToDouble())
                || args[0].ToDouble() < 0)
            {
                throw new PrimeExecException("invalid number for radius");
            }
            turtle.AddCircle(args[0].ToDouble());
        }

        //贝塞尔曲线
        public void BEZIER(Variable[] args)
        {
            string[] valNames = { "middle point X", "middle point Y", "end point X", "end point Y" };
            double[] pointVals = new double[4];
            for (int i = 0; i < 4; i++)
            {
                if (args[i].Type != VarType.Double)
                {
                    throw new PrimeExecException(String.Format("invalid {0} value", valNames[i]));
                }
                pointVals[i] = args[i].ToDouble();
            }
            turtle.AddBezier(pointVals[0], pointVals[1], pointVals[2], pointVals[3]);
        }

        //椭圆
        public void ELLIPSE(Variable[] args)
        {
            if (args[0].Type != VarType.Double
                || Double.IsInfinity(args[0].ToDouble())
                || Double.IsNaN(args[0].ToDouble())
                || args[0].ToDouble() < 0)
            {
                throw new PrimeExecException("invalid number for major axe");
            }
            if (args[1].Type != VarType.Double
                || Double.IsInfinity(args[1].ToDouble())
                || Double.IsNaN(args[1].ToDouble())
                || args[1].ToDouble() < 0)
            {
                throw new PrimeExecException("invalid number for minor axe");
            }
            turtle.AddEllipse(args[0].ToDouble(), args[1].ToDouble());
        }

        //圆角矩形
        public void RECT(Variable[] args)
        {
            if (args[0].Type != VarType.Double
                || Double.IsInfinity(args[0].ToDouble())
                || Double.IsNaN(args[0].ToDouble())
                || args[0].ToDouble() < 0)
            {
                throw new PrimeExecException("invalid number for width");
            }
            if (args[1].Type != VarType.Double
                || Double.IsInfinity(args[1].ToDouble())
                || Double.IsNaN(args[1].ToDouble())
                || args[1].ToDouble() < 0)
            {
                throw new PrimeExecException("invalid number for height");
            }
            if (args[2].Type != VarType.Double
                || Double.IsInfinity(args[2].ToDouble())
                || Double.IsNaN(args[2].ToDouble())
                || args[2].ToDouble() < 0)
            {
                throw new PrimeExecException("invalid number for corner radius");
            }
            if (args[2].ToDouble() * 2 > Math.Min(args[0].ToDouble(), args[1].ToDouble()))
            {
                throw new PrimeExecException("corner radius too large");
            }
            turtle.AddRect(args[0].ToDouble(), args[1].ToDouble(), args[2].ToDouble());
        }

        //设置填充颜色
        public void FILL(Variable[] args)
        {
            string rgb = "RGB";
            int[] rgbValues = new int[3];
            for (int i = 0; i < 3; i++)
            {
                if (args[i].Type != VarType.Double)
                {
                    throw new PrimeExecException(String.Format("invalid {0} value for RGB", rgb[i]));
                }
                else if (args[i].ToDouble() < 0 || args[i].ToDouble() > 255)
                {
                    throw new PrimeExecException(String.Format("{0} should range from 0 to 255 in RGB", rgb[i]));
                }
                rgbValues[i] = (int)Math.Round(args[i].ToDouble());
            }
            turtle.SetFillColor(rgbValues[0], rgbValues[1], rgbValues[2]);
        }

        //设置填充透明度
        public void FILLOPACITY(Variable[] args)
        {
            if (args[0].Type != VarType.Double
                || Double.IsInfinity(args[0].ToDouble())
                || Double.IsNaN(args[0].ToDouble()))
            {
                throw new PrimeExecException("invalid opacity value");
            }
            if (args[0].ToDouble() < 0 || args[0].ToDouble() > 1)
            {
                throw new PrimeExecException("opacity should range from 0 to 1");
            }
            turtle.SetFillOpacity(args[0].ToDouble());
        }

        //画笔透明度
        public void OPACITY(Variable[] args)
        {
            if (args[0].Type != VarType.Double
                || Double.IsInfinity(args[0].ToDouble())
                || Double.IsNaN(args[0].ToDouble()))
            {
                throw new PrimeExecException("invalid opacity value");
            }
            if (args[0].ToDouble() < 0 || args[0].ToDouble() > 1)
            {
                throw new PrimeExecException("opacity should range from 0 to 1");
            }
            turtle.SetStrokeOpacity(args[0].ToDouble());
        }

        //设置背景颜色
        public void BACKGROUND(Variable[] args)
        {
            string rgb = "RGB";
            int[] rgbValues = new int[3];
            for (int i = 0; i < 3; i++)
            {
                if (args[i].Type != VarType.Double)
                {
                    throw new PrimeExecException(String.Format("invalid {0} value for RGB", rgb[i]));
                }
                else if (args[i].ToDouble() < 0 || args[i].ToDouble() > 255)
                {
                    throw new PrimeExecException(String.Format("{0} should range from 0 to 255 in RGB", rgb[i]));
                }
                rgbValues[i] = (int)Math.Round(args[i].ToDouble());
            }
            turtle.SetBackground(rgbValues[0], rgbValues[1], rgbValues[2]);
        }

        public void SetMargin(int marginU, int marginD, int marginL, int marginR)
        {
            turtle.SetMargin(marginU, marginD, marginL, marginR);
        }

        public void Print(PrintType type)
        {
            turtle.Print(type);
        }
    }
}
