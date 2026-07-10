# ZAxis-CreditCardStatement-Transform

A lightweight C# WPF utility for transforming credit card statement CSV files into a format that can be imported into Sage accounting software.

The application automates repetitive bookkeeping tasks by cleaning the statement, applying General Ledger (GL) account mappings, and generating a new Sage-ready CSV file.

## Features

* Load credit card statement CSV files.
* Automatically remove unnecessary columns.
* Rename columns to match the Sage import format.
* Automatically assign GL Account Numbers based on transaction descriptions.
* Export a transformed CSV while preserving the original statement.
* Automatically open the transformed CSV in Microsoft Excel.

## How It Works

1. Select a credit card statement CSV.
2. Load and transform the statement.
3. Transaction descriptions are compared against a configurable set of mapping rules.
4. Matching transactions are assigned the appropriate GL Account Number.
5. A new `*_Transformed.csv` file is generated and opened automatically.

## Mapping Engine

The application uses a keyword-based mapping engine to categorize transactions.

Examples include:

| Transaction Description | GL Account         |
| ----------------------- | ------------------ |
| FEDEX                   | Shipping Expense   |
| UPS                     | Shipping Expense   |
| MCMASTER                | Shop Supplies      |
| DIGI KEY                | Shop Supplies      |
| STAPLES                 | Office Expense     |
| VERIZON                 | Cellular Telephone |
| REACHLOCAL              | Advertising        |
| CANVA                   | Software Expense   |

The mapping engine is designed to be easily extended as new vendors and purchasing patterns are identified.

## Technologies

* C#
* .NET
* WPF
* CSV Parsing (`TextFieldParser`)
* Object-Oriented Design

## Current Status

Current functionality includes:

* ✅ CSV loading
* ✅ CSV transformation
* ✅ GL account mapping
* ✅ CSV export
* ✅ Automatic Excel launch

Planned improvements:

* Merchant mapping editor
* Card-specific mapping rules
* Logging of unmapped transactions
* Duplicate merchant detection
* Configuration file (JSON/XML) support
* Summary report by GL Account
* Unit tests

## Motivation

This project was created to automate monthly accounting tasks at Z-Axis Connector Company by reducing manual data entry and improving consistency when importing credit card transactions into Sage.
