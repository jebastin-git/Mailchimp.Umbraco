namespace Mailchimp.Umbraco.Options;

public sealed class MailchimpOptions
{
    public const string SectionName = "Mailchimp";

    public string ApiKey { get; set; } = string.Empty;
}
