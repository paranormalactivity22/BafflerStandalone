using System;
using System.Collections.ObjectModel;
using System.Xml;
using BafflerStandalone.Database.RSS;

[Serializable]
public class RssManager : IDisposable
{
	private string _url;
	private string _feedTitle;
	private string _feedDescription;
	private string _feedComments;
	private Collection<Rss.Items> _rssItems = new Collection<Rss.Items>();
	private bool _IsDisposed;
	
	public RssManager()
	{
		this._url = string.Empty;
	}

	public RssManager(string feedUrl)
	{
		this._url = feedUrl;
	}
	
	public string Url { get => this._url; set => this._url = value; }
	public Collection<Rss.Items> RssItems { get => this._rssItems; }
	public string FeedTitle { get => this._feedTitle;}
	public string FeedDescription { get => this._feedDescription;}

	public Collection<Rss.Items> GetFeed()
	{
		if (string.IsNullOrEmpty(this.Url))
		{
			throw new ArgumentException("You must provide a feed URL");
		}
		Collection<Rss.Items> rssItems;
		using (XmlReader xmlReader = XmlReader.Create(this.Url))
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(xmlReader);
			XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);
			xmlNamespaceManager.AddNamespace("slash", "http://purl.org/rss/1.0/modules/slash/");
			this.ParseDocElements(xmlDocument.SelectSingleNode("//channel"), "title", ref this._feedTitle);
			this.ParseDocElements(xmlDocument.SelectSingleNode("//channel"), "description", ref this._feedDescription);
			this.ParseDocElements(xmlDocument.SelectSingleNode("//channel"), "slash:comments", xmlNamespaceManager, ref this._feedComments);
			this.ParseRssItems(xmlDocument, xmlNamespaceManager);
			rssItems = this._rssItems;
		}
		return rssItems;
	}

	private void ParseRssItems(XmlDocument xmlDoc, XmlNamespaceManager NameSpaceManager)
	{
		this._rssItems.Clear();
		XmlNodeList xmlNodeList = xmlDoc.SelectNodes("rss/channel/item");
		foreach (object obj in xmlNodeList)
		{
			XmlNode parent = (XmlNode)obj;
			Rss.Items item = default(Rss.Items);
			this.ParseDocElements(parent, "title", ref item.Title);
			this.ParseDocElements(parent, "description", ref item.Description);
			this.ParseDocElements(parent, "link", ref item.Link);
			this.ParseDocElements(parent, "slash:comments", NameSpaceManager, ref item.Comments);
			string s = null;
			this.ParseDocElements(parent, "pubDate", ref s);
			DateTime.TryParse(s, out item.Date);
			this._rssItems.Add(item);
		}
	}

	private void ParseDocElements(XmlNode parent, string xPath, ref string property)
	{
		XmlNode xmlNode = parent.SelectSingleNode(xPath);
		if (xmlNode != null)
		{
			property = xmlNode.InnerText;
			return;
		}
		property = "Unresolvable";
	}

	private void ParseDocElements(XmlNode parent, string xPath, XmlNamespaceManager NameSpace, ref string property)
	{
		XmlNode xmlNode = parent.SelectSingleNode(xPath, NameSpace);
		if (xmlNode != null)
		{
			property = xmlNode.InnerText;
			return;
		}
		property = "Unresolvable";
	}

	private void Dispose(bool disposing)
	{
		if (disposing && !this._IsDisposed)
		{
			this._rssItems.Clear();
			this._url = null;
			this._feedTitle = null;
			this._feedDescription = null;
		}
		this._IsDisposed = true;
	}

	public void Dispose()
	{
		this.Dispose(true);
		GC.SuppressFinalize(this);
	}
}
