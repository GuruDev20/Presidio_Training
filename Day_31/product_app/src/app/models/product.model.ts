export interface ProductModel {
    id: number;
    title: string;
    description: string;
    price: number;
    thumbnail: string;
}

export interface ProductResponse {
    products: ProductModel[];
    total: number;
    skip: number;
    limit: number;
}
