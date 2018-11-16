import { SindicatoPatronal } from './sindicato-patronal';
import { SindicatoLaboral } from './sindicato-laboral';
import { Endereco } from './endereco';

export class Empresa {
    id: number;
    nome: string;
    cnpj: string;
    endereco: Endereco;
    qtdaTrabalhadores: number;
    massaSalarial: number;
    sindicatoLaboralId: number;
    sindicatoPatronalId: number;

    sindicatoLaboral: SindicatoLaboral;
    sindicatoPatronal: SindicatoPatronal;
}
