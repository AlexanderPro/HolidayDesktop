namespace HolidayDesktop.Common
{
    static class PathUtils
    {
        public static string MakeRelativePath(string absolutePath, string assemblyDirectoryPath)
        {
            return absolutePath.ToLower().StartsWith(assemblyDirectoryPath.ToLower()) ? ("~\\" + absolutePath.Substring(assemblyDirectoryPath.Length).TrimStart('\\')) : absolutePath;
        }

        public static string MakeAbsolutePath(string relativePath, string assemblyDirectoryPath)
        {
            return relativePath.StartsWith("~") ? relativePath.Replace("~", assemblyDirectoryPath.TrimEnd('\\')) : relativePath;
        }
    }
}
