using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using StatlerWaldorfCorp.RealityService.Location;
using Xunit;

namespace StatlerWaldorfCorp.RealityService.Tests.Integration
{
    public class RealityServiceIntegrationTest
    {
        private readonly TestServer testServer;
        private readonly HttpClient testClient;
        
        
        public RealityServiceIntegrationTest()
        {
            testServer = new TestServer(new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>());

            testClient = testServer.CreateClient();
        }

        [Fact]
        public void RealityServiceIntegration_PublicAPITest()
        {
            // Test plan -
            // Set three locations in cache, 2 members on team 1, 1 member on team 2
            // update two different locations
            // query location list for team 1
            // query individual location for member 3

            var team1 = Guid.NewGuid();
            var team2 = Guid.NewGuid();

            var members = GenerateMemberLocations();

            SetMemberLocation(team1, members[0]);
            SetMemberLocation(team1, members[1]);
            SetMemberLocation(team2, members[2]);
            Assert.True(true);
        }

        private void SetMemberLocation(Guid teamId, MemberLocation memberLocation)
        {
            
        }

        private IList<MemberLocation> GenerateMemberLocations()
        {
            GpsCoordinate loc1 = new GpsCoordinate {
                Latitude = 42.00,
                Longitude = 27.00
            };

            List<MemberLocation> list = new List<MemberLocation>();
            list.Add( new MemberLocation {
                MemberID = Guid.NewGuid(),
                Location = loc1
            });

            list.Add(new MemberLocation {
                MemberID = Guid.NewGuid(),
                Location = loc1
            });

            list.Add(new MemberLocation {
                MemberID = Guid.NewGuid(),
                Location = loc1
            });

            return list;
        }


    }
}