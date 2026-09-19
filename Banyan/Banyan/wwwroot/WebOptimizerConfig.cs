using WebOptimizer;

namespace Banyan
{
    public static class WebOptimizerConfig
    {
        public static void Configure(IAssetPipeline pipeline)
        {
            pipeline.AddCssBundle(
                "/css/banyan.css",
                "lib/bootstrap/dist/css/bootstrap.css",
                "css/site.css");

            pipeline.AddJavaScriptBundle(
                "/js/banyan.js",
                "lib/jquery/dist/jquery.js",
                "lib/jquery-validation/dist/jquery.validate.js",
                "lib/jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive.js",
                "lib/bootstrap/dist/js/bootstrap.bundle.js",
                "js/site.js");
        }
    }
}
