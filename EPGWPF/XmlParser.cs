using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace EPGWPF
{
    class XmlParser
    {
        public static EPG Load(string url)
        {

            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            string xmlString = client.DownloadString(url);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlString);

            XmlNodeList channelList = doc.SelectNodes("/tv/channel");
            List<Channel> channels = new List<Channel>();
            foreach (XmlNode node in channelList)
            {
                string id = node.Attributes["id"].InnerText;
                string name = node.SelectSingleNode("display-name").InnerText;
                Channel channel = new Channel { Id = id, Name = name };
                channels.Add(channel);
            }

            XmlNodeList programmeList = doc.SelectNodes("/tv/programme");
            List<Programme> programmes = new List<Programme>();
            foreach (XmlNode node in programmeList)
            {
                XmlNode titleNode = node.SelectSingleNode("title");
                XmlNode descNode = node.SelectSingleNode("desc");

                int hoursOffset = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now).Hours;
                Programme programme = new Programme
                {
                    Channel = channels.Where((Channel channel) => channel.Id == node.Attributes["channel"].InnerText).FirstOrDefault(),
                    Start = DateTime.ParseExact(node.Attributes["start"].InnerText, "yyyyMMddHHmmss +ffff", CultureInfo.InvariantCulture).AddHours(hoursOffset),
                    End = DateTime.ParseExact(node.Attributes["stop"].InnerText, "yyyyMMddHHmmss +ffff", CultureInfo.InvariantCulture).AddHours(hoursOffset),
                    Title = titleNode != null ? titleNode.InnerText : "",
                    Description = descNode != null ? descNode.InnerText : ""
                };
                programmes.Add(programme);
            }

            foreach(Channel channel in channels)
            {
                channel.Programmes = programmes.Where(p => p.Channel == channel).ToList();
            }

            List<Category> categories = new List<Category>
            {
                new Category { Name = "Sport", Patterns = new string[] { "sk", "sport", "hntv" } },
                new Category { Name = "Movie", Patterns = new string[] { "hbo", "cine", "fox", "tv1000" } },
                new Category { Name = "Croatia", Patterns = new string[] { "htv", "nova tv", "n1 hr", "rtl", "doma" } },
                new Category { Name = "Documentary", Patterns = new string[] { "doku", "national", "history" } }
            };

            AddCategoryToChannels(categories, channels);

            var epg = new EPG
            {
                LastUpdate = DateTime.Now,
                Channels = channels.OrderBy(c => c.Name).ToList(),
                Programmes = programmes,
                Categories = categories,
            };

            return epg;
        }

        private static void AddCategoryToChannels(List<Category> categories, List<Channel> channels)
        {
            foreach (Channel channel in channels)
            {
                foreach (Category category in categories)
                {
                    foreach(string pattern in category.Patterns)
                    {
                        if (channel.Name.ToLower().Contains(pattern.ToLower()))
                        {
                            channel.Category = category.Name;
                        }
                    }
                }
            }
        }
    }
}
