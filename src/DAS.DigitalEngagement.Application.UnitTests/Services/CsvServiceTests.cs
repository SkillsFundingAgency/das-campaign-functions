﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAS.DigitalEngagement.Application.Services;
using DAS.DigitalEngagement.Application.UnitTests.Helpers;
using DAS.DigitalEngagement.Domain.Services;
using Das.Marketo.RestApiClient.Models;
using FluentAssertions;
using NUnit.Framework;

namespace DAS.DigitalEngagement.Application.UnitTests.Import.Handlers
{
    public class CsvServiceTests
    {
        private CsvService _service;

        private string _testCsvSmall = CsvTestHelper.GetValidCsv(10, "Mickey", "Mouse", "Disney");
        private string _testCsvLarge = CsvTestHelper.GetValidCsv(700000,"Donald","Duck","Disney");

        private IChunkingService _chunkingService = new ChunkingService();

        [SetUp]
        public void Arrange()
        {
            _service = new CsvService();
        }

        [Test]
        public async Task ConvertToList_When_Ten_Rows_Then_Ten_Objects_In_List()
        {
            //Arrange

            List<NewLead> result;
            //Act
            using (var test_Stream = new MemoryStream(Encoding.UTF8.GetBytes(_testCsvSmall)))
            {
             result = await _service.ConvertToList<NewLead>(test_Stream);
            }
           
            //Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().HaveCount(10);
        }

        [Test]
        public async Task ConvertToList_When_700k_Rows_Then_700k_Objects_In_List()
        {
            //Arrange

            List<NewLead> result;
            //Act
            using (var test_Stream = new MemoryStream(Encoding.UTF8.GetBytes(_testCsvLarge)))
            {
                result = await _service.ConvertToList<NewLead>(test_Stream);
            }

            //Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().HaveCount(700000);
        }

        [Test]
        public async Task ConvertToList_When_Converted_Then_All_Properties_Are_Populated()
        {
            //Arrange

            List<NewLead> result;
            //Act
            using (var test_Stream = new MemoryStream(Encoding.UTF8.GetBytes(_testCsvSmall)))
            {
                result = await _service.ConvertToList<NewLead>(test_Stream);
            }

            //Assert
            result.Should().NotBeNullOrEmpty();

            var singleResult = result.FirstOrDefault();
            singleResult.Should().NotBeNull();
            singleResult.FirstName.Should().Be("Mickey1");
            singleResult.LastName.Should().Be("Mouse");
            singleResult.Company.Should().Be("Disney1");
            singleResult.Email.Should().Be("Mickey.Mouse.1@email.com");
        }

        [Test]
        public async Task ConvertToList_When_Converted_And_Property_Missing_Then_All_Other_Properties_Are_Populated()
        {
            //Arrange
            var _testCsvMissing = CsvTestHelper.GetValidCsv(10, "Mickey", null, "Disney");

            List<NewLead> result;
            //Act
            using (var test_Stream = new MemoryStream(Encoding.UTF8.GetBytes(_testCsvMissing)))
            {
                result = await _service.ConvertToList<NewLead>(test_Stream);
            }

            //Assert
            result.Should().NotBeNullOrEmpty();

            var singleResult = result.FirstOrDefault();
            singleResult.Should().NotBeNull();
            singleResult.FirstName.Should().Be("Mickey1");
            singleResult.LastName.Should().BeNull();
            singleResult.Company.Should().Be("Disney1");
            singleResult.Email.Should().Be("Mickey..1@email.com");
        }

        [Test]
        public async Task ConvertToList_When_Converted_And_Too_Many_Properties_Then_All_Other_Properties_Are_Populated()
        {
            //Arrange
            var _testCsvAdditional = CsvTestHelper.GetValidCsv_AdditionalProperties();

            List<NewLead> result;
            //Act
            using (var test_Stream = new MemoryStream(Encoding.UTF8.GetBytes(_testCsvAdditional)))
            {
                result = await _service.ConvertToList<NewLead>(test_Stream);
            }

            //Assert
            result.Should().NotBeNullOrEmpty();

            var singleResult = result.FirstOrDefault();
            singleResult.Should().NotBeNull();
            singleResult.FirstName.Should().Be("Person");
            singleResult.LastName.Should().Be("One");
            singleResult.Company.Should().Be("CompanyOne");
            singleResult.Email.Should().Be("Person.one@email.com");
        }
    }
}
