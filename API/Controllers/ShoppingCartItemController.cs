using API.Contracts.Business;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace API.Controllers
{
    [ApiController]
    [Route("api/ShoppingCartItems")]
    [Produces("application/json")]
    public class ShoppingCartItemController : ControllerBase
    {
        private IInventoryBusiness _inventoryItemBusiness;
        private IShoppingCartItemBusiness _shoppingCartItemBusiness;
        private ILogger _logger;

        public ShoppingCartItemController(IInventoryBusiness inventoryItemBusiness,
            IShoppingCartItemBusiness shoppingCartItemBusiness,
            ILogger<ShoppingCartItemController> logger)
        {
            _inventoryItemBusiness = inventoryItemBusiness;
            _shoppingCartItemBusiness = shoppingCartItemBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Gets all items currently in the shopping cart.
        /// </summary>
        /// <response code="200">Returns the list of purchase items in the shopping cart.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PurchaseItem>), Status200OK)]
        public IActionResult GetShoppingCartItems()
        {
            var result = _shoppingCartItemBusiness.GetShoppingCartItems();

            return Ok(result);
        }

        /// <summary>
        /// Adds inventory item to the shopping cart.
        /// </summary>
        /// <param name="inventoryItemId">Inventory item to create.</response>
        /// <response code="200">Returns the created purchase item.</response>
        /// <response code="404">Input inventory item does not exist.</response>
        /// <remarks>
        /// If a purchase item exists in the shopping cart for the given inventory item, the quantity is increased by one.
        /// If a purchase item does not exist in the shopping cart for the given inventory item, a purchase item is created for the inventory item with a quantity of one.
        /// </remarks>
        [HttpPut]
        [ProducesResponseType(typeof(PurchaseItem), Status200OK)]
        [Route("AddInventory")]
        public IActionResult AddInventoryItemToShoppingCart([FromBody] int inventoryItemId) // todo: part of Inventory controller instead?
        {
            _logger.LogInformation($"Adding inventory item with InventoryItemId {inventoryItemId} to shopping cart.");

            if (!_inventoryItemBusiness.InventoryItemExists(inventoryItemId))
            {
                _logger.LogError($"Inventory item with InventoryItemId {inventoryItemId} does not exist.");
                return NotFound();
            }

            var result = _shoppingCartItemBusiness.AddInventoryItemToShoppingCart(inventoryItemId);

            _logger.LogInformation($"Added inventory item with InventoryItemId {inventoryItemId} to shopping cart on purchase item with PurchaseItemId {result.PurchaseItemId}.");

            return Ok(result);
        }

        ///// <summary>
        ///// Adds input Item to the Shopping Cart
        ///// </summary>
        ///// <param name="itemInput">Item to create.</response>
        ///// <response code="200">Returns the created Item.</response>
        ///// <response code="422">Input Item is invalid.</response>
        //[HttpPost]
        //[ProducesResponseType(typeof(PurchaseItem), Status200OK)]
        //public IActionResult CreateShoppingCartItem([FromBody] InventoryItemInput itemInput)
        //{
        //    _logger.LogInformation("Creating shopping cart Item");

        //    var isValid = true;
        //    var errorMessage = "";
        //    // call validator => set error messages / return error messages (boolean, string)
        //    // validate input not null
        //    // validate price is not negative
        //    // validate that item type is valid / exists

        //    if (!isValid)
        //    {
        //        _logger.LogError($"Failed to create shopping cart Item due to validation errors: {errorMessage}.");
        //        return UnprocessableEntity();
        //    }

        //    var result = _shoppingCartItemBusiness.CreateShoppingCartItem(itemInput);

        //    _logger.LogInformation($"Created shopping cart Item with ItemId {result.ItemId}");

        //    return Ok(result);
        //}

        /// <summary>
        /// Removes the Item for the given Id from the Shopping Cart
        /// </summary>
        /// <param name="itemId">Item to remove.</response>
        /// <response code="200">Item was removed.</response>
        /// <response code="404">Item does not exist.</response>
        [HttpDelete]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status404NotFound)]
        [Route("{itemId}")]
        public IActionResult DeleteShoppintCartItem(int itemId)
        {
            _logger.LogInformation($"Removing Item with ItemId {itemId} from the shopping cart.");

            var itemExists = _shoppingCartItemBusiness.ShoppingCartItemExists(itemId);
            if (!itemExists)
            {
                _logger.LogError($"Failed to remove Item with ItemId {itemId} from the shopping cart. Item does not exist.");
                return NotFound();
            }

            _shoppingCartItemBusiness.DeleteShoppingCartItem(itemId);

            _logger.LogInformation($"Deleted shopping cart Item with ItemId {itemId}");

            return NoContent();
        }
    }
}
