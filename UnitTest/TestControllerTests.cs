using Acudir.Test.Apis.Controllers;

using Application.Common.Pagination;
using Application.Dtos;
using Application.Personas.Commands.CreatePersona;
using Application.Personas.Commands.DeletePersona;
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

        #region GET /api/v1/personas
        [Fact]
        public async Task GetAll_ReturnsOkResult_WhenPersonasFound()
        {
            // Arrange
            var items = new List<PersonaDto>
            {
                new PersonaDto { Id = 1, NombreCompleto = "John Doe" }
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetPersonasQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PagedResult<PersonaDto>(
                    Items: items,
                    Page: 1,
                    Size: 10,
                    TotalItems: items.Count,
                    TotalPages: 1,
                    SortBy: "Id",
                    SortDir: "asc"));

            var query = new GetPersonasQuery();

            // Act
            var result = await _controller.Get(query, CancellationToken.None);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var page = Assert.IsType<PagedResult<PersonaDto>>(ok.Value);
            Assert.Single(page.Items);
            Assert.Equal(1, page.TotalItems);
            Assert.Equal(1, page.TotalPages);
        }

        [Fact]
        public async Task GetAll_ReturnsNotFound_WhenNoPersonasFound()
        {
            // Arrange
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetPersonasQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PagedResult<PersonaDto>(
                    Items: new List<PersonaDto>(),
                    Page: 1,
                    Size: 10,
                    TotalItems: 0,
                    TotalPages: 0,
                    SortBy: "Id",
                    SortDir: "asc"));

            var query = new GetPersonasQuery();

            // Act
            var result = await _controller.Get(query, CancellationToken.None);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var pd = Assert.IsType<ProblemDetails>(notFound.Value);
            Assert.Equal("Sin resultados", pd.Title);
            Assert.Equal(404, pd.Status);
        }
        #endregion

        #region POST /api/v1/personas
        [Fact]
        public async Task Add_ReturnsCreatedResult_WhenPersonaAdded()
        {
            // Arrange
            var command = new CreatePersonaCommand("John Doe", 30, "123 Main St", "555-5555", "Developer");
            var expectedId = 1;

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<CreatePersonaCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedId);

            // Act
            var result = await _controller.Post(command, CancellationToken.None);

            // Assert
            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(TestController.GetById), created.ActionName);
            Assert.Equal(expectedId, created.Value);
        }
        #endregion

        #region PUT /api/v1/personas
        [Fact]
        public async Task Update_ReturnsOkResult_WhenPersonaUpdated()
        {
            // Arrange
            var command = new UpdatePersonaCommand(1, "John Doe", 30, "123 Main St", "555-5555", "Developer");
            var persona = new PersonaDto
            {
                Id = command.Id,
                NombreCompleto = command.NombreCompleto!,
                Edad = command.Edad ?? 0,
                Domicilio = command.Domicilio!,
                Telefono = command.Telefono!,
                Profesion = command.Profesion!
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<UpdatePersonaCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(persona);

            // Act
            var result = await _controller.Put(command, CancellationToken.None);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<PersonaDto>(ok.Value);
            Assert.Equal(persona.Id, value.Id);
            Assert.Equal("John Doe", value.NombreCompleto);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenPersonaNotFound()
        {
            // Arrange
            var command = new UpdatePersonaCommand(1, "John Doe", 30, "123 Main St", "555-5555", "Developer");

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<UpdatePersonaCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((PersonaDto?)null);

            // Act
            var result = await _controller.Put(command, CancellationToken.None);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var pd = Assert.IsType<ProblemDetails>(notFound.Value);
            Assert.Equal(404, pd.Status);
        }
        #endregion

        #region DELETE /api/v1/personas/{id}
        [Fact]
        public async Task Delete_ReturnsNoContent_WhenDeleted()
        {
            // Arrange
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<DeletePersonaCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(1, CancellationToken.None);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenPersonaDoesNotExist()
        {
            // Arrange
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<DeletePersonaCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(999, CancellationToken.None);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var pd = Assert.IsType<ProblemDetails>(notFound.Value);
            Assert.Equal(404, pd.Status);
        }
        #endregion
    }
}