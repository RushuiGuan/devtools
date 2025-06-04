using Albatross.CommandLine;

namespace Albatross.DevTools {
	[Verb("io", Description = "Command related to file and directory operations.")]
	[Verb("version", Alias = ["ver"], Description = "Commands related to semver version manupulation.")]
	[Verb("project", Description = "Commands related to project file manipulation.")]
	[Verb("xml", Description = "Commands related to XML file manipulation.")]
	[Verb("git", Description = "Commands related to Git operations.")]
	[Verb("azure-devops", Description = "Commands related to Azure DevOps operations.")]
	public class ParentOptions {
	}
}