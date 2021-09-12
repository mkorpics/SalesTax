using API.Business;
using API.Contracts.Utilities;
using API.DbContext;
using API.Models;
using API.Models.InputModels;
using AutoFixture;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Tests.Business
{
    [TestFixture]
    public class InventoryBusinessTest
    {
        private InventoryBusiness _inventoryBusiness;
        private Mock<IDataUtility> _dataUtility;
        private Fixture _fixture = new Fixture();

        [SetUp]
        public void Setup()
        {
            SalesTaxDbContext.ItemTypes = new List<ItemType>();
            SalesTaxDbContext.InventoryItems = new List<InventoryItem>();
            SalesTaxDbContext.PurchaseItems = new List<PurchaseItem>();

            _dataUtility = new Mock<IDataUtility>();

            _inventoryBusiness = new InventoryBusiness(
                _dataUtility.Object);
        }

        [Test]
        public void GetInventoryItems_SetsCanDelete()
        {
            var associatedInventoryItem = new InventoryItem() { InventoryItemId = _fixture.Create<int>() };
            var nonAssociatedInventoryItem = new InventoryItem() { InventoryItemId = _fixture.Create<int>() };
            SalesTaxDbContext.InventoryItems.Add(associatedInventoryItem);
            SalesTaxDbContext.InventoryItems.Add(nonAssociatedInventoryItem);

            var purchaseItem = new PurchaseItem() { InventoryItemId = associatedInventoryItem.InventoryItemId };
            SalesTaxDbContext.PurchaseItems.Add(purchaseItem);

            var result = _inventoryBusiness.GetInventoryItems();

            Assert.That(result.ElementAt(0).CanDelete, Is.False);
            Assert.That(result.ElementAt(1).CanDelete, Is.True);
        }

        [Test]
        [TestCase(0)]
        [TestCase(0.36)]
        [TestCase(1)]
        [TestCase(1.36)]
        [TestCase(2363)]
        [TestCase(2363.36)]
        [TestCase(14.99)]
        public void CreateInventoryItem_CalculatesSalesTax_WhenItemHasNoTax(decimal price)
        {
            var itemType = new ItemType() { ItemTypeId = _fixture.Create<int>(), HasBasicSalesTax = false, IsImported = false };
            SalesTaxDbContext.ItemTypes.Add(itemType);

            var inventoryItemInput = new InventoryItemInput() { ItemTypeId = itemType.ItemTypeId, Price = price };

            var result = _inventoryBusiness.CreateInventoryItem(inventoryItemInput);

            Assert.That(result.SalesTax, Is.EqualTo(0));
        }

        [Test]
        [TestCase(14.99, 1.50)]
        [TestCase(18.99, 1.90)]
        public void CreateInventoryItem_CalculatesSalesTax_WhenItemHasBaseSalesTax(decimal price, decimal expectedTax)
        {
            var itemType = new ItemType() { ItemTypeId = _fixture.Create<int>(), HasBasicSalesTax = true, IsImported = false };
            SalesTaxDbContext.ItemTypes.Add(itemType);

            var inventoryItemInput = new InventoryItemInput() { ItemTypeId = itemType.ItemTypeId, Price = price };

            var result = _inventoryBusiness.CreateInventoryItem(inventoryItemInput);

            Assert.That(result.SalesTax, Is.EqualTo(expectedTax));
        }

        [Test]
        [TestCase(10, 0.50)]
        [TestCase(11.25, 0.60)]
        public void CreateInventoryItem_CalculatesSalesTax_WhenItemHasImportTax(decimal price, decimal expectedTax)
        {
            var itemType = new ItemType() { ItemTypeId = _fixture.Create<int>(), HasBasicSalesTax = false, IsImported = true };
            SalesTaxDbContext.ItemTypes.Add(itemType);

            var inventoryItemInput = new InventoryItemInput() { ItemTypeId = itemType.ItemTypeId, Price = price };

            var result = _inventoryBusiness.CreateInventoryItem(inventoryItemInput);

            Assert.That(result.SalesTax, Is.EqualTo(expectedTax));
        }

        [Test]
        [TestCase(27.99, 4.20)]
        [TestCase(47.50, 7.15)]
        public void CreateInventoryItem_CalculatesSalesTax_WhenItemHasBaseAndImportTax(decimal price, decimal expectedTax)
        {
            var itemType = new ItemType() { ItemTypeId = _fixture.Create<int>(), HasBasicSalesTax = true, IsImported = true };
            SalesTaxDbContext.ItemTypes.Add(itemType);

            var inventoryItemInput = new InventoryItemInput() { ItemTypeId = itemType.ItemTypeId, Price = price };

            var result = _inventoryBusiness.CreateInventoryItem(inventoryItemInput);

            Assert.That(result.SalesTax, Is.EqualTo(expectedTax));
        }

        [Test]
        [TestCase(0.85, false, false, 0.85)]
        [TestCase(9.75, false, false, 9.75)]
        [TestCase(12.49, false, false, 12.49)]
        [TestCase(14.99, true, false, 16.49)]
        [TestCase(18.99, true, false, 20.89)]
        [TestCase(10.00, false, true, 10.50)]
        [TestCase(11.25, false, true, 11.85)]
        [TestCase(27.99, true, true, 32.19)]
        [TestCase(47.50, true, true, 54.65)]
        public void CreateInventoryItem_CalculatesTotalPrice(decimal price, bool hasBasicTax, bool isImported, decimal expectedTotal)
        {
            var itemType = new ItemType() { ItemTypeId = _fixture.Create<int>(), HasBasicSalesTax = hasBasicTax, IsImported = isImported };
            SalesTaxDbContext.ItemTypes.Add(itemType);

            var inventoryItemInput = new InventoryItemInput() { ItemTypeId = itemType.ItemTypeId, Price = price };

            var result = _inventoryBusiness.CreateInventoryItem(inventoryItemInput);

            Assert.That(result.TotalPrice, Is.EqualTo(expectedTotal));
        }
    }
}
