using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Novskiy.UI.TagHelpers;

[HtmlTargetElement("img", Attributes = "img-action, img-controller")]
public class ImageTagHelper : TagHelper
{
    private readonly LinkGenerator _linkGenerator;

    public ImageTagHelper(LinkGenerator linkGenerator)
    {
        _linkGenerator = linkGenerator;
    }

    public string ImgController { get; set; } = string.Empty;
    public string ImgAction { get; set; } = string.Empty;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var url = _linkGenerator.GetPathByAction(ImgAction, ImgController);
        if (url != null)
        {
            output.Attributes.Add("src", url);
        }
    }
}
