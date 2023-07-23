import { IHttpClient } from "./HttpClient";
import { SetView } from "./models/SetView";
import { SetCommitId, SetId } from "./models/typed-ids";

export interface SetEntity {
  id: string; // Guid
  displayName: string;
}

export interface SetCreateDto {
  displayName: string;
}

export interface IncorrectViews {
  committed: SetView;
  calculated: SetView;
}

export interface SetCommitValidationResult {
  valid: boolean;
  invalidViews: IncorrectViews[]
}

export interface ISetApi {
  getAll(): Promise<SetEntity[]>;
  getById(id: string): Promise<SetEntity>;
  create(dto: SetCreateDto): Promise<SetEntity>;
  delete(id: string): Promise<void>;
  validate(id: SetId, commitId: SetCommitId): Promise<SetCommitValidationResult>;
}

export class SetApi implements ISetApi {
  constructor(
    private readonly api: IHttpClient
  ) { }

  getAll() {
    return this.api.get<SetEntity[]>('api/set');
  }

  getById(id: string) {
    return this.api.get<SetEntity>(`api/set/${id}`);
  }

  create(dto: SetCreateDto) {
    return this.api.post<SetEntity>('api/set/create', dto);
  }

  delete(id: string) {
    return this.api.delete(`api/set/${id}`);
  }

  validate(id: SetId, commitId: SetCommitId) {
    return this.api.post<SetCommitValidationResult>(`api/set/${id.value}/commits/${commitId.value}/validate`, null);
  }
}
