import { ToastsService } from 'src/app/shared/toasts.service';
import { RelatedLink } from '../../../shared/related-link/related-link';
import { ActivatedRoute, Router } from '@angular/router';
import { Arquivo } from 'src/app/model/arquivo';
import { Contato } from './../../../model/contato';
import { Observable } from 'rxjs';
import { SindicatoLaboral } from './../../../model/sindicato-laboral';
import { FormGroup, FormBuilder, Validators, AbstractControl } from '@angular/forms';
import { Component, OnInit } from '@angular/core';
import { switchMap, filter } from 'rxjs/operators';
import { LaboraisApiService } from '../laborais-api.service';
import { ToastType } from 'src/app/shared/toasts/toasts.component';

@Component({
  selector: 'app-laboral-form',
  templateUrl: './laboral-form.component.html',
  styleUrls: ['./laboral-form.component.css']
})
export class LaboralFormComponent implements OnInit {

  form: FormGroup;
  sindicatoLaboral: SindicatoLaboral;
  contatos$: Observable<Contato[]>;
  arquivos: Arquivo[];
  relatedLinks: RelatedLink[];

  constructor(private route: ActivatedRoute,
    private formBuilder: FormBuilder,
    private service: LaboraisApiService,
    private router: Router,
    private toast: ToastsService) { }

  ngOnInit() {
    this.form = this.formBuilder.group({
      id: [{ value: '', disabled: true }],
      nome: ['', Validators.required],
      cnpj: ['', [Validators.required, Validators.minLength(14), Validators.maxLength(14)]],
      telefone1: [''],
      telefone2: [''],
      gestao: [''],
      site: [''],
      federacao: ['', Validators.required],
      database: ['Janeiro', Validators.required],
      cct_act: ['ACT', Validators.required]
    });

    this.route.data
    .pipe(
      filter(d => d.hasOwnProperty('sindicatoLaboral')),
      switchMap(d => {
        if (d.hasOwnProperty('sindicatoLaboral')) {
          this.sindicatoLaboral = d['sindicatoLaboral'];
          this.relatedLinks = [
            {
              label: 'Negociações',
              link: '/negociacoes/agenda',
              queryParams: { laboralId: this.sindicatoLaboral.id }
            },
            {
              label: 'Litígios',
              link: '/negociacoes/litigios',
              queryParams: { laboralId: this.sindicatoLaboral.id }
            },
            {
              label: 'Planos de Ação',
              link: '/negociacoes/planosacao',
              queryParams: { laboralId: this.sindicatoLaboral.id }
            }
          ];
          this.contatos$ = this.service.getContatos(this.sindicatoLaboral.id);
          this.updateForm(this.sindicatoLaboral);
          return this.service.getArquivos(this.sindicatoLaboral.id);
        }
      })
    )
    .subscribe(d => this.arquivos = d);
  }

  hasError(control: AbstractControl) {
    return control.dirty && control.invalid;
  }

  updateForm(sindicato: SindicatoLaboral) {
    this.form.patchValue(sindicato);
  }

  novoContato(contato: Contato) {
    this.contatos$ = this.service.addContato(this.sindicatoLaboral.id, contato)
    .pipe(
      switchMap(_ => this.service.getContatos(this.sindicatoLaboral.id))
    );
  }

  removeContato(idContato: number) {
    this.toast.swalMessage({
      title: 'Confirma exlusão?',
      message: 'Essa ação não poderá ser desfeita!',
      type: ToastType.warning
    }, () => this.service.deleteContato(this.sindicatoLaboral.id, idContato).subscribe(_ => {
      this.contatos$ = this.service.getContatos(this.sindicatoLaboral.id);
      this.toast.showMessage({
        message: 'Contato excluído com sucesso!',
        title: 'Sucesso!',
        type: ToastType.success
      });
    }));
  }

  upload(files: FileList) {
    this.service.uploadFiles(this.sindicatoLaboral.id, files)
    .pipe(
      switchMap(_ => this.service.getArquivos(this.sindicatoLaboral.id))
    )
    .subscribe(d => this.arquivos = d);
  }

  deleteFile(event: any) {
    this.service.getArquivos(this.sindicatoLaboral.id).subscribe(d => this.arquivos = d);
  }

  post(sindicato: SindicatoLaboral) {
    this.service.post(sindicato)
    .subscribe(s => this.router.navigate(['/sindicatos/laborais', s.id]));
  }

  put(sindicato: SindicatoLaboral) {
    this.service.put(sindicato.id, sindicato)
    .subscribe(_ => this.router.navigate(['/sindicatos/laborais']));
  }

  salvar() {
    const sind = this.form.getRawValue();
    if (this.sindicatoLaboral) {
      this.put(sind);
    } else {
      this.post(sind);
    }
  }

}
