using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using System;
using System.Text.RegularExpressions;


namespace ReadableUrls
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public static class ReadableUrls
    {        

        public static IApplicationBuilder UseReadableUrls(this IApplicationBuilder builder)
        {
            return builder.Use(new Func<RequestDelegate, RequestDelegate>(MakeRequestReadable));
        }

        private static RequestDelegate MakeRequestReadable(RequestDelegate next)
        {
            RequestDelegate requestDelegate = (HttpContext ctxt) =>
            {
                var requestUri = ctxt.Request.Path.Value.ToString();
                ctxt.Request.Path = new PathString(FormatUri(requestUri));
                return next(ctxt);
            };

            return requestDelegate;
        }


        private static string FormatUri(string uriToFormat)
        {
            if (string.IsNullOrWhiteSpace(uriToFormat))
            {
                throw new Exception("No uri was passed in.");
            }

            if (uriToFormat.Length <= 1)
            {
                return uriToFormat;
            }

            try
            {
                var regex = new Regex(@"(\-[a-z0-9])|\\[a-z0-9]");
                return regex.Replace(uriToFormat, MutateUri);
            }
            catch (Exception exc)
            {
                return uriToFormat;
            }
        }

        private static string MutateUri(Match m)
        {
            var uri = m.ToString();

            if (uri.Length <= 1)
            {
                return uri;
            }

            return uri[1].ToString().ToUpper();
        }
    }
}
