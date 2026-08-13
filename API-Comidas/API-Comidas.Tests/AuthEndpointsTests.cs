using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace API_Comidas.Tests
{
    /// <summary>
    /// Pruebas E2E para los endpoints de autenticación: Login, Register y RegisterRestaurant
    /// </summary>
    public class AuthEndpointsTests : IAsyncLifetime
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
            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient();
            _client.DefaultRequestHeaders.Add("Accept", "application/json");
            await Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            _client?.Dispose();
            _factory?.Dispose();
            await Task.CompletedTask;
        }

        #region POST /api/auth/register

        [Fact]
        [Trait("Category", "Auth")]
        public async Task Register_WithValidCustomerData_ReturnsOkStatus()
        {
            // Arrange
            var registerData = new
            {
                name = "Juan Pérez",
                email = $"juan{Guid.NewGuid()}@example.com",
                password = "Password123!",
                phone = "1234567890",
                roleId = 3 // Customer
            };

            var content = new StringContent(
                JsonSerializer.Serialize(registerData),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/auth/register", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(responseBody, _jsonOptions);
            
            Assert.True(jsonElement.TryGetProperty("userId", out var userId));
            Assert.True(jsonElement.TryGetProperty("roleId", out var roleId));
            Assert.Equal(3, roleId.GetInt32());
        }

        [Fact]
        [Trait("Category", "Auth")]
        public async Task Register_WithValidBusinessData_ReturnsOkStatus()
        {
            // Arrange
            var registerData = new
            {
                name = "Pedro García",
                email = $"pedro{Guid.NewGuid()}@example.com",
                password = "Password123!",
                phone = "0987654321",
                roleId = 2 // Business
            };

            var content = new StringContent(
                JsonSerializer.Serialize(registerData),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/auth/register", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(responseBody, _jsonOptions);
            
            Assert.True(jsonElement.TryGetProperty("userId", out _));
            Assert.True(jsonElement.TryGetProperty("roleId", out var roleId));
            Assert.Equal(2, roleId.GetInt32());
        }

        [Fact]
        [Trait("Category", "Auth")]
        public async Task Register_WithDuplicateEmail_ReturnsBadRequest()
        {
            // Arrange
            var email = $"duplicate{Guid.NewGuid()}@example.com";
            var registerData = new
            {
                name = "Test User",
                email = email,
                password = "Password123!",
                phone = "1234567890",
                roleId = 3
            };

            var content = new StringContent(
                JsonSerializer.Serialize(registerData),
                Encoding.UTF8,
                "application/json"
            );

            // Act - First registration
            await _client.PostAsync("/api/auth/register", content);

            // Act - Second registration with same email
            var response = await _client.PostAsync("/api/auth/register", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.Contains("ya está registrado", responseBody);
        }

        [Fact]
        [Trait("Category", "Auth")]
        public async Task Register_WithInvalidRoleId_ReturnsBadRequest()
        {
            // Arrange
            var registerData = new
            {
                name = "Test User",
                email = $"test{Guid.NewGuid()}@example.com",
                password = "Password123!",
                phone = "1234567890",
                roleId = 999 // Invalid
            };

            var content = new StringContent(
                JsonSerializer.Serialize(registerData),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/auth/register", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.Contains("inválido", responseBody);
        }

        #endregion

        #region POST /api/auth/register-restaurant

        [Fact]
        [Trait("Category", "Auth")]
        public async Task RegisterRestaurant_WithValidBusinessUser_ReturnsOkStatus()
        {
            // Arrange - Create a Business user first
            var registerData = new
            {
                name = "Roberto López",
                email = $"roberto{Guid.NewGuid()}@example.com",
                password = "Password123!",
                phone = "5555555555",
                roleId = 2 // Business
            };

            var registerContent = new StringContent(
                JsonSerializer.Serialize(registerData),
                Encoding.UTF8,
                "application/json"
            );

            var registerResponse = await _client.PostAsync("/api/auth/register", registerContent);
            var registerBody = await registerResponse.Content.ReadAsStringAsync();
            var registerJson = JsonSerializer.Deserialize<JsonElement>(registerBody, _jsonOptions);
            var userId = registerJson.GetProperty("userId").GetInt32();

            // Now register the restaurant
            var restaurantData = new
            {
                userId = userId,
                tradeName = "Mi Restaurante",
                address = "Calle Principal 123",
                categoryId = 1,
                openingTime = "09:00",
                closingTime = "23:00"
            };

            var restaurantContent = new StringContent(
                JsonSerializer.Serialize(restaurantData),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/auth/register-restaurant", restaurantContent);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(responseBody, _jsonOptions);
            
            Assert.True(jsonElement.TryGetProperty("restaurantId", out _));
            Assert.True(jsonElement.TryGetProperty("userId", out var returnedUserId));
            Assert.Equal(userId, returnedUserId.GetInt32());
        }

        [Fact]
        [Trait("Category", "Auth")]
        public async Task RegisterRestaurant_WithNonExistentUser_ReturnsNotFound()
        {
            // Arrange
            var restaurantData = new
            {
                userId = 99999,
                tradeName = "Restaurante Fantasma",
                address = "Dirección Desconocida",
                categoryId = 1,
                openingTime = "09:00",
                closingTime = "23:00"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(restaurantData),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/auth/register-restaurant", content);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.Contains("no encontrado", responseBody);
        }

        [Fact]
        [Trait("Category", "Auth")]
        public async Task RegisterRestaurant_WithCustomerUser_ReturnsBadRequest()
        {
            // Arrange - Create a Customer user first
            var registerData = new
            {
                name = "María García",
                email = $"maria{Guid.NewGuid()}@example.com",
                password = "Password123!",
                phone = "3333333333",
                roleId = 3 // Customer
            };

            var registerContent = new StringContent(
                JsonSerializer.Serialize(registerData),
                Encoding.UTF8,
                "application/json"
            );

            var registerResponse = await _client.PostAsync("/api/auth/register", registerContent);
            var registerBody = await registerResponse.Content.ReadAsStringAsync();
            var registerJson = JsonSerializer.Deserialize<JsonElement>(registerBody, _jsonOptions);
            var userId = registerJson.GetProperty("userId").GetInt32();

            // Try to register a restaurant with customer user
            var restaurantData = new
            {
                userId = userId,
                tradeName = "Restaurante Ilegal",
                address = "Calle Falsa 456",
                categoryId = 1,
                openingTime = "09:00",
                closingTime = "23:00"
            };

            var restaurantContent = new StringContent(
                JsonSerializer.Serialize(restaurantData),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/auth/register-restaurant", restaurantContent);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.Contains("Business", responseBody);
        }

        #endregion

        #region POST /api/auth/login

        [Fact]
        [Trait("Category", "Auth")]
        public async Task Login_WithValidCredentials_ReturnsOkStatus()
        {
            // Arrange - Create a user first
            var email = $"login{Guid.NewGuid()}@example.com";
            var password = "Password123!";

            var registerData = new
            {
                name = "Test Login",
                email = email,
                password = password,
                phone = "1111111111",
                roleId = 3
            };

            var registerContent = new StringContent(
                JsonSerializer.Serialize(registerData),
                Encoding.UTF8,
                "application/json"
            );

            await _client.PostAsync("/api/auth/register", registerContent);

            // Now try to login
            var loginData = new
            {
                email = email,
                password = password
            };

            var loginContent = new StringContent(
                JsonSerializer.Serialize(loginData),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/auth/login", loginContent);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(responseBody, _jsonOptions);
            
            Assert.True(jsonElement.TryGetProperty("user", out _));
        }

        [Fact]
        [Trait("Category", "Auth")]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var loginData = new
            {
                email = "noexiste@example.com",
                password = "WrongPassword123!"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(loginData),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/auth/login", content);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.Contains("incorrectas", responseBody);
        }

        #endregion
    }
}