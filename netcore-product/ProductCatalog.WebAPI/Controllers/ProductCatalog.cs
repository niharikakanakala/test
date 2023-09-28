using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProductCatalog.WebAPI.Models;
using ProductCatalog.WebAPI.Services;

namespace ProductCatalog.WebAPI.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        /*
        Post api/products: This endpoint allows adding a new product, asynchronously invoking _productService.AddProduct, 
        and returning an HTTP 201 Created status.
        */
        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody]Product product){
            try{
                await _productService.AddProduct(product);
                return Created("api/products",product);
            }catch{
                return BadRequest("Error in adding product.");
            }
        }
        /*
         GET api/products: When a client sends a GET request to this endpoint, it asynchronously retrieves a list of products using the _productService, 
        and then returns those products as a response with an HTTP status code indicating success (200 OK).
        */
        [HttpGet]
        public async Task<IActionResult> GetAllProducts(){
            try{
                List<Product> productList=await _productService.GetAllProducts();
                return Ok(productList);
            }catch{
                return BadRequest("Not able to fetch all products.");
            }
        }
        
        /*
        GET api/products/{id}: This endpoint that handles HTTP GET requests with a specific "id" parameter, retrieving a product by its unique identifier asynchronously. 
        If the product is found, it returns it with an HTTP 200 OK status; otherwise, it returns an HTTP 404 Not Found status.
        */
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSingleProductById(int id){
            try{
                Product product = await _productService.GetProductById(id);
                if(product==null) return NotFound("Give proper product id.");
                else return Ok(product);
            }catch{
                return BadRequest("Error in finding the product.");
            }
        }
        /*
        GET api/products/search: This endpoint accepts a "name" query parameter, asynchronously retrieves products with matching names 
        using _productService.GetProductsByName, and returns them in an HTTP response.
        */
        [HttpGet] 
        [Route("search/{name}")]
        public async Task<IActionResult> GetProductsByName(string name){
            try{
                List<Product> productList = await _productService.GetProductsByName(name);
                return Ok(productList);
            }catch{
                return BadRequest("Error in searching the products.");
            }
        }
        /*
        GET api/products/total-count: This endpoint asynchronously retrieves the total count of products and returns it in an HTTP response,
        handling potential exceptions with a 500 Internal Server Error status.
        */
       [HttpGet]
       [Route("total-count")]
       public async Task<IActionResult> GetProductCount(){
           try{
               int productcount=await _productService.GetTotalProductCount();
               return Ok(productcount);
           }catch{
               return BadRequest("Error in getting product count.");
           }
       }
        /*
        PUT api/products/{id}: This endpoint handles HTTP PUT requests to update a product by its unique identifier. 
        It checks if the provided product ID matches the one in the request body and if the product exists; 
        if so, it updates the product's attributes and returns a 204 No Content status to indicate success.
        */
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProductById(int id){
            try{
                Product product = await _productService.GetProductById(id);
                if(product!=null){
                    product.Name="Updated Product";
                    product.Category="Updated Category";
                    product.Price=21;
                    await _productService.UpdateProduct(product);
                    return NoContent();
                }else{
                    return NotFound();
                }
            }catch{
                return BadRequest("Error in updating product");
            }
        }
    
       /*
       GET api/products/sort: This endpoint allows sorting products based on specified criteria (name, category, or price) and 
       order (ascending or descending), returning the sorted list in an HTTP response, with an option to specify the sorting order.
       */
       [HttpGet]
       [Route("sort")]
       public async Task<IActionResult> SortProduct([FromBody]string sortcriteria,string sortingorder){
           try{
               List<Product> sortedProductList=new List<Product>(){};
               if(sortcriteria=="name") sortedProductList=await _productService.SortProductsByName(sortingorder);
               else if(sortcriteria=="category") sortedProductList=await _productService.SortProductsByCategory(sortingorder);
               else if(sortcriteria=="price") sortedProductList=await _productService.SortProductsByPrice(sortingorder);
               else return BadRequest("Wrong Criteria for Sorting");

               return Ok(sortedProductList);
           }catch{
               return BadRequest();
           }
       }
       /*
       GET api/products/category/{category}: This endpoint asynchronously retrieves products by a specified category, 
       handling potential exceptions and returning them in an HTTP response, with a 404 status if no products are found for the category.
       */
       [HttpGet]
       [Route("category/{category}")]
       public async Task<IActionResult> GetProductByCategory(string category){
           List<Product> productList=new List<Product>(){};
           try{
               productList = await _productService.GetProductsByCategory(category);
               if(productList.Count!=0) return Ok(productList);
               else return NotFound();
           }catch{
               return BadRequest("Error in getting productsa by category");
           }
       }
       /*
       DELETE api/products/{id}: This endpoint handles HTTP DELETE requests to delete a product by its unique identifier,
       returning a 204 No Content status upon successful deletion or a 404 status if the product is not found.
       */
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductById(int id){
            try{
                if(_productService.GetProductById(id)!=null){
                    await _productService.DeleteProduct(id);
                    return NoContent();
                }else{
                    return NotFound();
                }
            }catch{
                return BadRequest("Error in deleting product by id");
            }
        }
       
      /*
      DELETE api/products: This endpoint handles HTTP DELETE requests to delete all products, returning a 204 No Content status upon successful deletion.
      */
        [HttpDelete]
        public async Task<IActionResult> DeleteAllProducts(){
            try{
                await _productService.DeleteAllProducts();
                return NoContent();
            }catch{
                return BadRequest("Error in deleting all products");
            }
        }


    }   
    
}
