using GpsUtil.Location;
using TourGuide.Users;


namespace TourGuideTest;

// Classe de test pour le service RewardsService
public class RewardServiceTest : IClassFixture<DependencyFixture>
{
    private readonly DependencyFixture _fixture;

    public RewardServiceTest(DependencyFixture fixture)
    {
        _fixture = fixture;
    }

    // Test pour vérifier que les récompenses sont attribuées correctement lorsqu'un utilisateur visite une attraction
    [Fact]
    public void UserGetRewards()
    {
        _fixture.Initialize(0);
        var user = new User(Guid.NewGuid(), "jon", "000", "jon@tourGuide.com");
        var attraction = _fixture.GpsUtil.GetAttractions().First();
        user.AddToVisitedLocations(new VisitedLocation(user.UserId, attraction, DateTime.Now));
        _fixture.TourGuideService.TrackUserLocation(user);
        var userRewards = user.UserRewards;
        _fixture.TourGuideService.Tracker.StopTracking();
        Assert.True(userRewards.Count == 1);
    }

    // Test pour vérifier que la méthode IsWithinAttractionProximity fonctionne correctement
    [Fact]
    public void IsWithinAttractionProximity()
    {
        var attraction = _fixture.GpsUtil.GetAttractions().First();
        Assert.True(_fixture.RewardsService.IsWithinAttractionProximity(attraction, attraction));
    }


    // Test pour vérifier que les récompenses sont attribuées pour toutes les attractions à proximité
    [Fact]
    public void NearAllAttractions()
    {
        _fixture.Initialize(1);
        _fixture.RewardsService.SetProximityBuffer(int.MaxValue);

        var user = _fixture.TourGuideService.GetAllUsers().First();
        _fixture.RewardsService.CalculateRewards(user);
        var userRewards = _fixture.TourGuideService.GetUserRewards(user);
        _fixture.TourGuideService.Tracker.StopTracking();

        Assert.Equal(_fixture.GpsUtil.GetAttractions().Count, userRewards.Count);
    }

}
