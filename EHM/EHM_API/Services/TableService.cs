﻿using System.Collections.Generic;
using System.Threading.Tasks;
using EHM_API.Models;
using EHM_API.Repositories;

namespace EHM_API.Services
{
    public class TableService : ITableService
    {
        private readonly ITableRepository _repository;

        public TableService(ITableRepository repository)
        {
            _repository = repository;
        }

     
    }
}
