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
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using SmallImageZapper.Core;

namespace SmallImageZapper.Test
{
    [TestFixture]
    public class ZapperTester
    {
        [Test]
        public void min_pixels()
        {
            string folderPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "assets");
            Zapper zapper = new Zapper();
            zapper.IsDebugOnly = true;
            zapper.IsVerbose = true;
            zapper.MinPixels = 200 * 200;
            zapper.MaxBytes = int.MaxValue;
            zapper.Process(folderPath);
            Assert.AreEqual(1, zapper.TotalFolders);
            Assert.AreEqual(13, zapper.TotalFiles);
            Assert.AreEqual(3, zapper.DeletedFiles);
        }

        [Test]
        public void skip_extension()
        {
            string folderPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "assets");
            Zapper zapper = new Zapper();
            zapper.IsDebugOnly = true;
            zapper.IsVerbose = true;
            zapper.MinPixels = int.MaxValue;
            zapper.MaxBytes = int.MaxValue;
            zapper.SkipExtensions = new List<string> { ".gif" };
            zapper.Process(folderPath);
            Assert.AreEqual(1, zapper.TotalFolders);
            Assert.AreEqual(13, zapper.TotalFiles);
            Assert.AreEqual(12, zapper.DeletedFiles);
        }

        [Test]
        public void max_file_size()
        {
            string folderPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "assets");
            Zapper zapper = new Zapper();
            zapper.IsDebugOnly = true;
            zapper.IsVerbose = true;
            zapper.MinPixels = int.MaxValue;
            zapper.MaxBytes = 30000;
            zapper.Process(folderPath);
            Assert.AreEqual(1, zapper.TotalFolders);
            Assert.AreEqual(13, zapper.TotalFiles);
            Assert.AreEqual(7, zapper.DeletedFiles);
        }
    }
}