import { AuthService } from './../../login/auth.service';
import { Component, OnInit, AfterViewInit } from '@angular/core';
import { Router } from '@angular/router';
import { tap } from 'rxjs/operators';

declare var $: any;

export interface MenuItem {
  label: string;
  link: string;
  icon: string;
  children: MenuItem[];
}

@Component({
  selector: 'app-navigation',
  templateUrl: './navigation.component.html',
  styleUrls: ['./navigation.component.css']
})
export class NavigationComponent implements OnInit, AfterViewInit {

  menu: MenuItem[];
  isAdmin = false;
  constructor(private router: Router, private authService: AuthService) {
    this.menu = [
      {
        label: 'Dashboard',
        link: '',
        icon: 'fa fa-bar-chart',
        children: [
          {
            label: 'Por Ano',
            link: '/dashboard/anual',
            children: null,
            icon: null
          },
          {
            label: 'Por Empresa',
            link: '/dashboard/empresa',
            children: null,
            icon: null
          }
        ]
      },
      {
        label: 'Sindicatos Patronais',
        link: '/sindicatos/patronais',
        icon: 'fa fa-building',
        children: null
      },
      {
        label: 'Sindicatos Laborais',
        link: '/sindicatos/laborais',
        icon: 'fa fa-building-o',
        children: null
      },
      {
        label: 'Empresas',
        link: '/empresas',
        icon: 'fa fa-users',
        children: null
      },
      {
        label: 'Negociações',
        link: '',
        icon: 'fa fa-handshake-o',
        children: [
          {
            label: 'Gestão',
            link: '/negociacoes/gestao',
            children: null,
            icon: null
          },
          {
            label: 'Agenda',
            link: '/negociacoes/agenda',
            children: null,
            icon: null
          }
        ]
      },
      {
        label: 'Litígios',
        link: '/negociacoes/litigios',
        icon: 'fa fa-book',
        children: null
      }/*,
      {
        label: 'Planos de Ação',
        link: '/negociacoes/planosacao',
        icon: 'fa fa-bullseye',
        children: null
      }*/
    ];
  }

  ngOnInit() {
    this.authService.onUserChanges.subscribe(_ => {
      this.isAdmin = this.authService.isInRole('Administrador');
    });
    this.isAdmin = this.authService.isInRole('Administrador');

    // this.router.events.pipe(tap(console.log)).subscribe();
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  ngAfterViewInit() {
    $('#side-menu').metisMenu();
  }


}
