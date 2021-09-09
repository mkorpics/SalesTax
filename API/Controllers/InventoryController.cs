using API.Contracts.Business;
using API.Models;
using API.Models.InputModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace API.Controllers
{
    [ApiController]
    [Route("api/InventoryItems")]
    [Produces("application/json")]
    public class InventoryController : ControllerBase
    {
        private IInventoryBusiness _inventoryItemBusiness;
        private IItemTypeBusiness _itemTypeBusiness;
        private ILogger _logger;

        public InventoryController(IInventoryBusiness inventoryItemBusiness,
            IItemTypeBusiness itemTypeBusiness,
            ILogger<InventoryController> logger)
        {
            _inventoryItemBusiness = inventoryItemBusiness;
            _itemTypeBusiness = itemTypeBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Gets all inventory items.
        /// </summary>
        /// <response code="200">Returns the list of inventory items.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<InventoryItem>), Status200OK)]
        public IActionResult GetInventoryItems()
        {
            var result = _inventoryItemBusiness.GetInventoryItems();

            return Ok(result);
        }

        /// <summary>
        /// Adds inventory item.
        /// </summary>
        /// <param name="itemInput">Item to create.</response>
        /// <response code="200">Returns the created Item.</response>
        /// <response code="422">Input Item is invalid.</response>
        [HttpPost]
        [ProducesResponseType(typeof(InventoryItem), Status200OK)]
        public IActionResult CreateInventoryItem([FromBody] InventoryItemInput itemInput)
        {
            _logger.LogInformation("Creating inventory item");

            (var isValid, var errorMessage) = ValidateInventoryItemInput(itemInput);

            if (!isValid)
            {
                _logger.LogError($"Failed to create inventory item due to validation errors: {errorMessage}.");
                return UnprocessableEntity();
            }

            var result = _inventoryItemBusiness.CreateInventoryItem(itemInput);

            _logger.LogInformation($"Created inventory item with InventoryItemId {result.InventoryItemId}");

            return Ok(result);
        }

        /// <summary>
        /// Removes the inventory item for the given Id
        /// </summary>
        /// <param name="itemId">Item to remove.</response>
        /// <response code="200">Item was removed.</response>
        /// <response code="404">Item does not exist.</response>
        /// <response code="422">Item is not valid to delete. It is in use in the shopping cart or on an order.</response>
        [HttpDelete]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status404NotFound)]
        [Route("{itemId}")]
        public IActionResult DeleteInventoryItem(int itemId)
        {
            _logger.LogInformation($"Deleting inventory item with InventoryItemId {itemId}.");

            var itemExists = _inventoryItemBusiness.InventoryItemExists(itemId);
            if (!itemExists)
            {
                _logger.LogError($"Failed to delete inventory item with InventoryItemId {itemId}. Item does not exist.");
                return NotFound();
            }

            var usedOnPurchaseItem = _inventoryItemBusiness.InventoryItemInUse(itemId);
            if (usedOnPurchaseItem)
            {
                _logger.LogError($"Cannot delete inventory item with InventoryItemId {itemId}. Item is in the shopping cart or on an order.");
                return UnprocessableEntity();
            }

            _inventoryItemBusiness.DeleteInventoryItem(itemId);

            _logger.LogInformation($"Deleted inventory item with ItemId {itemId}");

            return NoContent();
        }

        private (bool, string) ValidateInventoryItemInput(InventoryItemInput input)
        {
            var errorMessages = new List<string>();
            if (input == null)
            {
                errorMessages.Add("Input cannot be null.");
            }
            else
            {
                if (input.Price < 0) // todo: also validate max value?
                {
                    errorMessages.Add("Price must be greater than or equal to 0.");
                }

                string expression = @"^[\d]+([.][\d]{0,2})?$";
                var validDecimalFormat = Regex.IsMatch(input.Price.ToString(), expression);
                if (!validDecimalFormat)
                {
                    errorMessages.Add("Price cannot contain more than 2 decimals.");
                }

                if (!_itemTypeBusiness.ItemTypeExists(input.ItemTypeId))
                {
                    errorMessages.Add($"Item Type with ItemTypeId {input.ItemTypeId} does not exist.");
                }
            }
            var isValid = !errorMessages.Any();
            var errorMessage = string.Join(" ", errorMessages).Trim();
            return (isValid, errorMessage);
        }
    }
}
