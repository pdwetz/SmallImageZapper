/*
    SmallImageZapper - Will delete all small images in a given folder and all of its subfolders.
    Copyright (C) 2018 Peter Wetzel

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
using SmallImageZapper.Core;

namespace SmallImageZapper
{
    public class SmallImageZapperOptions : ZapperSettings
    {
        [Option('f', "folder", Required = true, HelpText = "Folder path to process")]
        public string FolderPath { get; set; }

        [Option('p', "minpixels", Default = 240000, HelpText = "Minimum pixels. Anything below is considered too small and will be deleted.")]
        public override int MinPixels { get; set; }

        [Option('b', "maxbytes", Default = int.MaxValue, HelpText = "Maximum file bytes for a small file. Anything larger will be ignored.")]
        public override int MaxBytes { get; set; }

        [Option('s', "skipext", HelpText = "Skip extensions (comma separated)")]
        public string SkipExt { get; set; }

        [Option('h', "harddelete", HelpText = "Flag for doing a hard delete instead of using recycling bin.")]
        public override bool IsHardDelete { get; set; }

        [Option('d', "debug", HelpText = "Flag for debug only mode. No files will be deleted.")]
        public override bool IsDebugOnly { get; set; }

        [Option('x', "pause", Required = false, HelpText = "Pause on program completion.")]
        public bool PauseAtCompletion { get; set; }
    }
}