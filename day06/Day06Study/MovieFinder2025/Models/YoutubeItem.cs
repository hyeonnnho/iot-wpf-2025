﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MovieFinder2025.Models
{
    public class YoutubeItem : ObservableObject
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string ChannelTitle { get; set; }
        public string URL { get; set; }
        public BitmapImage Thumbnail { get; set; }

    }
}
