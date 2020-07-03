using System;
using System.Collections.Generic;
using BookStore.Api.Contracts.Responses;
using Microsoft.AspNetCore.Http;

namespace BookStore.Api.Helpers
{
    public interface ILinkGenerator
    {
        IEnumerable<Link> CreateLinks(HttpContext httpContext, string getActionName, string updateActionName, string deleteActionName, Guid id);
    }
}