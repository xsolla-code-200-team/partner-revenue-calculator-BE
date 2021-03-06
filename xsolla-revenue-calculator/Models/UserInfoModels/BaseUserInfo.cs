using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using xsolla_revenue_calculator.Models.ForecastModels;

namespace xsolla_revenue_calculator.Models.UserInfoModels
{
    public class BaseUserInfo
    {
        [JsonIgnore]
        public ObjectId Id { get; set; }
        
        [BsonRepresentation(BsonType.String)]
        [JsonIgnore]
        public  ForecastType ForecastType { get; set; }
        
        [JsonPropertyName("productName")]
        public string ProductName { get; set; }

        [JsonPropertyName("genres")]
        public List<string> Genres { get; set; }
        
        [JsonPropertyName("monetization")]
        public string Monetization { get; set; }
        
        [JsonPropertyName("platforms")]
        public List<string> Platforms { get; set; }
        
        [JsonPropertyName("regions")]
        public List<string> Regions { get; set; }

        [JsonPropertyName("companyName")]
        public string CompanyName { get; set; }
        
        [JsonPropertyName("email")]
        [EmailAddress]
        public string Email { get; set; }
        
        [JsonIgnore]
        public ObjectId RevenueForecastId { get; set; }
        
    }
}