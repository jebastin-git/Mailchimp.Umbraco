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

Configure the Mailchimp API key in `appsettings.json`, then use the Mailchimp Audience/List ID in the workflow settings for each form.

```json
{
  "Mailchimp": {
    "ApiKey": "your-mailchimp-api-key"
  }
}
```

## Usage

1. Install the package in an Umbraco site that already has Umbraco Forms installed.
2. Open a form in Umbraco Forms and add the `Mailchimp` workflow.
3. Set the Mailchimp Audience/List ID.
4. Map the form field alias that contains the subscriber email address.
5. Optionally add tags, merge field mappings, subscription status, and update-existing-member behavior.

Example merge field mappings:

```text
FNAME:firstName,LNAME:lastName,PHONE:phone,COMPANY:company
ADDRESS.addr1:addressLine1,ADDRESS.city:city,ADDRESS.state:state,ADDRESS.zip:zip,ADDRESS.country:country
```

## Features

- Supports Umbraco 14–17
- Secure API key configuration (`appsettings.json`)
- Tag support
- Merge field mapping
- Structured merge fields (e.g. `ADDRESS.*`)
- Optional subscription status
- Update existing Mailchimp members

## Screenshots

![Workflow Picker](assets/workflow-picker.png)
![Workflow Settings](assets/workflow-settings.png)

## License

MIT
