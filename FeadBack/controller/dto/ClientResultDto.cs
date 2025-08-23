namespace FeadBack.controller.dto;

public class ClientResultDto
{
    public string firstName;
    public string lastName;
    public string middleName;

    public ClientResultDto(string firstName, string lastName, string middleName)
    {
        this.firstName = firstName;
        this.lastName = lastName;
        this.middleName = middleName;
    }
}