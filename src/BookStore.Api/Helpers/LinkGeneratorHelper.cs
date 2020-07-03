using System;
using System.Collections.Generic;
using BookStore.Api.Contracts.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace BookStore.Api.Helpers
{
    public class LinkGeneratorHelper : ILinkGenerator
    {
        private readonly LinkGenerator _linkGenerator;

        public LinkGeneratorHelper(LinkGenerator linkGenerator)
        {
            _linkGenerator = linkGenerator;
        }

        public IEnumerable<Link> CreateLinks(HttpContext httpContext, string getActionName, string updateActionName, string deleteActionName, Guid id)
        {
            var links = new List<Link>();

            if (!string.IsNullOrWhiteSpace(getActionName))
            {
                links.Add(new Link(_linkGenerator.GetUriByAction(httpContext, getActionName, values: new { id }),
                    "self",
                    "GET"));
            };

            if (!string.IsNullOrWhiteSpace(updateActionName))
            {
                links.Add(new Link(_linkGenerator.GetUriByAction(httpContext, updateActionName, values: new { id }),
                    "update",
                    "PUT"));
            };

            if (!string.IsNullOrWhiteSpace(updateActionName))
            {
                links.Add(new Link(_linkGenerator.GetUriByAction(httpContext, deleteActionName, values: new { id }),
                    "delete",
                    "DELETE"));
            };

            return links;
        }
    }
}
