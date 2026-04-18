using Microsoft.Extensions.DependencyInjection;
using Mailchimp.Umbraco.Options;
using Mailchimp.Umbraco.Services;
using Mailchimp.Umbraco.Workflows;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Forms.Core.Providers.Extensions;

namespace Mailchimp.Umbraco.Composers;

public sealed class MailchimpComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddHttpClient();
        builder.Services.Configure<MailchimpOptions>(
            builder.Config.GetSection(MailchimpOptions.SectionName));
        builder.Services.AddTransient<MailchimpService>();

        builder.FormsWorkflows().Add<MailchimpWorkflow>();
    }
}
