using System;

namespace API.Models;

public sealed record CreateEventRequestModel(
    string Title,
    string Description,
    DateTime StartDate,
    long Duration,
    string Location);