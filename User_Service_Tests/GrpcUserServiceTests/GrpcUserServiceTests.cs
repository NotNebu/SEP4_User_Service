using Xunit;
using Moq;
using SEP4_User_Service.API.Services;
using SEP4_User_Service.Application.Interfaces;
using SEP4_User_Service.Application.UseCases;
using SEP4_User_Service.Domain.Entities;
using Grpc.Core;
using Google.Protobuf.WellKnownTypes;
using Protos.Grpc;

public class GrpcUserServiceTests
{
    // Mock af bruger-repositoriet, så vi kan teste uden en rigtig database
    private readonly Mock<IUserRepository> _mockUserRepository = new();

    // Use case objekter vi tester
    private readonly CreateUserUseCase _createUserUseCase;
    private readonly GetUserUseCase _getUserUseCase;
    private readonly GetAllUsersUseCase _getAllUsersUseCase;
    private readonly UpdateUserUseCase _updateUserUseCase;
    private readonly DeleteUserUseCase _deleteUserUseCase;

    // Constructor som initialiserer use cases med mock repository
    public GrpcUserServiceTests()
    {
        _createUserUseCase = new CreateUserUseCase(_mockUserRepository.Object);
        _getUserUseCase = new GetUserUseCase(_mockUserRepository.Object);
        _getAllUsersUseCase = new GetAllUsersUseCase(_mockUserRepository.Object);
        _updateUserUseCase = new UpdateUserUseCase(_mockUserRepository.Object);
        _deleteUserUseCase = new DeleteUserUseCase(_mockUserRepository.Object);
    }

    // Hjælpemetode til at oprette en testkontekst (for at undgå gentagelse)
    private ServerCallContext CreateTestContext(string methodName) =>
        TestServerCallContext.Create(
            method: methodName,
            host: null,
            deadline: DateTime.UtcNow.AddMinutes(1),
            requestHeaders: new Metadata(),
            cancellationToken: CancellationToken.None,
            peer: "",
            authContext: null,
            contextPropagationToken: null,
            writeHeadersFunc: _ => Task.CompletedTask,
            writeOptionsGetter: () => null,
            writeOptionsSetter: _ => { });

