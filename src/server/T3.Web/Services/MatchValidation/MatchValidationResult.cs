using T3.Web.Services.Commit;

namespace T3.Web.Services.MatchValidation;
public record IncorrectViews(MatchView Committed, MatchView Calculated);
public record MatchValidationResult(bool Valid, IncorrectViews[] InvalidViews);