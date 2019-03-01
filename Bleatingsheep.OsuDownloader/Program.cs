using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace Bleatingsheep.OsuDownloader
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            byte[] StringToByteArray(string hex)
            {
                return Enumerable.Range(0, hex.Length)
                                 .Where(x => x % 2 == 0)
                                 .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                                 .ToArray();
            }

            using (var httpClient = new HttpClient())
            {
                var infoJson = await httpClient.GetStringAsync(@"https://osu.ppy.sh/web/check-updates.php?action=check&stream=stable40");
                var osuFileInfo = JsonConvert.DeserializeObject<OsuFileInfo[]>(infoJson);
                var tempFile = Path.GetTempFileName();
                var destFile = args.Length == 0 || string.IsNullOrEmpty(args[0])
                    ? "osu!.zip"
                    : args[0];

                using (var fs = File.Create(tempFile))
                using (var archive = new ZipArchive(fs, ZipArchiveMode.Create))
                using (MD5 md5 = new MD5CryptoServiceProvider())
                {
                    foreach (var file in osuFileInfo)
                    {
                        ZipArchiveEntry fileEntry = archive.CreateEntry(file.Filename);
                        fileEntry.LastWriteTime = file.Timestamp;
                        using (Stream destination = fileEntry.Open())
                        {
                            var uriBuilder = new UriBuilder(file.UrlFull);
                            uriBuilder.Scheme = "https";
                            uriBuilder.Port = 443;
                            var uri = uriBuilder.Uri;

                            byte[] bytes = await httpClient.GetByteArrayAsync(uri);
                            var actualHash = md5.ComputeHash(bytes);
                            var expectedHash = StringToByteArray(file.FileHash);
                            if (!expectedHash.SequenceEqual(actualHash))
                                throw new HttpRequestException();
                            await destination.WriteAsync(bytes);
                        }
                    }
                }

                File.Copy(tempFile, destFile, true);

                try
                {
                    File.Delete(tempFile);
                }
                catch (Exception)
                {

                }
            }
        }
    }
}
