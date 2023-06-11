using Microsoft.AspNetCore.Mvc;

namespace T3.Web.Services.SecretNotes.ValueObjects;

[ModelBinder(typeof(TypedGuidBinder<SecretNoteVersionId>))]
public sealed record SecretNoteVersionId: TypedGuid;