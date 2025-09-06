using System.Data;
using FeadBack.controller.dto;
public class FeedBackAutoDtoResponse
{
    public string model { get; set; }
    public string description { get; set; }
    public DateTime dataTime{ get; set; }
    public List<ServiceCompanyDto> ServiceCompanies { get; set; }
}