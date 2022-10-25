using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using SciMaterials.LinkSearch.WebAPI.Data.Interfaces;
using SciMaterials.LinkSearch.WebAPI.Data.Repositories;

namespace SciMaterials.LinkSearch.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LinkSearchController : ControllerBase
    {
        private readonly ILinkSearch _link;
        private readonly ILogger<LinkSearchRepository> _logger;

        public LinkSearchController(ILinkSearch link, ILogger<LinkSearchRepository> logger)
        {
            _link = link;
            _logger = logger;
        }

        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateAsync([FromBody] Models.LinkSearch linkSearch)
        {
            await _link.CreateAsync(linkSearch);
            return Ok();
        }

        [HttpGet("all")]
        public Task<IActionResult> GetAllAsync()
        {
            return Task.FromResult<IActionResult>(Ok(_link.GetAllAsync().Result));
        }

        [HttpGet("get/{id}")]
        public Task<IActionResult> GetLinkByIdAsync([FromRoute] int id)
        {
            return Task.FromResult<IActionResult>(Ok(_link.GetByIdAsync(id).Result));
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteIdLinkAsync([FromRoute] int id)
        {
            await _link.DeleteAsync(id);
            return Ok();
        }
    }
}
