# Table Tennis Tournament variations

The goal of this document is to remind developers about the many variations of table tennis. 

Table tennis has gone through a lot of variations. We start with the ITTF version and then list some of the different versions.

## ITTF

Source: https://documents.ittf.sport/sites/default/files/public/2023-06/2023_ITTF_Statutes_clean_version.pdf

> #### 2.11 A GAME
> 2.11.1: A game shall be won by the player or pair first scoring 11 points unless both players or pairs score 10 points, when the game shall be won by the first player or pair subsequently gaining a lead of 2 points.
> 
> #### 2.12 A MATCH
> 2.12.1 A match shall consist of the best of any odd number of games.

Of course, there are more rules. But that is the most basic and common format.

NOTE: The ITTF uses "match" but currently we call this a "set" in the software.

## Game variations

The following are all the variations I have seen

- Without 2 points lead
- Play until a point limit is reached (with/without 2 points lead)
- Play until an alarm sounds (resulting in games won by 31-10)
- Classic score: Play until 21 instead of 11 (sometimes including the rule that the server changes every 5 points)
- Tennis rules: Only when serving you can receive points. The last player scoring becomes the server.

## Set/match variations

- Best of 3, 5, 7 and so on (this also covers the ITTF "match" rules)
- Play until x amount of games have been played.
- Play until the buzzer sounds

## Match/Tournament variations

- Round robin format (everyone plays everyone at least once, als)
- Brackets
- Single/double/triple/etc elimination
- Ladder competition
    - First round: Odds first (A vs B, C vs D, etc)
    - Second round: Even first (B vs C, D vs E, etc), the current champ stays
    - If you win you will go up, if you lose you go down a position.

And a lot more.