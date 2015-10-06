using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Permissions;
using AmazingCloudSearch.Contract;
using Telerik.Sitefinity.Services.Search.Data;
using Telerik.Sitefinity.Services.Search.Model;
using Telerik.Sitefinity.Services.Search.Publishing;

namespace Telerik.Sitefinity.AmazonCloudSearch
{
    [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
    public class AmazonDoc : Document, ICloudSearchDocument
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AmazonDoc"/> class.
        /// </summary>
        public AmazonDoc()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AmazonDoc"/> class.
        /// </summary>
        /// <param name="doc">Document that contains the fields.</param>
        public AmazonDoc(IDocument doc)
        {
            List<Field> fields = new List<Field>();

            if (doc == null)
                throw new ArgumentNullException("Document is not valid.");

            foreach (IField field in doc.Fields)
            {
                if (field.Value == null || field.Name == doc.IdentityField.Name)
                {
                    continue;
                }

                Field convertedField = new Field();
                convertedField.Name = field.Name.ToLower(CultureInfo.CurrentCulture);
                convertedField.Value = field.Value;
                fields.Add(convertedField);
            }

            this.Fields = fields;
        }

        /// <summary>
        /// Gets or sets a value indicating the identity field.
        /// </summary>
        public string Id
        {
            get
            {
                return this.IdentityField.Value.ToString();
            }

            set
            {
                this.IdentityField.Value = value;
            }
        }
    }
}