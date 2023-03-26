import {ApiService} from "../shared/api.service";
import {Observable} from "rxjs";
import {Injectable} from "@angular/core";

@Injectable()
export class AccountApiService {
    constructor(
        private readonly apiService: ApiService
    ) {
    }
    
    public create(username: string, password: string): Observable<void> {
        return this.apiService.post<void>('api/account/create', {
            username,
            password
        });
    }

    public login(username: string, password: string): Observable<{jwtToken: string}> {
        return this.apiService.post<{jwtToken: string}>('api/account/login', {
            username,
            password
        });
    }
}