using Albatross.CommandLine;
using Microsoft.Extensions.Options;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading.Tasks;

namespace Albatross.DevTools.Project  {
	[Verb("project property", typeof(ReadProjectProperty), Description = "Reads a property from a cshar project file.")]
	public class ReadProjectPropertyOptions {
		[Option("f")]
		public FileInfo ProjectFile { get; set; } = null!;
		[Option("p")]
		public string Property { get; set; } = string.Empty;
	}
	public class ReadProjectProperty : BaseHandler<ReadProjectPropertyOptions> {
		private readonly CsprojFileService service;

		public ReadProjectProperty(CsprojFileService service, IOptions<ReadProjectPropertyOptions> options) : base(options) {
			this.service = service;
		}
		public override Task<int> InvokeAsync(InvocationContext context) {
			var result = this.service.ReadProperty(options.ProjectFile, options.Property);
			if (!string.IsNullOrEmpty(result)) {
				writer.WriteLine(result);
				return Task.FromResult(0);
			} else {
				return Task.FromResult(1);
			}
		}
	}
}