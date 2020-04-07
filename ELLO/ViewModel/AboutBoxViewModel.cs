using System;
using System.IO;
using System.Reflection;
using System.Windows.Data;
using System.Xml;
using Thorlabs.Elliptec.ELLO.Support;

// namespace: Thorlabs.Elliptec.ELLO.ViewModel
//
// summary:	Provides the UI support classes for the Elliptec application
namespace Thorlabs.Elliptec.ELLO.ViewModel
{
    /// <summary>WPF AboutBox1 Model</summary>
    public sealed class AboutBoxViewModel : ObservableObject
    {
        #region AboutData Provider
        #region Member data
        private XmlDocument _xmlDoc;

        private const string PropertyNameTitle = "Title";
        private const string PropertyNameDescription = "Description";
        private const string PropertyNameProduct = "Product";
        private const string PropertyNameCopyright = "Copyright";
        private const string PropertyNameCompany = "Company";
        private const string XPathRoot = "ApplicationInfo/";
        private const string XPathTitle = XPathRoot + PropertyNameTitle;
        private const string XPathVersion = XPathRoot + "Version";
        private const string XPathDescription = XPathRoot + PropertyNameDescription;
        private const string XPathProduct = XPathRoot + PropertyNameProduct;
        private const string XPathCopyright = XPathRoot + PropertyNameCopyright;
        private const string XPathCompany = XPathRoot + PropertyNameCompany;
        private const string XPathLink = XPathRoot + "Link";
        private const string XPathLinkUri = XPathRoot + "Link/@Uri";

		private string _linkURL;
		private string _linkText;
		#endregion

        #region Properties

    	private object _urlResource;
    	/// <summary>Gets or sets the URL resource.</summary>
    	/// <value>The URL resource.</value>
    	public object URLResource
    	{
    		get { return _urlResource;}
			set
			{
				_urlResource = value;
				LinkUrl = GetLogicalResourceString(XPathLinkUri);
				LinkText = GetLogicalResourceString(XPathLink);
			}
    	}

        /// <summary>
        /// Gets the title property, which is display in the About dialogs window title.
        /// </summary>
        public string ProductTitle
        {
            get
            {
                string result = CalculatePropertyValue<AssemblyTitleAttribute>(PropertyNameTitle, XPathTitle);
                if (string.IsNullOrEmpty(result))
                {
                    // otherwise, just get the name of the assembly itself.
                    result = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
                }
                return result;
            }
        }

        /// <summary>
        /// Gets the application's version information to show.
        /// </summary>
        public string Version
        {
            get
            {
                // first, try to get the version string from the assembly.
                Version version = Assembly.GetExecutingAssembly().GetName().Version;
                return version != null ? version.ToString() : GetLogicalResourceString(XPathVersion);
             }
        }

        /// <summary>
        /// Gets the description about the application.
        /// </summary>
        public string Description
        {
            get { return CalculatePropertyValue<AssemblyDescriptionAttribute>(PropertyNameDescription, XPathDescription); }
        }

        /// <summary>
        ///  Gets the product's full name.
        /// </summary>
        public string Product
        {
            get { return CalculatePropertyValue<AssemblyProductAttribute>(PropertyNameProduct, XPathProduct); }
        }

#if (DEBUG)
		/// <summary> Gets the build configuration. </summary>
		/// <value> The configuration. </value>
		public string Configuration
		{
			get { return (Environment.Is64BitProcess ? " 64 Bit (Debug)" : " 32 Bit (Debug)"); }
		}
#else
		/// <summary> Gets the build configuration. </summary>
		/// <value> The configuration. </value>
		public string Configuration
		{
			get { return (Environment.Is64BitProcess ? " 64 Bit" : " 32 Bit"); }
		}
#endif

		/// <summary>
        /// Gets the copyright information for the product.
        /// </summary>
        public string Copyright
        {
            get { return CalculatePropertyValue<AssemblyCopyrightAttribute>(PropertyNameCopyright, XPathCopyright); }
        }

        /// <summary>
        /// Gets the product's company name.
        /// </summary>
        public string Company
        {
            get { return CalculatePropertyValue<AssemblyCompanyAttribute>(PropertyNameCompany, XPathCompany); }
        }

        /// <summary>
        /// Gets the link text to display in the About dialog.
        /// </summary>
        public string LinkText
        {
			get { return _linkText; }
			set
			{
				_linkText = value;
				RaisePropertyChanged(() => LinkText);
			}
        }

        /// <summary>
        /// Gets the link uri that is the navigation target of the link.
        /// </summary>
        public string LinkUrl
        {
			get { return _linkURL; }
			set
			{
				_linkURL = value;
				RaisePropertyChanged(() => LinkUrl);
			}
		}
        #endregion

        #region Resource location methods
        /// <summary>
        /// Gets the specified property value either from a specific attribute, or from a resource dictionary.
        /// </summary>
        /// <typeparam name="T">Attribute type that we're trying to retrieve.</typeparam>
        /// <param name="propertyName">Property name to use on the attribute.</param>
        /// <param name="xpathQuery">XPath to the element in the XML data resource.</param>
        /// <returns>The resulting string to use for a property.
        /// Returns null if no data could be retrieved.</returns>
        private string CalculatePropertyValue<T>(string propertyName, string xpathQuery)
        {
            string result = string.Empty;
            // first, try to get the property value from an attribute.
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(T), false);
            if (attributes.Length > 0)
            {
                var attrib = (T)attributes[0];
                PropertyInfo property = attrib.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                if (property != null)
                {
                    result = property.GetValue(attributes[0], null) as string;
                }
            }

            // if the attribute wasn't found or it did not have a value, then look in an xml resource.
            if (result == string.Empty)
            {
                // if that fails, try to get it from a resource.
                result = GetLogicalResourceString(xpathQuery);
            }
            return result;
        }

        /// <summary>
        /// Gets the XmlDataProvider's document from the resource dictionary.
        /// </summary>
        private XmlDocument ResourceXmlDocument
        {
            get
            {
                if (_xmlDoc == null)
                {
                    // if we haven't already found the resource XmlDocument, then try to find it.
                    object obj = URLResource;
                    if (obj != null)
                    {
                        var provider = obj as XmlDataProvider;
                        if (provider != null)
                        {
                            _xmlDoc = provider.Document;
                        }
                        
                    }
                }
                return _xmlDoc;
            }
        }

        /// <summary>
        /// Gets the specified data element from the XmlDataProvider in the resource dictionary.
        /// </summary>
        /// <param name="xpathQuery">An XPath query to the XML element to retrieve.</param>
        /// <returns>The resulting string value for the specified XML element. 
        /// Returns empty string if resource element couldn't be found.</returns>
        private string GetLogicalResourceString(string xpathQuery)
        {
            string result = string.Empty;
            // get the About xml information from the resources.
            XmlDocument doc = ResourceXmlDocument;
            if (doc != null)
            {
                // if we found the XmlDocument, then look for the specified data. 
                XmlNode node = doc.SelectSingleNode(xpathQuery);
                if (node != null)
                {
                    if (node is XmlAttribute)
                    {
                        // only an XmlAttribute has a Value set.
                        result = node.Value;
                    }
                    else
                    {
                        // otherwise, need to just return the inner text.
                        result = node.InnerText;
                    }
                }
            }
            return result;
        }
        #endregion
        #endregion
    }
}
