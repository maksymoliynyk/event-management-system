﻿using System.Collections.Generic;
using System.Net.Http.Headers;

using Application.Commands.Auth;

using Domain.Entities.Users;
using Domain.Interfaces;

using Infrastructure;

using IntegrationTests.ApiTests.TestClient;

using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.ApiTests;

[Collection("IntegrationTests")]
public class BaseApiTests : BaseTestsLogging, IClassFixture<WebApiClientTestFixture>
{
    protected const string ValidPassword = "ValidPassword1234";
    protected readonly WebApiClientTestFixture _fixture;

    public BaseApiTests(WebApiClientTestFixture fixture, ITestOutputHelper outputHelper) : base(outputHelper)
    {
        _fixture = fixture;
    }

    protected async Task<(HttpClient Client, string Email)> CreateValidUser()
    {
        var client = _fixture.CreateClient();
        var registerModel = new RegisterUserCommand(Internet.Email(), Name.First(), Name.Last(), ValidPassword);
        var registerResponse = await client.RegisterUser(registerModel);

        StatusCodeIsSuccessful(registerResponse.Code);

        var loginModel = new LoginUserCommand(registerModel.Email, registerModel.Password);

        (HttpStatusCode loginStatusCode, string jwtToken) = await client.LoginUser(loginModel);
        StatusCodeIsSuccessful(loginStatusCode);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        return (client, loginModel.Email);
    }

    protected async Task<Guid> GetUserId(string email)
    {
        var identityService = _fixture.GetService<IIdentityService>();
        var user = await identityService.GetUserByEmail(email);
        return user.Id;
    }

    protected static void StatusCodeIsSuccessful(HttpStatusCode statusCode)
    {
        ((int)statusCode).Should().BeInRange(200, 299);
    }

    #region Initialize DbServices

    protected static IUnitOfWork InitUnitOfWork()
    {
        var helper = new ConnectionStringHelper();
        var context = new EventManagementContext(new DbContextOptionsBuilder<EventManagementContext>()
            .UseSqlServer(helper.Options.DefaultConnection)
            .Options);
        return new UnitOfWork(context);
    }

    protected static TestHelpingService InitTestService()
    {
        return new TestHelpingService();
    }

    #endregion

    #region CreateTestData

    protected static async Task<IEnumerable<TResult>> PerformManyAsync<T, TResult>(
        Func<T, CancellationToken, Task<TResult>> func, T input, int count)
    {
        var tasks = new List<TResult>();
        for (int i = 0; i < count; i++)
        {
            tasks.Add(await func.Invoke(input, CancellationToken.None));
        }

        return tasks;
    }

    public static string GenerateRandomWord(int length)
    {
        if (length <= 0)
        {
            throw new ArgumentException("Length must be greater than 0.", nameof(length));
        }

        const string characters =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()-_=+[]{}|;:,.<>?";
        var random = new Random();
        var result = new char[length];

        for (int i = 0; i < length; i++)
        {
            result[i] = characters[random.Next(characters.Length)];
        }

        return new string(result);
    }

    #endregion
}