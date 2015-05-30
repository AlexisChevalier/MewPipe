using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MewPipe.Logic.Models;
using MewPipe.Logic.Repositories;

namespace MewPipe.Logic.Contracts
{
    public class CategoryContract
    {
        public CategoryContract()
        {
        }
        public CategoryContract(Category category)
        {
            if (category == null)
            {
                return;
            }

            Id = category.Id.ToString();
            Name = category.Name;
        }

        public string Id { get; set; }
        public string Name { get; set; }
    }
}