using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Zero_Web_GetGameContent.Model
{
    public class StoreItem
    {

        [BsonId]
        public string ID { get; set; }
        public string GameName { get; set; }
        public string GameImage { get; set; }
        public DisplayImages[] GameImages { get; set; }
        public string Release { get; set; }
        public string[] Developer { get; set; }
        public string[] DeveloperLink { get; set; }
        public string Publisher { get; set; }
        public string PublisherLink { get; set; }
        public string Category { get; set; }
        public bool EarlyAccess { get; set; }
        public string FSK { get; set; }
        public SystemInfo SystemInfoMin { get; set; }
        public SystemInfo SystemInfoMax { get; set; }
        public string InfoFrom { get; set; }
        public DownloadLinks Download { get; set; }

    }

    public class SystemInfo
    {
        public string OS { get; set; }
        public string CPU { get; set; }
        public string GPU { get; set; }
        public string RAM { get; set; }
        public string Storage { get; set; }
        public string DirectX { get; set; }
    }

    public class DisplayImages
    {
        public bool Video { get; set; }
        public string URL { get; set; }
    }

    public class DownloadLinks
    {
        [Display(Name = "WindowsX64")]
        public string WindowsX64 { get; set; }

        [Display(Name = "WindowsX32")]
        public string WindowsX32 { get; set; }

        [Display(Name = "Linux")]
        public string Linux { get; set; }

        [Display(Name = "Mac")]
        public string Mac { get; set; }
    }

}
