# Mailchimp for Umbraco Forms

Modern Mailchimp integration for Umbraco Forms.
Send form submissions directly to Mailchimp audiences with support for tags, merge fields, and structured data.

## Compatibility

<table>
  <thead>
    <tr>
      <th>Umbraco Version</th>
      <th>.NET Version</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td>17</td>
      <td>.NET 10</td>
    </tr>
    <tr>
      <td>16</td>
      <td>.NET 8</td>
    </tr>
    <tr>
      <td>15</td>
      <td>.NET 8</td>
    </tr>
    <tr>
      <td>14</td>
      <td>.NET 8</td>
    </tr>
  </tbody>
</table>

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

For local demo/testing, use .NET user secrets instead of storing a real API key in source control.

## Features

- Supports Umbraco 14&ndash;17
- Secure API key configuration
- Tags and merge field mapping
- Structured merge fields such as `ADDRESS.*`
- Optional subscription status
- Update existing Mailchimp members

## Screenshots

![Workflow Picker](https://raw.githubusercontent.com/jebastin-git/Mailchimp.Umbraco/main/assets/mailchimp-workflow-picker.png)
![Workflow Settings 1](https://raw.githubusercontent.com/jebastin-git/Mailchimp.Umbraco/main/assets/mailchimp-workflow-settings-1.png)
![Workflow Settings 2](https://raw.githubusercontent.com/jebastin-git/Mailchimp.Umbraco/main/assets/mailchimp-workflow-settings-2.png)
