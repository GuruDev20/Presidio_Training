export class RecipeModel {
    constructor(public id: number,public name: string,public cuisine: string,public cookTimeMinutes: number,public ingredients: string[],public image: string){}
}