    // Test: oprettelse af bruger returnerer korrekt svar
    [Fact]
    public async Task CreateUser_ShouldReturnUserResponse_WhenValidRequest()
    {
        var request = new CreateUserRequest
        {
            Username = "testUser",
            Password = "password1234",
            Email = "test@example.com",
            Firstname = "Test",
            Lastname = "User",
            Birthday = "1980-01-01",
            Locations = { new LocationMessage { Street = "Lærkevej", HouseNumber = "14", City = "Horsens", Country = "Danmark" } }
        };

        _mockUserRepository.Setup(x => x.CreateUserAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

        var service = new GrpcUserService(_createUserUseCase, _getUserUseCase, _getAllUsersUseCase, _updateUserUseCase, _deleteUserUseCase);
        var response = await service.CreateUser(request, CreateTestContext("CreateUser"));

        Assert.Equal("testUser", response.Username);
        Assert.Equal("Test", response.Firstname);
        Assert.Equal("User", response.Lastname);
        Assert.Equal("test@example.com", response.Email);
        Assert.Equal("1980-01-01", response.Birthday);
    }

    // Test: hent bruger med ID returnerer korrekt bruger
    [Fact]
    public async Task GetUserById_ShouldReturnUserResponse_WhenUserExists()
    {
        var userId = Guid.NewGuid();
        var expectedUser = new User("testUser", "password", "testemail@example.com", "User", "Test", "2000-01-01")
        {
            Id = userId,
            Locations = new List<Location> { new Location("Lærkevej", "10", "Aarhus", "Danmark") }
        };

        _mockUserRepository.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync(expectedUser);

        var service = new GrpcUserService(_createUserUseCase, _getUserUseCase, _getAllUsersUseCase, _updateUserUseCase, _deleteUserUseCase);
        var response = await service.GetUserById(new GetUserByIdRequest { Id = userId.ToString() }, CreateTestContext("GetUserById"));

        Assert.Equal(userId.ToString(), response.Id);
    }

    // Test: hent alle brugere returnerer korrekt liste
    [Fact]
    public async Task GetAllUsers_ShouldReturnAllUsers()
    {
        var expectedUsers = new List<User>
        {
            new User("k", "password1", "kim@example.com", "Kim", "Andersen", "1998-02-10") { Locations = new List<Location> { new Location("Havnealle", "1", "Horsens", "Danmark") } },
            new User("n", "password2", "ninna@example.com", "Ninna", "Bach", "1985-09-15") { Locations = new List<Location> { new Location("Tunvej", "2", "Snaptun", "Danmark") } }
        };

        _mockUserRepository.Setup(r => r.GetAllUsersAsync()).ReturnsAsync(expectedUsers);

        var service = new GrpcUserService(_createUserUseCase, _getUserUseCase, _getAllUsersUseCase, _updateUserUseCase, _deleteUserUseCase);
        var response = await service.GetAllUsers(new Empty(), CreateTestContext("GetAllUsers"));

        Assert.Equal(2, response.Users.Count);
    }

    // Test: opdatering af bruger kalder repository korrekt
    [Fact]
    public async Task UpdateUser_ShouldCallRepositoryWithCorrectUser()
    {
        var userId = Guid.NewGuid();
        var request = new UpdateUserRequest
        {
            Id = userId.ToString(), Username = "updatedUser", Password = "newPassword", Firstname = "Updated",
            Lastname = "User", Email = "updated@example.com", Birthday = "1990-01-01",
            Locations = { new LocationMessage { Street = "Nygade", HouseNumber = "25B", City = "Horsens", Country = "Danmark" } }
        };

        User? capturedUser = null;
        _mockUserRepository.Setup(r => r.UpdateUserAsync(It.IsAny<User>())).Callback<User>(u => capturedUser = u).Returns(Task.CompletedTask);

        var service = new GrpcUserService(_createUserUseCase, _getUserUseCase, _getAllUsersUseCase, _updateUserUseCase, _deleteUserUseCase);
        var result = await service.UpdateUser(request, CreateTestContext("UpdateUser"));

        Assert.NotNull(capturedUser);
        Assert.Equal(userId, capturedUser.Id);
    }

    // Test: sletning af ikke-eksisterende bruger returnerer false
    [Fact]
    public async Task DeleteUser_ShouldReturnSuccessFalse_WhenUserDoesNotExist()
    {
        var userId = Guid.NewGuid();
        _mockUserRepository.Setup(r => r.DeleteUserAsync(userId)).ReturnsAsync(false);

        var service = new GrpcUserService(_createUserUseCase, _getUserUseCase, _getAllUsersUseCase, _updateUserUseCase, _deleteUserUseCase);
        var response = await service.DeleteUser(new DeleteUserRequest { Id = userId.ToString() }, CreateTestContext("DeleteUser"));

        Assert.False(response.Success);
    }

    // Test: korrekt ændring af kodeord når det gamle er korrekt
    [Fact]
    public async Task ChangePassword_ShouldReturnSuccess_WhenOldPasswordIsCorrect()
    {
        var userId = Guid.NewGuid();
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("oldPassword");
        var user = new User("testUser", hashedPassword, "test@example.com", "Test", "User", "1991-01-01") { Id = userId };

        _mockUserRepository.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync(user);
        _mockUserRepository.Setup(r => r.UpdateUserAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

        var service = new GrpcUserService(_createUserUseCase, _getUserUseCase, _getAllUsersUseCase, _updateUserUseCase, _deleteUserUseCase);
        var request = new ChangePasswordRequest { UserId = userId.ToString(), OldPassword = "oldPassword", NewPassword = "newPass" };
        var response = await service.ChangePassword(request, CreateTestContext("ChangePassword"));

        Assert.True(response.Success);
    }

    // Test: ændring af kodeord fejler hvis det gamle kodeord er forkert
    [Fact]
    public async Task ChangePassword_ShouldReturnFailure_WhenOldPasswordIsIncorrect()
    {
        var userId = Guid.NewGuid();
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("correctOldPassword");
        var user = new User("testUser", hashedPassword, "test@example.com", "Test", "User", "1990-01-01") { Id = userId };

        _mockUserRepository.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync(user);

        var service = new GrpcUserService(_createUserUseCase, _getUserUseCase, _getAllUsersUseCase, _updateUserUseCase, _deleteUserUseCase);
        var request = new ChangePasswordRequest { UserId = userId.ToString(), OldPassword = "wrongPassword", NewPassword = "newSecurePassword" };
        var response = await service.ChangePassword(request, CreateTestContext("ChangePassword"));

        Assert.False(response.Success);
    }
}
