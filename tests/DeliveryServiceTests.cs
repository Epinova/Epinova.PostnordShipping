using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Epinova.PostnordShipping;
using EPiServer.Logging;
using Moq;
using Xunit;

namespace Epinova.PostnordShippingTests
{
    public class DeliveryServiceTests
    {
        private readonly ClientInfo _clientInfo;
        private readonly Mock<ILogger> _logMock;
        private readonly TestableHttpMessageHandler _messageHandler;
        private readonly DeliveryService _service;

        public DeliveryServiceTests()
        {
            var mapperConfiguration = new MapperConfiguration(cfg => { cfg.AddProfile(new DeliveryMappingProfile()); });
            _messageHandler = new TestableHttpMessageHandler();
            _logMock = new Mock<ILogger>();
            var cacheHelperMock = new Mock<ICacheHelper>();
            DeliveryService.Client = new HttpClient(_messageHandler) { BaseAddress = new Uri("https://fake.api.uri/") };
            _service = new DeliveryService(_logMock.Object, mapperConfiguration.CreateMapper(), cacheHelperMock.Object);

            _clientInfo = new ClientInfo { ApiKey = Factory.GetString(), Country = CountryCode.NO };
        }

        [Fact]
        public async Task GetAllServicePoints_FromAPI_ParseResultFails_ReturnsEmptyArray()
        {
            _messageHandler.SendAsyncReturns(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{ 'Some': 'random', 'unparasable': 'json' }")
            });
            ServicePointInformation[] result = await _service.GetAllServicePointsAsync(_clientInfo, true);

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllServicePoints_FromAPI_ServiceReturnsNull_LogsError()
        {
            _messageHandler.SendAsyncReturns(null);
            await _service.GetAllServicePointsAsync(_clientInfo, true);

            _logMock.VerifyLog(Level.Error, "Get all service points failed. Service response was NULL", Times.Once());
        }

        [Fact]
        public async Task GetAllServicePoints_FromAPI_ServiceReturnsNull_ReturnsEmptyArray()
        {
            _messageHandler.SendAsyncReturns(null);
            ServicePointInformation[] result = await _service.GetAllServicePointsAsync(_clientInfo, true);

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllServicePoints_FromAPI_ServiceReturnsUnauthorizedStatus_LogsError()
        {
            _messageHandler.SendAsyncReturns(new HttpResponseMessage(HttpStatusCode.Unauthorized));

            await _service.GetAllServicePointsAsync(_clientInfo, true);

            _logMock.VerifyLog<object>(Level.Error, Times.Once());
        }

        [Fact]
        public async Task GetAllServicePoints_FromAPI_ServiceReturnsUnauthorizedStatus_ReturnsEmptyArray()
        {
            _messageHandler.SendAsyncReturns(new HttpResponseMessage(HttpStatusCode.Unauthorized));
            ServicePointInformation[] result = await _service.GetAllServicePointsAsync(_clientInfo, true);

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllServicePoints_FromAPI_ServiceReturnsValidJson_ReturnsServicePointArray()
        {
            _messageHandler.SendAsyncReturns(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(GetValidServiceResultJson(singleResult: true))
            });
            ServicePointInformation[] result = await _service.GetAllServicePointsAsync(_clientInfo, true);

            Assert.True(result.Length > 0);
        }

        [Fact]
        public async Task GetServicePointLive_ParseResultFails_ReturnsNull()
        {
            _messageHandler.SendAsyncReturns(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{ 'Some': 'random', 'unparasable': 'json' }")
            });
            ServicePointInformation result = await _service.GetServicePointLiveAsync(_clientInfo, Factory.GetString(4));

            Assert.Null(result);
        }

        [Fact]
        public async Task GetServicePointLive_ServiceReturnsNull_LogsError()
        {
            _messageHandler.SendAsyncReturns(null);
            await _service.GetServicePointLiveAsync(_clientInfo, Factory.GetString(7));

            _logMock.VerifyLog<object>(Level.Error, Times.Once());
        }

        [Fact]
        public async Task GetServicePointLive_ServiceReturnsNull_ReturnsNull()
        {
            _messageHandler.SendAsyncReturns(null);
            ServicePointInformation result = await _service.GetServicePointLiveAsync(_clientInfo, Factory.GetString(7));

            Assert.Null(result);
        }

