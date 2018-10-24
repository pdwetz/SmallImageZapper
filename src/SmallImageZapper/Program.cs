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
using Serilog;
using SmallImageZapper.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmallImageZapper
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Debug()
                .WriteTo.Console(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)
                .CreateLogger();

            Parser.Default.ParseArguments<SmallImageZapperOptions>(args)
                .WithParsed<SmallImageZapperOptions>(opts => RunOptionsAndReturnExitCode(opts));
        }

        private static void RunOptionsAndReturnExitCode(SmallImageZapperOptions options)
        {
            try
            {
                options.SkipExtensions = ParseSkipExtensions(options.SkipExt);
                Log.Verbose("Running using {@Options}", options);
                var z = new Zapper(options);
                if (options.IsDebugOnly)
                {
                    Log.Information("Debug mode enabled. No files will be deleted.");
                }
                Log.Information("Working...");
                z.Process(options.FolderPath);
                Log.Information($"Processed {z.TotalFolders:#,##0} folders in {z.ElapsedMS:#,##0} ms. Deleted {z.DeletedFiles:#,##0} files out of {z.TotalFiles:#,##0} found.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
            }
            finally
            {
                Log.CloseAndFlush();
                if (options.PauseAtCompletion)
                {
                    Console.WriteLine("Press any key to continue.");
                    Console.ReadLine();
                }
            }
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