# Mailchimp.Umbraco

Mailchimp workflow integration for Umbraco Forms.

This package is built specifically for Umbraco Forms and provides a custom workflow for subscribing form submissions to a Mailchimp audience.

## Features

- Custom Umbraco Forms workflow for Mailchimp
- Supports Umbraco 17 (`.NET 10`) and Umbraco 14 (`.NET 8`)
- Supports Mailchimp tags
- Supports Mailchimp merge fields, including structured address fields

## Installation

Install the package in your Umbraco site:

```bash
dotnet add package Mailchimp.Umbraco
```

This package is intended for sites that already use Umbraco Forms.

## Configuration

Add your Mailchimp API key to configuration:

```json
{
  "Mailchimp": {
    "ApiKey": ""
  }
}
```

## Workflow Setup

After installing the package, add the `Mailchimp` workflow to an Umbraco Form and configure:

- `List ID`
- `Email Field Alias`
- `Tags` (optional)
- `Merge Fields` (optional)

Example merge fields:

```text
FNAME:firstName,LNAME:lastName,PHONE:phone,BIRTHDAY:birthday,COMPANY:company
```

Example structured address mapping:

```text
ADDRESS.addr1:addressLine1,ADDRESS.city:city,ADDRESS.state:state,ADDRESS.zip:zip,ADDRESS.country:country
```

## Notes

- `Email Field Alias` should point to the form field containing the subscriber email address.
- The Mailchimp API key is read from configuration and is not stored in workflow settings.
- `BIRTHDAY` values should be provided in the format expected by Mailchimp, typically `MM/DD`.

## License

MIT
