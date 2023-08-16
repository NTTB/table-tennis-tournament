namespace T3.Web.Services.Commit;

public enum PenaltyCard
{
    /// <summary>
    ///     From the ITTF rulebook:
    ///     3.5.1.3 Players may receive advice at any time except during rallies provided play is not thereby delayed
    ///     (3.4.4.1); if any authorised person gives advice illegally the umpire shall hold up a yellow card to warn him or
    ///     her that any further such offence will result in his or her dismissal from the playing area.
    ///     3.5.2.2 If at any time a player, a coach or another adviser commits a serious offence the umpire shall suspend play
    ///     and report immediately to the referee; for less serious offences the umpire may, on the first occasion, hold up a
    ///     yellow card and warn the offender that any further offence is liable to incur penalties.
    /// </summary>
    Yellow,

    /// <summary>
    ///     From the ITTF rulebook:
    ///     3.5.2.3 Except as provided in 3.5.2.2 and 3.5.2.5, if a player who has been warned commits a second offence in the
    ///     same individual match or team match, the umpire shall award 1 point to the offender's opponent and for a further
    ///     offence he or she shall award 2 points, each time holding up a yellow and a red card together.
    /// </summary>
    YellowAndRed,

    /// <summary>
    ///     From the ITTF rulebook:
    ///     3.5.1.2 In an individual event, a player or pair may receive advice only from one person, designated beforehand to
    ///     the umpire, except that where the players of a doubles pair are from different Associations each may designate an
    ///     adviser, but with regard to 3.5.1 and 3.5.2 these two advisers advisors shall be treated as a unit; if an
    ///     unauthorised person gives advice the umpire shall hold up a red card and send him or her away from the playing area
    ///     3.5.1.4 After a warning has been given, if in the same team match or the same match of an individual event anyone
    ///     again gives advice illegally, the umpire shall hold up a red card and send him or her away from the playing area,
    ///     whether or not he or she was the person warned.
    ///     3.5.2.7 Except as provided in 3.5.2.2, if a coach or another adviser who has been warned commits a further offence
    ///     in the same individual match or team match, the umpire shall hold up a red card and send him or her away from the
    ///     playing area until the end of the team match or, in an individual event, of the individual match
    /// </summary>
    Red
}