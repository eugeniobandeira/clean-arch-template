using CleanArch.Domain.Entities;
using CleanArch.Domain.Interfaces.Common;

namespace CleanArch.Domain.Interfaces.Examples;

public interface IExampleRepository :
    IAddRepository<ExampleEntity>,
    IGetByIdRepository<ExampleEntity>,
    IUpdateRepository<ExampleEntity>,
    IDeleteRepository<ExampleEntity>
{
}
