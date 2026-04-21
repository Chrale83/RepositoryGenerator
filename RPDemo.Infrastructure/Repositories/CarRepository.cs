using RepositoryGenerator.Library.Attributes;
using RPDemo.Domain.Entities;
using RPDemo.Domain.Interfaces;

namespace RPDemo.Infrastructure.Repositories
{
    [DbRepositoryFor<Car, DemoDbContext>]
    public partial class CarRepository : ICarRepository { }
}
