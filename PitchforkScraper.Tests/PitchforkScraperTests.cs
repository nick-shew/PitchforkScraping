using HtmlAgilityPack;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace PitchforkScraping.Tests
{
    public class PitchforkScraperTests
    {
        private readonly PitchforkScraper _pitchforkScraper;
        public PitchforkScraperTests()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "SceneVisualizer/0.1 (evisceratedcake ta gmial dto cmo)");
            _pitchforkScraper = new PitchforkScraper(client);
        }
        [Fact]
        public async Task IsArtistKnown_ReturnsFalse()
        {
            var isKnown = await _pitchforkScraper.IsArtistKnownAsync("The Bascinets");
            Assert.False(isKnown);
        }
        [Fact]
        public async Task IsArtistKnown_ReturnsTrue()
        {
            var isKnown = await _pitchforkScraper.IsArtistKnownAsync("Melkbelly");
            Assert.True(isKnown);
        }
        [Fact]
        public async Task GetAlbumReview_ReturnsReview()
        {
            var review = await _pitchforkScraper.GetAlbumReviewAsync("The Microphones", "Microphones in 2020");
            Assert.NotNull(review);//yep
        }
        [Fact]
        public void GetAlbumReviewFromHtml_ReturnsReview()
        {
            var doc = new HtmlDocument();
            doc.Load("pitchforksample.html");
            var review = _pitchforkScraper.GetAlbumReviewFromHtml(doc, "The Microphones", "Microphones in 2020");
            Assert.NotNull(review);//great test
        }
        [Fact]
        public async Task IsAlbumReviewed_ReturnsFalse()
        {
            var isReviewed = await _pitchforkScraper.IsAlbumReviewedAsync("The Bascinets", "Social Music");
            Assert.False(isReviewed);
        }
        [Fact]
        public async Task IsAlbumReviewed_ReturnsTrue()
        {
            var isReviewed = await _pitchforkScraper.IsAlbumReviewedAsync("The Microphones", "Microphones in 2020");
            Assert.True(isReviewed);
        }
        [Fact]
        public async Task GetArtistInfoAsync_ReturnsArtistInfo()
        {
            var artistInfo = await _pitchforkScraper.GetArtistInfoAsync("Melkbelly");
            Assert.True(artistInfo.Artist == "Melkbelly");
            Assert.True(artistInfo.Genres.FirstOrDefault() == "Rock");
            Assert.True(artistInfo.Genres.Count == 1);
            Assert.True(artistInfo.Summary.Length > 0);
            Assert.True(artistInfo.Reviews.Count == 2);
            Assert.True(artistInfo.Reviews[0].Url== "https://pitchfork.com/reviews/albums/melkbelly-pith/");
            Assert.True(artistInfo.Reviews[1].Url == "https://pitchfork.com/reviews/albums/melkbelly-nothing-valley/");
        }
        [Fact]
        public async Task GetArtistInfoAsync_ReturnsFalse()
        {
            await Assert.ThrowsAsync<MediaNotFoundException>(async () => await _pitchforkScraper.GetArtistInfoAsync("The Bascinets"));
        }
    }
}
