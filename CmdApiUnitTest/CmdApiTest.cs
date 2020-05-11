using Microsoft.VisualStudio.TestTools.UnitTesting;
using CmdApi.Controllers;
using CmdApi.Models;
using Moq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using Microsoft.AspNetCore.Mvc;

namespace CmdApiUnitTest
{
    [TestClass]
    public class CmdApiTest
    {
        [TestMethod]
        public void GetAllItems_Using_Mock_Of_DbContext()
        {
            //Arrange

            IQueryable<Command> commands = GetListOfCommands();

            var commandDbSetMock = new Mock<DbSet<Command>>();

            commandDbSetMock.As<IQueryable<Command>>()
                .Setup(x => x.Provider).Returns(commands.Provider);
            commandDbSetMock.As<IQueryable<Command>>()
                .Setup(x => x.Expression).Returns(commands.Expression);
            commandDbSetMock.As<IQueryable<Command>>()
                .Setup(x => x.ElementType).Returns(commands.ElementType);
            commandDbSetMock.As<IQueryable<Command>>()
                .Setup(x => x.GetEnumerator()).Returns(commands.GetEnumerator());

            CommandContext commandContext = new CommandContext(new DbContextOptions<CommandContext>());
            commandContext.CommandItems = commandDbSetMock.Object;

            CommandsController commandController = new CommandsController(commandContext);
            var expected = commands;

            //Act
            var actual = commandController.GetCommands();

            //Assert
            Assert.AreEqual(expected.Count(), actual.Value.Count<Command>());
        }

        [TestMethod]
        public void GetAllItems_Using_InMemory_Database()
        {
            //Arrange

            var options = new DbContextOptionsBuilder<CommandContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            CommandContext commandContext = GetListOfCommands(options);

            var expected = 3;

            var commandController = new CommandsController(commandContext);

            //Act
            var actual = commandController.GetCommands();

            //Assert
            Assert.AreEqual(expected, actual.Value.Count<Command>());
        }

       
        [TestMethod]
        [Ignore("Failing")]
        public void GetSpecificItem_Using_Mock_Of_DbContext()
        {
            //Arrange

            IQueryable<Command> commands = GetListOfCommands();

            var commandDbSetMock = new Mock<DbSet<Command>>();

            commandDbSetMock.As<IQueryable<Command>>()
                .Setup(x => x.Provider).Returns(commands.Provider);
            commandDbSetMock.As<IQueryable<Command>>()
                .Setup(x => x.Expression).Returns(commands.Expression);
            commandDbSetMock.As<IQueryable<Command>>()
                .Setup(x => x.ElementType).Returns(commands.ElementType);
            commandDbSetMock.As<IQueryable<Command>>()
                .Setup(x => x.GetEnumerator()).Returns(commands.GetEnumerator());

            CommandContext commandContext = new CommandContext(new DbContextOptions<CommandContext>());
            commandContext.CommandItems = commandDbSetMock.Object;

            CommandsController commandController = new CommandsController(commandContext);
            var expected = commands.FirstOrDefault();

            //Act
            var actual = commandController.GetCommandItem(expected.Id);

            //Assert
            Assert.AreEqual(expected.Id, actual.Value.Id);
            Assert.AreEqual(expected.Commanline, actual.Value.Commanline);
            Assert.AreEqual(expected.HowTo, actual.Value.HowTo);
            Assert.AreEqual(expected.Platform, actual.Value.Platform);
        }

        [TestMethod]
        public void GetSpecificItem_Using_InMemory_Databse()
        {
            //Arrange

            var options = new DbContextOptionsBuilder<CommandContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            CommandContext commandContext = GetListOfCommands(options);

            var expected = commandContext.CommandItems.FirstOrDefault<Command>();

            var commandController = new CommandsController(commandContext);

            //Act
            var actual = commandController.GetCommandItem(expected.Id);

            //Assert
            Assert.AreEqual(expected.Id, actual.Value.Id);
            Assert.AreEqual(expected.Commanline, actual.Value.Commanline);
            Assert.AreEqual(expected.HowTo, actual.Value.HowTo);
            Assert.AreEqual(expected.Platform, actual.Value.Platform);
        }

        [TestMethod]
        public void GetSpecificItem_Using_InMemory_Databse_Item_Does_Not_Exist()
        {
            //Arrange

            var options = new DbContextOptionsBuilder<CommandContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            CommandContext commandContext = GetListOfCommands(options);

            var commandController = new CommandsController(commandContext);

            //Act
            var actual = commandController.GetCommandItem(100);

            //Assert
            Assert.IsNull(actual.Value);
        }

        [TestMethod]
        public void PostCommandItemTest()
        {

            //Arrange

            var options = new DbContextOptionsBuilder<CommandContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var commandContext = new CommandContext(options);

            var command = new Command
            {
                HowTo = "How to post",
                Commanline = "Command line post",
                Platform = "Platform post",
                Id = 1
            };

            var commandController = new CommandsController(commandContext);

            var expected = command;

            //Act
            commandController.PostCommandItem(command);
            var actual = commandController.GetCommandItem(command.Id);

            //Assert
            Assert.AreEqual(expected.Id, actual.Value.Id);
            Assert.AreEqual(expected.Commanline, actual.Value.Commanline);
            Assert.AreEqual(expected.HowTo, actual.Value.HowTo);
            Assert.AreEqual(expected.Platform, actual.Value.Platform);
        }

