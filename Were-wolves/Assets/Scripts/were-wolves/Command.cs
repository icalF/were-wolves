using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WereWolves
{
    class Command
    {
        private string method;
        private string username;
        private int player_id;
        private string status;
        private string description;
        private string method;
        private string vote_status;
        private string vote_result;
        private Client[] clients;
        private int proposal_id;

        public Command ()
        {
            // bind to socket
        }

        public void send ()
        {

        }
    }
}
