using MongoDB.Bson.Serialization.Attributes;

namespace Zero_Web_GetGameContent.Model
{
    public class StoreItemDLC
    {

        [BsonId]
        public string GameID { get; set; }
        public string[] DLCID { get; set; }

    }
}
