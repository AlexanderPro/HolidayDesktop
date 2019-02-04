using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace HolidayDesktop.Common
{
    public static class DirectoryInfoExtensions
    {
        public static IEnumerable<FileInfo> GetFilesByExtensions(this DirectoryInfo dirInfo, params string[] extensions)
        {
            var allowedExtensions = new HashSet<string>(extensions, StringComparer.OrdinalIgnoreCase);
            return dirInfo.EnumerateFiles().Where(f => allowedExtensions.Contains(f.Extension));
        }
    }
}
