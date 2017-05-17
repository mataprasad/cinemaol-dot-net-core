using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApplication.Services;
using WebApplication.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

namespace WebApplication.Controllers
{
    [RouteAttribute("api/[Controller]")]
    [Authorize]
    public class AjaxXController : BaseController
    {
        public AjaxXController(IOptions<GlobalOption> globalOptions) : base(globalOptions)
        {

        }

        Product[] products = new Product[]
        {
            new Product { Id = 1, Name = "Tomato Soup", Category = "Groceries", Price = 1 },
            new Product { Id = 2, Name = "Yo-yo", Category = "Toys", Price = 3.75M },
            new Product { Id = 3, Name = "Hammer", Category = "Hardware", Price = 16.99M }
        };

        [HttpGet]
        public IEnumerable<Product> Get()
        {
            return products;
        }

        [HttpGet("{id:int}")]
        public Product Get(int id)
        {
            var product = products.FirstOrDefault((p) => p.Id == id);
            if (product == null)
            {
                return null;
            }
            return product;
        }

        [HttpPost]
        [Produces("application/xml",Type=typeof(Product))]
        //[Consumes("application/json",Type=typeof(Product))]
        public IActionResult Post([FromBody]Product obj)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return CreatedAtAction("Get", new { id = obj.Id }, obj);
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            return Ok();
        }

        [HttpPut("{id:int}")]
        public IActionResult Put(int id,[FromBody]Product obj)
        {
            return Ok();
        }

    }

    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
    }
}