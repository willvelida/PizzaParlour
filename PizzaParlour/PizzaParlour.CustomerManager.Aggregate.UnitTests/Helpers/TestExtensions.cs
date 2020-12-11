using Microsoft.Azure.Cosmos;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace PizzaParlour.CustomerManager.Aggregate.UnitTests.Helpers
{
    public static class TestExtensions
    {
        public static Mock<ItemResponse<T>> SetupUpsertItemAsync<T>(this Mock<Container> containerMock)
        {
            var itemResponseMock = new Mock<ItemResponse<T>>();

            containerMock
                .Setup(x => x.UpsertItemAsync(
                    It.IsAny<T>(),
                    It.IsAny<PartitionKey>(),
                    It.IsAny<ItemRequestOptions>(),
                    It.IsAny<CancellationToken>()))
                .Callback(
                    (T item, PartitionKey? pk, ItemRequestOptions opt, CancellationToken ct) => itemResponseMock.Setup(x => x.Resource).Returns(null))
                .ReturnsAsync(
                    (T item, PartitionKey? pk, ItemRequestOptions opt, CancellationToken ct) => itemResponseMock.Object);

            return itemResponseMock;
        }
    }
}
