namespace FeadBack.controller.dto;

public class KafkaFeedResponseDto
{
    public int Scope { get; set; }
    public string DealerName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public KafkaFeedResponseDto() { }

    public KafkaFeedResponseDto(int scope, string dealerName, string firstName, string lastName)
    {
        Scope = scope;
        DealerName = dealerName;
        FirstName = firstName;
        LastName = lastName;
    }

    public override string ToString()
    {
        return $"Scope: {Scope}, Dealer: {DealerName}, Name: {FirstName} {LastName}";
    }
}