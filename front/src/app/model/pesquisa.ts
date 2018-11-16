export enum EntityType {
    Empresa = 'Empresa',
    SindicatoLaboral = 'SindicatoLaboral',
    SindicatoPatronal = 'SindicatoPatronal'
}

export class Pesquisa {
    entityType: EntityType;
    obj: any;
}
