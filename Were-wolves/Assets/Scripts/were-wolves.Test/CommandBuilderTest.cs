using NUnit.Framework;

namespace WereWolves.Tests
{
    [TestFixture]
    public class CommandBuilderTest
    {
        private CommandBuilder sut;

        [TestFixtureSetUp]
        public void TestSetup() { sut = new CommandBuilder(); }

        [TestFixtureTearDown]
        public void TestTearDown() { sut = null; }

        // General response
        [Test]
        public void responseTestOk()
        {
            string result = sut.response(0, null).build();
            string expectedResult = "{\"status\":\"ok\"}";
            Assert.That(result, Is.EqualTo(expectedResult));
        }
        [Test]
        public void responseTestFail()
        {
            string result = sut.response(1, "Fail").build();
            string expectedResult = "{\"status\":\"fail\",\"description\":\"Fail\"}";
            Assert.That(result, Is.EqualTo(expectedResult));
        }
        [Test]
        public void responseTestError()
        {
            string result = sut.response(-1, "Error").build();
            string expectedResult = "{\"status\":\"error\",\"description\":\"Error\"}";
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        // Join game
        [Test]
        public void joinTest()
        {
            string result = sut.join("Gazandi", "127.0.0.1", 9999).build();
            string expectedResult = "{\"method\":\"join\",\"username\":\"Gazandi\",\"udp_address\":\"127.0.0.1\",\"udp_port\":9999}";
            Assert.That(result, Is.EqualTo(expectedResult));
        }
        // Join response
        [Test]
        public void joinRespTest()
        {
            string result = sut.joinResp(-1, "Gazandi").build();
            string expectedResult = "{\"status\":\"error\",\"description\":\"Gazandi\"}";
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        // Leave game
        [Test]
        public void leaveTest()
        {
            string result = sut.leave().build();
            string expectedResult = "{\"method\":\"leave\"}";
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        // Ready game
        [Test]
        public void readyTest()
        {
            string result = sut.ready().build();
            string expectedResult = "{\"method\":\"ready\"}";
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        // game over
        [Test]
        public void overTest()
        {
            string result = sut.over("Panjadi", "game_over").build();
            string expectedResult = "{\"method\":\"game_over\",\"winner\":\"Panjadi\",\"description\":\"game_over\"}";
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        // Change phase
        [Test]
        public void changeTest()
        {
            string result = sut.change(true, "change_phase", 2).build();
            string expectedResult = "{\"method\":\"change_phase\",\"time\":\"night\",\"days\":2,\"description\":\"change_phase\"}";
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        // Start game
        [Test]
        public void startTest()
        {
            string result = sut.start(false, "game is started", "werewolf", new string[]{"ahmad", "dariel"} ).build();
            string expectedResult = "{\"method\":\"start\",\"time\":\"day\",\"role\":\"werewolf\",\"description\":\"game is started\",\"friend\":[\"ahmad\",\"dariel\"]}";
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        // Player list 
        [Test]
        public void listClientTest()
        {
            string result = sut.listClient().build();
            string expectedResult = "{\"method\":\"client_address\"}";
            Assert.That(result, Is.EqualTo(expectedResult));
        }
        [Test]
        public void listClientRespTest()
        {
            var clients = new ClientData[] {
                new ClientData("Panjadi", "10.1.5.254", 3333),
                new ClientData("Panjadi2", "10.1.5.350", 6666)
            };
            clients[1].killed();
            clients[1].setRole(true);
            string result = sut.listClientResp(0, "Gazandi", clients).build();
            string expectedResult = "{\"status\":\"ok\",\"description\":\"Gazandi\",\"clients\":[{\"player_id\":0,\"is_alive\":1,\"address\":\"10.1.5.254\",\"port\":3333,\"username\":\"Panjadi\"},{\"player_id\":1,\"is_alive\":0,\"address\":\"10.1.5.350\",\"port\":6666,\"username\":\"Panjadi2\",\"role\":\"werewolf\"}]}";
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        // Kill civilian 
        [Test]
        public void killCivTest()
        {
            string result = sut.killCiv(1).build();
            string expectedResult = "{\"method\":\"vote_civilian\",\"player_id\":1}";
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        // Kill werewolf 
        [Test]
        public void killWereTest()
        {
            string result = sut.killWere(1).build();
            string expectedResult = "{\"method\":\"vote_werewolf\",\"player_id\":1}";
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        // Paxos prepare proposal
        [Test]
        public void proposeTest()
        {
            string result = sut.propose(1,1).build();
            string expectedResult = "{\"method\":\"prepare_proposal\",\"kpu_id\":1,\"proposal_id\":[1,1]}";
            Assert.That(result, Is.EqualTo(expectedResult));
        }
        [Test]
        public void proposeRespTest()
        {
            string result = sut.proposeResp(0, "Gazandi", 3).build();
            string expectedResult = "{\"status\":\"ok\",\"description\":\"Gazandi\",\"previous_accepted\":3}";
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        // Paxos accept proposal
        [Test]
        public void acceptTest()
        {
            string result = sut.accept(1, 2).build();
            string expectedResult = "{\"method\":\"accept_proposal\",\"kpu_id\":1,\"proposal_id\":[1,2]}";
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        // Client accept proposal
        [Test]
        public void clientAcceptTest()
        {
            string result = sut.clientAccept(1).build();
            string expectedResult = "{\"method\":\"prepare_proposal\",\"kpu_id\":1,\"Description\":\"Kpu is selected\"}";
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        // Kill civilian result
        [Test]
        public void killCivResTest()
        {
            Tuple[] res = {
                new Tuple(0,1),
                new Tuple(1,2)
            };
            string result = sut.killCivRes(1, res).build();
            string expectedResult = "{\"method\":\"vote_result_civilian\",\"vote_status\":1,\"player_id\":1,\"vote_result\":[[0,1],[1,2]]}";
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        // Kill werewolf result
        [Test]
        public void killWereResTest()
        {
            Tuple[] res = {
                new Tuple(0,1),
                new Tuple(1,2)
            };
            string result = sut.killWereRes(-1, res).build();
            string expectedResult = "{\"method\":\"vote_result\",\"vote_status\":-1,\"vote_result\":[[0,1],[1,2]]}";
            Assert.That(result, Is.EqualTo(expectedResult));
        }
    }
}
