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

Configure the Mailchimp API key in `appsettings.json`, then use the Mailchimp Audience/List ID in the workflow settings for each form.

```json
{
  "Mailchimp": {
    "ApiKey": "your-mailchimp-api-key"
  }
}
```

For local demo/testing, prefer .NET user secrets instead of committing a real API key:

```bash
dotnet user-secrets set "Mailchimp:ApiKey" "your-mailchimp-api-key" --project demo/Mailchimp.Umbraco.Demo
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

- Supports Umbraco 14&ndash;17
- Secure API key configuration (`appsettings.json`)
- Tag support
- Merge field mapping
- Structured merge fields (e.g. `ADDRESS.*`)
- Optional subscription status
- Update existing Mailchimp members

## Screenshots

![Workflow Picker](https://raw.githubusercontent.com/jebastin-git/Mailchimp.Umbraco/main/assets/mailchimp-workflow-picker.png)
![Workflow Settings 1](https://raw.githubusercontent.com/jebastin-git/Mailchimp.Umbraco/main/assets/mailchimp-workflow-settings-1.png)
![Workflow Settings 2](https://raw.githubusercontent.com/jebastin-git/Mailchimp.Umbraco/main/assets/mailchimp-workflow-settings-2.png)

## License

MIT
