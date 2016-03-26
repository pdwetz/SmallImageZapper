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
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.VisualBasic.FileIO;

namespace SmallImageZapper.Core
{
    public class Zapper
    {
        // Settings
        /// <summary>
        /// Minimum number of pixels (height * width) required; anything smaller will be deleted. E.g. 400 x 600 = 240,000.
        /// </summary>
        public int MinPixels { get; set; } = 240000;
        /// <summary>
        /// Maximum number of bytes allowed. This is primarily a shortcut for skipping larger files, since taglib isn't particularly fast.
        /// </summary>
        public int MaxBytes { get; set; } = int.MaxValue;
        /// <summary>
        /// Flag for whether we try to use the recycling bin or fo a hard delete (physically delete the file).
        /// </summary>
        public bool IsHardDelete { get; set; } = false;
        /// <summary>
        /// Current design will try to load any file via taglib for processing. Use this list to avoid any file types you'd like to skip (e.g. ".gif", ".zip").
        /// </summary>
        public List<string> SkipExtensions { get; set; }
        /// <summary>
        /// Flag for whether we write output for all steps or not.
        /// </summary>
        public bool IsVerbose { get; set; }
        /// <summary>
        /// Flag for whether we actually delete the files or just do a dry run.
        /// </summary>
        public bool IsDebugOnly { get; set; }

        // Tracking
        public int TotalFolders { get; private set; }
        public int TotalFiles { get; private set; }
        public int DeletedFiles { get; private set; }
        public long ElapsedMS { get; private set; }

        /// <summary>
        /// Zapper will recursively remove all small images in the given folder and its subfolders.
        /// </summary>
        /// <param name="path">Folder path to process</param>
        public void Process(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Path required");
                return;
            }

            if (!Directory.Exists(path))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Folder does not exist: {path}");
                return;
            }
            var timer = new Stopwatch();
            timer.Start();
            ProcessFolder(path);
            timer.Stop();
            ElapsedMS = timer.ElapsedMilliseconds;
        }

        private void ProcessFolder(string path)
        {
            var folders = Directory.GetDirectories(path);
            foreach (var f in folders)
            {
                ProcessFolder(f);
            }
            var files = Directory.GetFiles(path);
            if (files.Length == 0)
            {
                return;
            }
            if (IsVerbose)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"Processing folder: {path} ({files.Length} files)");
            }
            if (SkipExtensions == null)
            {
                SkipExtensions = new List<string>();
            }
            TotalFolders++;
            TotalFiles += files.Length;
            foreach (var f in files)
            {
                if (SkipExtensions.Contains(Path.GetExtension(f)))
                {
                    if (IsVerbose)
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine($"Skipping extension for file: {f}");
                    }
                    continue;
                }
                if (new FileInfo(f).Length > MaxBytes)
                {
                    if (IsVerbose)
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine($"Skipping large file: {f}");
                    }
                    continue;
                }
                TagLib.File file = null;
                try
                {
                    file = TagLib.File.Create(f);
                }
                catch (TagLib.UnsupportedFormatException)
                {
                    if (IsVerbose)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"Unsupported file: {f}");
                    }
                    continue;
                }
                catch (TagLib.CorruptFileException)
                {
                    if (IsVerbose)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"Corrupt file: {f}");
                    }
                    continue;
                }
                catch (Exception)
                {
                    if (IsVerbose)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"TagLib Unable to open file: {f}");
                    }
                    continue;
                }

                var image = file as TagLib.Image.File;
                if (image == null)
                {
                    if (IsVerbose)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"Not an image file: {f}");
                    }
                    continue;
                }

                if (image.PossiblyCorrupt)
                {
                    StringBuilder msg = new StringBuilder();
                    foreach (string reason in image.CorruptionReasons)
                    {
                        msg.AppendLine("    * " + reason);
                    }
                    if (IsVerbose)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"Skipping possibly corrupt image: {f} {msg.ToString()}");
                    }
                    continue;
                }
                else if (image.Properties != null)
                {
                    int pixels = image.Properties.PhotoWidth * image.Properties.PhotoHeight;
                    if (pixels < MinPixels)
                    {
                        if (IsVerbose)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Small image: {f}");
                        }
                        DeletedFiles++;
                        if (!IsDebugOnly)
                        {
                            if (IsHardDelete)
                            {
                                File.Delete(f);
                            }
                            else
                            {
                                FileSystem.DeleteFile(f, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                            }
                        }
                    }
                }
                else
                {
                    if (IsVerbose)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"No image properties: {f}");
                    }
                }
            }
        }
    }
}