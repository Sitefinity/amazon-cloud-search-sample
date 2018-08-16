using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Telerik.Sitefinity;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Search.Configuration;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Services.Search;
using Telerik.Sitefinity.Services.Search.Configuration;
using Telerik.Sitefinity.Utilities.TypeConverters;

namespace Telerik.Sitefinity.AmazonCloudSearch
{
    public static class AmazonCloudSearchInstaller
    {
        /// <summary>
        /// Called before the application start.
        /// </summary>
        public static void PreApplicationStart()
        {
            Bootstrapper.Initialized += Bootstrapper_Initialized;
        }

        static void Bootstrapper_Initialized(object sender, Telerik.Sitefinity.Data.ExecutedEventArgs e)
        {
            if ((Bootstrapper.IsDataInitialized) && (e.CommandName == "Bootstrapped"))
            {
                if (SystemManager.ApplicationModules.Any(p => p.Key == SearchModule.ModuleName))
                {
                    var typeName = typeof(AmazonSearchService).FullName;
                    App.WorkWith()
                       .Module(SearchModule.ModuleName)
                       .Initialize()
                       .Localization<AmazonResources>();

                    AddAmazonService(typeName);
                    SetProperties();
                }
            }
        }

        private static void UpdateParameter(NameValueCollection parameters, string key, string value, ref bool updated)
        {
            if (parameters.Keys.Contains(key)) return;

            parameters.Add(key, value);
            updated = true;
        }

        private static void SetProperties()
        {
            bool updated = false;
            var manager = ConfigManager.GetManager();
            var searchConfig = manager.GetSection<SearchConfig>();

            var amazonSearchParameters = searchConfig.SearchServices[AmazonSearchService.ServiceName].Parameters;

            UpdateParameter(amazonSearchParameters, AmazonSearchService.AccessKey, string.Empty, ref updated);
            UpdateParameter(amazonSearchParameters, AmazonSearchService.ApiVersion, "2013-01-01", ref updated);
            UpdateParameter(amazonSearchParameters, AmazonSearchService.SearchEndPoint, string.Empty, ref updated);
            UpdateParameter(amazonSearchParameters, AmazonSearchService.DocumentEndPoint, string.Empty, ref updated);
            UpdateParameter(amazonSearchParameters, AmazonSearchService.SecretAccessKey, string.Empty, ref updated);
            UpdateParameter(amazonSearchParameters, AmazonSearchService.Region, string.Empty, ref updated);

            if (updated)
            {
                using (ElevatedConfigModeRegion config = new ElevatedConfigModeRegion())
                {
                    manager.SaveSection(searchConfig);
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private static void AddAmazonService(string typeName)
        {
            ConfigManager manager = ConfigManager.GetManager();
            var searchConfig = manager.GetSection<SearchConfig>();

            if (!searchConfig.SearchServices.ContainsKey(AmazonSearchService.ServiceName))
            {
                searchConfig.SearchServices.Add(new SearchServiceSettings(searchConfig.SearchServices)
                {
                    Name = AmazonSearchService.ServiceName,
                    Title = "AmazonSearchServiceTitle",
                    TypeName = typeName,
                    ResourceClassId = "AmazonResources"
                });

                using (ElevatedConfigModeRegion config = new ElevatedConfigModeRegion())
                {
                    manager.SaveSection(searchConfig);
                }
            }
        }
    }
}