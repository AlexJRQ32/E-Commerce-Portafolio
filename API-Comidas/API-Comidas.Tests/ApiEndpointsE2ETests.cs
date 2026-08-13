using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace API_Comidas.Tests
{
    /// <summary>
    /// Pruebas de integración E2E para validar todos los endpoints principales de la API
    /// </summary>
    public class ApiEndpointsE2ETests : IAsyncLifetime
    {
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client;

        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() }
        };

        public async Task InitializeAsync()
        {
            try
            {
                _factory = new WebApplicationFactory<Program>()
                    .WithWebHostBuilder(builder =>
                    {
                        // Las configuraciones del contenedor se pueden agregar aquí si es necesario
                    });

                _client = _factory.CreateClient();
                _client.DefaultRequestHeaders.Add("Accept", "application/json");
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error initializing test factory: {ex.Message}", ex);
            }
        }

        public async Task DisposeAsync()
        {
            try
            {
                _client?.Dispose();
                _factory?.Dispose();
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error disposing test resources: {ex.Message}", ex);
            }
        }

        #region GET /api/restaurants

        [Fact]
        [Trait("Category", "Integration")]
        public async Task GetRestaurants_ReturnsOkStatus_AndValidJsonList()
        {
            // Arrange
            var endpoint = "/api/restaurants";

            // Act
            var response = await _client.GetAsync(endpoint);

            // Assert - Status Code
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Assert - Content
            Assert.NotNull(response.Content);
            var jsonContent = await response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrWhiteSpace(jsonContent), "Response body is empty");

            // Assert - Valid JSON
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(jsonContent, _jsonOptions);
            Assert.NotEqual(JsonValueKind.Null, jsonElement.ValueKind);
            Assert.True(
                jsonElement.ValueKind == JsonValueKind.Array || jsonElement.ValueKind == JsonValueKind.Object,
                "Response must be an array or object"
            );
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task GetRestaurants_ReturnsValidContentType()
        {
            // Arrange
            var endpoint = "/api/restaurants";

            // Act
            var response = await _client.GetAsync(endpoint);

            // Assert
            Assert.NotNull(response.Content.Headers.ContentType);
            Assert.Contains("application/json", response.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task GetRestaurants_ResponseTimeIsAcceptable()
        {
            // Arrange
            var endpoint = "/api/restaurants";
            var startTime = DateTime.UtcNow;
            var timeoutThreshold = TimeSpan.FromSeconds(5);

            // Act
            var response = await _client.GetAsync(endpoint);
            var elapsedTime = DateTime.UtcNow - startTime;

            // Assert
            Assert.True(elapsedTime < timeoutThreshold, $"Response took {elapsedTime.TotalSeconds}s, exceeded threshold");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        #endregion

        #region GET /api/dishes

        [Fact]
        [Trait("Category", "Integration")]
        public async Task GetDishes_ReturnsOkStatus_AndValidJsonList()
        {
            // Arrange
            var endpoint = "/api/dishes";

            // Act
            var response = await _client.GetAsync(endpoint);

            // Assert - Status Code
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Assert - Content
            Assert.NotNull(response.Content);
            var jsonContent = await response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrWhiteSpace(jsonContent), "Response body is empty");

            // Assert - Valid JSON
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(jsonContent, _jsonOptions);
            Assert.NotEqual(JsonValueKind.Null, jsonElement.ValueKind);
            Assert.True(
                jsonElement.ValueKind == JsonValueKind.Array || jsonElement.ValueKind == JsonValueKind.Object,
                "Response must be an array or object"
            );
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task GetDishes_ReturnsValidContentType()
        {
            // Arrange
            var endpoint = "/api/dishes";

            // Act
            var response = await _client.GetAsync(endpoint);

            // Assert
            Assert.NotNull(response.Content.Headers.ContentType);
            Assert.Contains("application/json", response.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task GetDishes_ResponseTimeIsAcceptable()
        {
            // Arrange
            var endpoint = "/api/dishes";
            var startTime = DateTime.UtcNow;
            var timeoutThreshold = TimeSpan.FromSeconds(5);

            // Act
            var response = await _client.GetAsync(endpoint);
            var elapsedTime = DateTime.UtcNow - startTime;

            // Assert
            Assert.True(elapsedTime < timeoutThreshold, $"Response took {elapsedTime.TotalSeconds}s, exceeded threshold");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        #endregion

        #region POST /api/orders

        [Fact]
        [Trait("Category", "Integration")]
        public async Task PostOrder_WithValidData_ReturnsCreatedStatus()
        {
            // Arrange
            var orderData = new
            {
                restaurant = "Test Restaurant",
                status = "Pending",
                date = "2026-06-03",
                time = "14:30",
                customerId = 1,
                paymentMethodId = "1",
                addressId = "1",
                total = 45.50m,
                restaurantId = 1,
                items = new[]
                {
                    new
                    {
                        orderId = 0,
                        dishId = 1,
                        quantity = 2,
                        name = "Test Dish",
                        price = 22.75m
                    }
                }
            };

            var json = JsonSerializer.Serialize(orderData, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/orders", content);

            // Assert - Status Code (201 Created or 200 OK)
            Assert.NotNull(response);
            Assert.True(
                response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK,
                $"Expected 201 Created or 200 OK, but received {response.StatusCode}"
            );

            // Assert - Response Content
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrWhiteSpace(responseContent), "Response body is empty");

            // Assert - Valid JSON Response
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(responseContent, _jsonOptions);
            Assert.NotEqual(JsonValueKind.Null, jsonElement.ValueKind);

            // Assert - Content-Type Header
            Assert.NotNull(response.Content.Headers.ContentType);
            Assert.Contains("application/json", response.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task PostOrder_WithInvalidData_ReturnsBadRequest()
        {
            // Arrange - Missing required fields
            var invalidOrderData = new
            {
                restaurant = "Test Restaurant"
                // Missing customerId, restaurantId, total, etc.
            };

            var json = JsonSerializer.Serialize(invalidOrderData, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/orders", content);

            // Assert
            Assert.NotNull(response);
            Assert.True(
                response.StatusCode == HttpStatusCode.BadRequest ||
                response.StatusCode == HttpStatusCode.UnprocessableEntity ||
                response.StatusCode == HttpStatusCode.InternalServerError,
                $"Expected error status, but received {response.StatusCode}"
            );
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task PostOrder_WithEmptyBody_ReturnsBadRequest()
        {
            // Arrange
            var content = new StringContent("{}", Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/orders", content);

            // Assert
            Assert.NotNull(response);
            Assert.True(
                response.StatusCode == HttpStatusCode.BadRequest ||
                response.StatusCode == HttpStatusCode.UnprocessableEntity,
                $"Expected error status, but received {response.StatusCode}"
            );
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task PostOrder_ResponseTimeIsAcceptable()
        {
            // Arrange
            var orderData = new
            {
                restaurant = "Test Restaurant",
                status = "Pending",
                date = "2026-06-03",
                time = "14:30",
                customerId = 1,
                paymentMethodId = "1",
                addressId = "1",
                total = 45.50m,
                restaurantId = 1
            };

            var json = JsonSerializer.Serialize(orderData, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var startTime = DateTime.UtcNow;
            var timeoutThreshold = TimeSpan.FromSeconds(5);

            // Act
            var response = await _client.PostAsync("/api/orders", content);
            var elapsedTime = DateTime.UtcNow - startTime;

            // Assert
            Assert.True(elapsedTime < timeoutThreshold, $"Response took {elapsedTime.TotalSeconds}s, exceeded threshold");
        }

        #endregion

        #region General Health & Performance Checks

        [Fact]
        [Trait("Category", "Health")]
        public async Task ApiIsHealthy_RespondsToPing()
        {
            // Arrange
            var endpoint = "/api/restaurants";

            // Act
            var response = await _client.GetAsync(endpoint);

            // Assert
            Assert.True(
                response.StatusCode != HttpStatusCode.ServiceUnavailable,
                "API is not available (Service Unavailable)"
            );

            Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        [Trait("Category", "Health")]
        public async Task ApiServersAreRunning_MultipleEndpoints()
        {
            // Arrange
            var endpoints = new[] { "/api/restaurants", "/api/dishes" };

            // Act & Assert
            foreach (var endpoint in endpoints)
            {
                var response = await _client.GetAsync(endpoint);
                Assert.NotNull(response);
                Assert.NotEqual(HttpStatusCode.NotFound, response.StatusCode);
                Assert.NotEqual(HttpStatusCode.ServiceUnavailable, response.StatusCode);
            }
        }

        #endregion

        #region Edge Cases & Error Handling

        [Fact]
        [Trait("Category", "Integration")]
        public async Task GetRestaurant_WithNonExistentId_ReturnsNotFound()
        {
            // Arrange
            const int nonExistentId = 999999;

            // Act
            var response = await _client.GetAsync($"/api/restaurants/{nonExistentId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task GetDish_WithNonExistentId_ReturnsNotFound()
        {
            // Arrange
            const int nonExistentId = 999999;

            // Act
            var response = await _client.GetAsync($"/api/dishes/{nonExistentId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        #endregion
    }
}