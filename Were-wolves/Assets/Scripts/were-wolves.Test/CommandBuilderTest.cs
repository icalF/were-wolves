using NUnit.Framework;

namespace WereWolves.Tests
{
    [TestFixture]
    public class CommandBuilderTest
    {
        // General response
        [Test]
        public void responseTestOk()
        {
            CommandBuilder sut = new CommandBuilder();
            string result = sut.response(0, null).build();
            string expectedResult = "{\"status\":\"ok\"}";
            Assert.That(expectedResult, Is.EqualTo(result));
        }
        [Test]
        public void responseTestFail()
        {
            CommandBuilder sut = new CommandBuilder();
            string result = sut.response(1, "Fail").build();
            string expectedResult = "{\"status\":\"fail\",\"description\":\"Fail\"}";
            Assert.That(expectedResult, Is.EqualTo(result));
        }
        [Test]
        public void responseTestError()
        {
            CommandBuilder sut = new CommandBuilder();
            string result = sut.response(-1, "Error").build();
            string expectedResult = "{\"status\":\"error\",\"description\":\"Error\"}";
            Assert.That(expectedResult, Is.EqualTo(result));
        }

        //// Join game
        //[Test]
        //public void joinTest(string username)
        //{
        //    if (command.Count > 1)
        //        Clear();

        //    command.Add("method", "join");
        //    command.Add("username", username);
        //}
        //[Test]
        //public void joinRespTest(int status, object desc)
        //{
        //    if (command.Count > 1)
        //        Clear();

        //    command.Add("status", status > 0 ? "fail" : status != 0 ? "error" : "ok");
        //    if (status == 0)
        //        command.Add("player_id", desc);
        //    else
        //        command.Add("description", desc);
        //}

        //// Leave game
        //[Test]
        //public void leaveTest()
        //{
        //    if (command.Count > 1)
        //        Clear();

        //    command.Add("method", "leave");
        //}

        //// Ready game
        //[Test]
        //public void readyTest()
        //{
        //    if (command.Count > 1)
        //        Clear();

        //    command.Add("method", "ready");
        //}

        //// Game over
        //[Test]
        //public void overTest(string winner, string desc)
        //{
        //    if (command.Count > 1)
        //        Clear();

        //    command.Add("method", "game_over");
        //    command.Add("winner", winner);
        //    command.Add("description", desc);
        //}

        //// Change phase
        //[Test]
        //public void changeTest(string time, string desc, int days)
        //{
        //    if (command.Count > 1)
        //        Clear();

        //    command.Add("method", "change_phase");
        //    command.Add("time", time);
        //    command.Add("days", days);
        //    command.Add("description", desc);
        //}

        //// Start game
        //[Test]
        //public void startTest(string time, string desc, string role, object friends)
        //{
        //    if (command.Count > 1)
        //        Clear();

        //    command.Add("method", "start");
        //    command.Add("time", time);
        //    command.Add("role", role);
        //    command.Add("description", desc);

        //    if (friends != null)
        //        command.Add("friend", friends);
        //}

        //// Player list 
        //[Test]
        //public void listClientTest()
        //{
        //    if (command.Count > 1)
        //        Clear();

        //    command.Add("method", "client_address");
        //}
        //[Test]
        //public void listClientRespTest(int status, string desc, object clientList)
        //{
        //    if (command.Count > 1)
        //        Clear();

        //    command.Add("status", status > 0 ? "fail" : status != 0 ? "error" : "ok");
        //    command.Add("description", desc);

        //    if (clientList != null)
        //        command.Add("clients", clientList);
        //}

        //// Kill civilian 
        //[Test]
        //public void killCivTest(int id)
        //{
        //    if (command.Count > 1)
        //        Clear();

        //    command.Add("method", "vote_civilian");
        //    command.Add("player_id", id);
        //}

        //// Kill werewolf 
        //[Test]
        //public void killWereTest(int id)
        //{
        //    if (command.Count > 1)
        //        Clear();

        //    command.Add("method", "vote_werewolf");
        //    command.Add("player_id", id);
        //}

        //// Paxos prepare proposal
        //[Test]
        //public void proposeTest(int kpu, int id)
        //{
        //    if (command.Count > 1)
        //        Clear();

        //    command.Add("method", "prepare_proposal");
        //    command.Add("kpu_id", kpu);
        //    command.Add("proposal_id", new Tuple<int, int>(1, id));
        //}
        //[Test]
        //public void proposeRespTest(int status, string desc, int prev)
        //{
        //    if (command.Count > 1)
        //        Clear();

        //    command.Add("status", status > 0 ? "fail" : status != 0 ? "error" : "ok");
        //    command.Add("description", desc);
        //    command.Add("previous_accepted", prev);
        //}

        //// Paxos accept proposal
        //[Test]
        //public void acceptTest(int kpu, int id)
        //{
        //    if (command.Count > 1)
        //        Clear();

        //    command.Add("method", "accept_proposal");
        //    command.Add("kpu_id", kpu);
        //    command.Add("proposal_id", new Tuple<int, int>(1, id));
        //}

        //// Client accept proposal
        //[Test]
        //public void clientAcceptTest(int id)
        //{
        //    if (command.Count > 1)
        //        Clear();

        //    command.Add("method", "prepare_proposal");
        //    command.Add("kpu_id", id);
        //    command.Add("Description", "Kpu is selected");
        //}

        //// Kill civilian result
        //[Test]
        //public void killCivResTest(int id, object res)
        //{
        //    if (command.Count > 1)
        //        Clear();

        //    command.Add("method", "vote_result_civilian");
        //    command.Add("vote_status", id > 0 ? 1 : -1);
        //    if (id > 0)
        //        command.Add("player_id", id);
        //    command.Add("vote_result", res);
        //}

        //// Kill werewolf result
        //[Test]
        //public void killWereResTest(int id, object res)
        //{
        //    if (command.Count > 1)
        //        Clear();

        //    command.Add("method", "vote_result_werewolf");
        //    command.Add("vote_status", id > 0 ? 1 : -1);
        //    if (id > 0)
        //        command.Add("player_id", id);
        //    command.Add("vote_result", res);
        //}
    }
}
