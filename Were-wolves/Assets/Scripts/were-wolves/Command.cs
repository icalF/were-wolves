namespace WereWolves
{
    class Command
    {
        private string method;
        private string username;
        private int player_id;
        private string status;
        private string description;
        private string vote_status;
        private string vote_result;
        private ClientData[] clients;
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
