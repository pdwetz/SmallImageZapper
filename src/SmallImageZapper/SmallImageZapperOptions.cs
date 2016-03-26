/*
    SmallImageZapper - Will delete all small images in a given folder and all of its subfolders.
    Copyright (C) 2016 Peter Wetzel

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
using CommandLine;
using CommandLine.Text;

namespace SmallImageZapper
{
    public class SmallImageZapperOptions
    {
        [Option('f', "folder", Required = true, HelpText = "Folder path to process")]
        public string FolderPath { get; set; }

        [Option('p', "minpixels", DefaultValue = 240000, HelpText = "Minimum pixels. Anything below is considered too small and will be deleted.")]
        public int Pixels { get; set; }

        [Option('b', "maxbytes", DefaultValue = int.MaxValue, HelpText = "Maximum file bytes for a small file. Anything larger will be ignored.")]
        public int MaxBytes { get; set; }

        [Option('s', "skipext", HelpText = "Skip extensions (comma separated)")]
        public string SkipExt { get; set; }

        [Option('h', "harddelete", HelpText = "Flag for doing a hard delete instead of using recycling bin.")]
        public bool HardDelete { get; set; }

        [Option('i', "immediate", HelpText = "Flag for skipping initial pause button prior to running. Useful for automated scripts.")]
        public bool Immediate { get; set; }

        [Option('v', "verbose", HelpText = "Displays all event messages")]
        public bool Verbose { get; set; }

        [Option('d', "debug", HelpText = "Debug only mode. No files will be deleted.")]
        public bool Debug { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var help = new HelpText
            {
                Heading = new HeadingInfo("SmallImageZapper", "1.0.0.0"),
                Copyright = new CopyrightInfo("Peter Wetzel", 2016),
                AdditionalNewLineAfterOption = true,
                AddDashesToOption = true
            };
            help.AddPreOptionsLine("This program comes with ABSOLUTELY NO WARRANTY; for details see license.txt.");
            help.AddPreOptionsLine(@"Usage: SmallImageZapper -f c:\somepath\xyx -p 240000 -s gif,png");
            help.AddOptions(this);
            return help;
        }
    }
}