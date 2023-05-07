import {Injectable} from '@angular/core';
import {ApiService} from "../shared/api.service";
import {CommitId} from "./models/typed-ids";
import {SetView} from "./models/set-view";

export interface SetCommitValidationResult {
  valid: boolean;
  invalidViews: {
    committed: SetView;
    calculated: SetView;
  }[];
}

@Injectable({
  providedIn: 'root'
})
export class SetCommitApiService {
  constructor(
    private readonly api: ApiService
  ) {
  }

  validate(id: CommitId) {
    return this.api.get<SetCommitValidationResult>(`api/SetCommit/${id.value}/validate`);
  }
}
