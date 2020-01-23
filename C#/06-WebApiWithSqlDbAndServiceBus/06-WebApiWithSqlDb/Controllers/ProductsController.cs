using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _06_WebApiWithSqlDb.Data;
using _06_WebApiWithSqlDb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _06_WebApiWithSqlDb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private DemoDbContext db;

        public ProductsController(DemoDbContext db)
        {
            this.db = db;
        }

        [HttpGet("")]
        public IEnumerable<Product> GetProducts()
        {
            return db.Products.ToList();
        }

        [HttpPost("")]
        public async Task<ActionResult<Product>> AddProductAsync(Product product)
        {
            await db.Products.AddAsync(product);
            await db.SaveChangesAsync();
            return product;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProductByIdAsync(int id)
        {
            var product=await db.Products.FindAsync(id);            
            return product;
        }
    }
}