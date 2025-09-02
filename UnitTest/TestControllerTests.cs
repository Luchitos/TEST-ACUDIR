using Acudir.Test.Apis.Controllers;

using Application.Dtos;
using Application.Personas.Commands.CreatePersona;
using Application.Personas.Commands.UpdatePersona;
using Application.Personas.Queries.GetPersonas;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using Moq;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Xunit;

using TestController = Acudir.Test.Apis.Controllers.PersonasController;

namespace UnitTest
{
    public class TestControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly TestController _controller;

        public TestControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new TestController(_mediatorMock.Object);
        }

        #region GetAll Tests
        [Fact]
        public async Task GetAll_ReturnsOkResult_WhenPersonasFound()
        {
            // Arrange
            List<PersonaDto> personas = new List<PersonaDto>
            {
                new PersonaDto { Id = 1, NombreCompleto = "John Doe" }
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPersonasQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(personas);

            GetPersonasQuery query = new GetPersonasQuery();

            // Act
            IActionResult result = await _controller.Get(query);

            // Assert
            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
            List<PersonaDto> returnValue = Assert.IsType<List<PersonaDto>>(okResult.Value);
            Assert.Single(returnValue);
        }

        [Fact]
        public async Task GetAll_ReturnsNotFound_WhenNoPersonasFound()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPersonasQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new List<PersonaDto>());

            GetPersonasQuery query = new GetPersonasQuery();

            // Act
            IActionResult result = await _controller.Get(query);

            // Assert
            NotFoundObjectResult notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No se encontraron personas con los datos proporcionados.", notFoundResult.Value);
        }
        #endregion

        #region Add Tests
        [Fact]
        public async Task Add_ReturnsCreatedResult_WhenPersonaAdded()
        {
            // Arrange
            CreatePersonaCommand command = new CreatePersonaCommand("John Doe", 30, "123 Main St", "555-5555", "Developer");
            int expectedId = 1;

            _mediatorMock.Setup(m => m.Send(It.IsAny<CreatePersonaCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(expectedId);

            // Act
            IActionResult result = await _controller.Post(command);

            // Assert
            CreatedAtActionResult createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(expectedId, createdResult.Value);
        }
        #endregion

        #region Update Tests
        [Fact]
        public async Task Update_ReturnsOkResult_WhenPersonaUpdated()
        {
            // Arrange
            UpdatePersonaCommand command = new UpdatePersonaCommand(1, "John Doe", 30, "123 Main St", "555-5555", "Developer");

            PersonaDto persona = new PersonaDto
            {
                Id = command.Id,
                NombreCompleto = command.NombreCompleto,
                Edad = command.Edad ?? 0,
                Domicilio = command.Domicilio!,
                Telefono = command.Telefono!,
                Profesion = command.Profesion!
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdatePersonaCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(persona);

            // Act
            IActionResult result = await _controller.Put(command);

            // Assert
            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
            PersonaDto returnValue = Assert.IsType<PersonaDto>(okResult.Value);
            Assert.Equal(persona.Id, returnValue.Id);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenPersonaNotFound()
        {
            // Arrange
            UpdatePersonaCommand command = new UpdatePersonaCommand(1, "John Doe", 30, "123 Main St", "555-5555", "Developer");

            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdatePersonaCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync((PersonaDto?)null);

            // Act
            IActionResult result = await _controller.Put(command);

            // Assert
            NotFoundObjectResult notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFound.StatusCode);
        }
        #endregion
    }
}