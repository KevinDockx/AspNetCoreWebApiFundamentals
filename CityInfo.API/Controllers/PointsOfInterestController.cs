﻿using Asp.Versioning;
using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers;

[Route("api/v{version:apiVersion}/cities/{cityId}/pointsofinterest")]
[Authorize(Policy = "MustBeFromAntwerp")]
[ApiVersion(2)]
[ApiController]
public class PointsOfInterestController(ILogger<PointsOfInterestController> logger,
    IMailService mailService,
    ICityInfoRepository cityInfoRepository,
    IMapper mapper) : ControllerBase
{
    private readonly ILogger<PointsOfInterestController> _logger = logger ??
            throw new ArgumentNullException(nameof(logger));
    private readonly IMailService _mailService = mailService ??
            throw new ArgumentNullException(nameof(mailService));
    private readonly ICityInfoRepository _cityInfoRepository = cityInfoRepository ??
            throw new ArgumentNullException(nameof(cityInfoRepository));
    private readonly IMapper _mapper = mapper ??
            throw new ArgumentNullException(nameof(mapper));

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterest(
        int cityId)
    {
        if (!await _cityInfoRepository.CityExistsAsync(cityId))
        {
            _logger.LogInformation(
                $"City with id {cityId} wasn't found when accessing points of interest.");
            return NotFound();
        }

        var pointsOfInterestForCity = await _cityInfoRepository
            .GetPointsOfInterestForCityAsync(cityId);

        return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterestForCity));
    }

    [HttpGet("{pointofinterestid}", Name = "GetPointOfInterest")]
    public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterest(
        int cityId, int pointOfInterestId)
    {
        if (!await _cityInfoRepository.CityExistsAsync(cityId))
        {
            return NotFound();
        }

        var pointOfInterest = await _cityInfoRepository
            .GetPointOfInterestForCityAsync(cityId, pointOfInterestId);

        if (pointOfInterest == null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<PointOfInterestDto>(pointOfInterest));
    }

    [HttpPost]
    public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterest(
           int cityId,
           PointOfInterestForCreationDto pointOfInterest)
    {
        if (!await _cityInfoRepository.CityExistsAsync(cityId))
        {
            return NotFound();
        }

        var finalPointOfInterest = _mapper.Map<Entities.PointOfInterest>(pointOfInterest);

        await _cityInfoRepository.AddPointOfInterestForCityAsync(
            cityId, finalPointOfInterest);

        await _cityInfoRepository.SaveChangesAsync();

        var createdPointOfInterestToReturn =
            _mapper.Map<Models.PointOfInterestDto>(finalPointOfInterest);

        return CreatedAtRoute("GetPointOfInterest",
             new
             {
                 cityId,
                 pointOfInterestId = createdPointOfInterestToReturn.Id
             },
             createdPointOfInterestToReturn);
    }

    [HttpPut("{pointofinterestid}")]
    public async Task<ActionResult> UpdatePointOfInterest(int cityId, int pointOfInterestId,
        PointOfInterestForUpdateDto pointOfInterest)
    {
        if (!await _cityInfoRepository.CityExistsAsync(cityId))
        {
            return NotFound();
        }

        var pointOfInterestEntity = await _cityInfoRepository
            .GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
        if (pointOfInterestEntity == null)
        {
            return NotFound();
        }

        _mapper.Map(pointOfInterest, pointOfInterestEntity);

        await _cityInfoRepository.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{pointofinterestid}")]
    public async Task<ActionResult> PartiallyUpdatePointOfInterest(
       int cityId, int pointOfInterestId,
       JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
    {
        if (!await _cityInfoRepository.CityExistsAsync(cityId))
        {
            return NotFound();
        }

        var pointOfInterestEntity = await _cityInfoRepository
            .GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
        if (pointOfInterestEntity == null)
        {
            return NotFound();
        }

        var pointOfInterestToPatch = _mapper.Map<PointOfInterestForUpdateDto>(
            pointOfInterestEntity);

        patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!TryValidateModel(pointOfInterestToPatch))
        {
            return BadRequest(ModelState);
        }

        _mapper.Map(pointOfInterestToPatch, pointOfInterestEntity);
        await _cityInfoRepository.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{pointOfInterestId}")]
    public async Task<ActionResult> DeletePointOfInterest(
        int cityId, int pointOfInterestId)
    {
        if (!await _cityInfoRepository.CityExistsAsync(cityId))
        {
            return NotFound();
        }

        var pointOfInterestEntity = await _cityInfoRepository
            .GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
        if (pointOfInterestEntity == null)
        {
            return NotFound();
        }

        _cityInfoRepository.DeletePointOfInterest(pointOfInterestEntity);
        await _cityInfoRepository.SaveChangesAsync();

        _mailService.Send(
            "Point of interest deleted.",
            $"Point of interest {pointOfInterestEntity.Name} with id {pointOfInterestEntity.Id} was deleted.");

        return NoContent();
    }

}
