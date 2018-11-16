export interface IBreadcrump {
    title: string;
    paths: IPaths[];
}

export interface IPaths {
    label: string;
    url?: string;
}