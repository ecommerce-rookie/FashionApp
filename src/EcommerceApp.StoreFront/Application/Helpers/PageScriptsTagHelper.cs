using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace StoreFront.Application.Helpers
{
    [HtmlTargetElement("page-scripts")]
    public class PageScriptsTagHelper : TagHelper
    {
        private readonly IFileVersionProvider _fileVersionProvider;
        private readonly IUrlHelperFactory _urlHelperFactory;

        public PageScriptsTagHelper(
            IFileVersionProvider fileVersionProvider,
            IUrlHelperFactory urlHelperFactory)
        {
            _fileVersionProvider = fileVersionProvider;
            _urlHelperFactory = urlHelperFactory;
        }

        [ViewContext]
        public ViewContext ViewContext { get; set; } = null!;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = null;

            var scripts = ViewContext.ViewData["Scripts"] as List<string>;
            if (scripts == null) return;

            var urlHelper = _urlHelperFactory.GetUrlHelper(ViewContext);

            var links = new StringBuilder();
            foreach (var script in scripts)
            {
                var resolvedUrl = urlHelper.Content(script);
                var versionedUrl = _fileVersionProvider.AddFileVersionToPath(ViewContext.HttpContext.Request.PathBase, resolvedUrl);
                links.AppendLine($"<script src=\"{versionedUrl}\"></script>");
            }

            output.Content.SetHtmlContent(links.ToString());
        }
    }
}
