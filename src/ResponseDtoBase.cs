using System;
using Epinova.Infrastructure;

namespace Epinova.PostnordShipping
{
    public abstract class ResponseDtoBase : IServiceResponseMessage
    {
        public string ErrorMessage { get; set; }
        public bool HasError => !String.IsNullOrWhiteSpace(ErrorMessage);
    }
}