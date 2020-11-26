using System;
using System.IO;
using System.Text;
using Eklee.Azure.Functions.Http.Tests.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eklee.Azure.Functions.Http.Tests
{
    [Trait(Constants.Category, Constants.UnitTests)]
    public class HttpRequestContextTests
    {
        private readonly IHttpRequestContext _httpRequestContext;

        public HttpRequestContextTests()
        {
            var logger = Substitute.For<ILogger>();
            _httpRequestContext = new HttpRequestContext
            {
                Request = new MockHttpRequest(),
                Logger = logger
            };
        }

        [Fact]
        public void Able_to_get_model_from_request_body()
        {

            _httpRequestContext.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(
                JsonConvert.SerializeObject(new Foo { Name = "happy" })));

            var foo = _httpRequestContext.GetModelFromBody<Foo>();
            foo.ShouldNotBeNull();
            foo.Name.ShouldBe("happy");
        }

        [Fact]
        public void Null_model_from_empty_request_body()
        {

            _httpRequestContext.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(""));

            var foo = _httpRequestContext.GetModelFromBody<Foo>();
            foo.ShouldBeNull();
        }

        [Fact]
        public void Principal_name_exist_when_in_header()
        {
            var mock = (MockHttpRequest)_httpRequestContext.Request;
            mock.MockHeader(HttpRequestContext.X_MS_CLIENT_PRINCIPAL_NAME, "david lee");

            _httpRequestContext.Security.Principal.Name.ShouldBe("david lee");
        }


        [Fact]
        public void Principal_id_exist_when_in_header()
        {
            var id = Guid.NewGuid().ToString();
            var mock = (MockHttpRequest)_httpRequestContext.Request;
            mock.MockHeader(HttpRequestContext.X_MS_CLIENT_PRINCIPAL_ID, id);

            _httpRequestContext.Security.Principal.Id.ShouldBe(id);
        }
    }
}
