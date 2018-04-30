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

/*
    About the WUERFELWARE.TXT:
  
    The file was downloaded from:
    http://world.std.com/~reinhold/diceware_german.txt
    "A German word list provided by Benjamin Tenne under the terms of the GNU General Public License."

    It was linked here:
    http://world.std.com/~reinhold/diceware.html

    I did not alter its content, just renamed the file.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;

using KeePassLib;
using KeePassLib.Cryptography;
using KeePassLib.Cryptography.PasswordGenerator;
using KeePassLib.Security;
using PwGenWuerfelware.Configuration;
using PwGenWuerfelware.Forms;
using System.Windows.Forms;
using System.IO;
using PwGenWuerfelware.Classes;

namespace PwGenWuerfelware
{
    /// <summary>
    /// Generates passphrases consisting of multiple words.
    /// </summary>
    /// <remarks>
    /// Generates passphrases. Here are some infos on background and principle:
    /// - https://en.wikipedia.org/wiki/Diceware
    /// - https://xkcd.com/936/
    /// 
    /// The passphrases look like this (using a German word set):
    /// 
    /// spund etws wuchs zagreb freund floez
    /// rasant salbe wenn sehne yy drops
    /// stinkt edwin weizen unreif erlass vl
    /// kandis hobel kalk etappe kauer strick
    /// 
    /// Working principle of the password generator:
    /// - load text file with set of words
    /// - remove any whitespace and line breaks as well as numbers
    /// - choose a set of words from the remaining entries
    /// - the set size is configurable
    /// 
    /// There are no "virtual dice" but only random numbers used to choose from the list of words. The word list can contain
    /// numbers but they will be ignored. You can basically use any file containing space delimited words.
    /// 
    /// TODO:
    /// - optimize file list handling - currently the list is loaded for _every_ generated password
    /// - more error handling - currently errors crash Keepass
    /// - add profile support - currently there is only one configuration and one configuration file
    /// - more configuration options (maximum number of characters, multiple languages etc.) - those only make sense with profile support
    /// - internationalization - currently all texts are static and English
    /// - add options to include special characters for sites _insisting_ that it's only safe if it contains those...
    /// - add optional length limit for sites having such (...)
    /// </remarks>
    public sealed class WuerfelwareGen : CustomPwGenerator
    {
        private WuerfelwareFileReader WuerfelwareFileReader;
        private static readonly PwUuid m_uuid = new PwUuid(new byte[] { 0x72, 0x4E, 0xF7, 0xCD, 0x84, 0x47, 0x0B, 0x4B, 0x88, 0xE4, 0x2B, 0x6D, 0xDD, 0x98, 0xE6, 0x79 });

        public override PwUuid Uuid => m_uuid;
        public override string Name => "Würfelware - Passphrases for everybody!!1";

        /// <summary>
        /// Get path of file containing words for passphrases.
        /// </summary>
        /// <returns></returns>
        private string getWuerfelwareFilePath()
        {
            /** 
             * Sample windows path: 
             *   CodeBase path:     file:\C:\User Files\Portable\KeePass\Plugins\Wuerfelware
             *   As Uri:            file:///C:/User Files/Portable/KeePass/Plugins/Wuerfelware
             *   Uri.LocalPath:     C:\User Files\Portable\KeePass\Plugins\Wuerfelware
             *sample linux path (Keepass with Mono): 
             *   CodeBase path: file:/home/heinrich/Keepass/Plugins/Wuerfelware
             */
            var path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            string assemblyFolder;
            try
            {
                // tbd: possible cause for error if path contains "#" - but we'll see this easily in the configuration UI and fix it if somebody trips over this
                assemblyFolder = new Uri(path).LocalPath;
            } catch (UriFormatException)
            {
                // oh well - maybe somebody runs us on Linux?
                if (path.StartsWith("file:/"))
                {
                    // ok we'll try to handle this
                    assemblyFolder = path.Substring("file:".Length);
                } else
                {
                    // nope, this is something we don't recognize - let's fail
                    throw;
                }
            }
            return Path.Combine(assemblyFolder, Constants.WuerfelwareFilename);
        }

        /// <summary>
        /// Initialize the reader responsible for reading the words file.
        /// </summary>
        private void InitWuerfelwareFileReaderIfNecessary()
        {
            var wuerfelwareFilePath = getWuerfelwareFilePath();
            // the file list is loaded for _every_ generated password
            // todo: optimize this, but don't introduce caching related security flaws
            if (WuerfelwareFileReader == null || string.Compare(WuerfelwareFileReader.FilePath, wuerfelwareFilePath) != 0)
            {
                WuerfelwareFileReader = new WuerfelwareFileReader(wuerfelwareFilePath);
                WuerfelwareFileReader.LoadList();
            }
        }

        /// <summary>
        /// Get a random value from 0..maxValue_Exclusive-1.
        /// 
        /// This methods prevents mod bias - see here for more details: https://stackoverflow.com/a/10989061/56658
        /// 
        /// </summary>
        /// <param name="crsRandomSource">Keepass random number generator</param>
        /// <param name="maxValue_Exclusive">Upper range for random numbers; result will be in range 0..<paramref name="maxValue_Exclusive"/>-1</param>
        /// <returns></returns>
        private ulong GetRandomIndex(CryptoRandomStream crsRandomSource, ulong maxValue_Exclusive)
        {
            var RAND_MAX = UInt64.MaxValue;
            ulong maxValid = RAND_MAX - (RAND_MAX % maxValue_Exclusive);
            UInt64 x;
            // Keep searching for an x in a range divisible by maxValid
            do
            {
                x = crsRandomSource.GetRandomUInt64();
            } while (x >= maxValid);

            return x % maxValue_Exclusive;
        }

        public override ProtectedString Generate(PwProfile prf, CryptoRandomStream crsRandomSource)
        {
            if (prf == null) { Debug.Assert(false); }
            else
            {
                Debug.Assert(prf.CustomAlgorithmUuid == Convert.ToBase64String(
                    m_uuid.UuidBytes, Base64FormattingOptions.None));
            }

            var persistence = new ConfigurationPersistence();
            var config = persistence.LoadFromUserFile();
            InitWuerfelwareFileReaderIfNecessary();
            // no entries?
            if (WuerfelwareFileReader.MaxValidIndex < 0)
            {
                throw new IndexOutOfRangeException("Word list ist empty");
            }

            var resultingWords = new List<string>();
            for (var i = 0; i < Math.Min(Math.Max(Constants.MinWordsPerPassphrase, config.WordCount), Constants.MaxWordsPerPassphrase); i++)
            {
                var index = GetRandomIndex(crsRandomSource, (ulong)WuerfelwareFileReader.EntryCount); // note: we checked earlier that entryCount is >= 0
                // index cannot be higher than int.MaxValue since MaxIndex is of type int
                resultingWords.Add(WuerfelwareFileReader.GetEntry((int)index));
            }

            return new ProtectedString(false, string.Join(" ", resultingWords.ToArray()));
        }

        public override bool SupportsOptions => true;

        /// <summary>
        /// Show options dialog.
        /// </summary>
        /// <remarks>
        /// Currently the <paramref name="strCurrentOptions"/> parameter will be ignored.
        /// </remarks>
        /// <param name="strCurrentOptions">Seems to be the Keepass profile name</param>
        /// <returns></returns>
        public override string GetOptions(string strCurrentOptions)
        {
            try
            {
                InitWuerfelwareFileReaderIfNecessary();
                var persistence = new ConfigurationPersistence();
                var config = persistence.LoadFromUserFile();
                if (null == config)
                {
                    config = new ConfigurationModel();
                }
                var form = new FormConfiguration(config);
                form.setData(WuerfelwareFileReader.FilePath, WuerfelwareFileReader.EntryCount);
                form.ShowDialog();
                persistence.SaveToUserFile(form.Configuration);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error while loading or saving properties.\n\nError message: '" + e.Message + "'\n\nError details: " + e.ToString());
            }
            return base.GetOptions(strCurrentOptions);
        }
    }
}
