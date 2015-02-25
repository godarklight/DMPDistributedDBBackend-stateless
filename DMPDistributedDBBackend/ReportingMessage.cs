using System;
using System.Collections.Generic;

namespace DMPDistributedDBBackend
{
    public class ReportingMessage
    {
        public string serverHash;
        public string serverName;
        public string description;
        public int gamePort;
        public string gameAddress;
        public int protocolVersion;
        public string programVersion;
        public int maxPlayers;
        public int modControl;
        public string modControlSha;
        public int gameMode;
        public bool cheats;
        public int warpMode;
        public long universeSize;
        public string banner;
        public string homepage;
        public int httpPort;
        public string admin;
        public string team;
        public string location;
        public bool fixedIP;
        public string[] players;

        public static ReportingMessage FromBytesBE(byte[] inputBytes)
        {
            ReportingMessage returnMessage = new ReportingMessage();
            using (MessageStream2.MessageReader mr = new MessageStream2.MessageReader(inputBytes))
            {
                returnMessage.serverHash = mr.Read<string>();
                returnMessage.serverName = mr.Read<string>();
                returnMessage.description = mr.Read<string>();
                returnMessage.gamePort = mr.Read<int>();
                returnMessage.gameAddress = mr.Read<string>();
                returnMessage.protocolVersion = mr.Read<int>();
                returnMessage.programVersion = mr.Read<string>();
                returnMessage.maxPlayers = mr.Read<int>();
                returnMessage.modControl = mr.Read<int>();
                returnMessage.modControlSha = mr.Read<string>();
                returnMessage.gameMode = mr.Read<int>();
                returnMessage.cheats = mr.Read<bool>();
                returnMessage.warpMode = mr.Read<int>();
                returnMessage.universeSize = mr.Read<long>();
                returnMessage.banner = mr.Read<string>();
                returnMessage.homepage = mr.Read<string>();
                returnMessage.httpPort = mr.Read<int>();
                returnMessage.admin = mr.Read<string>();
                returnMessage.team = mr.Read<string>();
                returnMessage.location = mr.Read<string>();
                returnMessage.fixedIP = mr.Read<bool>();
                returnMessage.players = mr.Read<string[]>();
            }
            return returnMessage;
        }

        public Dictionary<string, object> GetParameters(int trackNumber)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["@id"] = trackNumber;
            parameters["@serverhash"] = serverHash;
            parameters["@serverName"] = serverName;
            if (serverName.Length > 255)
            {
                serverName = serverName.Substring(0, 255);
            }
            parameters["@description"] = description;
            parameters["@gamePort"] = gamePort;
            parameters["@gameAddress"] = gameAddress;
            parameters["@protocolVersion"] = protocolVersion;
            parameters["@programVersion"] = programVersion;
            parameters["@maxPlayers"] = maxPlayers;
            parameters["@modControl"] = modControl;
            parameters["@modControlSha"] = modControlSha;
            parameters["@gameMode"] = gameMode;
            parameters["@cheats"] = cheats;
            parameters["@warpMode"] = warpMode;
            parameters["@universeSize"] = universeSize;
            parameters["@banner"] = banner;
            parameters["@homepage"] = homepage;
            parameters["@httpPort"] = httpPort;
            parameters["@admin"] = admin;
            parameters["@team"] = team;
            parameters["@location"] = location;
            parameters["@fixedIP"] = fixedIP;
            parameters["@players"] = String.Join(", ", players);
            parameters["@playerCount"] = players.Length;
            return parameters;
        }
    }
}

