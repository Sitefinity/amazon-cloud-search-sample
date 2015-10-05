using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telerik.Sitefinity.Localization;
using Telerik.Sitefinity.Localization.Data;

namespace AmazonCloudSearch
{
    /// <summary>
    /// Represents string resources for UI labels.
    /// </summary>
    [ObjectInfo("AmazonResources", ResourceClassId = "AmazonResources")]
    public class AmazonResources : Resource
    {
        #region Constructions

        /// <summary>
        /// Initializes a new instance of the <see cref="AmazonResources"/> class with the default <see cref="ResourceDataProvider"/>.
        /// </summary>
        public AmazonResources()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AmazonResources"/> class with the provided <see cref="ResourceDataProvider"/>.
        /// </summary>
        /// <param name="dataProvider"><see cref="ResourceDataProvider"/></param>
        public AmazonResources(ResourceDataProvider dataProvider)
            : base(dataProvider)
        {
        }
        #endregion

        #region Class Description
        /// <summary>
        /// Search and Indexing
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1304:SpecifyCultureInfo", MessageId = "Telerik.Sitefinity.Localization.Resource.get_Item(System.String)"), ResourceEntry("SearchResourcesTitle",
            Value = "Search and Indexes",
            Description = "The title of this class.",
            LastModified = "2009/07/02")]
        public string SearchResourcesTitle
        {
            get
            {
                return this["SearchResourcesTitle"];
            }
        }

        /// <summary>
        /// Search and Indexing
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1304:SpecifyCultureInfo", MessageId = "Telerik.Sitefinity.Localization.Resource.get_Item(System.String)"), ResourceEntry("AmazonResourcesTitlePlural",
            Value = "Search and Indexes",
            Description = "The title plural of this class.",
            LastModified = "2009/07/02")]
        public string AmazonResourcesTitlePlural
        {
            get
            {
                return this["AmazonResourcesTitlePlural"];
            }
        }

        /// <summary>
        /// Contains localizable resources for Search and Indexing service module.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1304:SpecifyCultureInfo", MessageId = "Telerik.Sitefinity.Localization.Resource.get_Item(System.String)"), ResourceEntry("AmazonResourcesDescription",
            Value = "Contains localizable resources for Search and Indexing service module.",
            Description = "The description of this class.",
            LastModified = "2009/07/29")]
        public string AmazonResourcesDescription
        {
            get
            {
                return this["AmazonResourcesDescription"];
            }
        }
        #endregion

        /// <summary>
        /// phrase: Amazon Search
        /// </summary>
        /// <value>Amazon Search</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1304:SpecifyCultureInfo", MessageId = "Telerik.Sitefinity.Localization.Resource.get_Item(System.String)"), ResourceEntry("AmazonSearchServiceTitle",
            Value = "Amazon Search",
            Description = "phrase: Amazon Search",
            LastModified = "2014/11/26")]
        public string AmazonSearchServiceTitle
        {
            get
            {
                return this["AmazonSearchServiceTitle"];
            }
        }
    }
}