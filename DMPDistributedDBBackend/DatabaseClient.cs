﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using DarkNetwork;
using MessageStream2;

namespace DMPDistributedDBBackend
{
    public class DatabaseClient
    {
        private bool connected;
        private NetworkClient<object> networkClient;
        private DatabaseDriver databaseDriver;
        private IPEndPoint currentEndpoint;

        public DatabaseClient(TcpClient connectedConnection, DatabaseDriver databaseDriver)
        {
            connected = true;
            currentEndpoint = (IPEndPoint)connectedConnection.Client.RemoteEndPoint;
            this.databaseDriver = databaseDriver;
            NetworkHandler<object> networkHandler = new NetworkHandler<object>();
            networkHandler.SetHeartbeatCallback(HandleSendHeartbeat, 5000, 20000);
            networkHandler.SetDisconnectCallback(HandleDisconnect);
            networkHandler.SetMessageCallback((int)MessageType.HEARTBEAT, HandleHeartbeat);
            networkHandler.SetMessageCallback((int)MessageType.CONNECT, HandleConnect);
            networkHandler.SetMessageCallback((int)MessageType.REPORT, HandleReport);
            networkHandler.SetMessageCallback((int)MessageType.DISCONNECT, HandleDisconnect);
            networkClient = new NetworkClient<object>(networkHandler, connectedConnection, false);
            if (networkClient == null)
            {
                //Shutup compiler.
            }
        }

        private NetworkMessage HandleSendHeartbeat(NetworkClient<object> client)
        {
            return new NetworkMessage((int)MessageType.HEARTBEAT, null);
        }

        private void HandleHeartbeat(NetworkClient<object> client, byte[] data)
        {
            //Don't care
        }

        private void HandleConnect(NetworkClient<object> client, byte[] data)
        {
            using (MessageReader mr = new MessageReader(data))
            {
                string serverID = mr.Read<string>();
                int clientID = mr.Read<int>();
                string remoteAddress = mr.Read<string>();
                int remotePort = mr.Read<int>();
                databaseDriver.HandleConnect(serverID, clientID, remoteAddress, remotePort);
                Console.WriteLine("Server " + serverID + ":" + clientID + " connected from " + remoteAddress + " port " + remotePort);
            }
        }

        private void HandleReport(NetworkClient<object> client, byte[] data)
        {
            using (MessageReader mr = new MessageReader(data))
            {
                string serverID = mr.Read<string>();
                int clientID = mr.Read<int>();
                byte[] reportBytes = mr.Read<byte[]>();
                ReportingMessage reportMessage = ReportingMessage.FromBytesBE(reportBytes);
                databaseDriver.HandleReport(serverID, clientID, reportMessage);
                Console.WriteLine("Server " + serverID + ":" + clientID + " reported new state");
            }
        }

        private void HandleDisconnect(NetworkClient<object> client, byte[] data)
        {
            using (MessageReader mr = new MessageReader(data))
            {
                string serverID = mr.Read<string>();
                int clientID = mr.Read<int>();
                databaseDriver.HandleDisconnect(serverID, clientID);
                Console.WriteLine("Server " + serverID + ":" + clientID + " disconnected");
            }
        }

        private void HandleDisconnect(NetworkClient<object> client, Exception disconnectException)
        {
            if (disconnectException != null)
            {
                Console.WriteLine("Connection error: " + disconnectException);
            }
            connected = false;
        }

        public void Run()
        {
            while (connected)
            {
                Thread.Sleep(1000);
            }
        }

        public void Disconnect()
        {
            networkClient.Disconnect();
        }

        public void DisplayEndpoint()
        {
            Console.WriteLine("Currently connected to endpoint at " + currentEndpoint);
        }

        private enum MessageType
        {
            HEARTBEAT,
            CONNECT,
            REPORT,
            DISCONNECT,           
        }
    }
}

