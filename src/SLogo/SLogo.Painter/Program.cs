using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine;
using SLogo.Helper;
using SLogo.Lex;
using System.IO;
using SLogo.Canvas;

namespace SLogo.Painter
{
    sealed partial class Program
    {
        static bool useMargin = false;
        static int marginU, marginD, marginL, marginR;
        static PrintType outputType;

        static void Main(string[] args)
        {
            var options = new CmdOptions();
            ICommandLineParser parser = new CommandLineParser(new CommandLineParserSettings(Console.Error));
            if (!parser.ParseArguments(args, options))
            {
                Environment.Exit(1);
            }

            //处理命令行
            handleOpt(options);

            #region 解释器

            Lexer lexer;
            try
            {
                using (StreamReader sr = new StreamReader(options.InputFile[0]))
                {
                    lexer = new Lexer(sr);
                    sr.Close();
                }
            }
            catch (FileNotFoundException)
            {
                Stderr.WriteLine("Input file not found.");
                return;
            }
            catch (IOException e)
            {
                Stderr.WriteLine(e.Message);
                return;
            }

            try
            {
                Interpret.Interpreter inter = new Interpret.Interpreter(lexer);
                Canvas canvas = new Canvas(new Turtle());
                if (useMargin)
                {
                    //设置边距
                    canvas.SetMargin(marginU, marginD, marginL, marginR);
                }
                inter.AddPrimitive("bk", 1, canvas.BK);
                inter.AddPrimitive("cs", 0, canvas.CS);
                inter.AddPrimitive("fd", 1, canvas.FD);
                inter.AddPrimitive("home", 0, canvas.HOME);
                inter.AddPrimitive("lt", 1, canvas.LT);
                inter.AddPrimitive("print", 1, canvas.PRINT);
                inter.AddPrimitive("pu", 0, canvas.PU);
                inter.AddPrimitive("pd", 0, canvas.PD);
                inter.AddPrimitive("rt", 1, canvas.RT);
                inter.AddPrimitive("seth", 1, canvas.SETH);
                inter.AddPrimitive("setx", 1, canvas.SETX);
                inter.AddPrimitive("setxy", 2, canvas.SETXY);
                inter.AddPrimitive("sety", 1, canvas.SETY);
                inter.AddPrimitive("stop", 0, canvas.STOP);
                inter.AddPrimitive("circle", 1, canvas.CIRCLE);
                inter.AddPrimitive("bezier", 4, canvas.BEZIER);
                inter.AddPrimitive("fill", 3, canvas.FILL);
                inter.AddPrimitive("color", 3, canvas.COLOR);
                inter.AddPrimitive("fillopacity", 1, canvas.FILLOPACITY);
                inter.AddPrimitive("opacity", 1, canvas.OPACITY);
                inter.AddPrimitive("stroke", 1, canvas.STROKE);
                inter.AddPrimitive("ellipse", 2, canvas.ELLIPSE);
                inter.AddPrimitive("background", 3, canvas.BACKGROUND);
                inter.AddPrimitive("rect", 3, canvas.RECT);
                inter.AddPrimitive("fontsize", 1, canvas.FONTSIZE);

                inter.Go();

                if (!inter.IsError)
                {
                    canvas.Print(outputType);
                }
            }
            catch (BadQuoteException e)
            {
                //引号错误
                Stderr.WriteLine("Bad quoted string @ line {0}, column {1}",
                    e.QuotedString.Line, e.QuotedString.Column);
            }
            catch (IOException e)
            {
                Stderr.WriteLine(e.Message);
            }

            #endregion
        }

        private static void handleOpt(CmdOptions options)
        {
            if (options.InputFile == null
                || options.InputFile.Count == 0)
            {
                Stderr.WriteLine("You must specify input file.");
                Environment.Exit(-1);
            }
            if (options.Margins == null
                || options.Margins.Count == 0)
            {
                useMargin = false;
            }
            else if (options.Margins.Count > 4)
            {
                Stderr.WriteLine("You have specified too many margins.");
                Environment.Exit(-1);
            }
            else
            {
                List<uint> marginList = new List<uint>();
                for (int i = 0; i < options.Margins.Count; i++)
                {
                    uint margin;
                    if (!UInt32.TryParse(options.Margins[i], out margin))
                    {
                        Stderr.WriteLine("Invalid margin '{0}'", options.Margins[i]);
                        Environment.Exit(-1);
                    }
                    marginList.Add(margin);
                }
                //按照CSS规则定义margin
                switch (marginList.Count)
                {
                    case 1:
                        marginD = marginL = marginR = marginU = (int)marginList[0];
                        break;
                    case 2:
                        marginU = marginD = (int)marginList[0];
                        marginL = marginR = (int)marginList[1];
                        break;
                    case 3:
                        marginU = (int)marginList[0];
                        marginR = marginL = (int)marginList[1];
                        marginD = (int)marginList[2];
                        break;
                    case 4:
                        marginU = (int)marginList[0];
                        marginR = (int)marginList[1];
                        marginD = (int)marginList[2];
                        marginL = (int)marginList[3];
                        break;
                    default:
                        break;
                }
                useMargin = true;
            }
            outputType = options.ImageType;
        }
    }
}
