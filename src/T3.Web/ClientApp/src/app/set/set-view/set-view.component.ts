import { Component, Input, OnInit } from '@angular/core';
import { SetCommitService } from "../set-commit.service";
import { SetCommit } from "../models/set-commit";
import { TimestampService } from "../timestamp.service";
import { v4 as uuidv4, NIL as guidZero } from 'uuid';
import {CryptoKeyService} from "../../account/crypto-key.service";
import { JwtService } from 'src/app/shared/jwt.service';
import { firstValueFrom, lastValueFrom, map } from 'rxjs';
import { decodeJwt } from 'jose';
import { KeyStorageService } from 'src/app/account/key-storage.service';
@Component({
  selector: 'app-set-view',
  templateUrl: './set-view.component.html',
  styleUrls: ['./set-view.component.css']
})
export class SetViewComponent implements OnInit {
  jwt$: any;

  constructor(
    private readonly setCommitService: SetCommitService,
    private readonly timestampService: TimestampService,
    private readonly cryptoKeyService: CryptoKeyService,
    private readonly jwtService: JwtService,
    private readonly keyStorageService: KeyStorageService,
    ) {
  }

  @Input() setId?: string;

  lastMessage$ = this.setCommitService.messages$;
  lastState$ = this.setCommitService.state$;


  ngOnInit(): void {
    this.setCommitService.start().then(()=>{
      this.setCommitService.addSetWatch({value: this.setId!}).then(()=> console.log("Subscribed"));

    });
  }

  async sendUpdate() {
    if(!this.setId) return;

    let jwt$ = this.jwtService.jwt$.pipe(
      map(x => x ? decodeJwt(x) : null)
    );
  
    var userId$ = jwt$.pipe(map(x => x ? x.sub: undefined));
    var userId = await firstValueFrom(userId$);

    if(!userId) {
      throw new Error(" No user id");
    }

    const commitWithoutSignature: Omit<SetCommit, 'signature'> = {
      header: {
        commitId: { value: uuidv4() },
        setId: { value: this.setId },
        previousCommitId: undefined,
        author: {
          userId: { value: userId },
          sessionId: { value: guidZero },
          displayName: "test",
          clientApp: undefined,
          deviceName: undefined
        },
        createdAt: await this.timestampService.getTimestamp(),
      },
      body: { type: 'NoOp' },
      view: {
        gamesWon: { home: 0, away: 0 },
      },
    };

    const privateKey = await this.keyStorageService.getPrivateKey();
    const publicKey = await this.keyStorageService.getPublicKeyJwk();

    if(!privateKey) throw new Error("No private key known on client side");
    if(!publicKey) throw new Error("No public key known on client side");

    const signature = await this.cryptoKeyService.createSignature(privateKey, publicKey,  commitWithoutSignature);

    const commit = {
      ...commitWithoutSignature,
      signature
    };

    await this.setCommitService.push(commit);
  }

}
