/**
 * Interface for the JWT token provider.
 */
export interface IJwtTokenProvider {

  /**
   * Returns the JWT token if it exists.
   * @returns The JWT token if it exists, otherwise `null`
   */
  getJwtToken(): Promise<string | null>;
}
