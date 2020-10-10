using System;
using System.Collections.Generic;
using System.Text;

namespace PitchforkScraping
{
    public class Review
    {
        public Review() { }
        public Review(float score, string summary, bool isBestNewMusic, string fullReview, 
            string albumCoverLink, string artist, string album, string label, string releaseYear, 
            List<string> genre, string url)
        {
            Score = score;
            Summary = summary;
            IsBestNewMusic = isBestNewMusic;
            FullReview = fullReview;
            AlbumCoverLink = albumCoverLink;
            Artist = artist;
            Album = album;
            Label = label;
            ReleaseYear = releaseYear;
            Genre = genre;
            Url = url;
        }
        /// <summary>
        /// The score, expressed as a float to facilitate comparisons.
        /// </summary>
        public float Score { get; set; }
        /// <summary>
        /// The abstract given at the beginning of the review.
        /// </summary>
        public string Summary { get; set; }
        /// <summary>
        /// Whether the album was given the coveted title of Best New Music.
        /// </summary>
        public bool IsBestNewMusic { get; set; }
        /// <summary>
        /// The full text of the review.
        /// </summary>
        public string FullReview { get; set; }
        /// <summary>
        /// A link to the album cover.
        /// </summary>
        public string AlbumCoverLink { get; set; }
        /// <summary>
        /// The artist name.
        /// </summary>
        public string Artist { get; set; }
        /// <summary>
        /// The album title.
        /// </summary>
        public string Album { get; set; }
        /// <summary>
        /// The label on which the album was released.
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// The year in which the album was released.
        /// </summary>
        //NOTE... sometimes in case of reissue, the format is [original release year]/[reissue year]
        public string ReleaseYear { get; set; }
        /// <summary>
        /// The genre(s) of the album
        /// </summary>
        public List<string> Genre { get; set; }
        public string Url { get; set; }
    }
}
