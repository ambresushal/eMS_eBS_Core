using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using tmg.equinox.services.genericWebApi.Areas.HelpPage.Models;

namespace tmg.equinox.services.genericWebApi.Controllers
{
    [Authorize]
    public class ProductsController : ApiController
    {
        // GET api/<controller>
        /// <summary>
        /// Gets all product data from the eBenefitSync.
        /// </summary>
        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            if (id < 0)
                return BadRequest("Please Enter Valid Product ID!");
            Products prod = new Products();
            List<Products> product = prod.GetProductById(id);
            if (product == null)
                return BadRequest("Product Not Found!");
            else
                return Ok(product);
        }
        // GET api/<controller>
        /// <summary>
        /// Gets specific section data from the eBenefitSync.
        /// </summary>
        [HttpGet]
        [ActionName("ProductSections")]
        public IHttpActionResult Get(int id, string sections)
        {
            if (string.IsNullOrEmpty(sections))
                return BadRequest("Please Enter Valid Section Name!");
            Products prod = new Products();
            List<Products> productList = prod.GetProductBySecions(id, sections);
            if (productList == null)
                return BadRequest("Product Section's Not Found!");
            else
                return Ok(productList);
        }
        // GET api/<controller>
        /// <summary>
        /// Gets specific repeaters data from the eBenefitSync.
        /// </summary>
        [HttpGet]
        [ActionName("ProductRepeaters")]
        public IHttpActionResult GetRepeaters(int id, string repeaters)
        {
            if (string.IsNullOrEmpty(repeaters))
                return BadRequest("Please Enter Valid Repeater Name!");
            Products prod = new Products();
            List<Products> productList = prod.GetProductBySecions(id, repeaters);
            if (productList == null)
                return BadRequest("Product Repeater's Not Found!");
            else
                return Ok(productList); 
        }
    }
}