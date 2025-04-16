using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace IdentityService.Pages.Shared.helpers
{
    [HtmlTargetElement("page-styles")]
    public class PageStylesTagHelper : TagHelper
    {
        private readonly IFileVersionProvider _fileVersionProvider;
        private readonly IUrlHelperFactory _urlHelperFactory;

        public PageStylesTagHelper(
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

            var styles = ViewContext.ViewData["Styles"] as List<string>;
            if (styles == null) return;

            var urlHelper = _urlHelperFactory.GetUrlHelper(ViewContext);

            var links = new StringBuilder();
            foreach (var style in styles)
            {
                var resolvedUrl = urlHelper.Content(style);
                var versionedUrl = _fileVersionProvider.AddFileVersionToPath(ViewContext.HttpContext.Request.PathBase, resolvedUrl);
                links.AppendLine($"<link rel=\"stylesheet\" href=\"{versionedUrl}\" />");
            }

            output.Content.SetHtmlContent(links.ToString());
        }
    }
}
