using RepositoryGenerator.Library.Attributes;
using RPDemo.Domain.Entities;

namespace RPDemo.Application.Interfaces
{
    [RPInterface<Person>]
    public partial interface IPersonRepo { }

    [RPInterface<Car>]
    public partial interface ICarRepo { }
}
