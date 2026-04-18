using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mailchimp.Umbraco.Options;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Enums;
using Mailchimp.Umbraco.Services;
using Record = Umbraco.Forms.Core.Persistence.Dtos.Record;

namespace Mailchimp.Umbraco.Workflows;

public class MailchimpWorkflow : WorkflowType
{
    private static readonly HashSet<string> AllowedSubscriptionStatuses =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "subscribed",
            "pending"
        };

    private readonly ILogger<MailchimpWorkflow> _logger;
    private readonly MailchimpService _mailchimpService;
    private readonly MailchimpOptions _options;

    [Setting("List ID", Description = "Mailchimp audience list ID", DisplayOrder = 10)]
    public string ListId { get; set; } = string.Empty;

    [Setting("Email Field Alias", Description = "Alias of the form field containing the email address", DisplayOrder = 20)]
    public string EmailFieldAlias { get; set; } = string.Empty;

    [Setting("Tags", Description = "Comma-separated list of Mailchimp tags", DisplayOrder = 30)]
    public string Tags { get; set; } = string.Empty;

    [Setting(
        "Subscription Status",
        Description = "Use subscribed for immediate subscription or pending to send Mailchimp double opt-in confirmation.",
        DisplayOrder = 40)]
    public string SubscriptionStatus { get; set; } = "subscribed";

    [Setting(
        "Update Existing Member",
        Description = "When enabled, existing Mailchimp members are updated instead of being skipped.",
        View = "Umb.PropertyEditorUi.Toggle",
        DisplayOrder = 50)]
    public bool UpdateExistingMember { get; set; }

    [Setting(
        "Merge Fields",
        Description = "Comma-separated mergeTag:formAlias pairs. Use dotted merge tags for structured Mailchimp fields. Examples: FNAME:firstName,LNAME:lastName,PHONE:phone,BIRTHDAY:birthday,COMPANY:company,ADDRESS.addr1:addressLine1,ADDRESS.city:city,ADDRESS.state:state,ADDRESS.zip:zip,ADDRESS.country:country",
        DisplayOrder = 60)]
    public string MergeFields { get; set; } = string.Empty;

    public MailchimpWorkflow(
        ILogger<MailchimpWorkflow> logger,
        MailchimpService mailchimpService,
        IOptions<MailchimpOptions> options)
    {
        _logger = logger;
        _mailchimpService = mailchimpService;
        _options = options.Value;
        Id = new Guid("2e4c5f8a-1b3d-4e7f-9a2c-6d8e0f1b3c5d");
        Name = "Mailchimp";
        Description = "Subscribe user to Mailchimp audience";
    }

    public override List<Exception> ValidateSettings()
    {
        var errors = new List<Exception>();

        if (string.IsNullOrWhiteSpace(_options.ApiKey))
            errors.Add(new Exception("Mailchimp API key is not configured. Set Mailchimp:ApiKey in configuration."));

        if (string.IsNullOrWhiteSpace(ListId))
            errors.Add(new Exception("Mailchimp List ID is required"));

        if (string.IsNullOrWhiteSpace(EmailFieldAlias))
            errors.Add(new Exception("Email field alias is required"));

        if (!AllowedSubscriptionStatuses.Contains(SubscriptionStatus))
            errors.Add(new Exception("Subscription status must be either subscribed or pending"));

        return errors;
    }

    public override async Task<WorkflowExecutionStatus> ExecuteAsync(WorkflowExecutionContext context)
    {
        var record = context.Record;

        var email = record.GetValue<string>(EmailFieldAlias);

        if (string.IsNullOrWhiteSpace(email))
        {
            _logger.LogWarning("Mailchimp workflow skipped - email not found for alias {Alias}", EmailFieldAlias);

            return WorkflowExecutionStatus.Completed;
        }

        var resolvedMergeFields = ResolveMergeFields(record, MergeFields);

        if (resolvedMergeFields.Count == 0 && string.IsNullOrWhiteSpace(MergeFields))
        {
            resolvedMergeFields = ResolveLegacyMergeFields(record);
        }

        await _mailchimpService.SubscribeAsync(
            email,
            ListId,
            SubscriptionStatus,
            UpdateExistingMember,
            Tags,
            resolvedMergeFields);

        return WorkflowExecutionStatus.Completed;
    }

    // Reads raw stored settings (Workflow.Settings) for keys that the v13 workflow may have
    // persisted under different names. Called only when the new MergeFields setting is absent.
    private Dictionary<string, object> ResolveLegacyMergeFields(Record record)
    {
        var rawSettings = Workflow?.Settings;
        if (rawSettings is null || rawSettings.Count == 0)
            return [];

        // Keys used by common v13 community Mailchimp workflow packages.
        // Order matters: most specific / most likely first.
        string[] legacyKeys = ["Fields", "FieldMappings", "MergeFieldMappings"];

        foreach (var key in legacyKeys)
        {
            if (!rawSettings.TryGetValue(key, out var rawValue) || string.IsNullOrWhiteSpace(rawValue))
                continue;

            _logger.LogInformation(
                "Mailchimp workflow: using legacy setting key '{Key}' (v13 compatibility). Raw value: {Value}",
                key, rawValue);

            var parsed = ParseLegacyFieldEntries(record, rawValue);

            _logger.LogInformation(
                "Mailchimp workflow: resolved {Count} legacy merge field(s): {Fields}",
                parsed.Count,
                string.Join(", ", parsed.Keys));

            return parsed;
        }

        return [];
    }

    // Parses a comma-separated list of field mappings. Supports two formats:
    //   KEY:alias  – merge tag first (same as current MergeFields setting)
    //   alias|KEY  – field alias first, pipe-separated (used by some v13 packages)
    // Invalid or incomplete entries are silently skipped.
    private static Dictionary<string, object> ParseLegacyFieldEntries(Record record, string rawValue)
    {
        var result = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        foreach (var entry in rawValue.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            string? mergeTag = null;
            string? fieldAlias = null;

            var colonIdx = entry.IndexOf(':');
            if (colonIdx > 0 && colonIdx < entry.Length - 1)
            {
                // KEY:alias
                mergeTag   = entry[..colonIdx].Trim();
                fieldAlias = entry[(colonIdx + 1)..].Trim();
            }
            else
            {
                var pipeIdx = entry.IndexOf('|');
                if (pipeIdx > 0 && pipeIdx < entry.Length - 1)
                {
                    // alias|KEY
                    fieldAlias = entry[..pipeIdx].Trim();
                    mergeTag   = entry[(pipeIdx + 1)..].Trim();
                }
            }

            if (string.IsNullOrWhiteSpace(mergeTag) || string.IsNullOrWhiteSpace(fieldAlias))
                continue;

            var value = record.GetValue<string>(fieldAlias);
            if (!string.IsNullOrWhiteSpace(value))
                AddMergeFieldValue(result, mergeTag, value);
        }

        return result;
    }

    private static Dictionary<string, object> ResolveMergeFields(Record record, string? mergeFields)
    {
        var result = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        if (string.IsNullOrWhiteSpace(mergeFields))
            return result;

        foreach (var entry in mergeFields.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var separatorIndex = entry.IndexOf(':');
            if (separatorIndex <= 0 || separatorIndex == entry.Length - 1)
                continue;

            var key = entry[..separatorIndex].Trim();
            var alias = entry[(separatorIndex + 1)..].Trim();

            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(alias))
                continue;

            var value = record.GetValue<string>(alias);
            if (!string.IsNullOrWhiteSpace(value))
                AddMergeFieldValue(result, key, value);
        }

        return result;
    }

    private static void AddMergeFieldValue(Dictionary<string, object> result, string mergeTag, string value)
    {
        var path = mergeTag
            .Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (path.Length == 0)
            return;

        if (path.Length == 1)
        {
            result[path[0]] = value;
            return;
        }

        var current = result;

        for (var i = 0; i < path.Length - 1; i++)
        {
            var segment = path[i];

            if (!current.TryGetValue(segment, out var nested) ||
                nested is not Dictionary<string, object> nestedDictionary)
            {
                nestedDictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                current[segment] = nestedDictionary;
            }

            current = nestedDictionary;
        }

        current[path[^1]] = value;
    }
}
