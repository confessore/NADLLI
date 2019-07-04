using System.IO;
using System.Linq;

namespace NADLLI.Extensions
{
    public static class StringExtensions
    {
        public static void CheckFile(this string value, byte[] bytes)
        {
            if (!value.FileEqualTo(bytes))
                value.CreateFile(bytes);
        }

        public static void CreateFile(this string value, byte[] bytes)
        {
            if (File.Exists(value))
            {
                if (File.ReadAllBytes(value).SequenceEqual(bytes)) return;
                File.WriteAllBytes(value, bytes);
            }
            else
                File.WriteAllBytes(value, bytes);
        }

        public static void DeleteFile(this string value)
        {
            if (File.Exists(value))
            {
                File.SetAttributes(value, FileAttributes.Normal);
                File.Delete(value);
            }
        }

        public static bool FileEqualTo(this string value, byte[] bytes)
        {
            if (File.Exists(value))
                return File.ReadAllBytes(value).SequenceEqual(bytes);
            return false;
        }
    }
}
