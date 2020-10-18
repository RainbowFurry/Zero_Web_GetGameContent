using MongoDB.Bson.Serialization.Attributes;

namespace Zero_Web_GetGameContent.Model
{
    public class StoreItemLanguage
    {
        [BsonId]
        public string ID { get; set; } = "";
        public string PageName { get; set; }
        public string PageURL { get; set; }
        public Language[] language { get; set; } = { };
    }

    public class Language
    {
        public string ShortDescription { get; set; } = "";
        public string LongDescription { get; set; } = "";
        public string[] Genre { get; set; } = { };
        public string[] GameTags { get; set; } = { };
    }

}
