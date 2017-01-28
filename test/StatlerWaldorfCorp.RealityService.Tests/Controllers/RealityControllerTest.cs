using Xunit;
using Moq;
using StatlerWaldorfCorp.RealityService.Location;
using Microsoft.Extensions.Logging;
using StatlerWaldorfCorp.RealityService.Controllers;
using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace StatlerWaldorfCorp.RealityService.Tests.Controllers
{
    public class RealityControllerTest
    {
        [Fact]
        public void GetTeamMembersCallsLocationCache()
        {
            var cache = new Mock<ILocationCache>();

            Guid teamId = Guid.NewGuid();
            var fakeMembers = GenerateFakeMembers();
            cache.Setup( c=> c.GetMemberLocations(It.Is<Guid>( g => g == teamId ))).Returns( fakeMembers );
            var logger = new Mock<ILogger<RealityController>>();
            var controller = new RealityController(cache.Object, logger.Object);
            
            var result = controller.GetTeamMembers(teamId);

            var objectResult = (result as OkObjectResult);

            Assert.NotNull(objectResult);
                        
            List<MemberLocation> locations = (List<MemberLocation>)objectResult.Value;

            Assert.Equal(5, locations.Count);            
        }

        [Fact]
        public void PutTeamMemberCallsCachePut()
        {
            var cache = new Mock<ILocationCache>();

            Guid teamId = Guid.NewGuid();
            Guid memberId = Guid.NewGuid();
            var ml = new MemberLocation {
                MemberID = memberId,
                Location = new GpsCoordinate {
                    Latitude = 12.0,
                    Longitude = 50.0
                }
            };

            cache.Setup( c=> c.Put(It.Is<Guid>(g => g == teamId), It.IsAny<MemberLocation>()));

            var logger = new Mock<ILogger<RealityController>>();
            var controller = new RealityController(cache.Object, logger.Object);

            var result = controller.UpdateMemberLocation(teamId, memberId, ml);
            var objectResult = (result as OkObjectResult);
            Assert.NotNull(objectResult);
            var newMl = (MemberLocation)objectResult.Value;
            Assert.Equal(ml.MemberID, newMl.MemberID);
            cache.VerifyAll();
        }

        [Fact]
        public void GetTeamMemberCallsCacheGet()
        {
            var cache = new Mock<ILocationCache>();

            Guid teamId = Guid.NewGuid();
            Guid memberId = Guid.NewGuid();
             var ml = new MemberLocation {
                MemberID = memberId,
                Location = new GpsCoordinate {
                    Latitude = 12.0,
                    Longitude = 50.0
                }
            };
            cache.Setup( c => c.Get(It.Is<Guid>(g => g == teamId), It.Is<Guid>(g => g == memberId))).Returns( ml ); 
            var logger = new Mock<ILogger<RealityController>>();
            var controller = new RealityController(cache.Object, logger.Object);

            var result = controller.GetMemberLocation(teamId, memberId);
            var objectResult = result as OkObjectResult;
            Assert.NotNull(objectResult);
            var newMl = (MemberLocation)objectResult.Value;

            Assert.Equal(ml.MemberID, newMl.MemberID);
            cache.VerifyAll();
        }

        private List<MemberLocation> GenerateFakeMembers()
        {
            List<MemberLocation> locations = new List<MemberLocation>();
            for (int x=0; x<5; x++) 
            {
                MemberLocation ml = new MemberLocation()
                {
                    MemberID = Guid.NewGuid(),
                    Location = new GpsCoordinate() {
                        Latitude = 30.0,
                        Longitude = 20.0
                    }
                };
                locations.Add(ml);
            }
            return locations;
        }
    }
}