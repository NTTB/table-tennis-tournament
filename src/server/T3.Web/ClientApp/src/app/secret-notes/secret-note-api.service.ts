import {Injectable} from "@angular/core";
import {Observable} from "rxjs";
import {ApiService} from "../shared/api.service";
import {SecretNoteId, SecretNoteVersionId} from "./models/typed-ids";
import {SecretNote} from "./models/secret-note";

@Injectable({providedIn: 'root'})
export class SecretNoteApiService {
  constructor(
    private readonly apiService: ApiService,
  ) {
  }

  public findById(id: SecretNoteId): Observable<SecretNote[]> {
    return this.apiService.get<SecretNote[]>(`api/secret-notes/${id}`);
  }

  public getById(id: SecretNoteId, versionId: SecretNoteVersionId): Observable<SecretNote> {
    return this.apiService.get<SecretNote>(`api/secret-notes/${id}/${versionId}`);
  }

  public create(body: SecretNote): Observable<SecretNote> {
    return this.apiService.post<SecretNote>('api/secret-notes', body);
  }
}
