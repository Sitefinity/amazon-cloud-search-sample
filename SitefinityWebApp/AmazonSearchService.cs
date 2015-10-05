using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.Text;
using AmazingCloudSearch;
using Amazon;
using Amazon.CloudSearch;
using Amazon.CloudSearch.Model;
using Amazon.CloudSearchDomain;
using Amazon.CloudSearchDomain.Model;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Services.Search;
using Telerik.Sitefinity.Services.Search.Data;
using Telerik.Sitefinity.Services.Search.Web.UI.Public;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Services.Search.Configuration;

namespace SitefinityWebApp
{
    public class AmazonSearchService : ISearchService
    {
        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        public void CreateIndex(string name, IEnumerable<IFieldDefinition> fieldDefinitions)
        {
            ConfigManager manager = ConfigManager.GetManager();
            var searchConfig = manager.GetSection<SearchConfig>();

            var amazonSearchParameters = searchConfig.SearchServices[AmazonSearchService.ServiceName].Parameters;
            //You must add here your accessKey and SecretAccessKey. See here how to get them: http://docs.aws.amazon.com/AWSSimpleQueueService/latest/SQSGettingStartedGuide/AWSCredentials.html
            using (IAmazonCloudSearch cloudSearchClient = AWSClientFactory.CreateAmazonCloudSearchClient(amazonSearchParameters[AccessKey], amazonSearchParameters[SecretAccessKey], RegionEndpoint.EUWest1))
            {
                try
                {
                    
                    var domainNames = cloudSearchClient.ListDomainNames();
                    if(!domainNames.DomainNames.ContainsKey(name))
                    {
                        CreateDomainRequest domainRequest = new CreateDomainRequest();
                        domainRequest.DomainName = name;
                        cloudSearchClient.CreateDomain(domainRequest);
                    }

                    if (fieldDefinitions == null)
                        throw new ArgumentNullException("fieldDefinitions");

                    foreach (var fieldDefinition in fieldDefinitions)
                    {
                        DefineIndexFieldRequest request = new DefineIndexFieldRequest();
                        request.DomainName = name;
                        request.IndexField = new IndexField();
                        request.IndexField.IndexFieldName = fieldDefinition.Name.ToLowerInvariant();
                        if (fieldDefinition.Type == null || fieldDefinition.Type == typeof(string))
                            request.IndexField.IndexFieldType = IndexFieldType.Text;
                        if (fieldDefinition.Type == typeof(string[]))
                            request.IndexField.IndexFieldType = IndexFieldType.TextArray;
                        if (fieldDefinition.Type == typeof(int))
                            request.IndexField.IndexFieldType = IndexFieldType.Int;
                        if (fieldDefinition.Type == typeof(DateTime))
                            request.IndexField.IndexFieldType = IndexFieldType.Date;
                        cloudSearchClient.DefineIndexField(request);
                    }

                    SearchResults searchResults = new SearchResults();
                    foreach (var field in searchResults.HighlightedFields)
                    {
                        Suggester suggester = new Suggester();
                        DocumentSuggesterOptions suggesterOptions = new DocumentSuggesterOptions();
                        suggesterOptions.FuzzyMatching = SuggesterFuzzyMatching.None;
                        suggesterOptions.SourceField = field.ToLowerInvariant();
                        suggester.DocumentSuggesterOptions = suggesterOptions;
                        suggester.SuggesterName = field.ToLowerInvariant() + "_suggester";
                        DefineSuggesterRequest defineRequest = new DefineSuggesterRequest();
                        defineRequest.DomainName = name;
                        defineRequest.Suggester = suggester;
                        cloudSearchClient.DefineSuggester(defineRequest);
                    }

                    searchResults.Dispose();

                    IndexDocumentsRequest documentRequest = new IndexDocumentsRequest();
                    documentRequest.DomainName = name;
                    cloudSearchClient.IndexDocuments(documentRequest);
                }
                catch (BaseException ex)
                {
                    Log.Write(ex.InnerException.Message);
                }
                catch (LimitExceededException ex)
                {
                    Log.Write(ex.InnerException.Message);
                }
                catch (InternalException ex)
                {
                    Log.Write(ex.InnerException.Message);
                }
            }
        }

        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        public void DeleteIndex(string name)
        {
            ConfigManager manager = ConfigManager.GetManager();
            var searchConfig = manager.GetSection<SearchConfig>();

            var amazonSearchParameters = searchConfig.SearchServices[AmazonSearchService.ServiceName].Parameters;
            using (IAmazonCloudSearch cloudSearchClient = AWSClientFactory.CreateAmazonCloudSearchClient(amazonSearchParameters[AccessKey], amazonSearchParameters[SecretAccessKey], RegionEndpoint.EUWest1))
            {
                DeleteDomainRequest domainRequest = new DeleteDomainRequest();
                domainRequest.DomainName = name;
                try
                {
                    cloudSearchClient.DeleteDomain(domainRequest);
                }
                catch (BaseException ex)
                {
                    Log.Write(ex.InnerException.Message);
                }
                catch (InternalException ex)
                {
                    Log.Write(ex.InnerException.Message);
                }
            }
        }

        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        public bool IndexExists(string indexName)
        {
            ConfigManager manager = ConfigManager.GetManager();
            var searchConfig = manager.GetSection<SearchConfig>();

            var amazonSearchParameters = searchConfig.SearchServices[AmazonSearchService.ServiceName].Parameters;
            using (IAmazonCloudSearch cloudSearchClient = AWSClientFactory.CreateAmazonCloudSearchClient(amazonSearchParameters[AccessKey], amazonSearchParameters[SecretAccessKey], RegionEndpoint.EUWest1))
            {
                bool exists = false;
                try
                {
                    ListDomainNamesResponse response = cloudSearchClient.ListDomainNames();
                    var index = response.DomainNames.Where(dn => dn.Key == indexName).First();
                    exists = index.Value != null;
                }
                catch (BaseException ex)
                {
                    Log.Write(ex.InnerException.Message);
                }
                catch (ArgumentNullException ex)
                {
                    Log.Write(ex.InnerException.Message);
                }

                return exists;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public void RemoveDocument(string indexName, IField identityField)
        {
        }

        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        public void RemoveDocuments(string indexName, IEnumerable<IDocument> documents)
        {
            CloudSearch<AmazonDoc> cloudSearch = new CloudSearch<AmazonDoc>("index2-cdduimbipgk3rpnfgny6posyzy.eu-west-1.cloudsearch.amazonaws.com", "2013-01-01");
            cloudSearch.Delete(documents.Select(d => new AmazonDoc(d)).ToList());
        }

        private string BuildQueryFilter(ISearchFilter filter)
        {
            var result = new StringBuilder();
            result.Append("(");
            var groupOperator = filter.Operator == QueryOperator.And ? " and " : " or ";
            result.Append(groupOperator);

            foreach (var clause in filter.Clauses)
            {
                var value = clause.Value;
                if (value is DateTime)
                {
                    var dateTime = (DateTime)clause.Value;

                    value = dateTime.ToUniversalTime().ToString("o", CultureInfo.InvariantCulture);
                }
                else if (value is int)
                {
                    value = value.ToString();
                }
                else if (value is string)
                {
                    // Amazon search does not accept null values
                    if (value == "nullvalue")
                        continue;

                    // String values should be surrounded by quotation marks
                    value = string.Format("'{0}'", value);
                }

                switch (clause.FilterOperator)
                {
                    case FilterOperator.Equals: result.Append(string.Format("{0}:{1} ", clause.Field.ToUpperInvariant(), value));
                        break;
                    case FilterOperator.Contains: result.Append(string.Format("{0}:{1}", clause.Field.ToUpperInvariant(), value));
                        break;
                    case FilterOperator.Greater: result.Append(string.Format("{0}:[{1}, {2}", clause.Field, value, "}"));
                        break;
                    case FilterOperator.Less: result.Append(string.Format("{0}:{2}, {1}]", clause.Field, value, "{"));
                        break;
                }
            }

            var groupResult = new StringBuilder();

            foreach (var filterGroup in filter.Groups)
            {
                groupResult.Append(this.BuildQueryFilter(filterGroup));
            }

            result.Append(groupResult.ToString());
            result.Append(")");
            return result.ToString();
        }

        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        public IResultSet Search(ISearchQuery query)
        {
            AmazonCloudSearchDomainConfig config = new AmazonCloudSearchDomainConfig();
            config.ServiceURL = "http://search-index2-cdduimbipgk3rpnfgny6posyzy.eu-west-1.cloudsearch.amazonaws.com/";
            ConfigManager manager = ConfigManager.GetManager();
            var searchConfig = manager.GetSection<SearchConfig>();

            var amazonSearchParameters = searchConfig.SearchServices[AmazonSearchService.ServiceName].Parameters;            
            AmazonCloudSearchDomainClient domainClient = new AmazonCloudSearchDomainClient(amazonSearchParameters[AccessKey], amazonSearchParameters[SecretAccessKey], config);
            SearchRequest searchRequest = new SearchRequest();
            List<string> suggestions = new List<string>();
            StringBuilder highlights = new StringBuilder();
            highlights.Append("{\'");

            if (query == null)
                throw new ArgumentNullException("query");

            foreach (var field in query.HighlightedFields)
            {
                if (highlights.Length > 2)
                {
                    highlights.Append(", \'");
                }

                highlights.Append(field.ToUpperInvariant());
                highlights.Append("\':{} ");
                SuggestRequest suggestRequest = new SuggestRequest();
                Suggester suggester = new Suggester();
                suggester.SuggesterName = field.ToUpperInvariant() + "_suggester";
                suggestRequest.Suggester = suggester.SuggesterName;
                suggestRequest.Size = query.Take;
                suggestRequest.Query = query.Text;
                SuggestResponse suggestion = domainClient.Suggest(suggestRequest);
                foreach (var suggest in suggestion.Suggest.Suggestions)
                {
                    suggestions.Add(suggest.Suggestion);
                }
            }

            highlights.Append("}");

            if (query.Filter != null)
            {
                searchRequest.FilterQuery = this.BuildQueryFilter(query.Filter);
            }

            if (query.OrderBy != null)
            {
                searchRequest.Sort = string.Join(",", query.OrderBy);
            }

            if (query.Take > 0)
            {
                searchRequest.Size = query.Take;
            }

            if (query.Skip > 0)
            {
                searchRequest.Start = query.Skip;
            }

            searchRequest.Highlight = highlights.ToString();
            searchRequest.Query = query.Text;
            searchRequest.QueryParser = QueryParser.Simple;
            var result = domainClient.Search(searchRequest).SearchResult;
            
            return new AmazonResultSet(result, suggestions);
        }

        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        public void UpdateIndex(string name, IEnumerable<IDocument> documents)
        {
            CloudSearch<AmazonDoc> cloudSearch = new CloudSearch<AmazonDoc>("index2-cdduimbipgk3rpnfgny6posyzy.eu-west-1.cloudsearch.amazonaws.com", "2013-01-01");
            cloudSearch.Update(documents.Select(d => new AmazonDoc(d)).ToList());
        }

        public const string ServiceName = "AmazonSearchService";
        public const string AccessKey = "AccessKey";
        public const string SecretAccessKey = "SecretAccessKey";
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Api")]
        public const string ApiVersion = "ApiVersion";
        public const string DocumentEndPoint = "DocumentEndPoint";
        public const string SearchEndPoint = "SearchEndPoint";

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Aws")]
        public const string AwsAccessKey = "AwsAccessKey";
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Aws")]
        public readonly string AwsSecretAccessKey = string.Empty;
    }
}