using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mailchimp.Umbraco.Options;

namespace Mailchimp.Umbraco.Services;

public class MailchimpService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<MailchimpService> _logger;
    private readonly MailchimpOptions _options;

    public MailchimpService(
        IHttpClientFactory httpClientFactory,
        ILogger<MailchimpService> logger,
        IOptions<MailchimpOptions> options)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _options = options.Value;
    }

    public async Task SubscribeAsync(
        string email,
        string listId,
        string subscriptionStatus,
        bool updateExistingMember,
        string? tags = null,
        Dictionary<string, object>? mergeFields = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            _logger.LogWarning("Mailchimp API key is not configured. Skipping subscription for {Email}", email);
            return;
        }

        var apiKey = _options.ApiKey;
        var dc = ExtractDatacenter(apiKey);
        if (dc is null)
        {
            _logger.LogWarning("Mailchimp could not extract datacenter from API key for list {ListId}", listId);
            return;
        }

        var normalizedStatus = NormalizeSubscriptionStatus(subscriptionStatus);
        var memberHash = GetSubscriberHash(email);
        var url = updateExistingMember
            ? $"https://{dc}.api.mailchimp.com/3.0/lists/{listId}/members/{memberHash}"
            : $"https://{dc}.api.mailchimp.com/3.0/lists/{listId}/members";

        var parsedTags = tags?
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .ToArray() ?? Array.Empty<string>();

        var hasTags = parsedTags.Length > 0;
        var hasMergeFields = mergeFields is { Count: > 0 };

        var body = BuildRequestBody(email, normalizedStatus, updateExistingMember, parsedTags, mergeFields);

        var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(10);

        var request = new HttpRequestMessage(updateExistingMember ? HttpMethod.Put : HttpMethod.Post, url)
        {
            Content = new StringContent(body, Encoding.UTF8, "application/json")
        };

        var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"anystring:{apiKey}"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(TimeSpan.FromSeconds(10));

        HttpResponseMessage response;
        try
        {
            response = await client.SendAsync(request, cts.Token);
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogWarning(ex, "Mailchimp request timed out for {Email}", email);
            return;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Mailchimp HTTP request failed for {Email}", email);
            return;
        }

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation(
                "Mailchimp subscription successful for {Email} in list {ListId} with status {Status} (update existing: {UpdateExistingMember})",
                email,
                listId,
                normalizedStatus,
                updateExistingMember);
            return;
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var content = await response.Content.ReadAsStringAsync();
            if (content.Contains("Member Exists", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation("Mailchimp member already exists for {Email} in list {ListId}", email, listId);
                return;
            }
        }

        var errorContent = await response.Content.ReadAsStringAsync();
        _logger.LogError(
            "Mailchimp subscription failed for {Email} in list {ListId} with status {StatusCode}. Response: {Response}",
            email,
            listId,
            (int)response.StatusCode,
            errorContent);
    }

    private static string? ExtractDatacenter(string apiKey)
    {
        var dashIndex = apiKey.LastIndexOf('-');
        if (dashIndex < 0 || dashIndex == apiKey.Length - 1)
            return null;

        return apiKey[(dashIndex + 1)..];
    }

    private static string NormalizeSubscriptionStatus(string subscriptionStatus) =>
        string.Equals(subscriptionStatus, "pending", StringComparison.OrdinalIgnoreCase)
            ? "pending"
            : "subscribed";

    private static string BuildRequestBody(
        string email,
        string subscriptionStatus,
        bool updateExistingMember,
        string[] tags,
        Dictionary<string, object>? mergeFields)
    {
        var hasTags = tags.Length > 0;
        var hasMergeFields = mergeFields is { Count: > 0 };

        if (updateExistingMember)
        {
            return (hasTags, hasMergeFields) switch
            {
                (true, true) => JsonSerializer.Serialize(new
                {
                    email_address = email,
                    status = subscriptionStatus,
                    status_if_new = subscriptionStatus,
                    tags,
                    merge_fields = mergeFields
                }),
                (true, false) => JsonSerializer.Serialize(new
                {
                    email_address = email,
                    status = subscriptionStatus,
                    status_if_new = subscriptionStatus,
                    tags
                }),
                (false, true) => JsonSerializer.Serialize(new
                {
                    email_address = email,
                    status = subscriptionStatus,
                    status_if_new = subscriptionStatus,
                    merge_fields = mergeFields
                }),
                _ => JsonSerializer.Serialize(new
                {
                    email_address = email,
                    status = subscriptionStatus,
                    status_if_new = subscriptionStatus
                })
            };
        }

        return (hasTags, hasMergeFields) switch
        {
            (true, true) => JsonSerializer.Serialize(new
            {
                email_address = email,
                status = subscriptionStatus,
                tags,
                merge_fields = mergeFields
            }),
            (true, false) => JsonSerializer.Serialize(new
            {
                email_address = email,
                status = subscriptionStatus,
                tags
            }),
            (false, true) => JsonSerializer.Serialize(new
            {
                email_address = email,
                status = subscriptionStatus,
                merge_fields = mergeFields
            }),
            _ => JsonSerializer.Serialize(new
            {
                email_address = email,
                status = subscriptionStatus
            })
        };
    }

    private static string GetSubscriberHash(string email)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var bytes = Encoding.UTF8.GetBytes(normalizedEmail);
        var hash = MD5.HashData(bytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
