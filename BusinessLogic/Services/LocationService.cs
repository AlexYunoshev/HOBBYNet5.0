using DataAccess.Context;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class LocationService
    {
        private readonly HobbyNetContext _context;

        public LocationService(HobbyNetContext context)
        {
            _context = context;
        }

        public Location GetUserLocation(string userId)
        {
            return _context.Users
                .Where(u => u.Id == userId)
                .Include(u => u.Location)
                .FirstOrDefault().Location;
        }

        public void SaveLocation(Location location, string userId)
        {
            _context.Users.Where(u => u.Id == userId).FirstOrDefault().Location = location;
            _context.SaveChanges();
        }

        public List<Location> GetLocations(string address)
        {
            List<Location> locations = new List<Location>();
            string[] addressArray = address.Split(new char[] { ' ' });

            //string Url = "http://nominatim.openstreetmap.org/search.php?q=Героїв+Праці+7+Харків&format=jsonv2";
            string Url = "http://nominatim.openstreetmap.org/search.php?q=";
            foreach (var addressItem in addressArray)
            {
                Url += addressItem + "+";
            }
            Url = Url.Remove(Url.Length - 1);
            Url += "&format=jsonv2";


            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(Url);
            webRequest.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.8.0.6) Gecko/20060728 Firefox/1.5";
            webRequest.CookieContainer = new CookieContainer();
            WebResponse webResponse;

            try
            {
                webRequest.Accept = "*/*";
                webResponse = webRequest.GetResponse();
                String htmlString;
                using (var reader = new StreamReader(webResponse.GetResponseStream()))
                {
                    htmlString = reader.ReadToEnd();
                    JArray json = JArray.Parse(htmlString);
                    foreach (JObject parsedObject in json.Children<JObject>())
                    {
                        Location locationModel = new Location();
                        foreach (JProperty parsedProperty in parsedObject.Properties())
                        {
                            if (parsedProperty.Name.Equals("place_id"))
                            {
                                int propertyValue = (int)parsedProperty.Value;
                                locationModel.PlaceId = propertyValue;
                            }

                            else if (parsedProperty.Name.Equals("lon"))
                            {
                                string propertyValue = (string)parsedProperty.Value;
                                locationModel.Longitude = propertyValue;
                            }
                            else if (parsedProperty.Name.Equals("lat"))
                            {
                                string propertyValue = (string)parsedProperty.Value;
                                locationModel.Latitude = propertyValue;
                            }

                            else if (parsedProperty.Name.Equals("display_name"))
                            {
                                string propertyValue = (string)parsedProperty.Value;
                                locationModel.Name = propertyValue;
                            }
                        }

                        locations.Add(locationModel);
                    }

                }
            }

            catch
            {

            }

            return locations;
        }
    }
}
