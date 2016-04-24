using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WereWolves
{
    Dictionary<string, object> command;

    class CommandBuilder
    {
        public CommandBuilder ()
        {
            command = new Dictionary<string, object> ();
        }

        // General response
        public void response (int status, string desc)
        {
            if (command.IsNullOrEmpty()) 
                Clear();

            command.Add("status", status > 0 ? "fail" : status != 0 ? "error" : "ok");

            if (desc != null)
                command.Add("description", desc);
        }

        // Join game
        public void join (string username)
        {
            if (command.IsNullOrEmpty()) 
                Clear();

            command.Add("method", "join");
            command.Add("username", username);
        }
        public void joinResp (int status, object desc)
        {
            if (command.IsNullOrEmpty()) 
                Clear();

            command.Add("status", status > 0 ? "fail" : status != 0 ? "error" : "ok");
            if (status == 0)
                command.Add("player_id", desc);
            else
                command.Add("description", desc);
        }

        // Leave game
        public void leave ()
        {
            if (command.IsNullOrEmpty()) 
                Clear();

            command.Add("method", "leave");
        }

        // Ready game
        public void ready ()
        {
            if (command.IsNullOrEmpty()) 
                Clear();

            command.Add("method", "ready");
        }

        // Game over
        public void over (string winner, string desc)
        {
            if (command.IsNullOrEmpty()) 
                Clear();

            command.Add("method", "game_over");
            command.Add("winner", winner);
            command.Add("description", desc);
        }

        // Change phase
        public void change (string time, string desc, int days)
        {
            if (command.IsNullOrEmpty()) 
                Clear();

            command.Add("method", "change_phase");
            command.Add("time", time);
            command.Add("days", days);
            command.Add("description", desc);
        }

        // Start game
        public void start (string time, string desc, string role, object friends)
        {
            if (command.IsNullOrEmpty()) 
                Clear();

            command.Add("method", "start");
            command.Add("time", time);
            command.Add("role", role);
            command.Add("description", desc);

            if (friends != null)
              command.Add("friend", friends);
        }

        // Player list 
        public void listClient ()
        {
            if (command.IsNullOrEmpty()) 
                Clear();

            command.Add("method", "client_address");
        }
        public void listClientResp (int status, string desc, object clientList)
        {
            if (command.IsNullOrEmpty()) 
                Clear();

            command.Add("status", status > 0 ? "fail" : status != 0 ? "error" : "ok");
            command.Add("description", desc);

            if (clientList != null)
              command.Add("clients", clientList);
        }

        // Kill civilian 
        public void killCiv (int id)
        {
            if (command.IsNullOrEmpty()) 
                Clear();

            command.Add("method", "vote_civilian");
            command.Add("player_id", id);
        }

        // Kill werewolf 
        public void killWere (int id)
        {
            if (command.IsNullOrEmpty()) 
                Clear();

            command.Add("method", "vote_werewolf");
            command.Add("player_id", id);
        }

        // Paxos prepare proposal
        public void propose (int kpu, int id)
        {
            if (command.IsNullOrEmpty()) 
                Clear();

            command.Add("method", "prepare_proposal");
            command.Add("kpu_id", kpu_id);
            command.Add("proposal_id", tuple(1, id));
        }
        public void proposeResp (int status, string desc, int prev)
        {
            if (command.IsNullOrEmpty()) 
                Clear();

            command.Add("status", status > 0 ? "fail" : status != 0 ? "error" : "ok");
            command.Add("description", desc);
            command.Add("previous_accepted", prev);
        }

        // Paxos accept proposal
        public void accept (int kpu, int id)
        {
            if (command.IsNullOrEmpty()) 
                Clear();

            command.Add("method", "accept_proposal");
            command.Add("kpu_id", kpu_id);
            command.Add("proposal_id", tuple(1, id));
        }

        // Client accept proposal
        public void clientAccept (int id)
        {
            if (command.IsNullOrEmpty()) 
                Clear();

            command.Add("method", "prepare_proposal");
            command.Add("kpu_id", id);
            command.Add("Description", "Kpu is selected");
        }

        // Kill civilian result
        public void killCivRes (int id, object res)
        {
            if (command.IsNullOrEmpty()) 
                Clear();

            command.Add("method", "vote_result_civilian");
            command.Add("vote_status", id > 0 ? 1 : -1);
            if (id > 0)
                command.Add("player_id", id);
            command.Add("vote_result", res);
        }

        // Kill werewolf result
        public void killWereRes (int id, object res)
        {
            if (command.IsNullOrEmpty()) 
                Clear();

            command.Add("method", "vote_result_werewolf");
            command.Add("vote_status", id > 0 ? 1 : -1);
            if (id > 0)
                command.Add("player_id", id);
            command.Add("vote_result", res);
        }

        public string build ()
        {   
            string jsonCom = JsonConvert.SerializeObject(command);
        }

        private void Clear()
        {
            command.Clear();
        }
    }
}
