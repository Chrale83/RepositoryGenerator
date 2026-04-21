using System;
using System.Collections.Generic;
using System.Text;
using RepositoryGenerator.Library.Attributes;
using RPDemo.Application.Interfaces;
using RPDemo.Domain.Entities;

namespace RPDemo.Infrastructure.Repositories
{
    [DbRepositoryFor<Car, DemoDbContext>]
    public partial class CarRepository : ICarRepo { }
}
