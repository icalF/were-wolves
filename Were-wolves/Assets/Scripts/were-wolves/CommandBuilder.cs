using Newtonsoft.Json;
using System.Collections.Generic;

namespace WereWolves
{
    public class CommandBuilder
    {
        public static void Main() { }

        private Dictionary<string, object> command;

        public CommandBuilder ()
        {
            command = new Dictionary<string, object> ();
        }

        // General response
        public CommandBuilder response (int status, string desc)
        {
            if (command.Count > 0) 
                Clear();

            command.Add("status", status > 0 ? "fail" : status != 0 ? "error" : "ok");

            if (desc != null)
                command.Add("description", desc);
            return this;
        }

        // Join game
        public CommandBuilder join (string username, string address, short port)
        {
            if (command.Count > 0) 
                Clear();

            command.Add("method", "join");
            command.Add("username", username);
            command.Add("udp_address", address);
            command.Add("udp_port", port);
            return this;
        }
        public CommandBuilder joinResp (int status, object desc)
        {
            if (command.Count > 0) 
                Clear();

            command.Add("status", status > 0 ? "fail" : status != 0 ? "error" : "ok");
            if (status == 0)
                command.Add("player_id", desc);
            else
                command.Add("description", desc);
            return this;
        }

        // Leave game
        public CommandBuilder leave ()
        {
            if (command.Count > 0) 
                Clear();

            command.Add("method", "leave");
            return this;
        }

        // Ready game
        public CommandBuilder ready ()
        {
            if (command.Count > 0) 
                Clear();

            command.Add("method", "ready");
            return this;
        }

        // Game over
        public CommandBuilder over (string winner, string desc)
        {
            if (command.Count > 0) 
                Clear();

            command.Add("method", "game_over");
            command.Add("winner", winner);
            command.Add("description", desc);
            return this;
        }

        // Change phase
        public CommandBuilder change (bool night, string desc, int days)
        {
            if (command.Count > 0) 
                Clear();

            command.Add("method", "change_phase");
            command.Add("time", night ? "night" : "day");
            command.Add("days", days);
            command.Add("description", desc);
            return this;
        }

        // Start game
        public CommandBuilder start (bool night, string desc, string role, object friends)
        {
            if (command.Count > 0) 
                Clear();

            command.Add("method", "start");
            command.Add("time", night ? "night" : "day");
            command.Add("role", role);
            command.Add("description", desc);

            if (friends != null)
              command.Add("friend", friends);
            return this;
        }

        // Player list 
        public CommandBuilder listClient ()
        {
            if (command.Count > 0) 
                Clear();

            command.Add("method", "client_address");
            return this;
        }
        public CommandBuilder listClientResp (int status, string desc, object clientList)
        {
            if (command.Count > 0) 
                Clear();

            command.Add("status", status > 0 ? "fail" : status != 0 ? "error" : "ok");
            command.Add("description", desc);

            if (clientList != null)
              command.Add("clients", clientList);
            return this;
        }

        // Kill civilian 
        public CommandBuilder killCiv (int id)
        {
            if (command.Count > 0) 
                Clear();

            command.Add("method", "vote_civilian");
            command.Add("player_id", id);
            return this;
        }

        // Kill werewolf 
        public CommandBuilder killWere (int id)
        {
            if (command.Count > 0) 
                Clear();

            command.Add("method", "vote_werewolf");
            command.Add("player_id", id);
            return this;
        }

        // Paxos prepare proposal
        public CommandBuilder propose (int kpu, int id)
        {
            if (command.Count > 0) 
                Clear();

            command.Add("method", "prepare_proposal");
            command.Add("kpu_id", kpu);
            command.Add("proposal_id", new Tuple(1, id));
            return this;
        }
        public CommandBuilder proposeResp (int status, string desc, int prev)
        {
            if (command.Count > 0) 
                Clear();

            command.Add("status", status > 0 ? "fail" : status != 0 ? "error" : "ok");
            command.Add("description", desc);
            command.Add("previous_accepted", prev);
            return this;
        }

        // Paxos accept proposal
        public CommandBuilder accept (int kpu, int id)
        {
            if (command.Count > 0) 
                Clear();

            command.Add("method", "accept_proposal");
            command.Add("kpu_id", kpu);
            command.Add("proposal_id", new Tuple(1, id));
            return this;
        }

        // Client accept proposal
        public CommandBuilder clientAccept (int id)
        {
            if (command.Count > 0) 
                Clear();

            command.Add("method", "prepare_proposal");
            command.Add("kpu_id", id);
            command.Add("Description", "Kpu is selected");
            return this;
        }

        // Kill civilian result
        public CommandBuilder killCivRes (int id, object res)
        {
            if (command.Count > 0) 
                Clear();

            bool valid = id > 0;
            command.Add("method", "vote_result" + (valid ? "_civilian" : ""));
            command.Add("vote_status", valid ? 1 : -1);
            if (valid)
                command.Add("player_id", id);
            command.Add("vote_result", res);
            return this;
        }

        // Kill werewolf result
        public CommandBuilder killWereRes (int id, object res)
        {
            if (command.Count > 0) 
                Clear();

            bool valid = id > 0;
            command.Add("method", "vote_result" + (valid ? "_werewolf" : ""));
            command.Add("vote_status", valid ? 1 : -1);
            if (valid)
                command.Add("player_id", id);
            command.Add("vote_result", res);
            return this;
        }

        public string build()
        {   
            string jsonCom = JsonConvert.SerializeObject(command);
            return jsonCom;
        }

        private void Clear()
        {
            command.Clear();
        }
    }
}
