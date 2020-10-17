using MongoDB.Bson.Serialization.Attributes;

namespace Zero_Web_GetGameContent.Model
{
    public class StoreItem
    {

        [BsonId]
        public string ID { get; set; }
        public string GameName { get; set; }
        public Price Price { get; set; }
        public string GameImage { get; set; }
        public string[] GameImages { get; set; }
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
        public LanguageContent[] LanguageContents { get; set; }

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

    public class LanguageContent
    {
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string[] Genre { get; set; }
        public string[] GameTags { get; set; }
    }

    public class Price
    {
        public string GamePrice { get; set; }
        public string PriceOld { get; set; }
        public string PriceNew { get; set; }
        public string Reduced { get; set; }
        public bool Free { get; set; }
    }

}
