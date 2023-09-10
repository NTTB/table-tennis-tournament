using T3.Web.Services.Teams;

namespace T3.Web.Services.Commit;

public record TeamView(TeamId TeamId, string DisplayName, PlayerView[] Players);