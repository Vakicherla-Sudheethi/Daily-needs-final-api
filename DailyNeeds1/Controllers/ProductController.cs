using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DailyNeeds1.DTO;
using DailyNeeds1.Repo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using log4net;
using System.Net.Http.Headers;


namespace DailyNeeds1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private UnitOfWork unitOfWork;
        private ILog _logger;

        public ProductController(UnitOfWork uw, ILog logger)
        {
            unitOfWork = uw;
            _logger = logger;
        }

        [HttpGet,Route("GetAllProducts")]
        [Authorize(Roles = "4,1,10")]
        public IActionResult GetAllProducts()
        {
            var products = unitOfWork.ProductImplObject.GetAll();
            return Ok(products);
        }

        //newly-added code
        [HttpGet, Route("GetProductById/{id}")]
        [Authorize]
        public IActionResult GetProductById(int id)
        {
            try
            {
                var product = unitOfWork.ProductImplObject.GetProductById(id);
                if (product != null)
                {
                    return Ok(product);
                }
                else
                {
                    return NotFound($"Product with ID {id} not found");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost,Route("AddNewProduct")]
        [Authorize(Roles ="4,10")]
        public IActionResult AddNewProduct(ProductDTO product)
        {
            try
            {
                bool status = unitOfWork.ProductImplObject.Add(product);

                if (status)
                {
                    unitOfWork.SaveAll();
                    return Ok(product);
                }
                else
                {
                    return BadRequest("Error in adding product");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return StatusCode(500, ex.Message);
                //throw;
            }
        }

        [HttpPut,Route("UpdateProduct/{id}")]
       [Authorize(Roles ="4,10,1")]
        public IActionResult UpdateProduct(int id, ProductDTO productUpdate)
        {
            productUpdate.ProductID = id;

            try
            {
                bool status = unitOfWork.ProductImplObject.Update(productUpdate);

                if (status)
                {
                    unitOfWork.SaveAll();
                    return Ok(productUpdate);
                }
                else
                {
                    return BadRequest($"Product with ID {id} not found or update failed");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return StatusCode(500, ex.Message);
                //throw;
            }
        }

        [HttpDelete,Route("DeleteProduct/{id}")]
        [Authorize(Roles = "4,10")]
        public IActionResult DeleteProduct(int id)
        {
            try
            {
                bool status = unitOfWork.ProductImplObject.Delete(id);

                if (status)
                {
                    unitOfWork.SaveAll();
                    return Ok($"Product with ID {id} deleted");
                }
                else
                {
                    return BadRequest($"Product with ID {id} not found or delete failed");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return StatusCode(500, ex.Message);
                //throw;
            }
        }

        [HttpGet, Route("GetProductsByLocation/{locationId}")]
        [Authorize(Roles = "4,1,10")]
        public IActionResult GetProductsByLocation(int locationId)
        {
            try
            {
                var products = unitOfWork.ProductImplObject.GetProductsByLocationId(locationId);

                if (products != null && products.Count > 0)
                {
                    return Ok(products);
                }
                else
                {
                    return NotFound($"No products found for Location ID {locationId}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
        //newly implemented
        [HttpGet, Route("GetProductsByUserId/{userId}")]
        [Authorize(Roles = "4,1,10")]
        public IActionResult GetProductsByUserId(int userId)
        {
            try
            {
                var products = unitOfWork.ProductImplObject.GetProductsByUserId(userId);

                if (products != null && products.Count > 0)
                {
                    return Ok(products);
                }
                else
                {
                    return NotFound($"No products found for User ID {userId}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        //newly-added
        [HttpPost, Route("UploadProductImage"), DisableRequestSizeLimit]
        //[Authorize(Roles = "4,10,1")] // Adjust roles as needed
        public async Task<IActionResult> UploadProductImage()
        {
            try
            {
                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.First();
                var folderName = Path.Combine("Resources", "Images");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    return Ok(new { dbPath });
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }
    }
}
