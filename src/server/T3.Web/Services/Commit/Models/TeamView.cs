using T3.Web.Services.Teams.ValueObjects;

namespace T3.Web.Services.Commit.Models;

public record TeamView(TeamId TeamId, string DisplayName, PlayerView[] Players);