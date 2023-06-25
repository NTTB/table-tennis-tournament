
export interface IT3Client {
  start(): Promise<void>;
  stop(): Promise<void>;
}
