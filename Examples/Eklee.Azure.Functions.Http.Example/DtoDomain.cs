using System;
using System.ComponentModel.DataAnnotations;
using Eklee.Azure.Functions.Http.Example.Models;

namespace Eklee.Azure.Functions.Http.Example
{
    public class DtoDomain : IDtoDomain
    {
        private readonly IHttpRequestContext _httpRequestContext;

        public DtoDomain(IHttpRequestContext httpRequestContext)
        {
            _httpRequestContext = httpRequestContext;
        }

        public DtoResponse DoWork()
        {
            var dto = _httpRequestContext.GetModelFromBody<DtoRequest>();

            if (string.IsNullOrEmpty(dto.Key1))
            {
                // Error is already going to translate back to a 400 bad request.
                throw new ValidationException("Key1 is a required field.");
            }

            if (dto.Key1.StartsWith("bar"))
            {
                // Our custom handler will be fired to handle this exception into a 400 bad request.
                throw new ArgumentException("Cannot start with the word bar!");
            }

            if (dto.Key1.Contains("="))
            {
                // Our custom handler will be fired to handle this exception into a 400 bad request.
                throw new MyCustomException("Cannot contain char '='!");
            }

            if (dto.Key1.Contains(":"))
            {
                // This is an example of a unhandled exception firing.
                throw new ApplicationException("Cannot contain char ':'!");
            }

            return new DtoResponse { Key1Result = "result" + dto.Key1 };
        }
    }
}