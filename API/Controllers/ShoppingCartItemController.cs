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
        /// Gets all items in the shopping cart.
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
        /// Updates the count of the inventory item in the shopping cart, incrementing or decrementing by one.
        /// </summary>
        /// <param name="inventoryItemId">Inventory item whose count will be updated.</response>
        /// <param name="increaseCount">Determines if the count is increased or decreased by one.</response>
        /// <response code="200">Returns the upserted purchase item. Result will be null if all inventory items for the given inventoryItemId were removed from the cart.</response>
        /// <response code="400">Attempt made to decrease the count for an item not in the shopping cart.</response>
        /// <response code="404">Input inventory item does not exist.</response>
        /// <remarks>
        /// Manages the count of the inventory items in the shopping cart. 
        /// Based on the current quantity and increaseCount value, a purchase item will be created with a quantiy of one, update its quantity, or be deleted.
        /// </remarks>
        [HttpPut]
        [ProducesResponseType(typeof(PurchaseItem), Status200OK)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status404NotFound)]
        [Route("UpsertInventory")]
        public IActionResult AddInventoryItemToShoppingCart([FromBody] int inventoryItemId, bool increaseCount)
        {
            _logger.LogInformation($"Updating count of inventory item with InventoryItemId {inventoryItemId} in shopping cart.");

            if (!_inventoryItemBusiness.InventoryItemExists(inventoryItemId))
            {
                _logger.LogError($"Inventory item with InventoryItemId {inventoryItemId} does not exist.");
                return NotFound();
            }
            if (!increaseCount && !_shoppingCartItemBusiness.ShoppingCartItemExists(x => x.InventoryItemId == inventoryItemId && x.IsInShoppingCart))
            {
                _logger.LogError($"Cannot decrement the count for an inventory item that is not in the shopping cart. Quantity cannot be less than 0.");
                return BadRequest();
            }

            var result = _shoppingCartItemBusiness.UpsertInventoryItemInShoppingCart(inventoryItemId, increaseCount);

            _logger.LogInformation($"Updated count of inventory item with InventoryItemId {inventoryItemId} in shopping cart on purchase item with PurchaseItemId {result?.PurchaseItemId}.");

            return Ok(result);
        }

        /// <summary>
        /// Removes the purchase item for the given itemId from the Shopping Cart
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
