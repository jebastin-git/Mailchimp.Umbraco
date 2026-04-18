## Compatibility

- Umbraco 17: .NET 10
- Umbraco 16: .NET 8
- Umbraco 15: .NET 8
- Umbraco 14: .NET 8

Supports Umbraco 14&ndash;17.

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

For local demo/testing, use .NET user secrets instead of storing a real API key in source control:

```bash
dotnet user-secrets set "Mailchimp:ApiKey" "your-mailchimp-api-key" --project demo/Mailchimp.Umbraco.Demo
```

## Features

- Secure API key configuration
- Tag support
- Merge field mapping
- Structured merge fields such as `ADDRESS.*`
- Optional subscription status
- Update existing Mailchimp members

## Screenshots

- [Workflow Picker](https://raw.githubusercontent.com/jebastin-git/Mailchimp.Umbraco/main/assets/mailchimp-workflow-picker.png)
- [Workflow Settings](https://raw.githubusercontent.com/jebastin-git/Mailchimp.Umbraco/main/assets/mailchimp-workflow-settings-1.png)
- [Merge Fields Settings](https://raw.githubusercontent.com/jebastin-git/Mailchimp.Umbraco/main/assets/mailchimp-workflow-settings-2.png)
