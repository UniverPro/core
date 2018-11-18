﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Uni.WebApi.Models.Requests;
using Uni.WebApi.Models.Responses;

namespace Uni.WebApi.Controllers
{
    [ApiController]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/universities")]
    public class UniversitiesController : ControllerBase
    {
        [HttpGet]
        public async Task<IEnumerable<UniversityResponseModel>> Get()
        {
            throw new NotImplementedException();
        }

        [HttpGet("{id}")]
        public async Task<UniversityResponseModel> Get(int id)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public async Task<UniversityResponseModel> Post([FromBody] UniversityRequestModel model)
        {
            throw new NotImplementedException();
        }

        [HttpPut("{id}")]
        public async Task<UniversityResponseModel> Put(int id, [FromBody] UniversityRequestModel model)
        {
            throw new NotImplementedException();
        }

        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