        [Fact]
        public async Task GetServicePointLive_ServiceReturnsUnauthorizedStatus_LogsError()
        {
            _messageHandler.SendAsyncReturns(new HttpResponseMessage(HttpStatusCode.Unauthorized));

            await _service.GetServicePointLiveAsync(_clientInfo, Factory.GetString(7));

            _logMock.VerifyLog<object>(Level.Error, Times.Once());
        }

        [Fact]
        public async Task GetServicePointLive_ServiceReturnsUnauthorizedStatus_ReturnsNull()
        {
            _messageHandler.SendAsyncReturns(new HttpResponseMessage(HttpStatusCode.Unauthorized));
            ServicePointInformation result = await _service.GetServicePointLiveAsync(_clientInfo, Factory.GetString(7));

            Assert.Null(result);
        }

        [Fact]
        public async Task GetServicePointLive_ServiceReturnsValidJson_ReturnsServicePointInfo()
        {
            _messageHandler.SendAsyncReturns(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(GetValidServiceResultJson(singleResult: true))
            });
            ServicePointInformation result = await _service.GetServicePointLiveAsync(_clientInfo, Factory.GetString(7));

            Assert.IsType<ServicePointInformation>(result);
        }

        [Fact]
        public async Task GetServicePointLive_ServiceReturnsValidJson_ReturnsServiceWithCorrectCoordinates()
        {
            _messageHandler.SendAsyncReturns(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(GetValidServiceResultJson(singleResult: true))
            });
            ServicePointInformation result = await _service.GetServicePointLiveAsync(_clientInfo, Factory.GetString(7));

