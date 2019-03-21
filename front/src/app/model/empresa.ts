import { SindicatoPatronal } from './sindicato-patronal';
import { SindicatoLaboral, Mes } from './sindicato-laboral';
import { Endereco } from './endereco';

export class Empresa {
    id: number;
    nome: string;
    cnpj: string;
    endereco: Endereco;
    qtdaTrabalhadores: number;
    massaSalarial: number;
    database: Mes;
    sindicatoLaboralId: number;
    sindicatoPatronalId: number;

    sindicatoLaboral: SindicatoLaboral;
    sindicatoPatronal: SindicatoPatronal;
}
