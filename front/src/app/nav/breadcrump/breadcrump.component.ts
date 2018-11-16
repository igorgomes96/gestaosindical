import { ActivatedRoute } from '@angular/router';
import { Router, NavigationEnd } from '@angular/router';
import { Component, OnInit } from '@angular/core';

import { filter, distinctUntilChanged, map, tap } from 'rxjs/operators';
import { Observable } from 'rxjs';

interface BreadCrumb {
  label: string;
  url: string;
}


@Component({
  selector: 'app-breadcrump',
  templateUrl: './breadcrump.component.html',
  styleUrls: ['./breadcrump.component.css']
})
export class BreadcrumpComponent implements OnInit {

  breadcrumbs$: Observable<BreadCrumb[]>;
  title = '';

  constructor(
    private router: Router,
    private activatedRoute: ActivatedRoute
  ) { }

  ngOnInit() {
    this.breadcrumbs$ = this.router.events.pipe(
      filter(event => event instanceof NavigationEnd),
      distinctUntilChanged(),
      tap(_ => {
        this.title = '';
        this.getTitle(this.activatedRoute);
      }),
      map(_ => this.buildBreadCrumb(this.activatedRoute))
    );
  }

  getTitle(route: ActivatedRoute) {
    if (route.routeConfig && route.routeConfig.data && route.routeConfig.data.hasOwnProperty('title')) {
      this.title = route.routeConfig.data['title'];
    }
    if (route.firstChild) {
      this.getTitle(route.firstChild);
    }
  }

  buildBreadCrumb(route: ActivatedRoute, url: string = '',
    breadcrumbs: Array<BreadCrumb> = []): Array<BreadCrumb> {
    // If no routeConfig is avalailable we are on the root path
    let label: string =
      (route.routeConfig && route.routeConfig.data && route.routeConfig.data.hasOwnProperty('breadcrumb'))
        ? route.routeConfig.data['breadcrumb']
        : '';

    // Se for parâmetros
    if (label.startsWith(':')) {
      const param: string = label.substring(1);
      route.paramMap.subscribe(p => {
        if (p.has(param)) {
          label = p.get(param);
        }
      });
    }
    const path = route.routeConfig ? route.routeConfig.path : '';

    // In the routeConfig the complete path is not available,
    // so we rebuild it each time
    const nextUrl = `${url}${path}/`;
    const breadcrumb = {
      label: label,
      url: nextUrl
    };
    let newBreadcrumbs = [...breadcrumbs];
    if (label !== '') {
      newBreadcrumbs = [...breadcrumbs, breadcrumb];
    }
    if (route.firstChild) {
      // If we are not on our current path yet,
      // there will be more children to look after, to build our breadcumb
      return this.buildBreadCrumb(route.firstChild, nextUrl, newBreadcrumbs);
    }
    return newBreadcrumbs;
  }

}
