using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beat_Saber_Song_Downloader
{
    class DownloadSettings
    {
        private DateTime? from, to;
        private String chroma, noodle, curated, verified, full, page;
        private List<String> tags;

        public DownloadSettings()
        {
            tags = new List<String>();
        }

        public DateTime? From
        {
            get { return from; }
            set { from = value; }
        }

        public DateTime? To
        {
            get { return to; }
            set { to = value; }
        }

        public String Chroma
        {
            get { return chroma; }
            set { chroma = value; }
        }

        public String Noodle
        {
            get { return noodle; }
            set { noodle = value; }
        }

        public String Curated
        {
            get { return curated; }
            set { curated = value; }
        }

        public String Verified
        {
            get { return verified; }
            set { verified = value; }
        }

        public String Full
        {
            get { return full; }
            set { full = value; }
        }

        public String Page
        {
            get { return page; }
            set { page = value; }
        }

        public List<String> Tags
        {
            get { return tags; }
            set { tags = value; }
        }




    }
}
