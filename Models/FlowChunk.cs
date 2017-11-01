using System;

namespace SocialClubNI.Models
{
    public class FlowChunk
    {
        public int FlowChunkNumber { get; set; }
        
        public int FlowChunkSize { get; set; }

        public int FlowTotalSize { get; set; }

        public string FlowIdentifier { get; set; }
        public string FlowFilename { get; set; }
        public string FlowRelativePath { get; set; }

        public int FlowCurrentChunkSize { get; set; }
    }
}