using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.AspNetCore.Routing.Template;
using System;
using System.Collections.Generic;
using System.Text;

namespace src.Core
{
    public class ResourceIdProvider : IResourceIdProvider
    {
        protected readonly IHttpContextAccessor HttpContextAccessor;
        public ResourceIdProvider(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }

        protected HttpRequest GetHttpRequest()
        {
            HttpRequest request = HttpContextAccessor?.HttpContext?.Request;
            if (request == null)
            {
                throw new NotSupportedException(nameof(HttpRequest));
            }
            return request;
        }
        protected string GetRoutePatternText(RouteEndpoint routeEndpoint)
        {
            StringBuilder templateBuilder = new StringBuilder();
            IEnumerable<RoutePatternPathSegment> pathSegments = routeEndpoint.RoutePattern.PathSegments;
            if (pathSegments.IsNullOrEmpty())
            {
                return null;
            }
            foreach (RoutePatternPathSegment segment in pathSegments)
            {
                templateBuilder.Append("/");
                foreach (RoutePatternPart part in segment.Parts)
                {
                    if (part.IsLiteral)
                    {
                        RoutePatternLiteralPart literalPart = part as RoutePatternLiteralPart;
                        templateBuilder.Append(literalPart.Content);
                    }
                    else if (part.IsSeparator)
                    {
                        RoutePatternSeparatorPart separatorPart = part as RoutePatternSeparatorPart;
                        templateBuilder.Append(separatorPart.Content);
                    }
                    else
                    {
                        RoutePatternParameterPart parameterPart = part as RoutePatternParameterPart;
                        templateBuilder.Append("{").Append(parameterPart.Name).Append("}");
                    }
                }
            }
            return templateBuilder.ToString();
        }

        protected string GetRoutePatternText(RouteTemplate routeTemplate)
        {   
            StringBuilder templateBuilder=new StringBuilder();
            foreach(TemplateSegment segment in routeTemplate.Segments)
            {
                 foreach(TemplatePart part in segment.Parts)
                 {
                     if(part.IsParameter)
                     {
                         templateBuilder.Append("{").Append(part.Name).Append("}");
                     }
                     else
                     {
                         templateBuilder.Append(part.Text);
                     }
                 }
            }
            return templateBuilder.ToString();
        }
        protected string GetRoutePatternText()
        {
            Endpoint endpoint = HttpContextAccessor?.HttpContext.GetEndpoint();
            if (endpoint is RouteEndpoint routeEndpoint)
            {
                return GetRoutePatternText(routeEndpoint);
            }
            RouteData routeData = HttpContextAccessor?.HttpContext.GetRouteData();
            Route router = routeData.Routers.GetOrDefault(1) as Route;
            if (router != null)
            {
                return GetRoutePatternText(router.ParsedTemplate);
            }
            return null;
        }
        public string GetResourceId()
        {
            HttpRequest request = GetHttpRequest();
            //return $"{request.Method}|{request.Scheme}|{request.PathBase}{request.Path}";
            throw new NotImplementedException();
        }
    }
}