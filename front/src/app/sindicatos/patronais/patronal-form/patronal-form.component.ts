import { ToastsService } from './../../../shared/toasts.service';
import { RelatedLink } from './../../../shared/related-link/related-link';
import { ActivatedRoute, Route, Router } from '@angular/router';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, AbstractControl } from '@angular/forms';

import { Observable } from 'rxjs';

import { Contato } from 'src/app/model/contato';
import { SindicatoPatronal } from 'src/app/model/sindicato-patronal';
import { switchMap, filter, finalize, tap } from 'rxjs/operators';
import { Arquivo } from 'src/app/model/arquivo';
import { ToastType } from 'src/app/shared/toasts/toasts.component';
import { PatronaisApiService } from 'src/app/shared/api/patronais-api.service';


@Component({
  selector: 'app-patronal-form',
  templateUrl: './patronal-form.component.html',
  styleUrls: ['./patronal-form.component.css']
})
export class PatronalFormComponent implements OnInit {

  form: FormGroup;
  sindicatoPatronal: SindicatoPatronal;
  contatos$: Observable<Contato[]>;
  arquivos: Arquivo[];
  relatedLinks: RelatedLink[];
  spinnerArquivos = false;

  constructor(private route: ActivatedRoute,
    private formBuilder: FormBuilder,
    private service: PatronaisApiService,
    private router: Router,
    private toast: ToastsService) { }

  ngOnInit() {
    this.form = this.formBuilder.group({
      id: [{ value: '', disabled: true }],
      nome: ['', Validators.required],
      cnpj: ['', [Validators.required, Validators.maxLength(14), Validators.minLength(14)]],
      telefone1: [''],
      telefone2: [''],
      gestao: [''],
      site: ['']
    });

    this.route.data
      .pipe(
        filter(d => d.hasOwnProperty('sindicatoPatronal')),
        switchMap(d => {
          if (d.hasOwnProperty('sindicatoPatronal')) {
            this.sindicatoPatronal = d['sindicatoPatronal'];
            this.relatedLinks = [
              {
                label: 'Negociações',
                link: '/negociacoes/gestao',
                queryParams: { patronalId: this.sindicatoPatronal.id }
              },
              {
                label: 'Litígios',
                link: '/negociacoes/litigios',
                queryParams: { patronalId: this.sindicatoPatronal.id }
              },
              // {
              //   label: 'Planos de Ação',
              //   link: '/negociacoes/planosacao',
              //   queryParams: { patronalId: this.sindicatoPatronal.id }
              // }
            ];
            this.contatos$ = this.service.getContatos(this.sindicatoPatronal.id);
            this.updateForm(this.sindicatoPatronal);
            return this.service.getArquivos(this.sindicatoPatronal.id);
          }
        })
      )
      .subscribe(d => this.arquivos = d);
  }

  hasError(control: AbstractControl) {
    return control.dirty && control.invalid;
  }

  updateForm(sindicato: SindicatoPatronal) {
    this.form.patchValue(sindicato);
  }

  novoContato(contato: Contato) {
    this.contatos$ = this.service.addContato(this.sindicatoPatronal.id, contato)
      .pipe(
        switchMap(_ => this.service.getContatos(this.sindicatoPatronal.id))
      );
  }

  removeContato(idContato: number) {
    this.toast.swalMessage({
      title: 'Confirma exlusão?',
      message: 'Essa ação não poderá ser desfeita!',
      type: ToastType.warning
    }, () => this.service.deleteContato(this.sindicatoPatronal.id, idContato).subscribe(_ => {
      this.contatos$ = this.service.getContatos(this.sindicatoPatronal.id);
      this.toast.showMessage({
        message: 'Contato excluído com sucesso!',
        title: 'Sucesso!',
        type: ToastType.success
      });
    }));
  }

  upload(files: FileList) {
    this.spinnerArquivos = true;
    this.service.uploadFiles(this.sindicatoPatronal.id, files)
      .pipe(switchMap(_ => this.service.getArquivos(this.sindicatoPatronal.id)),
        finalize(() => this.spinnerArquivos = false))
      .subscribe(d => this.arquivos = d);
  }

  deleteFile(event: any) {
    this.service.getArquivos(this.sindicatoPatronal.id).subscribe(d => this.arquivos = d);
  }

  post(patronal: SindicatoPatronal) {
    this.service.post(patronal)
      .pipe(tap(_ => this.toast.showMessage({
        message: 'Sindicato salvo com sucesso!',
        title: 'Sucesso!',
        type: ToastType.success
      })))
      .subscribe((s: SindicatoPatronal) => this.router.navigate(['/sindicatos/patronais', s.id]));
  }

  put(patronal: SindicatoPatronal) {
    this.service.put(patronal.id, patronal)
      .pipe(tap(_ => this.toast.showMessage({
        message: 'Sindicato salvo com sucesso!',
        title: 'Sucesso!',
        type: ToastType.success
      })))
      .subscribe(_ => this.router.navigate(['/sindicatos/patronais', patronal.id]));
  }

  salvar() {
    const sind = this.form.getRawValue();
    if (this.sindicatoPatronal) {
      this.put(sind);
    } else {
      this.post(sind);
    }
  }

}
