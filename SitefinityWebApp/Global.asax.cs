using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Telerik.Sitefinity;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Samples.Common;
using Telerik.Sitefinity.Search.Configuration;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Services.Search;
using Telerik.Sitefinity.Services.Search.Configuration;
using Telerik.Sitefinity.Utilities.TypeConverters;

namespace SitefinityWebApp
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            SystemManager.ApplicationStart += this.SystemManager_ApplicationStart;
        }

        private void SystemManager_ApplicationStart(object sender, EventArgs e)
        {
            SampleUtilities.CreateUsersAndRoles();  
   
            var typeName = typeof(AmazonSearchService).FullName;
            App.WorkWith()
              .Module(SearchModule.ModuleName)
              .Initialize()
              .Localization<AmazonResources>();

            AddAmazonService(typeName);
            RegisterAmazonService(typeName);
            
            SetProperties();
        }       

        private static void SetProperties()
        {
            ConfigManager manager = ConfigManager.GetManager();
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

        protected void Session_Start(object sender, EventArgs e)
        {
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
        }

        protected void Application_Error(object sender, EventArgs e)
        {
        }

        protected void Session_End(object sender, EventArgs e)
        {
        }

        protected void Application_End(object sender, EventArgs e)
        {
        }
    }
}