            Assert.Equal(10.754335161831872F, result.Easting);
            Assert.Equal(59.91169773310983F, result.Northing);
            Assert.Equal("EPSG:4326", result.CoordinateId);
        }


        private static string GetValidServiceResultJson(bool withDuplicates = false, bool singleResult = false)
        {
            if (singleResult)
                return @"{
  ""servicePointInformationResponse"": {
    ""customerSupports"": [
      {
        ""customerSupportPhoneNo"": ""+4722329022"",
        ""country"": ""NO""
      }
    ],
    ""servicePoints"": [
      {
        ""name"": ""COOP PRIX POSTGIROBYGGET"",
        ""servicePointId"": ""3690898"",
        ""routingCode"": ""ALF"",
        ""visitingAddress"": {
          ""countryCode"": ""NO"",
          ""city"": ""OSLO"",
          ""streetName"": ""BISKOP GUNNERUSG"",
          ""streetNumber"": ""14"",
          ""postalCode"": ""0155""
        },
        ""deliveryAddress"": {
          ""countryCode"": ""NO"",
          ""city"": ""OSLO"",
          ""streetName"": ""BISKOP GUNNERUSG"",
          ""streetNumber"": ""14"",
          ""postalCode"": ""0155""
        },
        ""openingHours"": [
          {
            ""day"": ""MONDAY"",
            ""from1"": ""06:00"",
            ""to1"": ""23:00""
          },
          {
            ""day"": ""TUESDAY"",
            ""from1"": ""06:00"",
            ""to1"": ""23:00""
          },
          {
            ""day"": ""WEDNESDAY"",
            ""from1"": ""06:00"",
            ""to1"": ""23:00""
          },
          {
            ""day"": ""THURSDAY"",
            ""from1"": ""06:00"",
            ""to1"": ""23:00""
          },
          {
            ""day"": ""FRIDAY"",
            ""from1"": ""06:00"",
            ""to1"": ""23:00""
          },
          {
            ""day"": ""SATURDAY"",
            ""from1"": ""07:00"",
            ""to1"": ""22:00""
          }
        ],
        ""eligibleParcelOutlet"": true,
        ""notificationArea"": {
          ""postalCodes"": [
            ""0025"",
            ""0050"",
            ""0051"",
            ""0101"",
            ""0102"",
            ""0103"",
            ""0104"",
            ""0105"",
            ""0106"",
            ""0107"",
            ""0109"",
            ""0133"",
            ""0134"",
            ""0135"",
            ""0137"",
            ""0150"",
            ""0151"",
            ""0152"",
            ""0153"",
            ""0154"",
            ""0155"",
            ""0184"",
            ""0185"",
            ""0188"",
            ""0191"",
            ""0194""
          ]
        },
        ""coordinates"": [
          {
            ""northing"": 59.91169773310983,
            ""easting"": 10.754335161831872,
            ""srId"": ""EPSG:4326""
          }
        ]
      }
    ]
  }
}";

            return withDuplicates
                ? @"{
  ""servicePointInformationResponse"": {
    ""customerSupportPhoneNo"": ""+4722329022"",
    ""servicePoints"": [
      {
        ""servicePointId"": ""3761681"",
        ""name"": ""REMA 1000 TRØGSTAD"",
        ""routeDistance"": 580,
        ""routingCode"": ""LAN"",
        ""visitingAddress"": {
          ""streetName"": ""KIRKEV"",
          ""streetNumber"": ""19"",
          ""postalCode"": ""1860"",
          ""city"": ""TRØGSTAD"",
          ""countryCode"": ""NO""
        },
        ""deliveryAddress"": {
          ""streetName"": ""KIRKEV"",
          ""streetNumber"": ""19"",
          ""postalCode"": ""1860"",
          ""city"": ""TRØGSTAD"",
          ""countryCode"": ""NO""
        },
        ""coordinate"": {
          ""srId"": ""EPSG:4326"",
          ""northing"": 59.64119872401216,
          ""easting"": 11.315418069080712
        },
        ""openingHours"": [
          {
            ""from1"": ""0700"",
            ""to1"": ""2300"",
            ""day"": ""MO""
          },
          {
            ""from1"": ""0700"",
            ""to1"": ""2300"",
            ""day"": ""TU""
          },
          {
            ""from1"": ""0700"",
            ""to1"": ""2300"",
            ""day"": ""WE""
          },
          {
            ""from1"": ""0700"",
            ""to1"": ""2300"",
            ""day"": ""TH""
          },
          {
            ""from1"": ""0700"",
            ""to1"": ""2300"",
            ""day"": ""FR""
          },
          {
            ""from1"": ""0700"",
            ""to1"": ""2300"",
            ""day"": ""SA""
          }
        ]
      },
      {
        ""servicePointId"": ""3659950"",
        ""name"": ""BUNNPRIS BÅSTAD"",
        ""routeDistance"": 8233,
        ""routingCode"": ""LAN"",
        ""visitingAddress"": {
          ""streetName"": ""BÅSTADV"",
          ""streetNumber"": ""651"",
          ""postalCode"": ""1866"",
          ""city"": ""BÅSTAD"",
          ""countryCode"": ""NO""
        },
        ""deliveryAddress"": {
          ""streetName"": ""BÅSTADV"",
          ""streetNumber"": ""651"",
          ""postalCode"": ""1866"",
          ""city"": ""BÅSTAD"",
          ""countryCode"": ""NO""
        },
        ""coordinate"": {
          ""srId"": ""EPSG:4326"",
          ""northing"": 59.70515144096135,
          ""easting"": 11.294324883133685
        },
        ""openingHours"": [
          {
            ""from1"": ""0800"",
            ""to1"": ""2200"",
            ""day"": ""MO""
          },
          {
            ""from1"": ""0800"",
            ""to1"": ""2200"",
            ""day"": ""TU""
          },
          {
            ""from1"": ""0800"",
            ""to1"": ""2200"",
            ""day"": ""WE""
          },
          {
            ""from1"": ""0800"",
            ""to1"": ""2200"",
            ""day"": ""TH""
          },
          {
            ""from1"": ""0800"",
            ""to1"": ""2200"",
            ""day"": ""FR""
          },
          {
            ""from1"": ""0800"",
            ""to1"": ""2000"",
            ""day"": ""SA""
          }
        ]
      },
      {
        ""servicePointId"": ""0322644"",
        ""name"": ""COOP OBS SLITU - COOP ØST AVD 715"",
        ""routeDistance"": 9813,
        ""routingCode"": ""LAN"",
        ""visitingAddress"": {
          ""streetName"": ""MORSTONGV"",
          ""streetNumber"": ""47"",
          ""postalCode"": ""1859"",
          ""city"": ""SLITU"",
          ""countryCode"": ""NO""
        },
        ""deliveryAddress"": {
          ""streetName"": ""MORSTONGV"",
          ""streetNumber"": ""47"",
          ""postalCode"": ""1859"",
          ""city"": ""SLITU"",
          ""countryCode"": ""NO""
        },
        ""coordinate"": {
          ""srId"": ""EPSG:4326"",
          ""northing"": 59.57882475593153,
          ""easting"": 11.277372044941389
        },
        ""openingHours"": [
          {
            ""from1"": ""0800"",
            ""to1"": ""2200"",
            ""day"": ""MO""
          },
          {
            ""from1"": ""0800"",
            ""to1"": ""2200"",
            ""day"": ""TU""
          },
          {
            ""from1"": ""0800"",
            ""to1"": ""2200"",
            ""day"": ""WE""
          },
          {
            ""from1"": ""0800"",
            ""to1"": ""2200"",
            ""day"": ""TH""
          },
          {
            ""from1"": ""0800"",
            ""to1"": ""2200"",
            ""day"": ""FR""
          },
          {
            ""from1"": ""0800"",
            ""to1"": ""2000"",
            ""day"": ""SA""
          }
        ]
      },
      {
        ""servicePointId"": ""0322644"",
        ""name"": ""COOP OBS SLITU - COOP ØST AVD 715"",
        ""routeDistance"": 9813,
        ""routingCode"": ""LAN"",
        ""visitingAddress"": {
          ""streetName"": ""MORSTONGV"",
          ""streetNumber"": ""47"",
          ""postalCode"": ""1859"",
          ""city"": ""SLITU"",
          ""countryCode"": ""NO""
        },
        ""deliveryAddress"": {
          ""streetName"": ""MORSTONGV"",
          ""streetNumber"": ""47"",
          ""postalCode"": ""1859"",
          ""city"": ""SLITU"",
          ""countryCode"": ""NO""
        },
        ""coordinate"": {
          ""srId"": ""EPSG:4326"",
          ""northing"": 59.57882475593153,
          ""easting"": 11.277372044941389
        },
        ""openingHours"": [
          {
            ""from1"": ""0800"",
            ""to1"": ""2200"",
            ""day"": ""MO""
          },
          {
            ""from1"": ""0800"",
            ""to1"": ""2200"",
            ""day"": ""TU""
          },
          {
            ""from1"": ""0800"",
            ""to1"": ""2200"",
            ""day"": ""WE""
          },
          {
            ""from1"": ""0800"",
            ""to1"": ""2200"",
            ""day"": ""TH""
          },
          {
            ""from1"": ""0800"",
            ""to1"": ""2200"",
            ""day"": ""FR""
          },
          {
            ""from1"": ""0800"",
            ""to1"": ""2000"",
            ""day"": ""SA""
          }
        ]
      },
      {
        ""servicePointId"": ""3544491"",
        ""name"": ""BUNNPRIS MYSEN AS"",
        ""routeDistance"": 11364,
        ""routingCode"": ""LAN"",
        ""visitingAddress"": {
          ""streetName"": ""JERNBANEG"",
          ""streetNumber"": ""13"",
          ""postalCode"": ""1850"",
          ""city"": ""MYSEN"",
          ""countryCode"": ""NO""
        },
        ""deliveryAddress"": {
          ""streetName"": ""JERNBANEG"",
          ""streetNumber"": ""13"",
          ""postalCode"": ""1850"",
          ""city"": ""MYSEN"",
          ""countryCode"": ""NO""
        },
        ""coordinate"": {
          ""srId"": ""EPSG:4326"",
          ""northing"": 59.5522382054129,
          ""easting"": 11.324827496269972
        },
        ""openingHours"": [
          {
            ""from1"": ""0700"",
            ""to1"": ""2300"",
            ""day"": ""MO""
          },
          {
            ""from1"": ""0700"",
            ""to1"": ""2300"",
            ""day"": ""TU""
          },
          {
            ""from1"": ""0700"",
            ""to1"": ""2300"",
            ""day"": ""WE""
          },
          {
            ""from1"": ""0700"",
            ""to1"": ""2300"",
            ""day"": ""TH""
          },
          {
            ""from1"": ""0700"",
            ""to1"": ""2300"",
            ""day"": ""FR""
          },
          {
            ""from1"": ""0900"",
            ""to1"": ""2100"",
            ""day"": ""SA""
          }
        ]
      }
    ]
  }
}"
                : @"{
  ""servicePointInformationResponse"": {
    ""customerSupportPhoneNo"": ""+4722329022"",
    ""servicePoints"": [
      {
        ""servicePointId"": ""3755659"",
        ""name"": ""EXTRA HAUGERUD SENTER"",
        ""routeDistance"": 1828,
        ""routingCode"": ""ALF"",
        ""visitingAddress"": {
          ""streetName"": ""HAUGERUD SENTER"",
          ""streetNumber"": ""1-7"",
          ""postalCode"": ""0673"",
          ""city"": ""OSLO"",
          ""countryCode"": ""NO""
        },
        ""deliveryAddress"": {
          ""streetName"": ""HAUGERUD SENTER"",
          ""streetNumber"": ""1-7"",
          ""postalCode"": ""0673"",
          ""city"": ""OSLO"",
          ""countryCode"": ""NO""
        },
        ""coordinate"": {
          ""srId"": ""EPSG:4326"",
          ""northing"": 59.92268523056556,
          ""easting"": 10.8564218057687
        },
        ""openingHours"": [
          {
            ""from1"": ""0700"",
            ""to1"": ""2300"",
            ""day"": ""MO""
          },
          {
            ""from1"": ""0700"",
            ""to1"": ""2300"",
            ""day"": ""TU""
          },
          {
            ""from1"": ""0700"",
            ""to1"": ""2300"",
            ""day"": ""WE""
          },
          {
            ""from1"": ""0700"",
            ""to1"": ""2300"",
            ""day"": ""TH""
          },
          {
            ""from1"": ""0700"",
            ""to1"": ""2300"",
            ""day"": ""FR""
          },
          {
            ""from1"": ""0700"",
            ""to1"": ""2300"",
            ""day"": ""SA""
          }
        ]
      },
      {
        ""servicePointId"": ""3783958"",
        ""name"": ""NARVESEN HAUGERUD"",
        ""routeDistance"": 1955,
        ""routingCode"": ""ALF"",
        ""visitingAddress"": {
          ""streetName"": ""HAUGERUD T-BANESTASJON"",
          ""postalCode"": ""0673"",
          ""city"": ""OSLO"",
          ""countryCode"": ""NO""
        },
        ""deliveryAddress"": {
          ""streetName"": ""HAUGERUD T-BANESTASJON"",
          ""postalCode"": ""0673"",
          ""city"": ""OSLO"",
          ""countryCode"": ""NO""
        },
        ""coordinate"": {
          ""srId"": ""EPSG:4326"",
          ""northing"": 59.92257803716301,
          ""easting"": 10.855018698678254
        },
        ""openingHours"": [
          {
            ""from1"": ""0630"",
            ""to1"": ""1900"",
            ""day"": ""MO""
          },
          {
            ""from1"": ""0630"",
            ""to1"": ""1900"",
            ""day"": ""TU""
          },
          {
            ""from1"": ""0630"",
            ""to1"": ""1900"",
            ""day"": ""WE""
          },
          {
            ""from1"": ""0630"",
            ""to1"": ""1900"",
            ""day"": ""TH""
          },
          {
            ""from1"": ""0630"",
            ""to1"": ""1900"",
            ""day"": ""FR""
          },
          {
            ""from1"": ""0900"",
            ""to1"": ""1800"",
            ""day"": ""SA""
          }
        ]
      },
      {
        ""servicePointId"": ""3696820"",
        ""name"": ""COOP EXTRA TVEITA SENTER"",
        ""routeDistance"": 2133,
        ""routingCode"": ""ALF"",
        ""visitingAddress"": {
          ""streetName"": ""TVETENV"",
          ""streetNumber"": ""150"",
          ""postalCode"": ""0671"",
          ""city"": ""OSLO"",
          ""countryCode"": ""NO""
        },
        ""deliveryAddress"": {
          ""streetName"": ""TVETENV"",
          ""streetNumber"": ""150"",
          ""postalCode"": ""0671"",
          ""city"": ""OSLO"",
          ""countryCode"": ""NO""
        },
        ""coordinate"": {
          ""srId"": ""EPSG:4326"",
          ""northing"": 59.91383624668802,
          ""easting"": 10.841889860914119
        },
        ""openingHours"": [
          {
            ""from1"": ""0700"",
            ""to1"": ""2300"",
            ""day"": ""MO""
          },
          {
            ""from1"": ""0700"",
            ""to1"": ""2300"",
            ""day"": ""TU""
          },
          {
            ""from1"": ""0700"",
            ""to1"": ""2300"",
            ""day"": ""WE""
          },
          {
            ""from1"": ""0700"",
            ""to1"": ""2300"",
            ""day"": ""TH""
          },
          {
            ""from1"": ""0700"",
            ""to1"": ""2300"",
            ""day"": ""FR""
          },
          {
            ""from1"": ""0800"",
            ""to1"": ""2100"",
            ""day"": ""SA""
          }
        ]
      },
      {
        ""servicePointId"": ""3718814"",
        ""name"": ""COOP PRIX OPPSALSTUBBEN AVD 4693"",
        ""routeDistance"": 2886,
        ""routingCode"": ""ALF"",
        ""visitingAddress"": {
          ""streetName"": ""OPPSALSTUBBEN"",
          ""streetNumber"": ""3"",
          ""postalCode"": ""0685"",
          ""city"": ""OSLO"",
          ""countryCode"": ""NO""
        },
        ""deliveryAddress"": {
          ""streetName"": ""OPPSALSTUBBEN"",
          ""streetNumber"": ""3"",
          ""postalCode"": ""0685"",
          ""city"": ""OSLO"",
          ""countryCode"": ""NO""
        },
        ""coordinate"": {
          ""srId"": ""EPSG:4326"",
          ""northing"": 59.89754985297417,
          ""easting"": 10.848044943236888
        },
        ""openingHours"": [
          {
            ""from1"": ""0700"",
            ""to1"": ""2200"",
            ""day"": ""MO""
          },
          {
            ""from1"": ""0700"",
            ""to1"": ""2200"",
            ""day"": ""TU""
          },
          {
            ""from1"": ""0700"",
            ""to1"": ""2200"",
            ""day"": ""WE""
          },
          {
            ""from1"": ""0700"",
            ""to1"": ""2200"",
            ""day"": ""TH""
          },
          {
            ""from1"": ""0700"",
            ""to1"": ""2200"",
            ""day"": ""FR""
          },
          {
            ""from1"": ""0800"",
            ""to1"": ""2000"",
            ""day"": ""SA""
          }
        ]
      },
      {
        ""servicePointId"": ""0951814"",
        ""name"": ""COOP PRIX SKØYENÅSEN"",
        ""routeDistance"": 3127,
        ""routingCode"": ""ALF"",
        ""visitingAddress"": {
          ""streetName"": ""HAAKON TVETERSV"",
          ""streetNumber"": ""8"",
          ""postalCode"": ""0682"",
          ""city"": ""OSLO"",
          ""countryCode"": ""NO""
        },
        ""deliveryAddress"": {
          ""streetName"": ""HAAKON TVETERSV"",
          ""streetNumber"": ""8"",
          ""postalCode"": ""0682"",
          ""city"": ""OSLO"",
          ""countryCode"": ""NO""
        },
        ""coordinate"": {
          ""srId"": ""EPSG:4326"",
          ""northing"": 59.90070750574168,
          ""easting"": 10.83472782301898
        },
        ""openingHours"": [
          {
            ""from1"": ""0800"",
            ""to1"": ""2200"",
            ""day"": ""MO""
          },
          {
            ""from1"": ""0800"",
            ""to1"": ""2200"",
            ""day"": ""TU""
          },
          {
            ""from1"": ""0800"",
            ""to1"": ""2200"",
            ""day"": ""WE""
          },
          {
            ""from1"": ""0800"",
            ""to1"": ""2200"",
            ""day"": ""TH""
          },
          {
            ""from1"": ""0800"",
            ""to1"": ""2200"",
            ""day"": ""FR""
          },
          {
            ""from1"": ""0900"",
            ""to1"": ""2000"",
            ""day"": ""SA""
          },
          {
            ""from1"": ""0900"",
            ""to1"": ""1800"",
            ""day"": ""SU""
          }
        ]
      }
    ]
  }
}";
        }
    }
}