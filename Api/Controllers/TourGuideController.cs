using GpsUtil.Location;
using Microsoft.AspNetCore.Mvc;
using TourGuide.Services.Interfaces;
using TourGuide.Users;
using TripPricer;
using TourGuide.Models;


namespace TourGuide.Controllers;

[ApiController]
[Route("[controller]")]
public class TourGuideController : ControllerBase
{
    private readonly ITourGuideService _tourGuideService;

    public TourGuideController(ITourGuideService tourGuideService)
    {
        _tourGuideService = tourGuideService;
    }

    // Endpoint pour récupérer la localisation actuelle de l'utilisateur
    [HttpGet("getLocation")]
    public ActionResult<VisitedLocation> GetLocation([FromQuery] string userName)
    {
        var user = _tourGuideService.GetUser(userName);

        if (user == null)
        {
            return NotFound($"Utilisateur '{userName}' introuvable.");
        }

        var location = _tourGuideService.GetUserLocation(user);
        return Ok(location);
    }

    // Endpoint pour récupérer les attractions à proximité de l'utilisateur
    [HttpGet("getNearbyAttractions")]
    public ActionResult<List<NearbyAttractionDto>> GetNearbyAttractions([FromQuery] string userName)
    {
        var user = _tourGuideService.GetUser(userName);

        if (user == null)
        {
            return NotFound($"Utilisateur '{userName}' introuvable.");
        }

        var visitedLocation = _tourGuideService.GetUserLocation(user);
        var attractions = _tourGuideService.GetNearByAttractions(visitedLocation);

        var result = attractions.Select(a =>
        {
            var attractionLocation = new Locations(a.Latitude, a.Longitude);

            return new NearbyAttractionDto
            {
                AttractionName = a.AttractionName,
                AttractionLatitude = a.Latitude,
                AttractionLongitude = a.Longitude,
                UserLatitude = visitedLocation.Location.Latitude,
                UserLongitude = visitedLocation.Location.Longitude,
                DistanceInMiles = _tourGuideService.GetDistance(
                    visitedLocation.Location,
                    attractionLocation),
                RewardPoints = _tourGuideService.GetAttractionRewardPoints(
                    a.AttractionId,
                    user.UserId)
            };
        }).ToList();

        return Ok(result);
    }

    [HttpGet("getRewards")]
    public ActionResult<List<UserReward>> GetRewards([FromQuery] string userName)
    {
        var user = _tourGuideService.GetUser(userName);

        if (user == null)
        {
            return NotFound($"Utilisateur '{userName}' introuvable.");
        }

        var rewards = _tourGuideService.GetUserRewards(user);
        return Ok(rewards);
    }

    [HttpGet("getTripDeals")]
    public ActionResult<List<Provider>> GetTripDeals([FromQuery] string userName)
    {
        var user = _tourGuideService.GetUser(userName);

        if (user == null)
        {
            return NotFound($"Utilisateur '{userName}' introuvable.");
        }

        var deals = _tourGuideService.GetTripDeals(user);
        return Ok(deals);
    }
}
