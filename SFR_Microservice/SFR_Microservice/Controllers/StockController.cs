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
    
    [HttpGet]
    public List<BondEntity> Get()
    {
        List<BondEntity> bonds = _stocksContext.Bonds.ToList();
        return bonds;
    }
    
    
    [Route("{companyname}")]
    [HttpGet]
    public List<BondEntity> GetBondsForCompany(string companyname)
    {
        List<BondEntity> bonds = _stocksContext.Bonds.Where(b => b.BondName.ToLower().Contains(companyname.ToLower())).ToList();
        return bonds;
    }
    
    [Route("trends")]
    [HttpGet]
    public List<BondTrendEntity> GetBondTrends()
    {
        List<BondTrendEntity> bondTrends = _stocksContext.BondTrends.ToList();
        return bondTrends;
    }
}