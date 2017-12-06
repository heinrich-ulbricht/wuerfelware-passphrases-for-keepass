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

using System;
using System.Collections.Generic;
using System.IO;

namespace PwGenWuerfelware
{
    /// <summary>
    /// Read words from a file.
    /// </summary>
    /// <remarks>
    /// Reads words from a text file. The text file can contain any list of words or text. Pure Numbers (like "123", "ab12cd" is fine) will be ignored.
    /// </remarks>
    public class WuerfelwareFileReader
    {
        public List<string> Entries = new List<string>();
        public string FilePath { get; private set; }
        public WuerfelwareFileReader(string filePath) => FilePath = filePath;

        public void LoadList()
        {
            if (!File.Exists(FilePath))
            {
                throw new FileNotFoundException("Cannot find word file at '" + FilePath + "'. Maybe you forgot copying it to the plugin directory?");
            }

            var content = File.ReadAllText(FilePath);
            // get every word, split at certain whitespacey characters
            var entryCandidates = content.Split(new string[] { " ", Environment.NewLine, "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
            Entries.Clear();
            foreach (var cand in entryCandidates)
            {
                // ignore numbers
                if (!int.TryParse(cand, out int dummy))
                {
                    Entries.Add(cand);
                }
            }
        }

        public string GetEntry(int index)
        {
            if (index >= Entries.Count || index < 0)
            {
                throw new IndexOutOfRangeException("Entry index out of range");
            }
            return Entries[index];
        }

        public int MaxValidIndex { get { return Entries.Count - 1; } }
        public int EntryCount { get { return Entries.Count; } }
    }
}
