using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Seeder.Contracts
{
    public abstract class Seeder(IServiceProvider services)
    {
        private readonly IServiceProvider services = services;

        public abstract Task<bool> Seed();
    }
}
