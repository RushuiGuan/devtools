using Albatross.CommandLine;
using Microsoft.Extensions.Options;
using System.CommandLine.Invocation;
using System.IO;

namespace Albatross.DevTools.IO {
	[Verb("io ensure-directory", typeof(EnsureDirectory), Alias = ["ensure"], Description = "Ensures that a directory exists, creating it if necessary.")]
	public class EnsureDirectoryOptions {
		[Argument(ArityMin = 1)]
		public string[] Segments { get; set; } = [];
	}

	public class EnsureDirectory : BaseHandler<EnsureDirectoryOptions> {
		public EnsureDirectory(IOptions<EnsureDirectoryOptions> options) : base(options) {
		}

		public override int Invoke(InvocationContext context) {
			var path = System.IO.Path.Join(this.options.Segments);
			var directory = new DirectoryInfo(path);
			if (!directory.Exists) {
				directory.Create();
			}
			this.writer.WriteLine(directory.FullName);
			return 0;
		}
	}
}