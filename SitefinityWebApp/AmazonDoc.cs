namespace SitefinityWebApp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AmazingCloudSearch.Contract;
    using Telerik.Sitefinity.Services.Search.Data;
    using Telerik.Sitefinity.Services.Search.Model;
    using Telerik.Sitefinity.Services.Search.Publishing;

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

            foreach (IField field in doc.Fields)
            {
                if (field.Value == null || field.Name == doc.IdentityField.Name)
                {
                    continue;
                }

                Field convertedField = new Field();
                convertedField.Name = field.Name.ToLower();                
                convertedField.Value = field.Value;
                fields.Add(convertedField);
            }

            this.Fields = fields;
            this.IdentityField = doc.IdentityField;
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