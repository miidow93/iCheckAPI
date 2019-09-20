using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iCheckAPI.Models
{
    // [BsonIgnoreExtraElements]
    public class CheckList
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("conducteur")]
        public Dictionary<string, string> Conducteur { get; set; }

        [BsonElement("tracteur")]
        public Dictionary<string, string> Tracteur { get; set; }

        [BsonElement("date")]
        [BsonDateTimeOptions(DateOnly = false, Kind = DateTimeKind.Utc, Representation = BsonType.DateTime)]
        public DateTime? Date { get; set; }

        /*[BsonExtraElements]
        [BsonDictionaryOptions(DictionaryRepresentation.Document)]
        // [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<string, object> CatchAll { get; set; }*/

        /*[BsonExtraElements]
        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfDocuments)]
        public Dictionary<string, object> CatchAll { get; set; }*/

        // [BsonElement("catchAll")]
        [BsonExtraElements]
        public Dictionary<string, object> CatchAll { get; set; }

        /*[BsonExtraElements]
        public BsonDocument ExtraElement { get; set; }*/


        /*[BsonExtraElements]
        public BsonDocument CatchAll { get; set; }*/

        
    }
}
