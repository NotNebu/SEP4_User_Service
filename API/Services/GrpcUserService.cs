using SEP4_User_Service.Application.UseCases;
using SEP4_User_Service.Domain.Entities;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Protos.Grpc;

namespace SEP4_User_Service.API.Services;

// gRPC-tjeneste til håndtering af brugeroperationer
public class GrpcUserService : Protos.Grpc.UserService.UserServiceBase
{
    private readonly CreateUserUseCase _createUser;
    private readonly GetUserUseCase _getUser;
    private readonly GetAllUsersUseCase _getAllUsers;
    private readonly UpdateUserUseCase _updateUser;
    private readonly DeleteUserUseCase _deleteUser;

    // Initialiserer tjenesten med nødvendige use cases
    public GrpcUserService(
        CreateUserUseCase createUser,
        GetUserUseCase getUser,
        GetAllUsersUseCase getAllUsers,
        UpdateUserUseCase updateUser,
        DeleteUserUseCase deleteUser)
    {
        _createUser = createUser;
        _getUser = getUser;
        _getAllUsers = getAllUsers;
        _updateUser = updateUser;
        _deleteUser = deleteUser;
    }

    // Opretter en ny bruger
    public override async Task<UserResponse> CreateUser(CreateUserRequest request, ServerCallContext context)
    {
        var user = new User(
            request.Username,
            request.Password,
            request.Email,
            request.Firstname,
            request.Lastname,
            request.Birthday
        );

        var locations = request.Locations.Select(loc => new Location(
            loc.Street, 
            loc.HouseNumber, 
            loc.City, 
            loc.Country
        )
        {
            UserID = user.Id 
        }).ToList();

        user.Locations = locations;

        var created = await _createUser.ExecuteAsync(user);

        return new UserResponse
        {
            Id = created.Id.ToString(),
            Username = created.Username,
            Firstname = created.Firstname,
            Lastname = created.Lastname,
            Email = created.Email,
            Birthday = created.Birthday,
            Locations = { created.Locations.Select(MapToLocationMessage) }
        };
    }

    // Henter en bruger baseret på ID
    public override async Task<UserResponse> GetUserById(GetUserByIdRequest request, ServerCallContext context)
    {
        var id = Guid.Parse(request.Id);
        var user = await _getUser.ExecuteByIdAsync(id);

        if (user == null)
            throw new RpcException(new Status(StatusCode.NotFound, "User not found"));

        return new UserResponse
        {
            Id = user.Id.ToString(),
            Username = user.Username,
            Firstname = user.Firstname,
            Lastname = user.Lastname,
            Email = user.Email,
            Birthday = user.Birthday,
            Locations = { user.Locations.Select(MapToLocationMessage) }
        };
    }

    // Henter alle brugere
    public override async Task<UserListResponse> GetAllUsers(Empty request, ServerCallContext context)
    {
        var users = await _getAllUsers.ExecuteAsync();
        var response = new UserListResponse();

        response.Users.AddRange(users.Select(user => new UserResponse
        {
            Id = user.Id.ToString(),
            Username = user.Username,
            Firstname = user.Firstname,
            Lastname = user.Lastname,
            Email = user.Email,
            Birthday = user.Birthday,
            Locations = { user.Locations.Select(MapToLocationMessage) }
        }));

        return response;
    }

    // Opdaterer en brugers data
    public override async Task<Empty> UpdateUser(UpdateUserRequest request, ServerCallContext context)
    {
        var locations = request.Locations.Select(loc => new Location(
            loc.Street, loc.HouseNumber, loc.City, loc.Country)).ToList();

        var user = new User
        {
            Id = Guid.Parse(request.Id),
            Username = request.Username,
            Password = request.Password,
            Firstname = request.Firstname,
            Lastname = request.Lastname,
            Email = request.Email,
            Birthday = request.Birthday,
            Locations = locations
        };

        await _updateUser.ExecuteAsync(user);
        return new Empty();
    }

    // Sletter en bruger baseret på ID
    public override async Task<DeleteUserResponse> DeleteUser(DeleteUserRequest request, ServerCallContext context)
    {
        var id = Guid.Parse(request.Id);
        var success = await _deleteUser.ExecuteAsync(id);
        return new DeleteUserResponse { Success = success };
    }

    // Mapper en lokation til en gRPC-besked
    private static LocationMessage MapToLocationMessage(Location location)
    {
        return new LocationMessage
        {
            Street = location.Street,
            HouseNumber = location.HouseNumber,
            City = location.City,
            Country = location.Country
        };
    }

    // Ændrer en brugers adgangskode
    public override async Task<ChangePasswordResponse> ChangePassword(ChangePasswordRequest request, ServerCallContext context)
    {
        var userId = Guid.Parse(request.UserId);
        var user = await _getUser.ExecuteByIdAsync(userId);

        if (user == null)
        {
            return new ChangePasswordResponse
            {
                Success = false,
                Message = "Bruger ikke fundet."
            };
        }

        bool valid = BCrypt.Net.BCrypt.Verify(request.OldPassword, user.Password);

        if (!valid)
        {
            return new ChangePasswordResponse
            {
                Success = false,
                Message = "Gammelt kodeord er forkert."
            };
        }

        user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        await _updateUser.ExecuteAsync(user);

        return new ChangePasswordResponse
        {
            Success = true,
            Message = "Kodeord ændret."
        };
    }
}
