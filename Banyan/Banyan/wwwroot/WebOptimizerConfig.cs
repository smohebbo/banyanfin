using WebOptimizer;

namespace Banyan
{
    public static class WebOptimizerConfig
    {
        public static void Configure(IAssetPipeline pipeline)
        {
            pipeline.AddCssBundle(
                "/bundles/bootstrap.css",
                "/lib/bootstrap/dist/css/bootstrap.css",
                "/lib/font-awesome/css/all.css"

            );
            pipeline.AddCssBundle(
                "/bundles/site.css",
                "/css/site.css"
            );
            pipeline.AddJavaScriptBundle(
                "/bundles/js.js",
                "/lib/jquery/dist/jquery.js",
                "/lib/jquery-validation/dist/jquery.validate.js",
                "/lib/jquery-validation/dist/additional-methods.js",
                "/lib/jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive.js",
                "/lib/bootstrap/dist/js/bootstrap.bundle.js",
                "/js/site.js"
            );
            pipeline.AddJavaScriptBundle(
                "/bundles/site.js",
                "/js/site.js"
            );
        }
    }
}
