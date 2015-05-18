using Lib_K_Relay;
using Lib_K_Relay.Interface;
using Lib_K_Relay.Networking;
using Lib_K_Relay.Networking.Packets;
using Lib_K_Relay.Networking.Packets.Client;
using Lib_K_Relay.Networking.Packets.DataObjects;
using Lib_K_Relay.Networking.Packets.Server;
using Lib_K_Relay.Utilities;
using System;

namespace DailyQ
{
    public class DailyQ:IPlugin
    {

        public string GetAuthor()
        {
            return "Plann";
        }

        public string[] GetCommands()
        {
            return new string[] { "/daily" };
        }

        public string GetDescription()
        {
            return "With this command you get the name of your current daily quest";
        }

        public string GetName()
        {
            return "DailyQ";
        }

        public void Initialize(Proxy proxy)
        {
            proxy.HookCommand("daily", HandlePluginCommand);
            proxy.HookPacket(PacketType.QUESTFETCHRESPONSE, HandleQuestFetchResponse);
        }

        private void HandleQuestFetchResponse(Client client, Packet packet)
        {
            QuestFetchResponsePacket qfr = (QuestFetchResponsePacket)packet;
            string needed_id = qfr.Goal;
            string clientMessage = GetClientMessageFromId(needed_id);
            client.SendToClient(PluginUtils.CreateNotification(client.ObjectId, clientMessage));
            Log(client.PlayerData.Name + " got message: " + clientMessage);
        }

        private string GetClientMessageFromId(string needed_id)
        {
            string message = "No quest available";
            foreach (string n in Serializer.Items.Keys)
            {
                if (Serializer.Items[n].ToString() == needed_id)
                {
                    message = "Quest: " + n;
                    break;
                }
            }
            return message;
        }

        private void HandlePluginCommand(Client client, string command, string[] args)
        {
            ViewQuestsPacket vq = (ViewQuestsPacket)Packet.Create(PacketType.VIEWQUESTS);
            client.SendToServer(vq);
            Log(client.PlayerData.Name + " requested his daily quest");
        }

        private void Log(string message)
        {
            Console.WriteLine("[DailyQ]" + message);
        }
    }
}
