using System;
using System.Threading;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace DMPDistributedDBBackend
{
    public class DatabaseDriver
    {
        private DatabaseConnection databaseConnection;
        private int freeID = 0;
        private Dictionary<ReferenceID, int> trackID = new Dictionary<ReferenceID, int>();


        public DatabaseDriver(DatabaseConnection databaseConnection)
        {
            this.databaseConnection = databaseConnection;
            SQLCleanupDatabase();
        }

        public void HandleConnect(string serverID, int clientID, string remoteAddress, int remotePort)
        {

            ReferenceID thisReference = new ReferenceID(serverID, clientID);
            trackID.Add(thisReference, Interlocked.Increment(ref freeID));
            int trackNumber = trackID[thisReference];
            SQLConnect(trackNumber, remoteAddress);
        }

        public void HandleReport(string serverID, int clientID, ReportingMessage reportMessage)
        {
            ReferenceID thisReference = new ReferenceID(serverID, clientID);
            int trackNumber = trackID[thisReference];
            SQLReport(trackNumber, reportMessage);
        }

        public void HandleDisconnect(string serverID, int clientID)
        {
            ReferenceID thisReference = new ReferenceID(serverID, clientID);
            int trackNumber = trackID[thisReference];
            SQLDisconnect(trackNumber);
            trackID.Remove(thisReference);
        }

        private void SQLConnect(int trackNumber, string remoteAddress)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("id", trackNumber);
            parameters.Add("serverName", "CONNECTING");
            parameters.Add("gameAddress", remoteAddress);
            parameters.Add("maxPlayers", 0);
            parameters.Add("playerCount", 0);
            databaseConnection.ExecuteNonReader("INSERT INTO DMPServerList (id, serverName, gameAddress, maxPlayers, playerCount) VALUES (@id, @serverName, @gameAddress, @maxPlayers, @playerCount)", parameters);
        }

        private void SQLReport(int trackNumber, ReportingMessage reportMessage)
        {
            Dictionary<string, object> parameters = reportMessage.GetParameters(trackNumber);
            //Build SQL text
            string sqlText = "UPDATE DMPServerList SET ";
            sqlText += "`serverHash`=@serverHash, ";
            sqlText += "`serverName`=@serverName, ";
            sqlText += "`description`=@description, ";
            sqlText += "`gamePort`=@gamePort, ";
            sqlText += "`gameAddress`=@gameAddress, ";
            sqlText += "`protocolVersion`=@protocolVersion, ";
            sqlText += "`programVersion`=@programVersion, ";
            sqlText += "`maxPlayers`=@maxPlayers, ";
            sqlText += "`modControl`=@modControl, ";
            sqlText += "`modControlSha`=@modControlSha, ";
            sqlText += "`gameMode`=@gameMode, ";
            sqlText += "`cheats`=@cheats, ";
            sqlText += "`warpMode`=@warpMode, ";
            sqlText += "`universeSize`=@universeSize, ";
            sqlText += "`banner`=@banner, ";
            sqlText += "`homepage`=@homepage, ";
            sqlText += "`httpPort`=@httpPort, ";
            sqlText += "`admin`=@admin, ";
            sqlText += "`team`=@team, ";
            sqlText += "`location`=@location, ";
            sqlText += "`fixedIP`=@fixedIP, ";
            sqlText += "`players`=@players, ";
            sqlText += "`playerCount`=@playerCount ";
            sqlText += "WHERE `id`=@id";
            //Run SQL
            try
            {
                databaseConnection.ExecuteNonReader(sqlText, parameters);
            }
            catch (Exception e)
            {
                Console.WriteLine("WARNING: Ignoring error on report, error: " + e);
            }
        }

        private void SQLDisconnect(int trackNumber)
        {
            Dictionary<string, object> offlineParams = new Dictionary<string, object>();
            offlineParams["@id"] = trackNumber;
            string mySql = "DELETE FROM DMPServerList WHERE `id` = @id";
            try
            {
                databaseConnection.ExecuteNonReader(mySql, offlineParams);
            }
            catch (Exception e)
            {
                Console.WriteLine("WARNING: Ignoring error on disconnect, error: " + e.Message);
            }
        }

        public void SQLCleanupDatabase()
        {
            try
            {
                databaseConnection.ExecuteNonReader("CREATE TABLE IF NOT EXISTS DMPServerList (id INT PRIMARY KEY, serverHash VARCHAR(255), serverName VARCHAR(255), description TEXT, gamePort INT, gameAddress VARCHAR(255), protocolVersion INT, programVersion VARCHAR(255), maxPlayers INT, modcontrol INT, modControlSha VARCHAR(255), gameMode INT, cheats INT, warpMode INT, universeSize INT, banner VARCHAR(255), homepage VARCHAR(255), httpPort INT, admin VARCHAR(255),team VARCHAR(255), location VARCHAR(255), fixedIP INT, players TEXT, playerCount INT) ENGINE=InnoDB DEFAULT CHARSET=utf8", null);
                databaseConnection.ExecuteNonReader("DELETE FROM DMPServerList");
            }
            catch (Exception e)
            {
                Console.WriteLine("WARNING: Ignoring error on cleanup, error: " + e.Message);
            }
        }

        private struct ReferenceID
        {
            public readonly string serverID;
            public readonly int clientID;

            public ReferenceID(string serverID, int clientID)
            {
                this.serverID = serverID;
                this.clientID = clientID;
            }

            public override bool Equals(object obj)
            {
                if (obj is ReferenceID)
                {
                    ReferenceID rhs = (ReferenceID)obj;
                    return (serverID == rhs.serverID && clientID == rhs.clientID);
                }
                return false;
            }

            public override int GetHashCode()
            {
                return (serverID + clientID).GetHashCode();
            }
        }
    }
}

