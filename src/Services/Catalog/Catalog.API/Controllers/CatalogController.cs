using Catalog.API.Entities;
using Catalog.API.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Xml.Linq;

namespace Catalog.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CatalogController : ControllerBase
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<CatalogController> _logger;

    public CatalogController(IProductRepository productRepository, ILogger<CatalogController> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        var products = await _productRepository.GetProducts();
        return Ok(products);
    }



    [HttpGet("{id:length(24)}", Name = "GetProduct")]
    [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<Product>> GetProduct(string id)
    {
        var product = await _productRepository.GetProduct(id);
        if (product == null)
        {
            _logger.LogError($"product with id product{id} is not found ");
            return NotFound();
        }
        return Ok(product);
    }

    [HttpGet]
    [Route("[action]/{category}", Name = "GetProductByCategory")]
    [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<Product>>> GetProductByCategory(string category)
    {
        var product = await _productRepository.GetProductByCategory(category);

        return Ok(product);
    }

    [HttpGet]
    [Route("[action]/{name}", Name = "GetProductByName")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(IEnumerable<Product>),(int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<Product>>> GetProductByName(string name)
    {
        var items = await _productRepository.GetProductByName(name);
        if (items == null)
        {
            _logger.LogError($"Product with this {name} is not found");
            return NotFound();
        }
        return Ok(items);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
    public async Task<ActionResult>CreateProduct (Product product)
    {
        await _productRepository.CreateProduct(product);
        return CreatedAtRoute("GetProduct", new {id=product.Id},product);
    }

    [HttpPut]
    [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> UpdateProduct([FromBody] Product product)
    {
        return Ok(await _productRepository.UpdateProduct(product));
    }

    [HttpDelete("{id:length(24)}", Name = "DeleteProduct")]
    [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> DeleteProductById(string id)
    {
        return Ok(await _productRepository.DeleteProduct(id));
    }
}
