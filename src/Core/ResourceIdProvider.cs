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
            return AuthorizationExtensionOptionsAccessor.CurrentValue?.Resource ?? new ResourceOptions();
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
        //protected string GetRoutePatternText(RouteTemplate routeTemplate)
        //{
        //    StringBuilder templateBuilder = new StringBuilder();
        //    foreach (TemplateSegment segment in routeTemplate.Segments)
        //    {
        //        templateBuilder.Append("/");
        //        foreach (TemplatePart part in segment.Parts)
        //        {
        //            if (part.IsParameter)
        //            {
        //                templateBuilder.Append("{").Append(part.Name).Append("}");
        //            }
        //            else
        //            {
        //                templateBuilder.Append(part.Text);
        //            }
        //        }
        //    }
        //    return templateBuilder.ToString();
        //}


        protected IEnumerable<string> GetRequiredKeys()
        {
            ResourceOptions resourceOptions = GetResourceOptions();
            if (resourceOptions.CustomRouteKeys.IsNullOrEmpty())
            {
                return resourceOptions.RequiredRouteKeys;
            }
            return resourceOptions.RequiredRouteKeys.Union(resourceOptions.CustomRouteKeys);
        }

        public string GetResourceId(RouteEndpoint routeEndpoint)
        {
            StringBuilder templateBuilder = new StringBuilder();
            IEnumerable<RoutePatternPathSegment> pathSegments = routeEndpoint.RoutePattern.PathSegments;
            if (pathSegments.IsNullOrEmpty())
            {
                return null;
            }
            IDictionary<string, object> values = GetRequiredValues();
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
                        templateBuilder.Append("{");
                        if (parameterPart.IsCatchAll)
                        {
                            templateBuilder.Append("*");
                            if (!parameterPart.EncodeSlashes)
                            {
                                templateBuilder.Append("*");
                            }
                        }
                        if (values.ContainsKey(parameterPart.Name))
                        {
                            templateBuilder.Append(values[parameterPart.Name]);
                        }
                        else
                        {
                            templateBuilder.Append(parameterPart.Name);
                        }
                        if (parameterPart.IsOptional)
                        {
                            templateBuilder.Append("?");
                        }
                        //需不要考虑规则呢？ 例如5:int 
                        //if (!parameterPart.ParameterPolicies.IsNullOrEmpty())
                        //{

                        //}
                        templateBuilder.Append("}");
                    }
                }
            }
            return templateBuilder.ToString();
        }

        private IDictionary<string, object> GetRequiredValues()
        {
            IEnumerable<string> requiredKeys = GetRequiredKeys();
            Dictionary<string, object> values = new Dictionary<string, object>();
            if (requiredKeys.IsNullOrEmpty())
            {
                return values;
            }
            RouteData routeData = HttpContextAccessor?.HttpContext.GetRouteData();
            if (routeData == null || routeData.Values.Count == 0)
            {
                return values;
            }
            foreach (string key in routeData.Values.Keys)
            {
                if (requiredKeys.Any(i => i == key))
                {
                    values.Add(key, routeData.Values[key]);
                }
            }
            return values;
        }
        public string GetResourceId()
        {
            HttpRequest request = GetHttpRequest();
            Endpoint endpoint = HttpContextAccessor?.HttpContext.GetEndpoint();
            string resourceId = null;
            if (endpoint is RouteEndpoint routeEndpoint)
            {
                resourceId = GetResourceId(routeEndpoint);
            }
            else
            {
                throw new NotSupportedException("获取RouteEndpoint失败！");
            }
            //放弃对传统路由的支持
            //else
            //{
            //    RouteData routeData = HttpContextAccessor?.HttpContext.GetRouteData();
            //    Route router = routeData.Routers.GetOrDefault(1) as Route;
            //    if (router != null)
            //    {
            //        resourceId= GetRoutePatternText(router.ParsedTemplate);
            //    }
            //}
            if (string.IsNullOrWhiteSpace(resourceId))
            {
                return $"{request.Method}|{request.Scheme}|{request.PathBase}{request.Path}";
            }
            return $"{request.Method}|{request.Scheme}|{resourceId}";
        }
    }
}