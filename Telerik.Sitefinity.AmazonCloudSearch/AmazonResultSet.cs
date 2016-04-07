using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using Amazon.CloudSearchDomain.Model;
using Telerik.Sitefinity.Publishing;
using Telerik.Sitefinity.Services.Search.Data;
using Telerik.Sitefinity.Services.Search.Model;
using Telerik.Sitefinity.Services.Search.Publishing;

namespace Telerik.Sitefinity.AmazonCloudSearch
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix"), EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
    public class AmazonResultSet : IResultSet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AmazonResultSet" /> class.
        /// </summary>
        /// <param name="result">The result that holds the data.</param>
        /// <param name="suggestions">The suggestions.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        public AmazonResultSet(SearchResult result, List<string> suggestions)
        {
            if (result == null)
                throw new ArgumentNullException("result");

            this.count = result.Hits.Found;
            this.response = result.Hits;

            this.suggestions = suggestions;
        }

        /// <summary>
        /// Gets a value indicating how many results were found.
        /// </summary>
        public int HitCount
        {
            get
            {
                return (int)this.count;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>IEnumerator<IDocument></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public IEnumerator<IDocument> GetEnumerator()
        {
            return this.response.Hit.Select(h => this.GetDocumentWithAdditionalFields(h)).Cast<IDocument>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<IDocument>)this).GetEnumerator();
        }

        private IDocument GetDocumentWithAdditionalFields(Hit hit)
        {
            List<IField> listOfFields = new List<IField>();

            var summaryField = new Field();
            summaryField.Name = PublishingConstants.FieldSummary;
            summaryField.Value = string.Empty;
            listOfFields.Add(summaryField);

            var highlight = GetHighlights(hit);
            listOfFields.Add(new Field() { Name = PublishingConstants.HighLighterResult, Value = highlight });
            listOfFields.Add(new Field() { Name = PublishingConstants.SuggestionsField, Value = this.suggestions });

            foreach (var field in hit.Fields)
            {
                if (field.Key != null)
                {
                    var currentField = new Field();
                    currentField.Name = field.Key;
                    currentField.Value = field.Value.FirstOrDefault();
                    listOfFields.Add(currentField);
                }
            }

            Document doc = new Document(listOfFields, null);
            return doc;
        }

        private static string GetHighlights(Hit hit)
        {
            var text = string.Empty;
            foreach (var field in hit.Highlights.Keys)
            {
                if (!string.IsNullOrEmpty(text))
                    text += SearchResultsSeparator;

                var value = hit.Highlights[field];
                text += value;
            }

            return text;
        }

        private readonly Hits response;
        private readonly long count;
        private readonly List<string> suggestions;
        private const string SearchResultsSeparator = ", ";
    }
}