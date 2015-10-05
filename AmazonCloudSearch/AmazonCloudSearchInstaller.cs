using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Sitefinity;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Search.Configuration;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Services.Search;
using Telerik.Sitefinity.Services.Search.Configuration;
using Telerik.Sitefinity.Utilities.TypeConverters;

namespace AmazonCloudSearch
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
                var typeName = typeof(AmazonSearchService).FullName;
                App.WorkWith()
                   .Module(SearchModule.ModuleName)
                   .Initialize()
                   .Localization<AmazonResources>();

                AddAmazonService(typeName);
                RegisterAmazonService(typeName);

                SetProperties();
            }
        }

             private static void SetProperties()
        {
            var manager = ConfigManager.GetManager();
            var searchConfig = manager.GetSection<SearchConfig>();
            
            var amazonSearchParameters = searchConfig.SearchServices[AmazonSearchService.ServiceName].Parameters;
            
            amazonSearchParameters.Add(AmazonSearchService.AccessKey, string.Empty);
            amazonSearchParameters.Add(AmazonSearchService.ApiVersion, string.Empty);
            amazonSearchParameters.Add(AmazonSearchService.DocumentEndPoint, string.Empty);
            amazonSearchParameters.Add(AmazonSearchService.SearchEndPoint, string.Empty);
            amazonSearchParameters.Add(AmazonSearchService.SecretAccessKey, string.Empty);

            using (ElevatedConfigModeRegion config = new ElevatedConfigModeRegion())
            {
                manager.SaveSection(searchConfig);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private static void RegisterAmazonService(string typeName)
        {
            try
            {
                var serviceType = TypeResolutionService.ResolveType(typeName, false);
                if (serviceType != null)
                {
                    var service = Activator.CreateInstance(serviceType);
                    if (service != null)
                    {
                        ServiceBus.UnregisterService<ISearchService>();
                        ServiceBus.RegisterService<ISearchService>(service);
                    }
                }
            }
            catch (Exception ex)  
            {
                Log.Write(ex.InnerException.Message);
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