using System;
using System.Collections.Generic;
using System.Text;
using RepositoryGenerator.Library.Attributes;
using RPDemo.Domain.Entities;

namespace RPDemo.Application.Interfaces
{
    [RPInterface<Car>]
    public partial interface ICarRepo { }
}
