﻿// <copyright file="PlanGridClient.cs" company="PlanGrid, Inc.">
//     Copyright (c) 2016 PlanGrid, Inc. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PlanGrid.Api.JsonConverters;
using Refit;

namespace PlanGrid.Api
{
    public class PlanGridClient
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="apiKey">The API key provided by PlanGrid.</param>
        /// <param name="baseUrl">The base URL -- you should not have to change this.</param>
        /// <param name="version">The version of the API you want to use.</param>
        /// <param name="timeout">The maximum time that may elapse when making a call before timing out.</param>
        /// <param name="maxRetries">The maximum number of attempts to make contacting the server in the event of a 503 service unavailable repsonse.  This defaults to unlimited retries, with a delay between each attempt that multiplies by two.</param>
        /// <returns>An interface upon which you can invoke the various methods exposed via the Public PlanGrid API.</returns>
        public static IPlanGridApi Create(string apiKey = null, string baseUrl = null, string version = "1", int timeout = 60, int? maxRetries = null)
        {
            apiKey = apiKey ?? Settings.ApiKey ?? Environment.GetEnvironmentVariable("PLANGRIDAPIKEY");
            if (apiKey == null)
            {
                throw new ArgumentException("An ApiKey is required. Either pass it in to this method, add it to your App.config file, or set the environment variable \"PLANGRIDAPIKEY\".", nameof(apiKey));
            }
            baseUrl = baseUrl ?? Settings.ApiBaseUrl ?? Environment.GetEnvironmentVariable("PLANGRIDAPIURL") ?? "https://io.plangrid.com";

            string url = baseUrl;
            var settings = new RefitSettings
            {
                UrlParameterFormatter = new PlanGridUrlParameterFormatter(),
                JsonSerializerSettings = new JsonSerializerSettings
                {
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                    Converters = new List<JsonConverter>(new JsonConverter[]
                    {
                        new StringEnumConverter(),
                        new DateConverter()
                    })
                }
            };
            var api = (AutoGeneratedIPlanGridApi)RestService.For<IPlanGridApi>(
                new HttpClient(new PlanGridHttpHandler(apiKey, settings, version, maxRetries))
                {
                    BaseAddress = new Uri(url), Timeout = TimeSpan.FromSeconds(timeout)
                },
                settings);
            api.Initialize(apiKey, settings, version, maxRetries);
            return api;
        }
    }
}