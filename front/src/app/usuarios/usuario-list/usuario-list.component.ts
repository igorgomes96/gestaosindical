import { Component, OnInit } from '@angular/core';
import { ToastsService } from 'src/app/shared/toasts.service';
import { Usuario } from 'src/app/model/usuario';
import { take } from 'rxjs/operators';
import { ToastType } from 'src/app/shared/toasts/toasts.component';
import { UsuariosApiService } from 'src/app/shared/api/usuarios-api.service';

declare var swal: any;

@Component({
  selector: 'app-usuario-list',
  templateUrl: './usuario-list.component.html',
  styleUrls: ['./usuario-list.component.css']
})
export class UsuarioListComponent implements OnInit {

  usuarios: Usuario[];
  usuariosFiltrados: Usuario[];
  constructor(private api: UsuariosApiService, private toastService: ToastsService) { }

  ngOnInit() {
    this.load();
  }

  load() {
    this.api.getAll()
      .pipe(take(1))
      .subscribe(d => {
        this.usuarios = d;
        this.usuariosFiltrados = d;
      });
  }

  onFilter(usuarios: Usuario[]) {
    this.usuariosFiltrados = usuarios;
  }

  excluir(usuario: Usuario) {
    swal({
      title: 'Confirma exlusão?',
      text: 'Todos os dados relacionados a este usuário serão excluídos!',
      icon: 'warning',
      buttons: true,
      dangerMode: true,
    }).then((willDelete) => {
      if (willDelete) {
        this.api.delete(usuario.login)
          .subscribe(_ => {
            this.load();
            this.toastService.showMessage({
              message: 'Usuário excluído com sucesso!',
              title: 'Sucesso!',
              type: ToastType.success
            });
          });
      }
    });
  }

}
