using System;
using System.Collections.Generic;
using System.Text;

namespace PitchforkScraping
{
    public class ArtistInfo
    {
        public ArtistInfo()
        {

        }
        public string Artist { get; set; }
        public string Summary { get; set; }
        public List<string> Genres { get; set; }
        public List<Review> Reviews { get; set; }
        public string Url { get; set; }
    }
}
