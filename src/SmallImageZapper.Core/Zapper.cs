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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.VisualBasic.FileIO;
using Serilog;

namespace SmallImageZapper.Core
{
    public class Zapper
    {
        private readonly ZapperSettings _settings;

        public int TotalFolders { get; private set; }
        public int TotalFiles { get; private set; }
        public int DeletedFiles { get; private set; }
        public long ElapsedMS { get; private set; }

        public Zapper(ZapperSettings settings)
        {
            _settings = settings;
            if (_settings.SkipExtensions == null)
            {
                _settings.SkipExtensions = new List<string>();
            }
        }

        /// <summary>
        /// Zapper will recursively remove all small images in the given folder and its subfolders.
        /// </summary>
        /// <param name="path">Folder path to process</param>
        public void Process(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                Log.Error("Path required");
                return;
            }
            if (!Directory.Exists(path))
            {
                Log.Error("Directory does not exist: {Path}", path);
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
            Log.Verbose("Processing {Path} ({FileCount})", path, files.Length);
            TotalFolders++;
            TotalFiles += files.Length;
            foreach (var f in files)
            {
                if (_settings.SkipExtensions.Contains(Path.GetExtension(f)))
                {
                    Log.Verbose("Skipping extension for {file}", f);
                    continue;
                }
                if (new FileInfo(f).Length > _settings.MaxBytes)
                {
                    Log.Verbose("Skipping large {file}", f);
                    continue;
                }
                TagLib.File file = null;
                try
                {
                    file = TagLib.File.Create(f);
                }
                catch (TagLib.UnsupportedFormatException)
                {
                    Log.Verbose("Unsupported format {file}", f);
                    continue;
                }
                catch (TagLib.CorruptFileException)
                {
                    Log.Verbose("Corrupt {file}", f);
                    continue;
                }
                catch (Exception ex)
                {
                    Log.Verbose("TagLib Unable to open {file}", f);
                    Log.Error(ex, "Error thrown by TagLib");
                    continue;
                }

                var image = file as TagLib.Image.File;
                if (image == null)
                {
                    Log.Verbose("Not an image {file}", f);
                    continue;
                }

                if (image.PossiblyCorrupt)
                {
                    StringBuilder msg = new StringBuilder();
                    foreach (string reason in image.CorruptionReasons)
                    {
                        msg.AppendLine("    * " + reason);
                    }
                    Log.Verbose("Skipping possibly corrupt image {file} {CorruptionReasons}", f, msg.ToString());
                    continue;
                }
                else if (image.Properties != null)
                {
                    int pixels = image.Properties.PhotoWidth * image.Properties.PhotoHeight;
                    if (pixels < _settings.MinPixels)
                    {
                        Log.Verbose("Deleting small image {file}", f);
                        DeletedFiles++;
                        if (!_settings.IsDebugOnly)
                        {
                            if (_settings.IsHardDelete)
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
                    Log.Verbose("No image properties {file}", f);
                }
            }
        }
    }
}