using System;
using System.Linq;
using BookStore.Api.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Moq;
using Xunit;

namespace BookStore.UnitTests.BookStore.Api
{
    public class LinkGeneratorHelperTests
    {
        private readonly LinkGeneratorHelper _linkGeneratorHelper;

        public LinkGeneratorHelperTests()
        {
            var linkGeneratorMock = new Mock<LinkGenerator>();

            // Cannot setup an extension method
            //linkGeneratorMock.Setup(g => g.GetUriByAction(It.IsAny<HttpContext>(),
            //        It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(),
            //        It.IsAny<string>(), It.IsAny<HostString?>(), It.IsAny<PathString?>(),
            //        It.IsAny<FragmentString>(), It.IsAny<LinkOptions>()))
            //    .Returns("/");

            _linkGeneratorHelper = new LinkGeneratorHelper(linkGeneratorMock.Object);
        }

        [Fact]
        public void Update_NoActionNameProvided_ReturnsEmptyListOfLinks()
        {
            var links =_linkGeneratorHelper.CreateLinks(new DefaultHttpContext(), string.Empty, string.Empty, string.Empty, Guid.NewGuid());
            links.Should().HaveCount(0);
        }

        [Fact]
        public void Update_ActionNamesProvided_ReturnsEmptyListOfLinks()
        {
            var links = _linkGeneratorHelper.CreateLinks(new DefaultHttpContext(), "GetById", "Update", "Delete", Guid.NewGuid()).ToArray();
            links.Should().HaveCount(3);

            var link = links[0];
            link.Rel.Should().Be("self");
            link.Method.Should().Be("GET");

            link = links[1];
            link.Rel.Should().Be("update");
            link.Method.Should().Be("PUT");

            link = links[2];
            link.Rel.Should().Be("delete");
            link.Method.Should().Be("DELETE");
        }
    }
}
