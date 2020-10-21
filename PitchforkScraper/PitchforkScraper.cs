using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace PitchforkScraping
{
    /// <summary>
    /// Class to get info from p4k's website. Currently works only for items added to their system post-June 2017.
    /// </summary>
    public class PitchforkScraper
    {
        //influenced by https://github.com/tejassharma96/pitchfork_api/blob/master/pitchfork_api/pitchfork.py
        //TODO url scheme was changed in june 2017? then it requires artist identifier. Figure out a way to intuit artist IDs without using search
        private static HttpClient _client;
        /// <summary>
        /// Initializes a new PitchforkScraper.
        /// </summary>
        /// <param name="client">HttpClient used for requests. Populate this with a UserAgent before use.</param>
        public PitchforkScraper(HttpClient client)
        {
            _client = client;
        }
        
        /// <summary>
        /// Asynchronously checks if an artist has a Pitchfork page.
        /// </summary>
        /// <param name="artist"></param>
        /// <returns></returns>
        public async Task<bool> IsArtistKnownAsync(string artist)
        {
            try
            {
                var url = GetUrlFromArtist(artist);
                var response = await MakeRequestAsync(url);
                return response.IsSuccessStatusCode;
            }
            catch (MediaNotFoundException)
            {
                return false;
            }
            
        }
        /// <summary>
        /// Checks if an artist has a Pitchfork page.
        /// </summary>
        /// <param name="artist"></param>
        /// <returns></returns>
        public bool IsArtistKnown(string artist)
        {
            try
            {
                var url = GetUrlFromArtist(artist);
                var response = MakeRequest(url);
                return response.IsSuccessStatusCode;
            }
            catch (MediaNotFoundException)
            {
                return false;
            }
            
        }
        /// <summary>
        /// Gets an artist's info from a Pitchfork HtmlDocument.
        /// </summary>
        /// <param name="doc">The Pitchfork page</param>
        /// <param name="artist">The artist's name</param>
        /// <param name="url">Optional URL for the page</param>
        /// <returns></returns>
        public ArtistInfo GetArtistInfoFromHtml(HtmlDocument doc, string artist, string url = "")
        {
            var genres = doc.DocumentNode.SelectNodes("//ul[@class='genre-list genre-list--inline artist-header__genre-list']/li[@class='genre-list__item']")
                .Select(n => n.InnerText).ToList();
            doc.TryGetTextOfFirstNodeWithClass("//div[@class='contents artist-header__bio']", out var summary);
            summary = summary.Trim();
            //populate album reviews for further lookup
            var reviews = new List<Review>();
            var reviewNodes = doc.DocumentNode.SelectNodes("//div[@class='review']");
            if(reviewNodes != null)
            {
                foreach (var reviewNode in reviewNodes)
                {
                    reviews.Add(new Review()
                    {
                        Artist = artist,
                        Album = reviewNode.SelectSingleNode(".//h2[@class='review__title-album']").InnerText,
                        Url = "https://pitchfork.com" + reviewNode.SelectSingleNode(".//a[@class='review__link']").GetAttributeValue("href", ""),
                    });
                }
            }
            
            return new ArtistInfo()
            {
                Artist = artist,
                Genres = genres,
                Summary = summary,
                Reviews = reviews,
                Url = url,
            };
        }
        /// <summary>
        /// Asynchronously looks up an artist. Throws MediaNotFoundException if not found.
        /// </summary>
        /// <param name="artist"></param>
        /// <returns></returns>
        public async Task<ArtistInfo> GetArtistInfoAsync(string artist)
        {
            var url = GetUrlFromArtist(artist);
            var response = await MakeRequestAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            var doc = new HtmlDocument();
            doc.LoadHtml(content);
            return GetArtistInfoFromHtml(doc, artist, url);
        }
        /// <summary>
        /// Looks up an artist. Throws MediaNotFoundException if not found.
        /// </summary>
        /// <param name="artist"></param>
        /// <returns></returns>
        public ArtistInfo GetArtistInfo(string artist)
        {
            var url = GetUrlFromArtist(artist);
            var response = MakeRequest(url);
            var task = response.Content.ReadAsStringAsync();
            task.Wait();
            var content = task.Result;
            var doc = new HtmlDocument();
            doc.LoadHtml(content);
            return GetArtistInfoFromHtml(doc, artist, url);
        }

        private string GetUrlFromArtist(string artist)
        {
            var formattedArtist = artist.ToLower().Replace(" ", "-");
            return $"https://pitchfork.com/artists/{formattedArtist}/";
        }
        /// <summary>
        /// Finds whether an album has been reviewed.
        /// </summary>
        /// <param name="artist"></param>
        /// <param name="album"></param>
        /// <returns></returns>
        public async Task<bool> IsAlbumReviewedAsync(string artist, string album)
        {
            try
            {
                var response = await MakeAlbumRequestAsync(artist, album);
                return response.IsSuccessStatusCode;
            }
            catch (MediaNotFoundException)
            {
                return false;
            }            
        }
        /// <summary>
        /// Asynchronously finds whether an album has been reviewed.
        /// </summary>
        /// <param name="artist"></param>
        /// <param name="album"></param>
        /// <returns></returns>
        public async Task<Review> GetAlbumReviewAsync(string artist, string album)
        {
            var response = await MakeAlbumRequestAsync(artist, album);
            var content = await response.Content.ReadAsStringAsync();
            //read from content
            var doc = new HtmlDocument();
            doc.LoadHtml(content);
            return GetAlbumReviewFromHtml(doc, artist, album, response.RequestMessage.RequestUri.AbsoluteUri);
        }
        private async Task<HttpResponseMessage> MakeAlbumRequestAsync(string artist, string album)
        {
            //format req
            var formattedAlbum = $"{artist.ToLower().Replace(" ", "-")}-{album.ToLower().Replace(" ", "-")}";//TODO remove parentheses
            var url = $"https://pitchfork.com/reviews/albums/{formattedAlbum}";
            return await MakeRequestAsync(url);
        }
        private async Task<HttpResponseMessage> MakeRequestAsync(string url)
        {
            using (var reqMsg = new HttpRequestMessage(HttpMethod.Get, url))
            {
                //reqMsg.Headers.Add("User-Agent", _userAgent);
                var response = await _client.SendAsync(reqMsg);
                if (!response.IsSuccessStatusCode) throw new MediaNotFoundException();
                return response;
            }
        }
        private HttpResponseMessage MakeRequest(string url)
        {
            using (var reqMsg = new HttpRequestMessage(HttpMethod.Get, url))
            {
                //apparently doing this is guaranteed not to deadlock per MS
                var task = _client.SendAsync(reqMsg);
                task.Wait();
                var response = task.Result;
                if (!response.IsSuccessStatusCode) throw new MediaNotFoundException();
                return response;
            }
        }
        /// <summary>
        /// Gets a review from a Pitchfork HtmlDocument.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="artist"></param>
        /// <param name="album"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public Review GetAlbumReviewFromHtml(HtmlDocument doc, string artist, string album, string url="")
        {
            doc.TryGetTextOfFirstNodeWithClass("//div[@class='review-detail__abstract']", out var summary);
            summary = summary.Trim();
            float score = -1;
            if(doc.TryGetTextOfFirstNodeWithClass("//span[@class='score']", out var scoreText))
            {
                score = float.Parse(scoreText);
            }
            var isBNM = doc.DocumentNode.SelectNodes("//svg[@class='bnm-arrows']").Count() != 0;
            doc.TryGetTextOfFirstNodeWithClass("//div[@class='contents dropcap']", out var fullReview);//TODO format this?
            fullReview = fullReview.Trim();
            var albumCoverLink = "";
            try
            {
                var albumCoverNode = doc.DocumentNode.SelectNodes("//div[@class='single-album-tombstone__art']").FirstOrDefault();
                albumCoverLink = albumCoverNode.Descendants("img").FirstOrDefault().GetAttributeValue("src", "");
            }
            catch (NullReferenceException) { }
           
            var labelNodes = doc.DocumentNode.SelectNodes("//li[@class='labels-list__item']").ToList();
            //too dumb to fully linq out
            var labelSb = new StringBuilder();
            for (var i = 0; i < labelNodes.Count; i++)
            {
                labelSb.Append(System.Net.WebUtility.HtmlDecode(labelNodes[i].InnerText.Trim()));
                if (i < labelNodes.Count - 1) labelSb.Append("/");
            }
            doc.TryGetTextOfFirstNodeWithClass("//span[@class='single-album-tombstone__meta-year']", out var year);
            year = new string(year.Where(c => char.IsDigit(c)).ToArray());
            var genreNodes = doc.DocumentNode.SelectNodes("//li[@class='genre-list__item']").ToList();
            var genreList = new List<string>();
            foreach (var n in genreNodes)
            {
                genreList.Add(n.InnerText);
            }
            return new Review(score, summary, isBNM, fullReview, albumCoverLink, artist, album, labelSb.ToString(), year, genreList, url);
        }
    }
}
