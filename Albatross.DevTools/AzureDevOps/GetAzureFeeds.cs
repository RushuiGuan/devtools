﻿using Albatross.CommandLine;
using Albatross.Text;
using AzureDevOpsProxy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.CommandLine.Invocation;
using System.Threading.Tasks;

namespace Albatross.DevTools.AzureDevOps {
	[Verb("azure-devops feeds", typeof(GetAzureFeeds), Description = "Show the Azure DevOps feeds in the specified project or for the whole organization.")]
	public record GetAzureFeedsOptions {
		public string? Project { get; set; }
	}
	public class GetAzureFeeds : BaseHandler<GetAzureFeedsOptions> {
		private readonly FeedManagementProxy proxy;

		public GetAzureFeeds(FeedManagementProxy proxy, IOptions<GetAzureFeedsOptions> options) : base(options) {
			this.proxy = proxy;
		}
		public override async Task<int> InvokeAsync(InvocationContext context) {
			var feeds = await proxy.GetFeeds(options.Project);
			await writer.PrintTable(feeds, new PrintOptionBuilder<PrintTableOption>().Property("Id", "Name").Build());
			// this.writer.WriteLine(feeds);
			return 0;
		}
	}
}
