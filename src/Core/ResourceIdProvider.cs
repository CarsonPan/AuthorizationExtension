using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AuthorizationExtension.Core
{
    public class ResourceIdProvider : IResourceIdProvider
    {
        protected readonly IHttpContextAccessor HttpContextAccessor;
        protected readonly IOptionsMonitor<AuthorizationExtensionOptions> AuthorizationExtensionOptionsAccessor;
        public ResourceIdProvider(IHttpContextAccessor httpContextAccessor,
                                  IOptionsMonitor<AuthorizationExtensionOptions> authorizationExtensionOptionsAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
            AuthorizationExtensionOptionsAccessor = authorizationExtensionOptionsAccessor;
        }

        protected ResourceOptions GetResourceOptions()
        {
            return AuthorizationExtensionOptionsAccessor.CurrentValue?.Resource??new ResourceOptions();
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
            StringBuilder templateBuilder = new StringBuilder();
            foreach (TemplateSegment segment in routeTemplate.Segments)
            {
                templateBuilder.Append("/");
                foreach (TemplatePart part in segment.Parts)
                {
                    if (part.IsParameter)
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
        protected virtual string GetRoutePatternText()
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

        protected IEnumerable<string> GetRequiredKeys()
        {
            ResourceOptions resourceOptions = GetResourceOptions();
            return resourceOptions.RequiredRouteKeys.Union(resourceOptions.CustomRouteKeys);
        }
        public string GetResourceId()
        {
            HttpRequest request = GetHttpRequest();
            string routePatternText = GetRoutePatternText();
            if (string.IsNullOrWhiteSpace(routePatternText))
            {
                return $"{request.Method}|{request.Scheme}|{request.PathBase}{request.Path}".ToLower();
            }
            RouteData routeData = HttpContextAccessor?.HttpContext.GetRouteData();
            if (routeData == null || routeData.Values.Count == 0)
            {
                return $"{request.Method}|{request.Scheme}|{routePatternText}";
            }
            IEnumerable<string> requiredKeys=GetRequiredKeys();
            if(requiredKeys.IsNullOrEmpty())
            {
                return $"{request.Method}|{request.Scheme}|{routePatternText}";
            }
            foreach (string key in routeData.Values.Keys)
            {
                    if (requiredKeys.Any(i => i == key))
                    {
                        routePatternText =routePatternText.Replace("{" + key + "}", routeData.Values[key].ToString());
                    }
            }
            return $"{request.Method}|{request.Scheme}|{routePatternText}";
        }
    }
}