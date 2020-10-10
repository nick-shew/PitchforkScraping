using System;
using System.Collections.Generic;
using System.Text;

namespace PitchforkScraping
{
    /// <summary>
    /// Exception to be thrown when an artist or album isn't found on Pitchfork's site.
    /// </summary>
    public class MediaNotFoundException : Exception
    {
        public MediaNotFoundException() { }
        public MediaNotFoundException(string message) : base(message) { }
        public MediaNotFoundException(string message, Exception inner) : base(message, inner) { }
    }
}
