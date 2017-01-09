﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using System.Xml;
using System.Configuration;
using log4net;
using Google.Cloud.Diagnostics.AspNet;

namespace GoogleAspNetWebApiWithLogging
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // To enable Google Cloud Stackdriver Logging and Error Reporting:
            // 1. Enable the Stackdriver Error Reporting API: 
            //    https://console.cloud.google.com/apis/api/clouderrorreporting.googleapis.com
            // 2. Edit Web.config. Replace "YOUR-PROJECT-ID" with your Google Cloud Project ID

            // [START logging_and_error_reporting]
            // Check to ensure that projectId has been changed from placeholder value.
            var section = (XmlElement)ConfigurationManager.GetSection("log4net");
            XmlElement element =
                (XmlElement)section.GetElementsByTagName("projectId").Item(0);
            string projectId = element.Attributes["value"].Value;
            if (projectId == ("YOUR-PROJECT-ID"))
            {
                throw new Exception("Update Web.config and replace YOUR-PROJECT-ID"
                   + " with your Google Cloud Project ID, and recompile.");
            }
            // [START enable_error_reporting]
            element =
                (XmlElement)section.GetElementsByTagName("serviceName").Item(0);
            string serviceName = element.Attributes["value"].Value;
            element =
                (XmlElement)section.GetElementsByTagName("version").Item(0);
            string version = element.Attributes["value"].Value;
            // Add a catch all to log all uncaught exceptions to Stackdriver Error Reporting.
            config.Services.Add(typeof(IExceptionLogger),
                ErrorReportingExceptionLogger.Create(projectId, serviceName, version));
            // [END enable_error_reporting]
            // [START enable_logging]
            // Retrieve a logger for this context.
            ILog log = LogManager.GetLogger(typeof(WebApiConfig));
            // [END enable_logging]
            // Log confirmation of set-up to Google Stackdriver Logging.
            log.Info("Stackdriver Logging with Log4net successfully configured for use.");
            log.Info("Stackdriver Error Reporting enabled: " +
                "https://console.cloud.google.com/errors/");
            // [END logging_and_error_reporting]

            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}