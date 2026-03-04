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
        var location = _tourGuideService.GetUserLocation(GetUser(userName));
        return Ok(location);
    }

    // Endpoint pour récupérer les attractions à proximité de l'utilisateur
    [HttpGet("getNearbyAttractions")]
    public ActionResult<List<NearbyAttractionDto>> GetNearbyAttractions([FromQuery] string userName)
    {
        var user = GetUser(userName);
        var visitedLocation = _tourGuideService.GetUserLocation(user);
        var attractions = _tourGuideService.GetNearByAttractions(visitedLocation);

        var result = attractions.Select(a =>
        {
            var attractionLocation = new Locations(a.Latitude, a.Longitude);

            // Calculer la distance entre l'utilisateur et l'attraction, ainsi que les points de récompense
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

    // Endpoint pour récupérer les récompenses de l'utilisateur
    [HttpGet("getRewards")]
    public ActionResult<List<UserReward>> GetRewards([FromQuery] string userName)
    {
        var rewards = _tourGuideService.GetUserRewards(GetUser(userName));
        return Ok(rewards);
    }
   
    // Endpoint pour récupérer les offres de voyage pour l'utilisateur
    [HttpGet("getTripDeals")]
    public ActionResult<List<Provider>> GetTripDeals([FromQuery] string userName)
    {
        var deals = _tourGuideService.GetTripDeals(GetUser(userName));
        return Ok(deals);
    }

    private User GetUser(string userName)
    {
        return _tourGuideService.GetUser(userName);
    }
}
