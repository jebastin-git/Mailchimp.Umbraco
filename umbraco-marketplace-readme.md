# Mailchimp.Umbraco

Mailchimp.Umbraco is an Umbraco Forms integration package for sending form submissions to a Mailchimp audience through a custom workflow.

## What It Does

- Adds a Mailchimp workflow to Umbraco Forms
- Subscribes form submissions to a Mailchimp audience
- Supports tags
- Supports merge field mapping, including structured address fields

## Supported Versions

- Umbraco 17
- Umbraco 14

## Typical Use Case

Use this package when you want editors to connect an Umbraco Form directly to Mailchimp without building custom submission handling code.

## Configuration

Add the Mailchimp API key in app configuration:

```json
{
  "Mailchimp": {
    "ApiKey": ""
  }
}
```

## Workflow Settings

The workflow supports these settings:

- List ID
- Email Field Alias
- Tags
- Merge Fields

Example merge field mapping:

```text
FNAME:firstName,LNAME:lastName,PHONE:phone,COMPANY:company
```

Example structured address mapping:

```text
ADDRESS.addr1:addressLine1,ADDRESS.city:city,ADDRESS.state:state,ADDRESS.zip:zip,ADDRESS.country:country
```

## Notes

- This package is focused on Umbraco Forms workflows, not a general Mailchimp SDK.
- The Mailchimp API key is read from configuration and is not stored in workflow settings.
- A new NuGet version is required before Marketplace changes appear on the public listing.
