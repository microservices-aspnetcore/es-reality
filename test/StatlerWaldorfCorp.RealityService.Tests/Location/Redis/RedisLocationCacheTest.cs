using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Moq;
using StackExchange.Redis;
using StatlerWaldorfCorp.RealityService.Location;
using StatlerWaldorfCorp.RealityService.Location.Redis;
using Xunit;

namespace StatlerWaldorfCorp.RealityService.Tests.Location.Redis
{
    public class RedisLocationCacheTest
    {
        [Fact]
        public void PutCallsHashSet_TeamAsHashKey_MemberAsKey()
        {
            var multiplexer = new Mock<IConnectionMultiplexer>();
            var logger = new Mock<ILogger<RedisLocationCache>>();
            var db = new Mock<IDatabase>();
            var teamId = Guid.NewGuid();            

            MemberLocation location = new MemberLocation {
                MemberID = Guid.NewGuid(),
                Location = new GpsCoordinate {
                    Latitude = 10.0,
                    Longitude = 20.0
                }
            };

            RedisKey key = teamId.ToString();
            RedisValue v1 = location.MemberID.ToString();
            RedisValue v2 = location.ToJsonString();

            db.Setup( d => d.HashSet(It.Is<RedisKey>( k => k == key),
                                     It.Is<RedisValue>( v => v == v1 ),
                                     It.Is<RedisValue>( v => v == v2),
                                     When.Always, CommandFlags.PreferMaster));
            
            multiplexer.Setup( m => m.GetDatabase(It.IsAny<int>(), null) ).Returns(db.Object);
            var cache = new RedisLocationCache(logger.Object, multiplexer.Object);        
            
            cache.Put(teamId, location);
            multiplexer.VerifyAll();
            db.VerifyAll();
        }

        [Fact]
        public void GetSingleMemberReturns_And_Converts()
        {
            var multiplexer = new Mock<IConnectionMultiplexer>();
            var logger = new Mock<ILogger<RedisLocationCache>>();
            var db = new Mock<IDatabase>();
            MemberLocation ml1 = new MemberLocation {
                MemberID = Guid.NewGuid(),
                Location = new GpsCoordinate {
                    Latitude = 10.0,
                    Longitude = 30.0
                }
            };

            var teamId = Guid.NewGuid();
            RedisValue hashValue = (RedisValue)ml1.ToJsonString();    
            multiplexer.Setup( m => m.GetDatabase(It.IsAny<int>(), null) ).Returns(db.Object);
            db.Setup( d => d.HashGet(It.Is<RedisKey>( r => r == teamId.ToString() ), It.Is<RedisValue>( f => f == ml1.MemberID.ToString()), 
                    CommandFlags.None)).Returns(hashValue);
            
            var cache = new RedisLocationCache(logger.Object, multiplexer.Object);

            MemberLocation mlExpected = cache.Get(teamId, ml1.MemberID);

            Assert.Equal(mlExpected.MemberID, ml1.MemberID);
            db.VerifyAll();
        }

        [Fact]
        public void GetMembers_CallsHashValues_And_Converts()
        {
            var multiplexer = new Mock<IConnectionMultiplexer>();
            var logger = new Mock<ILogger<RedisLocationCache>>();
            var db = new Mock<IDatabase>();
            MemberLocation ml1 = new MemberLocation {
                MemberID = Guid.NewGuid(),
                Location = new GpsCoordinate {
                    Latitude = 10.0,
                    Longitude = 30.0
                }
            };
            MemberLocation ml2 = new MemberLocation {
                MemberID = Guid.NewGuid(),
                Location = new GpsCoordinate {
                    Latitude = 10.0,
                    Longitude = 40.0
                }
            };
            MemberLocation ml3 = new MemberLocation {
                MemberID = Guid.NewGuid(),
                Location = new GpsCoordinate {
                    Latitude = 10.0,
                    Longitude = 50.0
                }
            };
            var teamId = Guid.NewGuid();
            RedisValue[] hashValues = new RedisValue[] {
                ml1.ToJsonString(), ml2.ToJsonString(), ml3.ToJsonString()
            };

            multiplexer.Setup( m => m.GetDatabase(It.IsAny<int>(), null) ).Returns(db.Object);
            db.Setup( d => d.HashValues(It.Is<RedisKey>( r => r== teamId.ToString()), CommandFlags.None)).Returns(hashValues);
            
            var cache = new RedisLocationCache(logger.Object, multiplexer.Object);


            IList<MemberLocation> members = cache.GetMemberLocations(teamId);
            
            multiplexer.VerifyAll();
            db.VerifyAll();

            Assert.Equal(3, members.Count);

        }
    }
}