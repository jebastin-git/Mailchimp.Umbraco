# Mailchimp for Umbraco Forms

Modern Mailchimp integration for Umbraco Forms.
Send form submissions directly to Mailchimp audiences with support for tags, merge fields, and structured data.

## Compatibility

| Umbraco Version | .NET Version |
|----------------|-------------|
| 14             | .NET 8      |
| 15             | .NET 8      |
| 16             | .NET 8      |
| 17             | .NET 10     |

Supports Umbraco 14–17.

## Requirements

- Umbraco Forms must be installed
- Mailchimp account and API key required

## Installation

```bash
dotnet add package Mailchimp.Umbraco
```

## Configuration

Add the Mailchimp API key to `appsettings.json` and configure the Mailchimp Audience/List ID on each workflow.

```json
{
  "Mailchimp": {
    "ApiKey": "your-mailchimp-api-key"
  }
}
```

## Features

- Supports Umbraco 14–17
- Secure API key configuration
- Tags and merge field mapping
- Structured merge fields such as `ADDRESS.*`
- Optional subscription status
- Update existing Mailchimp members

## Screenshots

![Workflow Picker](assets/workflow-picker.png)
![Workflow Settings](assets/workflow-settings.png)
