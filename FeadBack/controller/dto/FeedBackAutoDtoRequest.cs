using System.Data;
using System.Text.Json.Serialization;
public class FeedBackAutoDtoRequest
{
    [JsonPropertyName("vinAuto")]
    public string vinAuto { get; set; }
    
    [JsonPropertyName("firstNameClient")]
    public string firstNameClient { get; set; }
    
    [JsonPropertyName("lastNameClient")]
    public string lastNameClient { get; set; }
    
    [JsonPropertyName("middleNameClient")]
    public string middleNameClient { get; set; }
    
    [JsonPropertyName("description")]
    public string description { get; set; }
    [JsonPropertyName("feed")]
    public int feed { get; set; }

    public FeedBackAutoDtoRequest() {}
}