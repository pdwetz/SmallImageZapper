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
using NUnit.Framework;
using SmallImageZapper.Core;
using System.Collections.Generic;
using System.IO;

namespace SmallImageZapper.Test
{
    [TestFixture]
    public class ZapperTester
    {
        [Test]
        public void min_pixels()
        {
            string folderPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "assets");
            var settings = new ZapperSettings
            {
                IsDebugOnly = true,
                MinPixels = 200 * 200,
                MaxBytes = int.MaxValue,

            };
            var zapper = new Zapper(settings);
            zapper.Process(folderPath);
            Assert.AreEqual(1, zapper.TotalFolders);
            Assert.AreEqual(13, zapper.TotalFiles);
            Assert.AreEqual(3, zapper.DeletedFiles);
        }

        [Test]
        public void skip_extension()
        {
            string folderPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "assets");
            var settings = new ZapperSettings
            {
                IsDebugOnly = true,
                MinPixels = int.MaxValue,
                MaxBytes = int.MaxValue,
                SkipExtensions = new List<string> { ".gif" }

            };
            var zapper = new Zapper(settings);
            zapper.Process(folderPath);
            Assert.AreEqual(1, zapper.TotalFolders);
            Assert.AreEqual(13, zapper.TotalFiles);
            Assert.AreEqual(12, zapper.DeletedFiles);
        }

        [Test]
        public void max_file_size()
        {
            string folderPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "assets");
            var settings = new ZapperSettings
            {
                IsDebugOnly = true,
                MinPixels = int.MaxValue,
                MaxBytes = 30000
            };
            var zapper = new Zapper(settings);
            zapper.Process(folderPath);
            Assert.AreEqual(1, zapper.TotalFolders);
            Assert.AreEqual(13, zapper.TotalFiles);
            Assert.AreEqual(7, zapper.DeletedFiles);
        }
    }
}