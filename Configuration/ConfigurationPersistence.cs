/*
    Würfelware - Password Generator Plugin for Keepass
    Copyright (C) 2017  Heinrich Ulbricht

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using Newtonsoft.Json;
using PwGenWuerfelware.Classes;
using System;
using System.IO;

namespace PwGenWuerfelware.Configuration
{
    /// <summary>
    /// Takes care of loading and saving the current configuration.
    /// </summary>
    /// <remarks>
    /// TODO:
    /// - care about user profiles - currently only one configuration is supported
    /// - more error handling - currently every error would crash Keepass
    /// </remarks>
    /// 
    public class ConfigurationPersistence
    {
        public ConfigurationModel LoadFromUserFile()
        {
            var userFilePath = GetUserFilePath();
            if (userFilePath != null && File.Exists(userFilePath))
            {
                using (var r = new StreamReader(userFilePath))
                {
                    var json = r.ReadToEnd();
                    return JsonConvert.DeserializeObject<ConfigurationModel>(json);
                }
            }

            return new ConfigurationModel();
        }

        public void SaveToUserFile(ConfigurationModel config)
        {
            var userFilePath = GetUserFilePath();
            if (userFilePath != null)
            {
                var serializer = new JsonSerializer();
                serializer.NullValueHandling = NullValueHandling.Ignore;
                using (var sw = new StreamWriter(userFilePath))
                {
                    using (var writer = new JsonTextWriter(sw))
                    {
                        serializer.Serialize(writer, config);
                    }
                }
            }
        }

        public string GetUserFilePath()
        {
            return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    Constants.ConfigurationFilename);
        }
    }
}
