import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { IdentityService } from "../identity.service";
import { v4 as uuidv4 } from 'uuid';
import { KeyStorageService } from "../key-storage.service";
import { AccountApi, CryptoKeyService } from '@nttb/t3-api-client';

/**
 * This form is shown after the user has logged in.
 */
@Component({
  selector: 'app-post-login-form',
  templateUrl: './post-login-form.component.html',
  styleUrls: ['./post-login-form.component.css']
})
export class PostLoginFormComponent implements OnInit {
  constructor(
    private readonly identityService: IdentityService,
    private readonly keyStorageService: KeyStorageService,
    private readonly accountApi: AccountApi,
    private readonly cryptoKeyService: CryptoKeyService,
  ) {
  }

  form = new FormGroup({
    displayName: new FormControl(this.identityService.stored.displayName, {
      validators: [
        Validators.required,
        Validators.minLength(3)
      ],
      nonNullable: true
    }),
    deviceName: new FormControl(this.identityService.stored.deviceName, { nonNullable: true }),
    regenerateKeys: new FormControl(!this.keyStorageService.hasKeys(), { nonNullable: true }),
    newSession: new FormControl(!this.identityService.stored.sessionId, { nonNullable: true }),
  });

  @Output() updated = new EventEmitter<void>();

  ngOnInit(): void {
  }

  async onSubmit() {
    let sessionId = this.identityService.stored.sessionId;
    let userId = this.identityService.userId;
    let displayName = this.form.value.displayName;
    let deviceName = this.form.value.deviceName;
    if (!deviceName) deviceName = undefined;

    if (this.form.value.newSession || (!sessionId)) {
      sessionId = uuidv4();
    }

    if (!displayName) return;
    if (!userId) return;

    this.identityService.updateIdentity({
      displayName,
      sessionId,
      deviceName,
    });

    if (this.form.value.regenerateKeys || !this.keyStorageService.hasKeys()) {
      // Regenerate the crypto keys
      const pair = await this.cryptoKeyService.generateKey();
      await this.keyStorageService.set(pair);
      // Send public key to server
      await this.accountApi.addKey({
        publicKey: JSON.stringify(await this.cryptoKeyService.serialize(pair.publicKey))
      });
    }

    // At this point all the identity information is set up.
    this.updated.emit();
  }
}
