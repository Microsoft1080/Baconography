﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Baconography.PlatformServices.ImageAPI
{
    class Minus
    {
        //Transliterated from Reddit Enhancement Suite https://github.com/honestbleeps/Reddit-Enhancement-Suite/blob/master/lib/reddit_enhancement_suite.user.js
        private static Regex hashRe = new Regex(@"^http:\/\/min.us\/([\w]+)(?:#[\d+])?$");

        internal static async Task<IEnumerable<Tuple<string, string>>> GetImagesFromUri(string title, Uri uri)
        {
            var href = uri.OriginalString.Split('?')[0];
            var groups = hashRe.Match(href).Groups;

            if (groups.Count > 2 && string.IsNullOrWhiteSpace(groups[2].Value))
            {
                var hash = groups[1].Value;
                if (hash.StartsWith("m"))
                {
                    var apiURL = "http://min.us/api/GetItems/" + hash;
                    var getClient = new HttpClient();
                    var jsonResult = await getClient.GetStringAsync(apiURL);
                    dynamic result = JsonConvert.DeserializeObject(jsonResult);
                    return new Tuple<string, string>[] { Tuple.Create(title, (string)result.src) };
                }
                else
                    return Enumerable.Empty<Tuple<string, string>>();
            }
            else
                return Enumerable.Empty<Tuple<string, string>>();
        }
    }
}
