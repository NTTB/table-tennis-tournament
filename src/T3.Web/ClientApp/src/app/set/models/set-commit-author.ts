import {SessionId, UserId} from "./typed-ids";
import {ClientApp} from "./client-app";

export interface SetCommitAuthor {
  /** The user who created the commit, can be direct or indirect. */
  userId: UserId;

  /** The session that created the commit. People can follow a session and see all the changes that happen in it. */
  sessionId: SessionId;

  /**
   * How should the author be displayed in the UI? This can be a username or a display name.
   * @example "Wouter", "Table 3"
   */
  displayName: string;

  /**
   * The device name that created the commit. This is mostly used to find out which device created a commit.
   * @example "Wouter's iPhone", "Digital Scoreboard A"
   */
  deviceName?: string;

  /**
   * The client app that created the commit. This can be used to market other apps, but also by app creators to find out
   * which apps are used and if there app has an issue.
   */
  clientApp?: ClientApp;
}
