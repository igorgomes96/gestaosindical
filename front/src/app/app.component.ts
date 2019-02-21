import { ToastsService } from './shared/toasts.service';
import { ToastMessage } from './shared/toasts/toasts.component';
import { Router, NavigationEnd } from '@angular/router';
import { Component, OnInit, AfterViewInit, EventEmitter } from '@angular/core';
import { correctHeight, detectBody } from '../app.helpers';
import { filter, distinctUntilChanged, tap, map, take } from 'rxjs/operators';
import { Location } from '@angular/common';

declare var $: any;

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit, AfterViewInit {
  exibeMenu = true;
  urlSemMenu: string[] = [];
  constructor(private router: Router, public toastService: ToastsService) {
    this.urlSemMenu = ['/login', '/registro', '/envia-codigo', '/recuperacao'];
  }

  ngOnInit() {
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd),
      distinctUntilChanged()
    ).subscribe(_ => {
      this.exibeMenu = !this.urlSemMenu.some(u => this.router.url.startsWith(u));
      if (!this.exibeMenu) {
        $('#page-wrapper').css('margin-left', '0px');
      } else {
        $('#page-wrapper').css('margin-left', '');
      }
    });
  }

  ngAfterViewInit() {
    // Run correctHeight function on load and resize window event
    $(window).bind('load resize', function() {
      correctHeight();
      detectBody();
    });

    // Correct height of wrapper after metisMenu animation.
    $('.metismenu a').click(() => {
      setTimeout(() => {
        correctHeight();
      }, 300);
    });
  }

}
