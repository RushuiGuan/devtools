using Albatross.CommandLine;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading.Tasks;
using System.Xml;

namespace Albatross.DevTools.Project  {
	[Verb("project set-version", typeof(SetProjectVersion), Description = "Set the version of a Directory.Build.props file")]
	public record class SetProjectVersionOptions {
		[Option("d")]
		public DirectoryInfo Directory { get; set; } = null!;

		[Option("ver")]
		public string Version { get; set; } = string.Empty;
	}
	public class SetProjectVersion : BaseHandler<SetProjectVersionOptions> {
		private readonly ILogger<SetProjectVersion> logger;

		public SetProjectVersion(IOptions<SetProjectVersionOptions> options, ILogger<SetProjectVersion> logger) : base(options) {
			this.logger = logger;
		}
		public override Task<int> InvokeAsync(InvocationContext context) {
			var propsFile = Path.Join(options.Directory.FullName, "Directory.Build.props");
			if (!File.Exists(propsFile)) {
				throw new InvalidOperationException($"Directory.Build.props file is not found at {options.Directory.FullName}");
			} else {
				logger.LogInformation("Setting version for {projectFile}", propsFile);
				var doc = new XmlDocument();
				doc.Load(propsFile);
				SetVersion(doc);
				doc.Save(propsFile);
				return Task.FromResult(0);
			}
			void SetVersion(XmlDocument projectXml) {
				XmlElement? versionNode = projectXml.SelectSingleNode("/Project/PropertyGroup/Version") as XmlElement;
				if (versionNode == null) {
					versionNode = projectXml.CreateElement("Version");
					var propertyGroupNode = projectXml.SelectSingleNode("/Project/PropertyGroup");
					if (propertyGroupNode == null) {
						propertyGroupNode = projectXml.CreateElement("PropertyGroup");
						var projectNode = projectXml.SelectSingleNode("/Project");
						if (projectNode == null) {
							throw new System.Exception("Project node not found");
						} else {
							projectNode.AppendChild(propertyGroupNode);
						}
					} else {
						propertyGroupNode.AppendChild(versionNode);
					}
				}
				versionNode.InnerText = this.options.Version;
			}
		}
	}
}
