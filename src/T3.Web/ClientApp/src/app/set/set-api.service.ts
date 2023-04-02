import {Injectable} from '@angular/core';
import {ApiService} from "../shared/api.service";

export interface SetEntity {
  id: string; // Guid
  displayName: string;
}

export interface SetCreateDto {
  displayName: string;
}

@Injectable({
  providedIn: 'root'
})
export class SetApiService {

  constructor(
    private readonly api: ApiService
  ) {
  }

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
}
