using System;
using System.IO;
using BookingApp.Domain.Interfaces;

namespace BookingApp.Services
{
    public class ImageFileHandler : IImageFileHandler
    {
        public string CopyImageToResources(string sourcePath)
        {
            try
            {
                string baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Resources", "Images");
                baseDir = Path.GetFullPath(baseDir);
                if (!Directory.Exists(baseDir)) { Directory.CreateDirectory(baseDir); }

                string fileName = Path.GetFileName(sourcePath);
                string destPath = Path.Combine(baseDir, fileName);

                if (File.Exists(destPath))
                {
                    string fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
                    string extension = Path.GetExtension(fileName);
                    fileName = $"{fileNameWithoutExt}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
                    destPath = Path.Combine(baseDir, fileName);
                }

                File.Copy(sourcePath, destPath, true);

                return fileName;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error copying image: {ex.Message}");
                return Path.GetFileName(sourcePath);
            }
        }

    }
}