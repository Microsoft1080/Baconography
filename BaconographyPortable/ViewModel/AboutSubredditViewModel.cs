﻿using BaconographyPortable.Model.Reddit;
using BaconographyPortable.Services;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaconographyPortable.ViewModel
{
    public class AboutSubredditViewModel : ViewModelBase
    {
        public TypedThing<Subreddit> Thing { get; private set; }
        
        IRedditService _redditService;
        IBaconProvider _baconProvider;
        bool _subscribed;

        public AboutSubredditViewModel(IBaconProvider baconProvider, Thing thing, bool subscribed)
        {
            _baconProvider = baconProvider;
            Thing = new TypedThing<Subreddit>(thing);
            _redditService = _baconProvider.GetService<IRedditService>();
            _subscribed = subscribed;
        }

        public bool Over18
        {
            get
            {
                return Thing.Data.Over18;
            }
        }

        public long Subscribers
        {
            get
            {
                return Thing.Data.Subscribers;
            }
        }

        public DateTime CreatedUTC
        {
            get
            {
                return Thing.Data.CreatedUTC;
            }
        }

        public string Url
        {
            get
            {
                return Thing.Data.Url;
            }
        }

        public string DisplayName
        {
            get
            {
                return Thing.Data.DisplayName;
            }
        }

        public string PublicDescription
        {
            get
            {
                return Thing.Data.PublicDescription;
            }
        }

        public string HeaderImage
        {
            get
            {
                return Thing.Data.HeaderImage;
            }
        }

        public int HeaderImageHeight
        {
            get
            {
                if (Thing.Data.HeaderSize == null)
                    return 50;
                else
                    return Thing.Data.HeaderSize[0];
            }
        }

        public int HeaderImageWidth
        {
            get
            {
                if (Thing.Data.HeaderSize == null)
                    return 128;
                else
                    return Thing.Data.HeaderSize[0];
            }
        }

        public bool Subscribed
        {
            get
            {
                return _subscribed;
            }
            set
            {
                _subscribed = value;
                _redditService.AddSubredditSubscription(Thing.Data.Name, !value);
                RaisePropertyChanged("Subscribed");
            }
        }
    }
}
