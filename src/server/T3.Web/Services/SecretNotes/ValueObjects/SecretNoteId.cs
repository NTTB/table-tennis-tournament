using Microsoft.AspNetCore.Mvc;

namespace T3.Web.Services.SecretNotes.ValueObjects;

[ModelBinder(typeof(TypedGuidBinder<SecretNoteId>))]
public sealed record SecretNoteId: TypedGuid;