using API.Contracts.Business;
using API.Controllers;
using API.Models;
using API.Models.InputModels;
using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Tests.Controllers
{
    [TestFixture]
    public class InventoryControllerTest
    {
        private InventoryController _inventoryController;
        private Mock<IInventoryBusiness> _mockInventoryItemBusiness;
        private Mock<IItemTypeBusiness> _mockItemTypeBusiness;
        private Mock<ILogger<InventoryController>> _mockLogger;

        private Fixture _fixture = new Fixture();

        [SetUp]
        public void Setup()
        {
            _mockInventoryItemBusiness = new Mock<IInventoryBusiness>();
            _mockItemTypeBusiness = new Mock<IItemTypeBusiness>();
            _mockLogger = new Mock<ILogger<InventoryController>>();

            _mockItemTypeBusiness
                .Setup(x => x.ItemTypeExists(It.IsAny<int>()))
                .Returns(true);

            _inventoryController = new InventoryController(
                _mockInventoryItemBusiness.Object,
                _mockItemTypeBusiness.Object,
                _mockLogger.Object);
        }

        [Test]
        public void CreateInventoryItem_ReturnsUnprocessableEntity_WhenInputItemIsNull()
        {
            InventoryItemInput item = null;
            var result = _inventoryController.CreateInventoryItem(item);

            Assert.That(result, Is.InstanceOf<UnprocessableEntityObjectResult>());
        }

        [Test]
        [TestCase(0.123)]
        [TestCase(-1)]
        [TestCase(-1.111)]
        [TestCase(53233.235234)]
        public void CreateInventoryItem_ReturnsUnprocessableEntity_WhenPriceValidationFails(decimal price)
        {
            var item = new InventoryItemInput()
            {
                Price = price
            };
            var result = _inventoryController.CreateInventoryItem(item);

            Assert.That(result, Is.InstanceOf<UnprocessableEntityObjectResult>());
        }

        [Test]
        public void CreateInventoryItem_ReturnsUnprocessableEntity_WhenItemTypeValidationFails()
        {
            _mockItemTypeBusiness
                .Setup(x => x.ItemTypeExists(It.IsAny<int>()))
                .Returns(false);

            var item = new InventoryItemInput()
            {
                Price = 1
            };
            var result = _inventoryController.CreateInventoryItem(item);

            Assert.That(result, Is.InstanceOf<UnprocessableEntityObjectResult>());
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(1.23)]
        [TestCase(52352.23)]
        public void CreateInventoryItem_ReturnsOk_WhenValidationPasses(decimal price)
        {
            _mockInventoryItemBusiness
                .Setup(x => x.CreateInventoryItem(It.IsAny<InventoryItemInput>()))
                .Returns(new InventoryItem());

            var item = new InventoryItemInput()
            {
                Price = price
            };
            var result = _inventoryController.CreateInventoryItem(item);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public void DeleteInventoryItem_ReturnsNotFound_WhenInventoryItemDoesNotExist()
        {
            _mockInventoryItemBusiness
                .Setup(x => x.InventoryItemExists(It.IsAny<int>()))
                .Returns(false);

            var result = _inventoryController.DeleteInventoryItem(_fixture.Create<int>());

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public void DeleteInventoryItem_ReturnsUnprocessableEntity_WhenInventoryItemAssociatedWithPurchaseItem()
        {
            _mockInventoryItemBusiness
                .Setup(x => x.InventoryItemExists(It.IsAny<int>()))
                .Returns(true);
            _mockInventoryItemBusiness
                .Setup(x => x.InventoryItemInUse(It.IsAny<int>()))
                .Returns(true);

            var result = _inventoryController.DeleteInventoryItem(_fixture.Create<int>());

            Assert.That(result, Is.InstanceOf<UnprocessableEntityResult>());
        }

        [Test]
        public void DeleteInventoryItem_ReturnsNoContent_WhenInventoryItemDeleted()
        {
            _mockInventoryItemBusiness
                .Setup(x => x.InventoryItemExists(It.IsAny<int>()))
                .Returns(true);
            _mockInventoryItemBusiness
                .Setup(x => x.InventoryItemInUse(It.IsAny<int>()))
                .Returns(false);

            var result = _inventoryController.DeleteInventoryItem(_fixture.Create<int>());

            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }
    }
}
