import { RouterModule } from '@angular/router';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { PanelComponent } from './panel/panel.component';
import { ModalComponent } from './modal/modal.component';
import { ContatoComponent } from './contato/contato.component';
import { ArquivosComponent } from './arquivos/arquivos.component';
import { ValidatorMessageComponent } from './validator-message/validator-message.component';
import { CustomInputComponent } from './custom-input/custom-input.component';
import { ListContainerComponent } from './list-container/list-container.component';
import { AutocompleteComponent } from './autocomplete/autocomplete.component';
import { RelatedLinkComponent } from './related-link/related-link.component';
import { ToastsComponent } from './toasts/toasts.component';
import { NgxMaskModule } from 'ngx-mask';


@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule,
    NgxMaskModule.forRoot()
  ],
  declarations: [
    PanelComponent,
    ModalComponent,
    ContatoComponent,
    ArquivosComponent,
    ValidatorMessageComponent,
    CustomInputComponent,
    AutocompleteComponent,
    ListContainerComponent,
    RelatedLinkComponent,
    ToastsComponent
  ],
  exports: [
    PanelComponent,
    ModalComponent,
    ContatoComponent,
    ArquivosComponent,
    ValidatorMessageComponent,
    CustomInputComponent,
    AutocompleteComponent,
    ListContainerComponent,
    RelatedLinkComponent,
    ToastsComponent
  ]
})
export class SharedModule { }
