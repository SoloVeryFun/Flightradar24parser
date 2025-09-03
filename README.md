# Zvartnots Flight Parser

A C# console application built with Selenium that extracts passenger flight data from Zvartnots International Airport (EVN).

## Current Functionality

This version of the parser:

- Retrieves **arriving passenger flights** at Zvartnots Airport.
- Filters flights scheduled between **10:00 PM and 11:00 PM**.
- Covers a **2-day period**: **today** and **tomorrow**.

## How to Run

No setup is required — the project includes a precompiled version.

To run:

1. Download or clone the entire project folder.
2. Double-click the `Zvartnots.bat` file in the root directory.

The application will launch in a console window and display matching flight information.

⚠ Ensure you have an internet connection while running — live data is pulled from https://www.flightradar24.com.


## Technologies Used

- C# (.NET 9)
- Selenium WebDriver
- ChromeDriver (included via NuGet)

## Planned Features

The following improvements are planned for future versions:

- Customizable time ranges
- Airport selection support

## License

This project is provided for educational and non-commercial use.