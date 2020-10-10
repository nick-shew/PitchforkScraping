# PitchforkScraping
Simple .NET Core class library to get reviews and artist info from Pitchfork.com.

This differs from several other scrapers in that it doesn't use the Search API, thereby respecting robots.txt.

**NOTE:** Currently works only for albums and artists added post-June 2017, when Pitchfork removed numeric IDs for media.

# Usage
Simply instantiate a new PitchforkScraper object and pass in a HttpClient. Make sure to set the UserAgent for the client before use.
