using Microsoft.AspNetCore.Mvc;

namespace T3.Web.Services.Rules.ValueObjects;

[ModelBinder(typeof(TypedGuidBinder<RulebookId>))] 
public sealed record RulebookId : TypedGuid;