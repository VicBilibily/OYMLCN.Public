using Microsoft.AspNetCore.Razor.TagHelpers;

namespace OYMLCN.Web.Mvc.Layui
{
    /// <summary>
    /// layui-nav-itemed
    /// </summary>
    [HtmlTargetElement("li", Attributes = "layui-nav-itemed-controller")]
    public class LayuiNavItemedTagHelper : TagHelper
    {
        /// <summary>
        /// layui-nav-itemed-controller
        /// </summary>
        [HtmlAttributeName("layui-nav-itemed-controller")]
        public string Controller { get; set; }
        /// <summary>
        /// layui-nav-itemed-action
        /// </summary>
        [HtmlAttributeName("layui-nav-itemed-action")]
        public string Action { get; set; }

        /// <summary>
        /// Process
        /// </summary>
        /// <param name="context"></param>
        /// <param name="output"></param>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (IsEqualController(Controller) && (Action.IsNullOrEmpty() || IsEqualAction(Action)))
                output.AddClass("layui-nav-itemed");
            base.Process(context, output);
        }
    }
}