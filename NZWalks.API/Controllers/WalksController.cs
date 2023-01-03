﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WalksController : Controller
    {
        private readonly IWalkRepository walkRepository;
        private readonly IMapper mapper;

        public WalksController(IWalkRepository walkRepository, IMapper mapper
            )
        {
            this.walkRepository = walkRepository;
            this.mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllWalksAsync() {

            // Fetch data from database - domain walks
            var walksDomain = await walkRepository.GetAllAsync();

            //Convert domain walks to DTO Walks 

            var walkDTO = mapper.Map<List<Models.DTO.Walk>>(walksDomain);
            // Return Response
            return Ok(walkDTO);
        }

        [HttpGet]
        [Route("{id : guid}")]
        [ActionName("GetWalkAsync")]

        public async Task<IActionResult> GetWalkAsync(Guid id) {
            //Get Walk Domain object from database
            var walkDomain = await walkRepository.GetAsync(id);

            //Convert Domain object to DTO 
            var walkDTO = mapper.Map<Models.DTO.Walk>(walkDomain);

            //return response
            return Ok(walkDTO);
        }


        [HttpPost]
        public async Task<IActionResult> AddWalkAsync([FromBody] AddWalkRequest addWalkRequest) {

            // Convert the DTO to Domain object
            var walkDomain = new Models.Domain.Walk()
            {
                Length = addWalkRequest.Length,
                Name = addWalkRequest.Name,
                RegionId = addWalkRequest.RegionId,
                WalkDifficultyId = addWalkRequest.WalkDifficultyId
            };

            //Pass The Domain object to repository to persist this
            walkDomain = await walkRepository.AddAsync(walkDomain);

            //Convert the Domain back to DTO
            var walkDTO = new Models.DTO.Walk()
            {
                Id = walkDomain.Id,
                Length = walkDomain.Length,
                Name = walkDomain.Name,
                RegionId = walkDomain.RegionId,
                WalkDifficultyId = walkDomain.WalkDifficultyId
            };

            //Send DTO reponse back to Client
            return CreatedAtAction(nameof(GetWalkAsync), new { id = walkDTO.Id }, walkDTO);

        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateWalkAsync([FromRoute] Guid id, [FromBody] UpdateWalkRequest updateWalkRequest) {
            //Convert DTO to Domain object
            var walkDomain = new Models.Domain.Walk()
            {
                Length = updateWalkRequest.Length,
                Name = updateWalkRequest.Name,
                RegionId = updateWalkRequest.RegionId,
                WalkDifficultyId = updateWalkRequest.WalkDifficultyId

            };

            //Pass details to Repository - Get Domain object in response (or null)

            walkDomain = await walkRepository.UpdateAsync(id, walkDomain);

            //Handle Null (not found)
            if (walkDomain == null) {
                return NotFound();
            
            }
            // Convert back DOmain object to DTO object

            var walkDTO = new Models.DTO.Walk()
            {
                Id = walkDomain.Id,
                Length = walkDomain.Length,
                Name = walkDomain.Name,
                RegionId = walkDomain.RegionId,
                WalkDifficultyId = walkDomain.WalkDifficultyId
            };

            //Return Response 
            return Ok(walkDTO);
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteWalkAsync(Guid id) { 
            //call Repository to delete walk
            var walkDomain = await walkRepository.DeleteAsync(id);
            if (walkDomain == null)
            {
                return NotFound();
            }
            //convert domain object to DTO object
            var walkDTO = mapper.Map<Models.DTO.Walk>(walkDomain);

            return Ok(walkDTO);
        
        }
    }
}
