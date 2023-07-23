using Microsoft.AspNetCore.Mvc;

namespace T3.Web.Services.Teams.ValueObjects;

[ModelBinder(typeof(TypedGuidBinder<TeamId>))]
public sealed record TeamId : TypedGuid;