using System;
using System.Linq;
using System.Threading.Tasks;
using CodeSample.Application.Services;
using CodeSample.Domain.Entities;
using Grpc.Core;

namespace CodeSample.gRPC.Services;

public class PersonGrpcService : PersonService.PersonServiceBase
{
    private readonly CodeSample.Application.Services.PersonService _personService;
    private readonly ILogger<PersonGrpcService> _logger;

    public PersonGrpcService(
        CodeSample.Application.Services.PersonService personService,
        ILogger<PersonGrpcService> logger)
    {
        _personService = personService;
        _logger = logger;
    }

    public override Task<CreatePersonResponse> CreatePerson(CreatePersonRequest request, ServerCallContext context)
    {
        _logger.LogInformation("CreatePerson called: {First} {Last} nc={NC}",
            request.FirstName, request.LastName, request.NationalCode);

        var person = _personService.Create(
            request.FirstName,
            request.LastName,
            request.NationalCode,
            DateTime.Parse(request.BirthDate));

        return Task.FromResult(new CreatePersonResponse
        {
            Person = new PersonModel
            {
                Id = person.Id.ToString(),
                FirstName = person.FirstName,
                LastName = person.LastName,
                NationalCode = person.NationalCode,
                BirthDate = person.BirthDate.ToString("yyyy-MM-dd")
            }
        });
    }

    public override Task<GetPersonResponse> GetPerson(GetPersonRequest request, ServerCallContext context)
    {
        _logger.LogDebug($"GetPerson called: {request}");

        var person = _personService.Get(Guid.Parse(request.Id));
        if (person is null)
            throw new RpcException(new Status(StatusCode.NotFound, "Person not found"));

        return Task.FromResult(new GetPersonResponse
        {
            Person = new PersonModel
            {
                Id = person.Id.ToString(),
                FirstName = person.FirstName,
                LastName = person.LastName,
                NationalCode = person.NationalCode,
                BirthDate = person.BirthDate.ToString("yyyy-MM-dd")
            }
        });
    }

    public override Task<ListPeopleResponse> ListPeople(Google.Protobuf.WellKnownTypes.Empty request, ServerCallContext context)
    {
        _logger.LogDebug("ListPeople called");

        var list = _personService.GetAll()
            .Select(p => new PersonModel
            {
                Id = p.Id.ToString(),
                FirstName = p.FirstName,
                LastName = p.LastName,
                NationalCode = p.NationalCode,
                BirthDate = p.BirthDate.ToString("yyyy-MM-dd")
            });

        var resp = new ListPeopleResponse();
        resp.People.AddRange(list);
        return Task.FromResult(resp);
    }

    public override Task<UpdatePersonResponse> UpdatePerson(UpdatePersonRequest request, ServerCallContext context)
    {
        _logger.LogDebug($"UpdatePerson called: {request}");

        var success = _personService.Update(
            Guid.Parse(request.Id),
            request.FirstName,
            request.LastName,
            request.NationalCode,
            DateTime.Parse(request.BirthDate));

        if (!success)
            throw new RpcException(new Status(StatusCode.NotFound, "Person not found"));

        var updated = _personService.Get(Guid.Parse(request.Id))!;
        return Task.FromResult(new UpdatePersonResponse
        {
            Person = new PersonModel
            {
                Id = updated.Id.ToString(),
                FirstName = updated.FirstName,
                LastName = updated.LastName,
                NationalCode = updated.NationalCode,
                BirthDate = updated.BirthDate.ToString("yyyy-MM-dd")
            }
        });
    }

    public override Task<DeletePersonResponse> DeletePerson(DeletePersonRequest request, ServerCallContext context)
    {
        _logger.LogDebug($"DeletePerson called: {request}");

        var success = _personService.Delete(Guid.Parse(request.Id));
        return Task.FromResult(new DeletePersonResponse { Success = success });
    }
}
