using Microsoft.AspNetCore.Mvc;

namespace T3.Web.Services.Teams;

[ModelBinder(typeof(TypedGuidBinder<TeamId>))]
public sealed record TeamId : TypedGuid;