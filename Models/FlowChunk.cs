using System;
using System.Collections.Specialized;
using Microsoft.AspNetCore.Http;

namespace SocialClubNI.Models
{
    public class FlowChunk
    {
        public int Number { get; private set; }
        public int Size { get; private set; }
        public int TotalSize { get; private set; }
        public string Identifier { get; private set; }
        public string Filename { get; private set; }
        public int TotalChunks { get; private set; }

        public static FlowChunk ParseForm(IFormCollection form)
        {          
            FlowChunk chunkInfo = new FlowChunk();
            chunkInfo.Number = int.Parse(form["flowChunkNumber"]);
            chunkInfo.Size = Int32.Parse(form["flowChunkSize"]);
            chunkInfo.TotalSize = Int32.Parse(form["flowTotalSize"]);
            chunkInfo.Identifier = form["flowIdentifier"];
            chunkInfo.Filename = form["flowFilename"];
            chunkInfo.TotalChunks = int.Parse(form["flowTotalChunks"]);
            
            return chunkInfo;
        }

        private FlowChunk()
        {

        }
    }
}