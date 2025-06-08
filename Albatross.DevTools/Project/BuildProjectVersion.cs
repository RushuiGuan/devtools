using Albatross.CommandLine;
using Albatross.SemVer;
using LibGit2Sharp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Albatross.DevTools.Project  {
	[Verb("project version", typeof(BuildProjectVersion), Description = "Create a new version based on the current version and the input parameters.  The current version file will not be modified.  The resulting version will always use a 7 digit git hash as the version metadata.")]
	public record class BuildProjectVersionOptions {
		[Option("d", Description = "Specify the directory of the version file")]
		public DirectoryInfo Directory { get; set; } = null!;
		
		[Option("p", Description ="If true, create a release version.  Otherwise create a prerelease version.")]
		public bool Prod { get; set; }

		[Option(Description = "Use Directory.Build.props file instead of .version file")]
		public bool DirectoryBuildProps { get; set; }

		[Option("label", Description = "The label of the prerelease version, e.g. 'alpha', 'beta', 'rc'.  If not specified, format [commit_count].[branch] would be used.")]
		public string? PrereleaseLabel { get; set; }
	}
	public class BuildProjectVersion : BaseHandler<BuildProjectVersionOptions> {
		private readonly CsprojFileService csprojFileService;
		private readonly ILogger<BuildProjectVersion> logger;

		public BuildProjectVersion(CsprojFileService csprojFileService, IOptions<BuildProjectVersionOptions> options, ILogger<BuildProjectVersion> logger) : base(options) {
			this.csprojFileService = csprojFileService;
			this.logger = logger;
		}

		private string ReadFromDotVersionFile() {
			var versionFile = Path.Join(this.options.Directory.FullName, ".version");
			if (File.Exists(versionFile)) {
				var text = File.ReadAllText(versionFile);
				return text.Trim();
			} else {
				throw new Exception($".version file is not found at {this.options.Directory.FullName}");
			}
		}

		private string ReadFromDirectoryBuildPropsFile() {
			var buildPropsFile = Path.Join(this.options.Directory.FullName, "Directory.Build.props");
			var fileInfo = new FileInfo(buildPropsFile);
			if (fileInfo.Exists) {
				return csprojFileService.ReadRequiredVersion(fileInfo);
			} else {
				throw new Exception($"Directory.Build.props file is not found at {this.options.Directory.FullName}");
			}
		}
		public override Task<int> InvokeAsync(InvocationContext context) {
			string versionText;
			if(options.DirectoryBuildProps) {
				versionText = ReadFromDirectoryBuildPropsFile();
			} else {
				versionText = ReadFromDotVersionFile();
			}
			var gitDirectory = Repository.Discover(options.Directory.FullName);
			if (gitDirectory != null) {
				using var repo = new Repository(gitDirectory);
				string[] prerelease;
				if(string.IsNullOrEmpty(this.options.PrereleaseLabel)) {
					prerelease = new[] { repo.Commits.Count().ToString(), repo.Head.FriendlyName, };
				} else {
					prerelease = new[] { this.options.PrereleaseLabel };
				}
				var hash = repo.Head.Tip.Sha.Substring(0, 7);
				SematicVersion semver;
				if (options.Prod) {
					semver = new SematicVersion(versionText) {
						Metadata = [
							hash
						]
					};
				} else {
					semver = new SematicVersion(versionText) {
						PreRelease = prerelease,
						Metadata = [
							hash
						]
					};
				}
				this.writer.WriteLine(semver.ToString());
				return Task.FromResult(0);
			} else {
				logger.LogError("git directory not found");
				return Task.FromResult(1);
			}
		}
	}
}
