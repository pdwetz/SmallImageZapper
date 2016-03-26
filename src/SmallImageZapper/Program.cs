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
using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;
using SmallImageZapper.Core;

namespace SmallImageZapper
{
    class Program
    {
        static void Main(string[] args)
        {
            var initialColor = Console.ForegroundColor;
            var options = new SmallImageZapperOptions();
            if (!Parser.Default.ParseArguments(args, options))
            {
                return;
            }
            var z = new Zapper();
            z.MinPixels = options.Pixels;
            z.MaxBytes = options.MaxBytes;
            z.SkipExtensions = ParseSkipExtensions(options.SkipExt);
            z.IsVerbose = options.Verbose;
            z.IsDebugOnly = options.Debug;
            z.IsHardDelete = options.HardDelete;
            Console.ForegroundColor = ConsoleColor.Gray;
            if (!options.Immediate || z.IsVerbose)
            {
                Console.WriteLine($"Root path: {options.FolderPath}");
                Console.WriteLine($"Min pixel size: {z.MinPixels:#,##0}");
                Console.WriteLine($"Max file size: {z.MaxBytes:#,##0} bytes");
                if (z.SkipExtensions.Count > 0)
                {
                    Console.WriteLine($"Skipping extensions: {string.Join(", ", z.SkipExtensions)}");
                }
            }
            if (z.IsDebugOnly)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Debug mode enabled. No files will be deleted.");
            }
            Console.ForegroundColor = ConsoleColor.White;
            if (!options.Immediate)
            {
                Console.WriteLine("SmallImageZapper ready to begin. Press any key to continue.");
                Console.ReadLine();
            }
            Console.WriteLine("Working...");
            z.Process(options.FolderPath);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Processed {z.TotalFolders:#,##0} folders in {z.ElapsedMS:#,##0} ms. Deleted {z.DeletedFiles:#,##0} files out of {z.TotalFiles:#,##0} found.");
            Console.ForegroundColor = initialColor;
        }

        private static List<string> ParseSkipExtensions(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                return new List<string>();
            }
            List<string> skip = raw.Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            for (int i = 0; i < skip.Count; i++)
            {
                if (skip[i][0] != '.')
                {
                    skip[i] = $".{skip[i].Trim()}";
                }
                else
                {
                    skip[i] = skip[i].Trim();
                }
            }
            return skip;
        }
    }
}