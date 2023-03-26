using Demo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
namespace SFR_Microservice.Controllers;

[ApiController]
[Route("[controller]")]
public class StockController : ControllerBase
{
    private StocksContext _stocksContext;
    public StockController(StocksContext stocksContext)
    {
        _stocksContext = stocksContext;
    }
    
    [Route("api/stock")]
    [HttpGet]
    public ContentResult Index()
    {
        var html =
            "<p>/get for all information</p>" +
            "<p>/get{'companyname'} for receiving prices form specific company</p>";
        return new ContentResult
        {
            Content = html,
            ContentType = "text/html"
        };
    }

    [Route("api/get")]
    [HttpGet]
    public List<Bond> Get()
    {
        List<Bond> bonds = _stocksContext.Bonds.ToList();
        return bonds;
    }
}