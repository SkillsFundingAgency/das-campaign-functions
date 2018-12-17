﻿using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Campaign.Functions.Application.DataCollection.Handlers;
using SFA.DAS.Campaign.Functions.Domain.DataCollection;
using SFA.DAS.Campaign.Functions.Models.DataCollection;

namespace SFA.DAS.Campaign.Functions.Application.UnitTests.DataCollection.Handlers
{
    public class WhenHandlingUnregisterDataRequest
    {
        private UnregisterHandler _handler;
        private Mock<IUserUnregisterDataValidator> _validator;
        private Mock<IUserService> _userService;
        private Mock<IWiredPlusService> _wiredPlusService;
        private const string ExpectedUserEmail = "test@test.com";

        [SetUp]
        public void Arrange()
        {
            _userService = new Mock<IUserService>();
            _wiredPlusService = new Mock<IWiredPlusService>();
            _validator = new Mock<IUserUnregisterDataValidator>();
            
            _validator.Setup(x => x.Validate(It.IsAny<string>())).Returns(true);
            _handler = new UnregisterHandler(_validator.Object, _userService.Object, _wiredPlusService.Object);
        }

        [Test]
        public void Then_The_Message_Is_Validated_And_ArgumentException_Thrown_And_Not_Sent_To_The_Api_If_Not_Valid()
        {
            //Arrange
            _validator.Setup(x => x.Validate(It.IsAny<string>())).Returns(false);

            //Act
            Assert.ThrowsAsync<ArgumentException>(async () => await _handler.Handle(new UserData()));

            //Assert
            _validator.Verify(x => x.Validate(It.IsAny<string>()), Times.Once);
            _userService.Verify(x => x.UpdateUser(It.IsAny<UserData>()), Times.Never);
        }
        
        [Test]
        public async Task Then_If_The_Message_Is_Valid_Is_Sent_To_The_Api()
        {
            //Arrange
            var expectedUserData = new UserData
            {
                Email = ExpectedUserEmail
            };

            //Act
            await _handler.Handle(expectedUserData);

            //Assert
            _userService.Verify(x => x.UpdateUser(It.Is<UserData>(c => c.Equals(expectedUserData))), Times.Once);
        }


        [Test]
        public async Task Then_If_The_Message_Is_Valid_Is_Sent_To_The_WiredPlusApi()
        {
            //Arrange
            var expectedUserData = new UserData
            {
                Email = ExpectedUserEmail
            };

            //Act
            await _handler.Handle(expectedUserData);

            //Assert
            _wiredPlusService.Verify(x => x.UnsubscribeUser(It.Is<UserData>(c => c.Equals(expectedUserData))), Times.Once);
        }
    }
}
