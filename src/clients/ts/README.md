# T3-API-Client for FrontEnd
Player
Editor

Feed
1. All Feed (will show latest of all)
2. Limited feed, only show changes of certain sources
3. Local Feed (will start with a state and perform updates on that)

The main component of a set can be in a "Player" or "Editor" mode. The main component also takes a feed as argument.
We also need a method to select the changes (a timeline). 

Postfix rules:
- Viewer, shows and automatically performs update
- Input/Editor: Doesn't update, can only be used to update local state.

Things we need:
- Set
    - SetViewer
    - SetEditor
- Game
    - GameViewer
    - GameEditor
- SetScore
    - SetScoreViewer: Shows the score, but can't be edited
    - SetScoreInput: Textual input of a single set (so 3-2)
- SetPlayers
    - SetPlayerViewer: Shows the players of a set
    - SetPlayerInput: Enter the players
- SetPlayOrder
    - GameServerViewer: Shows the serving and receiving order
    - GameServerEditor: Allows
- Watch
    - WatchListEditor
    - WatchListViewer
    - WatchEditor
    - WatchViewer
- GameScore
    - GameScoreViewer: Shows the score, but can't be edited
    - GameScoreEditor: Allows clicking on a score to increase it
    - GameScoreInput: Textual input of a single game (so 11-9)
- Commits
    - CommitsList
    - CommitsFilter
- SecretNote
    - SecretNoteEditor
    - SecretNoteViewer
- Penalty