using GpsUtil.Location;
using System.Diagnostics;
using TourGuide.Users;
using Xunit.Abstractions;

namespace TourGuideTest
{

    // Classe de test pour vérifier les performances du service TourGuideService et RewardsService avec un grand nombre d'utilisateurs
    public class PerformanceTest : IClassFixture<DependencyFixture>
    {
      

        private readonly DependencyFixture _fixture;

        private readonly ITestOutputHelper _output;

        public PerformanceTest(DependencyFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _output = output;
        }

        // Test pour vérifier que la méthode de suivi de localisation fonctionne correctement même avec un grand nombre d'utilisateurs
        [Trait("Category", "Performance")]
        [Fact]
        public async Task HighVolumeTrackLocation()
        {
            _fixture.Initialize(100000);

            List<User> allUsers = _fixture.TourGuideService.GetAllUsers();
            Console.WriteLine($"Users: {allUsers.Count}");


            var stopWatch = Stopwatch.StartNew();

            const int maxConcurrency = 200;
            using var sem = new SemaphoreSlim(maxConcurrency);

            var tasks = allUsers.Select(async user =>
            {
                await sem.WaitAsync();
                try
                {
                    
                    await Task.Run(() => _fixture.TourGuideService.TrackUserLocation(user));
                }
                finally
                {
                    sem.Release();
                }
            });

            await Task.WhenAll(tasks);

            stopWatch.Stop();
            _fixture.TourGuideService.Tracker.StopTracking();

            _output.WriteLine($"highVolumeTrackLocation: Time Elapsed: {stopWatch.Elapsed.TotalSeconds} seconds.");
            Console.WriteLine($"Users: {allUsers.Count}");

            Assert.True(TimeSpan.FromMinutes(15).TotalSeconds >= stopWatch.Elapsed.TotalSeconds);
        }

        // Test pour vérifier que la méthode de calcul des récompenses fonctionne correctement même avec un grand nombre d'utilisateurs
        [Trait("Category", "Performance")]
        [Fact]
        public async Task HighVolumeGetRewards()
        {
            _fixture.Initialize(100000);

            var stopWatch = Stopwatch.StartNew();

            Attraction attraction = _fixture.GpsUtil.GetAttractions()[0];
            List<User> allUsers = _fixture.TourGuideService.GetAllUsers();
            Console.WriteLine($"Users: {allUsers.Count}");


            allUsers.ForEach(u => u.AddToVisitedLocations(new VisitedLocation(u.UserId, attraction, DateTime.Now)));

            const int maxConcurrency = 200;
            using var sem = new SemaphoreSlim(maxConcurrency);

            var tasks = allUsers.Select(async user =>
            {
                await sem.WaitAsync();
                try
                {
                    await Task.Run(() => _fixture.RewardsService.CalculateRewards(user));
                }
                finally
                {
                    sem.Release();
                }
            });

            await Task.WhenAll(tasks);

            foreach (var user in allUsers)
            {
                Assert.True(user.UserRewards.Count > 0);
            }

            stopWatch.Stop();
            _fixture.TourGuideService.Tracker.StopTracking();

            _output.WriteLine($"highVolumeGetRewards: Time Elapsed: {stopWatch.Elapsed.TotalSeconds} seconds.");
            Console.WriteLine($"highVolumeGetRewards: Time Elapsed: {stopWatch.Elapsed.TotalSeconds} seconds.");

            Assert.True(TimeSpan.FromMinutes(20).TotalSeconds >= stopWatch.Elapsed.TotalSeconds);
        }
    }
}
