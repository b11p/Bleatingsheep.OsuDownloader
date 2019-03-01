using System;
using Newtonsoft.Json;

namespace Bleatingsheep.OsuDownloader
{
    public sealed class OsuFileInfo
    {
        [JsonProperty("file_version")]
        public long FileVersion { get; set; }

        [JsonProperty("filename")]
        public string Filename { get; set; }

        [JsonProperty("file_hash")]
        public string FileHash { get; set; }

        [JsonProperty("filesize")]
        public long Filesize { get; set; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty("patch_id")]
        public long? PatchId { get; set; }

        [JsonProperty("url_full")]
        public Uri UrlFull { get; set; }

        [JsonProperty("url_patch", NullValueHandling = NullValueHandling.Ignore)]
        public Uri UrlPatch { get; set; }
    }
}