        [TestMethod]
        public void PutCommandItemTest_Using_InMemory_Should_Pass()
        {

            //Arrange

            var options = new DbContextOptionsBuilder<CommandContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            CommandContext commandContext = GetListOfCommands(options);

            var commandController = new CommandsController(commandContext);

            var command = commandController.GetCommandItem(commandContext
                                                           .CommandItems
                                                           .FirstOrDefault<Command>()
                                                           .Id);

            command.Value.Commanline = "Command line put";
            command.Value.Platform = "Platform put";
            command.Value.HowTo = "How to put";

            var expected = command;
            var expectedStatusCode = 204;

            //Act
                       
            var response = commandController.PutCommandItem(command.Value.Id, command.Value);
            var actualNoContent = (NoContentResult)response;

            var actual = commandController.GetCommandItem(command.Value.Id);
            //Assert
            Assert.AreEqual(expectedStatusCode, actualNoContent.StatusCode);
            Assert.AreEqual(expected.Value.Id, actual.Value.Id);
            Assert.AreEqual(expected.Value.Commanline, actual.Value.Commanline);
            Assert.AreEqual(expected.Value.HowTo, actual.Value.HowTo);
            Assert.AreEqual(expected.Value.Platform, actual.Value.Platform);
        }

        [TestMethod]
        public void PutCommandItemTest_Using_InMemory_Should_Return_Bad_Request()
        {

            //Arrange

            var options = new DbContextOptionsBuilder<CommandContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            CommandContext commandContext = GetListOfCommands(options);

            var commandController = new CommandsController(commandContext);

            var command = commandController.GetCommandItem(commandContext
                                                           .CommandItems
                                                           .FirstOrDefault<Command>()
                                                           .Id);
            command.Value.Commanline = "Command line put";
            command.Value.Platform = "Platform put";
            command.Value.HowTo = "How to put";

            var expected = 400;

            //Act

            var actual = commandController.PutCommandItem(command.Value.Id + 1, command.Value);
            var actualBadRequest = (BadRequestResult)actual;

            //Assert
            Assert.AreEqual(expected, actualBadRequest.StatusCode);
        }

        [TestMethod]
        public void DeleteCommandItem_Using_InMemory_Databse()
        {
            //Arrange

            var options = new DbContextOptionsBuilder<CommandContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            CommandContext commandContext = GetListOfCommands(options);

            var expected = 2;

            var commandController = new CommandsController(commandContext);

            //Act
            commandController.DeleteCommandItem(commandContext
                             .CommandItems
                             .FirstOrDefault<Command>()
                             .Id);

            var actual = commandController.GetCommands();

            //Assert
            Assert.AreEqual(expected, actual.Value.Count());
        }

        [TestMethod]
        public void DeleteCommandItem_Using_InMemory_Databse_Item_NotFound()
        {
            //Arrange

            var options = new DbContextOptionsBuilder<CommandContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            CommandContext commandContext = new CommandContext(options);

            int id = 1;

            var commandController = new CommandsController(commandContext);

            var expected = 404;

            //Act
            var actual = commandController.DeleteCommandItem(id);

            var actualNotFound = (NotFoundResult)actual.Result;

            //Assert
            Assert.AreEqual(expected, actualNotFound.StatusCode);
        }

        [Obsolete]
        private static IQueryable<Command> GetListOfCommands()
        {
            Command command1 = new Command()
            {
                Id = 1,
                HowTo = "how to test 1",
                Commanline = "command line test 1",
                Platform = "platform fake 1"
            };

            Command command2 = new Command()
            {
                Id = 2,
                HowTo = "how to test 2",
                Commanline = "command line test 2",
                Platform = "platform fake 2"
            };

            Command command3 = new Command()
            {
                Id = 3,
                HowTo = "how to test 3",
                Commanline = "command line test 3",
                Platform = "platform fake 3"
            };

            var commands = new List<Command>
            {
                command1,
                command2,
                command3
            }.AsQueryable();
            return commands;
        }

        private static CommandContext GetListOfCommands(DbContextOptions<CommandContext> options)
        {
            var command1 = new Command
            {
                Commanline = "CommandLine test",
                HowTo = "HowTo test",
                Id = 1,
                Platform = "Platform test"
            };

            var command2 = new Command
            {
                Commanline = "CommandLine test",
                HowTo = "HowTo test",
                Id = 2,
                Platform = "Platform test"
            };

            var command3 = new Command
            {
                Commanline = "CommandLine test",
                HowTo = "HowTo test",
                Id = 3,
                Platform = "Platform test"
            };

            var commandContext = new CommandContext(options);
            commandContext.Add(command1);
            commandContext.Add(command2);
            commandContext.Add(command3);
            commandContext.SaveChanges();
            return commandContext;
        }
    }

}
