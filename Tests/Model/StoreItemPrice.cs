using MongoDB.Bson.Serialization.Attributes;

namespace Zero_Web_GetGameContent.Model
{
    public class StoreItemPrice
    {
        [BsonId]
        public string ID { get; set; }
        public ItemPrice[] price { get; set; }
    }

    public class ItemPrice
    {
        public string GamePrice { get; set; }
        public string PriceOld { get; set; }
        public string PriceNew { get; set; }
        public string Reduced { get; set; }
        public bool Free { get; set; }
        public string PageName { get; set; }
        public string PageURL { get; set; }
    }

}
