using T3.Web.Services.Commit.Models;

namespace T3.Web.Services.SetValidation.Models;

public record IncorrectViews(SetView Committed, SetView Calculated);