# Zvartnots Flight Parser

A C# console application built with Selenium that extracts passenger flight data from Zvartnots International Airport (EVN).

## Current Functionality

This version of the parser:

Works via direct server requests (much faster than previous Selenium-based parsing).

Displays the flight number and time.

Is currently configured for departures at Zvartnots International Airport (EVN, Yerevan).

The airport can only be changed manually in the code (LN122, chars 86–89).

Note: Results may slightly differ from Flightradar24.com due to API structure differences.

## How to Run

No setup is required — the project includes a precompiled version.

To run:

1. Download or clone the entire project folder.
2. Double-click the `Zvartnots.bat` file in the root directory.

The application will launch in a console window and display matching flight information.

⚠ Ensure you have an internet connection while running — live data is pulled from https://www.flightradar24.com.
## Technologies Used

- C# (.NET 9)
- HTTP requests to Flightradar24 APIs
- Newtonsoft.Json (included via NuGet)

## Planned Features

Customizable time ranges
Airport selection support

## License

This project is provided for educational and non-commercial use.