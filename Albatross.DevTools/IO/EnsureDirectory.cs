using Albatross.CommandLine;
using Microsoft.Extensions.Options;
using System;
using System.CommandLine.Invocation;
using System.IO;

namespace Albatross.DevTools.IO {
	[Verb("io ensure-directory", typeof(EnsureDirectory), Alias = ["ensure"], Description = "Ensures that a directory exists, creating it if necessary.")]
	public class EnsureDirectoryOptions {
		[Argument(ArityMin = 1)]
		public string[] Segments { get; set; } = [];

		[Option("--file", "f", Description = "If true, the last segment is treated as a file, and the directory is created for that file.")]
		public bool IsFile { get; set; }
	}

	public class EnsureDirectory : BaseHandler<EnsureDirectoryOptions> {
		public EnsureDirectory(IOptions<EnsureDirectoryOptions> options) : base(options) {
		}

		public override int Invoke(InvocationContext context) {
			var path = System.IO.Path.Join(this.options.Segments);
			DirectoryInfo directory;
			if (options.IsFile) {
				var fileInfo = new FileInfo(path);
				directory = fileInfo.Directory ?? throw new ArgumentException("The specified path does not contain a valid directory for the file.");
			} else {
				directory = new DirectoryInfo(path);
			}

			if (!directory.Exists) {
				directory.Create();
			}

			this.writer.WriteLine(directory.FullName);
			return 0;
		}
	}
}