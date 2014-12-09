﻿using System.Xml.Serialization;

namespace VirtoCommerce.Framework.Web.Modularity
{
	[XmlType("module")]
	public class ModuleManifest
	{
		[XmlElement("id")]
		public string Id { get; set; }

		[XmlElement("version")]
		public string Version { get; set; }

		[XmlElement("title")]
		public string Title { get; set; }

		[XmlElement("description")]
		public string Description { get; set; }

		[XmlArray("authors")]
		[XmlArrayItem("author")]
		public string[] Authors { get; set; }

		[XmlArray("owners")]
		[XmlArrayItem("owner")]
		public string[] Owners { get; set; }

		[XmlElement("licenseUrl")]
		public string LicenseUrl { get; set; }

		[XmlElement("projectUrl")]
		public string ProjectUrl { get; set; }

		[XmlElement("iconUrl")]
		public string IconUrl { get; set; }

		[XmlElement("requireLicenseAcceptance")]
		public bool RequireLicenseAcceptance { get; set; }

		[XmlElement("releaseNotes")]
		public string ReleaseNotes { get; set; }

		[XmlElement("copyright")]
		public string Copyright { get; set; }

		[XmlElement("tags")]
		public string Tags { get; set; }

		[XmlElement("assemblyFile")]
		public string AssemblyFile { get; set; }

		[XmlElement("moduleType")]
		public string ModuleType { get; set; }

		[XmlArray("dependencies")]
		[XmlArrayItem("dependency")]
		public string[] Dependencies { get; set; }

		[XmlArray("styles")]
		[XmlArrayItem(typeof(ManifestBundleFile))]
		[XmlArrayItem(typeof(ManifestBundleDirectory))]
		public ManifestBundleItem[] Styles { get; set; }

		[XmlArray("scripts")]
		[XmlArrayItem(typeof(ManifestBundleFile))]
		[XmlArrayItem(typeof(ManifestBundleDirectory))]
		public ManifestBundleItem[] Scripts { get; set; }
	}
}
