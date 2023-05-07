namespace T3.Web.Services.SetValidation.Models;

public record SetCommitValidationResult(bool Valid, IncorrectViews[] InvalidViews);