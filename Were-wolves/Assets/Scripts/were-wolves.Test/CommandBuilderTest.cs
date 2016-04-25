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

        // Join game
        [Test]
        public void joinTest()
        {
            CommandBuilder sut = new CommandBuilder();
            string result = sut.join("Gazandi").build();
            string expectedResult = "{\"method\":\"join\",\"username\":\"Gazandi\"}";
            Assert.That(expectedResult, Is.EqualTo(result));
        }
        // Join response
        [Test]
        public void joinRespTest()
        {
            CommandBuilder sut = new CommandBuilder();
            string result = sut.joinResp(-1, "Gazandi").build();
            string expectedResult = "{\"status\":\"error\",\"description\":\"Gazandi\"}";
            Assert.That(expectedResult, Is.EqualTo(result));
        }

        // Leave game
        [Test]
        public void leaveTest()
        {
            CommandBuilder sut = new CommandBuilder();
            string result = sut.leave().build();
            string expectedResult = "{\"method\":\"leave\"}";
            Assert.That(expectedResult, Is.EqualTo(result));
        }

        // Ready game
        [Test]
        public void readyTest()
        {
            CommandBuilder sut = new CommandBuilder();
            string result = sut.ready().build();
            string expectedResult = "{\"method\":\"ready\"}";
            Assert.That(expectedResult, Is.EqualTo(result));
        }

        // game over
        [Test]
        public void overTest()
        {
            CommandBuilder sut = new CommandBuilder();
            string result = sut.over("Panjadi", "game_over").build();
            string expectedResult = "{\"method\":\"game_over\",\"winner\":\"Panjadi\",\"description\":\"game_over\"}";
            Assert.That(expectedResult, Is.EqualTo(result));
        }

        // Change phase
        [Test]
        public void changeTest()
        {
            CommandBuilder sut = new CommandBuilder();
            string result = sut.change(true, "change_phase", 2).build();
            string expectedResult = "{\"method\":\"change_phase\",\"time\":\"night\",\"days\":2,\"description\":\"change_phase\"}";
            Assert.That(expectedResult, Is.EqualTo(result));
        }

        // Start game
        [Test]
        public void startTest()
        {
            CommandBuilder sut = new CommandBuilder();
            string result = sut.start(false, "game is started", "werewolf", new string[]{"ahmad", "dariel"} ).build();
            string expectedResult = "{\"method\":\"start\",\"time\":\"day\",\"role\":\"werewolf\",\"description\":\"game is started\",\"friend\":[\"ahmad\",\"dariel\"]}";
            Assert.That(expectedResult, Is.EqualTo(result));
        }

        // Player list 
        [Test]
        public void listClientTest()
        {
            CommandBuilder sut = new CommandBuilder();
            string result = sut.listClient().build();
            string expectedResult = "{\"method\":\"client_address\"}";
            Assert.That(expectedResult, Is.EqualTo(result));
        }
        //[Test]
        public void listClientRespTest()
        {
            CommandBuilder sut = new CommandBuilder();
            string result = sut.listClientResp(0, "Gazandi", new Client[] {
                new Client("Panjadi", "10.1.5.254", 3333),
                new Client("Panjadi2", "10.1.5.350", 6666)
            }).build();
            string expectedResult = "{\"status\":\"ok\",\"description\":\"Gazandi\",\"clients\":[{\"player_id\":0,\"is_alive\":1,\"address\":\"10.1.5.254\",\"port\":3333,\"username\":\"Panjadi\"},{\"player_id\":1,\"is_alive\":1,\"address\":\"10.1.5.350\",\"port\":6666,\"username\":\"Panjadi2\"}]}";
            Assert.That(expectedResult, Is.EqualTo(result));
        }

        // Kill civilian 
        [Test]
        public void killCivTest()
        {
            CommandBuilder sut = new CommandBuilder();
            string result = sut.killCiv(1).build();
            string expectedResult = "{\"method\":\"vote_civilian\",\"player_id\":1}";
            Assert.That(expectedResult, Is.EqualTo(result));
        }

        // Kill werewolf 
        [Test]
        public void killWereTest()
        {
            CommandBuilder sut = new CommandBuilder();
            string result = sut.killWere(1).build();
            string expectedResult = "{\"method\":\"vote_werewolf\",\"player_id\":1}";
            Assert.That(expectedResult, Is.EqualTo(result));
        }

        // Paxos prepare proposal
        [Test]
        public void proposeTest()
        {
            CommandBuilder sut = new CommandBuilder();
            string result = sut.propose(1,1).build();
            string expectedResult = "{\"method\":\"prepare_proposal\",\"kpu_id\":1,\"proposal_id\":(1,1)}";
            Assert.That(expectedResult, Is.EqualTo(result));
        }
        [Test]
        public void proposeRespTest()
        {
            CommandBuilder sut = new CommandBuilder();
            string result = sut.proposeResp(0, "Gazandi", 3).build();
            string expectedResult = "{\"status\":\"ok\",\"description\":\"Gazandi\",\"previous_accepted\":3}";
            Assert.That(expectedResult, Is.EqualTo(result));
        }

        // Paxos accept proposal
        [Test]
        public void acceptTest()
        {
            CommandBuilder sut = new CommandBuilder();
            string result = sut.accept(1, 1).build();
            string expectedResult = "{\"method\":\"accept_proposal\",\"kpu_id\":1,\"proposal_id\":(1,1)}";
            Assert.That(expectedResult, Is.EqualTo(result));
        }

        // Client accept proposal
        [Test]
        public void clientAcceptTest()
        {
            CommandBuilder sut = new CommandBuilder();
            string result = sut.clientAccept(1).build();
            string expectedResult = "{\"method\":\"prepare_proposal\",\"kpu_id\":1,\"Description\":\"Kpu is selected\"}";
            Assert.That(expectedResult, Is.EqualTo(result));
        }

        // Kill civilian result
        [Test]
        public void killCivResTest()
        {
            CommandBuilder sut = new CommandBuilder();
            Tuple[] res = {
                new Tuple(0,1),
                new Tuple(1,2)
            };
            string result = sut.killCivRes(1, res).build();
            string expectedResult = "{\"method\":\"vote_result_civilian\",\"vote_status\":1,\"player_id\":1,\"vote_result\":[(0,1),(1,2)]}";
            Assert.That(expectedResult, Is.EqualTo(result));
        }

        // Kill werewolf result
        [Test]
        public void killWereResTest()
        {
            CommandBuilder sut = new CommandBuilder();
            Tuple[] res = {
                new Tuple(0,1),
                new Tuple(1,2)
            };
            string result = sut.killWereRes(-1, res).build();
            string expectedResult = "{\"method\":\"vote_result\",\"vote_status\":-1,\"vote_result\":[(0,1),(1,2)]}";
            Assert.That(expectedResult, Is.EqualTo(result));
        }
    }
}
