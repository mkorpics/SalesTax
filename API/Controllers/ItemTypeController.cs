using API.Contracts.Business;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace API.Controllers
{
    [ApiController]
    [Route("api/ItemTypes")]
    [Produces("application/json")]
    public class ItemTypeController : ControllerBase
    {
        private IItemTypeBusiness _itemTypeBusiness;
        private ILogger _logger;

        public ItemTypeController(IItemTypeBusiness itemTypeBusiness,
            ILogger<ItemTypeController> logger)
        {
            _itemTypeBusiness = itemTypeBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Gets all items types.
        /// </summary>
        /// <response code="200">Returns the list of item types.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ItemType>), Status200OK)]
        public IActionResult GetItemTypes()
        {
            var result = _itemTypeBusiness.GetItemTypes();

            return Ok(result);
        }
    }
}
