using System.Collections.Specialized;
using System.Web;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Reference.Commerce.Site.Features.Search.Controllers;
using EPiServer.Reference.Commerce.Site.Features.Search.ViewModelFactories;
using EPiServer.Reference.Commerce.Site.Features.Search.ViewModels;
using Moq;
using System.Web.Mvc;
using System.Web.Routing;
using Xunit;

namespace EPiServer.Reference.Commerce.Site.Tests.Features.Search.Controllers
{

    public class CategoryPartialControllerTests
    {
        [Fact]
        public void Index_WhenCallingViewModelFactory_ShouldSetPageSizeToThree()
        {
            // Act
            var result = (PartialViewResult)_subject.Index(null);

            // Assert
            _viewModelFactoryMock.Verify(v => v.Create(It.IsAny<NodeContent>(), It.Is<FilterOptionViewModel>(f => f.PageSize == 3), It.IsAny<string>()));
        }

        [Fact]
        public void Index_WhenCallingViewModelFactory_ShouldSetPageToOne()
        {
            // Act
            var result = (PartialViewResult)_subject.Index(null);

            // Assert
            _viewModelFactoryMock.Verify(v => v.Create(It.IsAny<NodeContent>(), It.Is<FilterOptionViewModel>(f => f.Page == 1), It.IsAny<string>()));
        }

        [Fact]
        public void Index_WhenCallingViewModelFactory_ShouldSetFacetGroupsToEmptyList()
        {
            // Act
            var result = (PartialViewResult)_subject.Index(null);

            // Assert
            _viewModelFactoryMock.Verify(v => v.Create(It.IsAny<NodeContent>(), It.Is<FilterOptionViewModel>(f => f.FacetGroups.Count == 0), It.IsAny<string>()));
        }

        [Fact]
        public void Index_WhenCallingViewModelFactory_ShouldPassAlongNodeContent()
        {
            // Arrange
            var nodeContent = new NodeContent();

            // Act
            var result = (PartialViewResult)_subject.Index(nodeContent);

            // Assert
            _viewModelFactoryMock.Verify(v => v.Create(nodeContent, It.IsAny<FilterOptionViewModel>(), It.IsAny<string>()));
        }

        CategoryPartialController _subject;
        Mock<SearchViewModelFactory> _viewModelFactoryMock;

        public CategoryPartialControllerTests()
        {
            _viewModelFactoryMock = new Mock<SearchViewModelFactory>(null, null);

            _viewModelFactoryMock
                .Setup(v => v.Create(It.IsAny<NodeContent>(), It.IsAny<FilterOptionViewModel>(), It.IsAny<string>()))
                .Returns(new SearchViewModel<NodeContent>());

            var httpRequestMock = new Mock<HttpRequestBase>();
            httpRequestMock.Setup(x => x.QueryString).Returns(new NameValueCollection());

            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(httpRequestMock.Object);
            _subject = new CategoryPartialController(_viewModelFactoryMock.Object);
            _subject.ControllerContext = new ControllerContext(context.Object, new RouteData(), _subject);
        }
    }
}
