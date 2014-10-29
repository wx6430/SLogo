using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine;
using CommandLine.Text;
using SLogo.Canvas;

namespace SLogo.Painter
{
    sealed partial class Program
    {
        private static readonly HeadingInfo headingInfo = new HeadingInfo("SLogo", "0.1");

        private sealed class CmdOptions
        {
            [Option("T", "Type", Required = false, HelpText = "Image type to output: png|svg")]
            public PrintType ImageType = PrintType.Png;

            [OptionList("m", "margin", Separator = ',', Required = false,
                HelpText = "Margin of image. You can specify the margin of each side in the order: top, right, down, left. "
                + "Separate each margin with a comma and do not include spaces between margins and separators. "
                + "Margins are applied just like CSS margin styles."
                )]
            public IList<string> Margins = null;

            [ValueList(typeof(List<string>), MaximumElements = 1)]
            public IList<string> InputFile = null;

            [HelpOption(
                HelpText = "Display this help screen.")]
            public string GetUsage()
            {
                var help = new HelpText(Program.headingInfo);
                help.AdditionalNewLineAfterOption = true;
                help.Copyright = new CopyrightInfo("普通青年", 2011);
                help.AddPreOptionsLine("This is free software. You may redistribute copies of it under the terms of");
                help.AddPreOptionsLine("the MIT License <http://www.opensource.org/licenses/mit-license.php>.");
                help.AddPreOptionsLine("Usage: SLogo [-m margin] [-T type] [filename]");
                help.AddOptions(this);

                return help;
            }
        }
    }
}
