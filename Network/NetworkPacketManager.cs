using System;
using System.Collections.Generic;
using System.IO;

namespace TerrarianBizzareAdventure.Network
{
    public sealed class NetworkPacketManager
    {
        private static NetworkPacketManager _instance;

        private byte _latestPacketTypeId = 1;
        private readonly Dictionary<byte, NetworkPacket> _networkPacketsById = new Dictionary<byte, NetworkPacket>();
        private readonly Dictionary<Type, NetworkPacket> _networkPacketsByType = new Dictionary<Type, NetworkPacket>();

        internal void DefaultInitialize()
        {

            Initialized = true;
        }


        public NetworkPacket Add(NetworkPacket networkPacket)
        {
            if (_networkPacketsById.ContainsValue(networkPacket))
                return _networkPacketsByType[networkPacket.GetType()];

            _networkPacketsById.Add(_latestPacketTypeId, networkPacket);
            _networkPacketsByType.Add(networkPacket.GetType(), networkPacket);

            networkPacket.PacketType = _latestPacketTypeId;
            _latestPacketTypeId++;

            return networkPacket;
        }

        public void HandlePacket(BinaryReader reader, int fromWho)
        {
            byte packetType = reader.ReadByte();

            _networkPacketsById[packetType].Receive(reader, fromWho);
        }


        internal void Unload()
        {
            _networkPacketsById.Clear();
            _networkPacketsByType.Clear();

            _instance = null;
        }


        public bool Initialized { get; private set; }

        public NetworkPacket this[byte packetType] => _networkPacketsById[packetType];


        public static NetworkPacketManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new NetworkPacketManager();

                if (!_instance.Initialized)
                    _instance.DefaultInitialize();

                return _instance;
            }
        }
    }
}