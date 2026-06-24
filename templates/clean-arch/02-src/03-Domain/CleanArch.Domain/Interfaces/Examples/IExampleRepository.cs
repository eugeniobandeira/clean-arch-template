using CleanArch.Domain.Entities;
using CleanArch.Domain.Filters.Examples;
using CleanArch.Domain.Interfaces.Common;

namespace CleanArch.Domain.Interfaces.Examples;

public interface IExampleRepository :
    IAddRepository<ExampleEntity>,
    IGetByIdRepository<ExampleEntity>,
    IGetAllRepository<ExampleEntity, ExampleFilter>,
    IUpdateRepository<ExampleEntity>,
    IDeleteRepository<ExampleEntity>
{
}
