using System;
using System.IO;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
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
            Assert.True(true);
        }


    }
}