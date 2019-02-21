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
import { PopoverModule } from 'ngx-popover';
import { PopoverTemplateComponent } from './popover-template/popover-template.component';
import { CalendarModule, DateAdapter } from 'angular-calendar';
import { adapterFactory } from 'angular-calendar/date-adapters/date-fns';
import { OnlyNumberDirective } from './directives/only-number.directive';

@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule,
    PopoverModule,
    NgxMaskModule.forRoot(),
    CalendarModule.forRoot({
      provide: DateAdapter,
      useFactory: adapterFactory
    })
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
    ToastsComponent,
    PopoverTemplateComponent,
    OnlyNumberDirective
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
    ToastsComponent,
    PopoverModule,
    PopoverTemplateComponent,
    CalendarModule,
    OnlyNumberDirective
  ]
})
export class SharedModule { }
