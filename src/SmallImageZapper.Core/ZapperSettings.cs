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

using System.Collections.Generic;

namespace SmallImageZapper.Core
{
    public class ZapperSettings
    {
        // Settings
        /// <summary>
        /// Minimum number of pixels (height * width) required; anything smaller will be deleted. E.g. 400 x 600 = 240,000.
        /// </summary>
        public virtual int MinPixels { get; set; } = 240000;

        /// <summary>
        /// Maximum number of bytes allowed. This is primarily a shortcut for skipping larger files, since taglib isn't particularly fast.
        /// </summary>
        public virtual int MaxBytes { get; set; } = int.MaxValue;

        /// <summary>
        /// Flag for whether we try to use the recycling bin or fo a hard delete (physically delete the file).
        /// </summary>
        public virtual bool IsHardDelete { get; set; } = false;

        /// <summary>
        /// Current design will try to load any file via taglib for processing. Use this list to avoid any file types you'd like to skip (e.g. ".gif", ".zip").
        /// </summary>
        public virtual List<string> SkipExtensions { get; set; }

        /// <summary>
        /// Flag for whether we actually delete the files or just do a dry run.
        /// </summary>
        public virtual bool IsDebugOnly { get; set; }
    }
}