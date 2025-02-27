using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Albatross.DevTools {
	public class CsprojFileService {
		public string? ReadProperty(FileInfo file, string property) {
			var doc = new System.Xml.XmlDocument();
			doc.Load(file.FullName);
			var result = doc.SelectSingleNode($"/Project/PropertyGroup/{property}")?.InnerText;
			return result;
		}
		public string ReadRequiredVersion(FileInfo file) {
			var version = ReadProperty(file, "Version");
			if (string.IsNullOrEmpty(version)) {
				throw new InvalidOperationException($"Version is not found in {file.FullName}");
			}
			return version.Trim();
		}
	}
}
