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

using KeePass.Plugins;

namespace PwGenWuerfelware
{
    public sealed class PwGenWuerfelwareExt : Plugin
    {
        private IPluginHost m_host = null;
        private WuerfelwareGen m_gen = null;

        public override bool Initialize(IPluginHost host)
        {
            if (host == null)
                return false;
            m_host = host;

            m_gen = new WuerfelwareGen();
            m_host.PwGeneratorPool.Add(m_gen);

            return true;
        }

        public override void Terminate()
        {
            if (m_host != null)
            {
                m_host.PwGeneratorPool.Remove(m_gen.Uuid);
                m_gen = null;
                m_host = null;
            }
        }
    }
}
