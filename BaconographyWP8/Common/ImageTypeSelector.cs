﻿using BaconographyPortable.ViewModel;
using BaconographyWP8.PlatformServices;
using ImageTools;
using ImageTools.Controls;
using ImageTools.Helpers;
using ImageTools.IO;
using ImageTools.IO.Gif;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace BaconographyWP8.Common
{
	public class ImageTypeSelector : DataTemplateSelector
	{
		private Dictionary<string, DataTemplate> _cachedDataTemplates;

        /// <summary>
        /// Fallback value for DataTemplate
        /// </summary>
        public string DefaultTemplateKey { get; set; }

        /// <summary>
        /// Cache search results for a type (defaults to Enabled)
        /// </summary>
        public bool IsCacheEnabled { get; set; }

		public ImageTypeSelector()
        {
            IsCacheEnabled = true;
        }

		DependencyObject _container;

		protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
		{
			_container = container;

			var picture = item as LinkedPictureViewModel.LinkedPicture;
			if (picture == null)
				return null;

			AsyncGetTemplateKey(picture.ImageSource as string);
			return GetDataTemplate("Temp");
		}


		private async void AsyncGetTemplateKey(string apiURL)
		{
			var request = HttpWebRequest.CreateHttp(apiURL);
			byte[] result;
			using (var response = (await SimpleHttpService.GetResponseAsync(request)))
			{
				result = await Task<byte[]>.Run(() =>
				{
					byte[] buffer = new byte[11];
					response.GetResponseStream().Read(buffer, 0, 11);
					return buffer;
				});
			}

			GifDecoder decoder = new GifDecoder();
			if (decoder.IsSupportedFileFormat(result))
			{
				this.ContentTemplate = GetDataTemplate("Type:Gif");
			}
			else
			{
				this.ContentTemplate = GetDataTemplate("Type:Else");
			}
			this.ApplyTemplate();
			
		}

		private DataTemplate GetDataTemplate(string key)
		{
			DataTemplate dt = GetCachedDataTemplate(key);
			try
			{
				if (dt != null) { return dt; }

				// look at all parents (visual parents)
				FrameworkElement fe = _container as FrameworkElement;
				while (fe != null)
				{
					dt = FindTemplate(fe, key);
					if (dt != null) { return dt; }
					// if you were to just look at logical parents,
					// you'd find that there isn't a Parent for Items set
					fe = VisualTreeHelper.GetParent(fe) as FrameworkElement;
				}

				dt = FindTemplate(null, key);
				return dt;
			}
			finally
			{
				if (dt != null)
				{
					AddCachedDataTemplate(key, dt);
				}
			}
		}

        private DataTemplate GetCachedDataTemplate(string key)
        {
            if (!IsCacheEnabled) { return null; }
            VerifyCachedDataTemplateStorage();
            if (_cachedDataTemplates.ContainsKey(key))
            {
                return _cachedDataTemplates[key];
            }

            return null;
        }

        private void AddCachedDataTemplate(string key, DataTemplate dt)
        {
            if (!IsCacheEnabled) { return; }
            VerifyCachedDataTemplateStorage();
            _cachedDataTemplates[key] = dt;
        }

        /// <summary>
        /// Delay creates storage
        /// </summary>
        private void VerifyCachedDataTemplateStorage()
        {
            if (_cachedDataTemplates == null)
            {
                _cachedDataTemplates = new Dictionary<string, DataTemplate>();
            }

        }

        /// <summary>
        /// Returns a template
        /// </summary>
        /// <param name="source">Pass null to search entire app</param>
        /// <param name="key"></param>
        /// <returns></returns>
        private static DataTemplate FindTemplate(object source, string key)
        {
            var fe = source as FrameworkElement;
            object obj;
            ResourceDictionary rd = fe != null ? fe.Resources : App.Current.Resources;
            if (rd.Contains(key))
            {
				obj = rd[key];
                DataTemplate dt = obj as DataTemplate;
                if (dt != null)
                {
                    return dt;
                }
            }
            return null;

        }
	}
}
