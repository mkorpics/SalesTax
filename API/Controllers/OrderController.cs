using API.Contracts.Business;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace API.Controllers
{
    [ApiController]
    [Route("api/Orders")]
    [Produces("application/json")]
    public class OrderController : ControllerBase
    {
        private IOrderBusiness _orderBusiness;
        private IShoppingCartItemBusiness _shoppingCartItemBusiness;
        private ILogger _logger;

        public OrderController(
            IOrderBusiness orderBusiness,
            IShoppingCartItemBusiness shoppingCartItemBusiness,
            ILogger<OrderController> logger)
        {
            _orderBusiness = orderBusiness;
            _shoppingCartItemBusiness = shoppingCartItemBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Gets all orders.
        /// </summary>
        /// <response code="200">Returns the list of orders.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Order>), Status200OK)]
        public IActionResult GetOrders()
        {
            var result = _orderBusiness.GetOrders();

            return Ok(result);
        }

        /// <summary>
        /// Gets the order for the given Id.
        /// </summary>
        /// <param name="orderId">Order to retrieve.</param>
        /// <response code="200">Returns the order.</response>
        /// <response code="404">Order not found.</response>
        [HttpGet]
        [Route("{orderId:int}")]
        [ProducesResponseType(typeof(IEnumerable<Order>), Status200OK)]
        public IActionResult GetOrder(int orderId)
        {
            _logger.LogInformation($"Retrieving order with OrderId={orderId}");
            var result = _orderBusiness.GetOrder(orderId);
            if (result == null)
            {
                _logger.LogError($"Failed to get Order with OrderId={orderId}. Order does not exist.");
                return NotFound();
            }
            _logger.LogInformation($"Retrieved order with OrderId={orderId}");
            return Ok(result);
        }

        /// <summary>
        /// Creates Order for the input shopping cart purchase items.
        /// </summary>
        /// <param name="purchaseItemIds">Shopping cart purchase item Ids for the order.</response>
        /// <response code="200">Returns the created order.</response>
        /// <response code="422">Input purchase items are invalid.</response>
        /// <remarks>When the input purchase items are added to the created order, they are removed from the shopping cart.</remarks>
        [HttpPost]
        [ProducesResponseType(typeof(Order), Status200OK)]
        [ProducesResponseType(Status422UnprocessableEntity)]
        public IActionResult CreateOrder([FromBody] IEnumerable<int> purchaseItemIds)
        {
            _logger.LogInformation("Creating order");

            var shoppingCartItemDoesNotExist = purchaseItemIds.Any(id => !_shoppingCartItemBusiness.ShoppingCartItemExists(id));

            if (shoppingCartItemDoesNotExist)
            {
                _logger.LogError($"Failed to create order. At least one input purchase item was not found in the shopping cart.");
                return UnprocessableEntity();
            }

            var result = _orderBusiness.CreateOrder(purchaseItemIds);

            _logger.LogInformation($"Created order with OrderId {result.OrderId}");

            return Ok(result);
        }
    }
}
